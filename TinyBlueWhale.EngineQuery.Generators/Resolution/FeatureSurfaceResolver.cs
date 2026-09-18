using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using TinyBlueWhale.EngineQuery.Generators.Discovery;
using TinyBlueWhale.EngineQuery.Generators.Models;
using TinyBlueWhale.EngineQuery.Generators.Rendering;

namespace TinyBlueWhale.EngineQuery.Generators.Resolution
{
    /// <summary>
    /// Resolves provider-specific feature surfaces from discovered provider profiles
    /// and generic root and composition surface definitions.
    /// </summary>
    internal static class FeatureSurfaceResolver
    {
        /// <summary>
        /// Builds provider profile models using compatible root and composition surfaces.
        /// </summary>
        internal static ImmutableArray<ProfileSurfaceModel> Resolve(Compilation compilation, ImmutableArray<INamedTypeSymbol> profiles, ImmutableArray<INamedTypeSymbol> rootSurfaceDefinitions, ImmutableArray<INamedTypeSymbol> compositionSurfaceDefinitions, INamedTypeSymbol profileContract)
        {
            return [.. profiles
                .Select(profile => new ProfileSurfaceModel(
                    profile,
                    ResolveRootProfileSurfaces(profile, rootSurfaceDefinitions, profileContract),
                    ResolveCompositionProfileSurfaces(profile, compositionSurfaceDefinitions, profileContract),
                    SymbolEqualityComparer.Default.Equals(profile.ContainingAssembly, compilation.Assembly)))
                .OrderBy(static model => model.Profile.ToDisplayString(), StringComparer.Ordinal)];
        }

        /// <summary>
        /// Gets ordinary instance methods declared directly by a feature surface.
        /// </summary>
        internal static IReadOnlyList<IMethodSymbol> GetDeclaredSurfaceMethods(INamedTypeSymbol surface)
        {
            return [.. surface
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(static method => method.MethodKind == MethodKind.Ordinary && !method.IsStatic)
                .OrderBy(static method => method.Name, StringComparer.Ordinal)
                .ThenBy(static method => method.Arity)
                .ThenBy(static method => method.Parameters.Length)];
        }

