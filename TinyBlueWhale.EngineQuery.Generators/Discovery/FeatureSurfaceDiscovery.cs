using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using TinyBlueWhale.EngineQuery.Generators.Models;
using TinyBlueWhale.EngineQuery.Generators.Rendering;

namespace TinyBlueWhale.EngineQuery.Generators.Discovery
{
    /// <summary>
    /// Discovers EngineQuery root and composition feature surfaces available through
    /// the current compilation and referenced EngineQuery assemblies.
    /// </summary>
    internal static class FeatureSurfaceDiscovery
    {
        private const string QueryBuilderInterfaceMetadataName =
            "TinyBlueWhale.EngineQuery.Abstractions.Interfaces.IQueryBuilder`1";

        private const string QueryCommandBuilderInterfaceMetadataName =
            "TinyBlueWhale.EngineQuery.Abstractions.Interfaces.IQueryCommandBuilder`2";

        /// <summary>
        /// Discovers generic root query feature surfaces.
        /// </summary>
        internal static ImmutableArray<INamedTypeSymbol> DiscoverRootSurfaces(Compilation compilation, INamedTypeSymbol profileContract, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryBuilderContract = compilation.GetTypeByMetadataName(QueryBuilderInterfaceMetadataName);

            if (queryBuilderContract is null)
                return [];

            var surfaces = ImmutableArray.CreateBuilder<INamedTypeSymbol>();

            CollectRootFeatureSurfaces(compilation.Assembly.GlobalNamespace, queryBuilderContract, profileContract, surfaces, cancellationToken);

            foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!assembly.Name.StartsWith("TinyBlueWhale.EngineQuery", StringComparison.Ordinal))
                    continue;

                CollectRootFeatureSurfaces(assembly.GlobalNamespace, queryBuilderContract, profileContract, surfaces, cancellationToken);
            }