        /// <summary>
        /// Determines whether the specified return type represents a resolved root surface.
        /// </summary>
        internal static bool IsRootSurfaceReturn(ITypeSymbol returnType, ProfileSurfaceModel model)
        {
            if (returnType is not INamedTypeSymbol namedReturnType)
                return false;

            foreach (var surface in model.RootSurfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(surface, namedReturnType))
                    return true;

                if (surface.AllInterfaces.Any(inheritedSurface => SymbolEqualityComparer.Default.Equals(inheritedSurface, namedReturnType)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Builds the method signature identity used to de-duplicate generated feature methods.
        /// </summary>
        internal static string BuildMethodIdentity(IMethodSymbol method)
        {
            return $"{method.Name}`{method.Arity}({string.Join(",", method.Parameters.Select(parameter => $"{parameter.RefKind}:{parameter.Type.ToDisplayString(CSharpSourceRenderer.GeneratedTypeDisplayFormat)}"))})";
        }

        /// <summary>
        /// Finds the IQueryCommandBuilder contract inherited by a composition surface.
        /// </summary>
        internal static INamedTypeSymbol? FindQueryCommandBuilderSurface(INamedTypeSymbol surface)
        {
            return surface.AllInterfaces.FirstOrDefault(implementedInterface =>
                implementedInterface.OriginalDefinition.ToDisplayString() ==
                "TinyBlueWhale.EngineQuery.Abstractions.Interfaces.IQueryCommandBuilder<T, TProfile>");
        }

        /// <summary>
        /// Resolves root feature surfaces compatible with the specified provider profile.
        /// </summary>
        private static IReadOnlyList<INamedTypeSymbol> ResolveRootProfileSurfaces(INamedTypeSymbol profile, ImmutableArray<INamedTypeSymbol> surfaceDefinitions, INamedTypeSymbol profileContract)
        {
            var compatibleSurfaces = new List<INamedTypeSymbol>();

            foreach (var surfaceDefinition in surfaceDefinitions)
            {
                if (!SatisfiesSurfaceConstraints(profile, surfaceDefinition, profileContract))
                    continue;

                compatibleSurfaces.Add(surfaceDefinition.Construct(profile));
            }

            return RemoveInheritedSurfaces(compatibleSurfaces);
        }

        /// <summary>
        /// Resolves composition feature surface definitions compatible with the specified profile.
        /// </summary>
        private static IReadOnlyList<INamedTypeSymbol> ResolveCompositionProfileSurfaces(INamedTypeSymbol profile, ImmutableArray<INamedTypeSymbol> surfaceDefinitions, INamedTypeSymbol profileContract)
        {
            return [.. surfaceDefinitions
                .Where(surface => SatisfiesSurfaceConstraints(profile, surface, profileContract))
                .Where(candidate => !surfaceDefinitions.Any(other =>
                    !SymbolEqualityComparer.Default.Equals(candidate, other) &&
                    SatisfiesSurfaceConstraints(profile, other, profileContract) &&
                    other.AllInterfaces.Any(inherited =>
                        SymbolEqualityComparer.Default.Equals(inherited.OriginalDefinition, candidate.OriginalDefinition))))
                .OrderBy(static surface => surface.ToDisplayString(), StringComparer.Ordinal)];
        }

        /// <summary>
        /// Determines whether the specified provider profile satisfies the profile
        /// type parameter constraints declared by a feature surface.
        /// </summary>
        private static bool SatisfiesSurfaceConstraints(INamedTypeSymbol profile, INamedTypeSymbol surfaceDefinition, INamedTypeSymbol profileContract)
        {
            var profileParameterIndex = FeatureSurfaceDiscovery.GetProfileTypeParameterIndex(surfaceDefinition, profileContract);

            if (profileParameterIndex < 0)
                return false;

            var typeParameter = surfaceDefinition.TypeParameters[profileParameterIndex];

            if (typeParameter.HasReferenceTypeConstraint && profile.IsValueType)
                return false;

            if (typeParameter.HasValueTypeConstraint && !profile.IsValueType)
                return false;

            if (typeParameter.HasConstructorConstraint && !HasPublicParameterlessConstructor(profile))
                return false;

            return typeParameter.ConstraintTypes.All(constraint => SatisfiesTypeConstraint(profile, constraint));
        }

        /// <summary>
        /// Determines whether the specified provider profile exposes a public parameterless constructor.
        /// </summary>
        private static bool HasPublicParameterlessConstructor(INamedTypeSymbol profile)
        {
            return profile.InstanceConstructors.Any(constructor =>
                constructor.Parameters.Length == 0 &&
                constructor.DeclaredAccessibility == Accessibility.Public);
        }

        /// <summary>
        /// Determines whether a provider profile satisfies the specified type constraint.
        /// </summary>
        private static bool SatisfiesTypeConstraint(INamedTypeSymbol profile, ITypeSymbol constraint)
        {
            if (SymbolEqualityComparer.Default.Equals(profile, constraint))
                return true;

            if (constraint.TypeKind == TypeKind.Interface)
                return profile.AllInterfaces.Any(implementedInterface => SymbolEqualityComparer.Default.Equals(implementedInterface, constraint));

            var currentType = profile.BaseType;

            while (currentType is not null)
            {
                if (SymbolEqualityComparer.Default.Equals(currentType, constraint))
                    return true;

                currentType = currentType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Removes surfaces already inherited by another compatible surface.
        /// </summary>
        private static IReadOnlyList<INamedTypeSymbol> RemoveInheritedSurfaces(IReadOnlyList<INamedTypeSymbol> surfaces)
        {
            return [.. surfaces
                .Where(candidate => !surfaces.Any(other =>
                    !SymbolEqualityComparer.Default.Equals(candidate, other) &&
                    other.AllInterfaces.Any(inheritedSurface =>
                        SymbolEqualityComparer.Default.Equals(inheritedSurface, candidate))))
                .OrderBy(static surface => surface.ToDisplayString(), StringComparer.Ordinal)];
        }
    }
}