            return [.. surfaces
                .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                .OrderBy(static surface => surface.ToDisplayString(), StringComparer.Ordinal)];
        }

        /// <summary>
        /// Discovers generic query composition feature surfaces.
        /// </summary>
        internal static ImmutableArray<INamedTypeSymbol> DiscoverCompositionSurfaces(Compilation compilation, INamedTypeSymbol profileContract, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryCommandBuilderContract = compilation.GetTypeByMetadataName(QueryCommandBuilderInterfaceMetadataName);

            if (queryCommandBuilderContract is null)
                return [];

            var surfaces = ImmutableArray.CreateBuilder<INamedTypeSymbol>();

            CollectCompositionFeatureSurfaces(compilation.Assembly.GlobalNamespace, queryCommandBuilderContract, profileContract, surfaces, cancellationToken);

            foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!assembly.Name.StartsWith("TinyBlueWhale.EngineQuery", StringComparison.Ordinal))
                    continue;

                CollectCompositionFeatureSurfaces(assembly.GlobalNamespace, queryCommandBuilderContract, profileContract, surfaces, cancellationToken);
            }

            return [.. surfaces
                .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                .OrderBy(static surface => surface.ToDisplayString(), StringComparer.Ordinal)];
        }

        /// <summary>
        /// Gets the index of the provider profile type parameter declared by a feature surface.
        /// </summary>
        internal static int GetProfileTypeParameterIndex(INamedTypeSymbol surfaceDefinition, INamedTypeSymbol profileContract)
        {
            for (var index = 0; index < surfaceDefinition.TypeParameters.Length; index++)
            {
                var typeParameter = surfaceDefinition.TypeParameters[index];

                var hasProfileConstraint = typeParameter.ConstraintTypes.Any(constraint =>
                    SymbolEqualityComparer.Default.Equals(constraint.OriginalDefinition, profileContract.OriginalDefinition));

                if (!hasProfileConstraint)
                    continue;

                var hasFeatureConstraint = typeParameter.ConstraintTypes.Any(constraint =>
                    !SymbolEqualityComparer.Default.Equals(constraint.OriginalDefinition, profileContract.OriginalDefinition));

                if (hasFeatureConstraint)
                    return index;
            }

            return -1;
        }

        /// <summary>
        /// Determines whether the specified constructed interface represents a root
        /// feature surface.
        /// </summary>
        internal static bool IsConstructedRootFeatureSurface(INamedTypeSymbol surface, INamedTypeSymbol profileContract)
        {
            return surface.TypeKind == TypeKind.Interface &&
                   surface.IsGenericType &&
                   surface.Arity == 1 &&
                   GetProfileTypeParameterIndex(surface.OriginalDefinition, profileContract) >= 0;
        }

        /// <summary>
        /// Traverses namespaces recursively and collects root feature surfaces.
        /// </summary>
        private static void CollectRootFeatureSurfaces(INamespaceSymbol namespaceSymbol, INamedTypeSymbol queryBuilderContract, INamedTypeSymbol profileContract, ImmutableArray<INamedTypeSymbol>.Builder surfaces, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var type in namespaceSymbol.GetTypeMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsRootFeatureSurface(type, queryBuilderContract, profileContract))
                    surfaces.Add(type);

                CollectNestedRootFeatureSurfaces(type, queryBuilderContract, profileContract, surfaces, cancellationToken);
            }

            foreach (var nestedNamespace in namespaceSymbol.GetNamespaceMembers())
                CollectRootFeatureSurfaces(nestedNamespace, queryBuilderContract, profileContract, surfaces, cancellationToken);
        }

        /// <summary>
        /// Traverses nested types and collects root feature surfaces.
        /// </summary>
        private static void CollectNestedRootFeatureSurfaces(INamedTypeSymbol containingType, INamedTypeSymbol queryBuilderContract, INamedTypeSymbol profileContract, ImmutableArray<INamedTypeSymbol>.Builder surfaces, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var nestedType in containingType.GetTypeMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsRootFeatureSurface(nestedType, queryBuilderContract, profileContract))
                    surfaces.Add(nestedType);

                CollectNestedRootFeatureSurfaces(nestedType, queryBuilderContract, profileContract, surfaces, cancellationToken);
            }
        }

        /// <summary>
        /// Determines whether the specified interface represents a root query feature surface.
        /// </summary>
        private static bool IsRootFeatureSurface(INamedTypeSymbol type, INamedTypeSymbol queryBuilderContract, INamedTypeSymbol profileContract)
        {
            if (type.TypeKind != TypeKind.Interface || !type.IsGenericType || type.Arity != 1)
                return false;

            if (SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, queryBuilderContract.OriginalDefinition))
                return false;

            var implementsQueryBuilder = type.AllInterfaces.Any(implementedInterface =>
                SymbolEqualityComparer.Default.Equals(implementedInterface.OriginalDefinition, queryBuilderContract.OriginalDefinition));

            return implementsQueryBuilder && GetProfileTypeParameterIndex(type.OriginalDefinition, profileContract) >= 0;
        }

        /// <summary>
        /// Traverses namespaces recursively and collects composition feature surfaces.
        /// </summary>
        private static void CollectCompositionFeatureSurfaces(INamespaceSymbol namespaceSymbol, INamedTypeSymbol queryCommandBuilderContract, INamedTypeSymbol profileContract, ImmutableArray<INamedTypeSymbol>.Builder surfaces, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var type in namespaceSymbol.GetTypeMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsCompositionFeatureSurface(type, queryCommandBuilderContract, profileContract))
                    surfaces.Add(type);

                CollectNestedCompositionFeatureSurfaces(type, queryCommandBuilderContract, profileContract, surfaces, cancellationToken);
            }

            foreach (var nestedNamespace in namespaceSymbol.GetNamespaceMembers())
                CollectCompositionFeatureSurfaces(nestedNamespace, queryCommandBuilderContract, profileContract, surfaces, cancellationToken);
        }

        /// <summary>
        /// Traverses nested types and collects composition feature surfaces.
        /// </summary>
        private static void CollectNestedCompositionFeatureSurfaces(INamedTypeSymbol containingType, INamedTypeSymbol queryCommandBuilderContract, INamedTypeSymbol profileContract, ImmutableArray<INamedTypeSymbol>.Builder surfaces, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var nestedType in containingType.GetTypeMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsCompositionFeatureSurface(nestedType, queryCommandBuilderContract, profileContract))
                    surfaces.Add(nestedType);

                CollectNestedCompositionFeatureSurfaces(nestedType, queryCommandBuilderContract, profileContract, surfaces, cancellationToken);
            }
        }

        /// <summary>
        /// Determines whether the specified interface represents a composition feature surface.
        /// </summary>
        private static bool IsCompositionFeatureSurface(INamedTypeSymbol type, INamedTypeSymbol queryCommandBuilderContract, INamedTypeSymbol profileContract)
        {
            if (type.TypeKind != TypeKind.Interface || !type.IsGenericType)
                return false;

            if (SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, queryCommandBuilderContract.OriginalDefinition))
                return false;

            var implementsQueryCommandBuilder = type.AllInterfaces.Any(implementedInterface =>
                SymbolEqualityComparer.Default.Equals(implementedInterface.OriginalDefinition, queryCommandBuilderContract.OriginalDefinition));

            return implementsQueryCommandBuilder && GetProfileTypeParameterIndex(type.OriginalDefinition, profileContract) >= 0;
        }
    }
}
