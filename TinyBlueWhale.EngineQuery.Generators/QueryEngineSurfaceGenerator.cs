using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TinyBlueWhale.EngineQuery.Generators
{
    /// <summary>
    /// Generates strongly typed EngineQuery root feature extensions, query engine
    /// surfaces, implementations and dependency injection registrations from
    /// concrete database provider profiles.
    /// </summary>
    /// <remarks>
    /// Provider profiles act as the single source of truth for provider version and
    /// feature availability. Root feature surfaces declare their requirements through
    /// generic profile constraints and are discovered structurally without requiring
    /// feature-specific knowledge inside the generator.
    /// </remarks>
    [Generator]
    public sealed class QueryEngineSurfaceGenerator : IIncrementalGenerator
    {
        private const string DatabaseProviderProfileInterface =
            "TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers.IDatabaseProviderProfile";

        private const string QueryBuilderInterfaceMetadataName =
            "TinyBlueWhale.EngineQuery.Abstractions.Interfaces.IQueryBuilder`1";

        private const string QueryEngineInterfaceMetadataName =
            "TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces.IQueryEngine`1";

        private const string QueryEngineInterface =
            "TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces.IQueryEngine";

        private const string GeneratedNamespace =
            "TinyBlueWhale.EngineQuery.Generated";

        private const string GeneratedExtensionsNamespace =
            "TinyBlueWhale.EngineQuery.Abstractions.Extensions";

        private static readonly SymbolDisplayFormat GeneratedTypeDisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
            SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions |
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var generationModel = context.CompilationProvider.Select(static (compilation, cancellationToken) => BuildGenerationModel(compilation, cancellationToken));

            context.RegisterSourceOutput(
                generationModel,
                static (productionContext, model) =>
                {
                    GenerateManualQueryBuilderExtensions(productionContext, model);

                    if (!model.SupportsQueryEngineGeneration)
                        return;

                    GenerateQueryEngineSurfaces(productionContext, model);

                    GenerateDependencyInjectionRegistration(productionContext, model);
                });
        }

        /// <summary>
        /// Builds the complete source generation model associated with the current compilation.
        /// </summary>
        /// <param name="compilation">
        /// Compilation being processed by the source generator.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel model construction.
        /// </param>
        /// <returns>
        /// Generation model containing every discovered provider profile and compatible root feature surface.
        /// </returns>
        private static GenerationModel BuildGenerationModel(Compilation compilation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var profileContract = compilation.GetTypeByMetadataName(DatabaseProviderProfileInterface);

            if (profileContract is null)
                return new GenerationModel(ImmutableArray<ProfileSurfaceModel>.Empty, null, false);

            var profiles = DiscoverProfiles(compilation, profileContract, cancellationToken);

            var surfaceDefinitions = DiscoverRootFeatureSurfaces(compilation, profileContract, cancellationToken);

            var profileModels = BuildProfileSurfaceModels(compilation, profiles, surfaceDefinitions);

            var supportsQueryEngineGeneration = compilation.GetTypeByMetadataName(QueryEngineInterfaceMetadataName) is not null;

            return new GenerationModel(profileModels, profileContract, supportsQueryEngineGeneration);
        }

        /// <summary>
        /// Discovers concrete database provider profiles available through the current
        /// compilation and referenced EngineQuery assemblies.
        /// </summary>
        /// <param name="compilation">
        /// Compilation being processed by the source generator.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to identify compatible types.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel profile discovery.
        /// </param>
        /// <returns>
        /// Immutable collection containing every discovered concrete provider profile.
        /// </returns>
        private static ImmutableArray<INamedTypeSymbol> DiscoverProfiles(Compilation compilation, INamedTypeSymbol profileContract, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var profiles = ImmutableArray.CreateBuilder<INamedTypeSymbol>();

            CollectProfiles(compilation.Assembly.GlobalNamespace, profileContract, profiles, cancellationToken);

            foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!assembly.Name.StartsWith("TinyBlueWhale.EngineQuery", StringComparison.Ordinal))
                    continue;

                CollectProfiles(assembly.GlobalNamespace, profileContract, profiles, cancellationToken);
            }

            return profiles
                .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                .OrderBy(static profile => profile.ToDisplayString(), StringComparer.Ordinal)
                .ToImmutableArray();
        }

        /// <summary>
        /// Traverses the specified namespace recursively and collects concrete classes
        /// implementing the database provider profile contract.
        /// </summary>
        /// <param name="namespaceSymbol">
        /// Namespace currently being inspected.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile interface used to identify compatible types.
        /// </param>
        /// <param name="profiles">
        /// Collection receiving discovered provider profiles.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel namespace traversal.
        /// </param>
        private static void CollectProfiles(INamespaceSymbol namespaceSymbol, INamedTypeSymbol profileContract, ImmutableArray<INamedTypeSymbol>.Builder profiles, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var type in namespaceSymbol.GetTypeMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsConcreteProfile(type, profileContract))
                    profiles.Add(type);

                CollectNestedProfiles(type, profileContract, profiles, cancellationToken);
            }

            foreach (var nestedNamespace in namespaceSymbol.GetNamespaceMembers())
                CollectProfiles(nestedNamespace, profileContract, profiles, cancellationToken);
        }

        /// <summary>
        /// Traverses nested types and collects concrete database provider profiles.
        /// </summary>
        /// <param name="containingType">
        /// Type whose nested types are being inspected.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile interface used to identify compatible types.
        /// </param>
        /// <param name="profiles">
        /// Collection receiving discovered provider profiles.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel nested type traversal.
        /// </param>
        private static void CollectNestedProfiles(INamedTypeSymbol containingType, INamedTypeSymbol profileContract, ImmutableArray<INamedTypeSymbol>.Builder profiles, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var nestedType in containingType.GetTypeMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsConcreteProfile(nestedType, profileContract))
                    profiles.Add(nestedType);

                CollectNestedProfiles(nestedType, profileContract, profiles, cancellationToken);
            }
        }

        /// <summary>
        /// Determines whether the specified type represents a concrete database provider profile.
        /// </summary>
        /// <param name="type">
        /// Type being inspected.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile interface used to identify compatible types.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the specified type is a concrete provider profile;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsConcreteProfile(INamedTypeSymbol type, INamedTypeSymbol profileContract)
        {
            if (type.TypeKind != TypeKind.Class || type.IsAbstract)
                return false;

            return type.AllInterfaces.Any(implementedInterface => SymbolEqualityComparer.Default.Equals(implementedInterface, profileContract));
        }

        /// <summary>
        /// Discovers generic root query feature surfaces available through the current
        /// compilation and referenced EngineQuery assemblies.
        /// </summary>
        /// <param name="compilation">
        /// Compilation being processed by the source generator.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to identify feature constraints.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel root feature surface discovery.
        /// </param>
        /// <returns>
        /// Immutable collection containing every discovered root feature surface definition.
        /// </returns>
        private static ImmutableArray<INamedTypeSymbol> DiscoverRootFeatureSurfaces(Compilation compilation, INamedTypeSymbol profileContract, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryBuilderContract = compilation.GetTypeByMetadataName(QueryBuilderInterfaceMetadataName);

            if (queryBuilderContract is null)
                return ImmutableArray<INamedTypeSymbol>.Empty;

            var surfaces = ImmutableArray.CreateBuilder<INamedTypeSymbol>();

            CollectRootFeatureSurfaces(compilation.Assembly.GlobalNamespace, queryBuilderContract, profileContract, surfaces, cancellationToken);

            foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!assembly.Name.StartsWith("TinyBlueWhale.EngineQuery", StringComparison.Ordinal))
                    continue;

                CollectRootFeatureSurfaces(assembly.GlobalNamespace, queryBuilderContract, profileContract, surfaces, cancellationToken);
            }

            return surfaces
                .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                .OrderBy(static surface => surface.ToDisplayString(), StringComparer.Ordinal)
                .ToImmutableArray();
        }

        /// <summary>
        /// Traverses the specified namespace recursively and collects root query feature surfaces.
        /// </summary>
        /// <param name="namespaceSymbol">
        /// Namespace currently being inspected.
        /// </param>
        /// <param name="queryBuilderContract">
        /// Root query builder contract.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract.
        /// </param>
        /// <param name="surfaces">
        /// Collection receiving discovered root feature surfaces.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel namespace traversal.
        /// </param>
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
        /// Traverses nested types and collects root query feature surfaces.
        /// </summary>
        /// <param name="containingType">
        /// Type whose nested types are being inspected.
        /// </param>
        /// <param name="queryBuilderContract">
        /// Root query builder contract.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract.
        /// </param>
        /// <param name="surfaces">
        /// Collection receiving discovered root feature surfaces.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel nested type traversal.
        /// </param>
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
        /// Determines whether the specified type represents a generic root query feature surface.
        /// </summary>
        /// <param name="type">
        /// Type being inspected.
        /// </param>
        /// <param name="queryBuilderContract">
        /// Root query builder contract.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the type represents a root feature surface;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsRootFeatureSurface(INamedTypeSymbol type, INamedTypeSymbol queryBuilderContract, INamedTypeSymbol profileContract)
        {
            if (type.TypeKind != TypeKind.Interface || !type.IsGenericType || type.Arity != 1)
                return false;

            if (SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, queryBuilderContract.OriginalDefinition))
                return false;

            var implementsQueryBuilder = type.AllInterfaces.Any(implementedInterface => SymbolEqualityComparer.Default.Equals(implementedInterface.OriginalDefinition, queryBuilderContract.OriginalDefinition));

            if (!implementsQueryBuilder)
                return false;

            return HasFeatureConstraint(type.OriginalDefinition, profileContract);
        }

        /// <summary>
        /// Determines whether the specified constructed interface represents a root query feature surface.
        /// </summary>
        /// <param name="surface">
        /// Constructed interface being inspected.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to identify feature constraints.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the interface represents a root query feature surface;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsConstructedRootFeatureSurface(INamedTypeSymbol surface, INamedTypeSymbol profileContract)
        {
            if (surface.TypeKind != TypeKind.Interface || !surface.IsGenericType || surface.Arity != 1)
                return false;

            return HasFeatureConstraint(surface.OriginalDefinition, profileContract);
        }

        /// <summary>
        /// Determines whether a generic root surface declares the provider profile contract
        /// together with at least one additional feature constraint.
        /// </summary>
        /// <param name="surfaceDefinition">
        /// Generic root surface definition being inspected.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to identify feature constraints.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the surface declares a feature constraint;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool HasFeatureConstraint(INamedTypeSymbol surfaceDefinition, INamedTypeSymbol profileContract)
        {
            if (surfaceDefinition.TypeParameters.Length != 1)
                return false;

            var typeParameter = surfaceDefinition.TypeParameters[0];

            var hasProfileConstraint = typeParameter.ConstraintTypes.Any(constraint => SymbolEqualityComparer.Default.Equals(constraint.OriginalDefinition, profileContract.OriginalDefinition));

            if (!hasProfileConstraint)
                return false;

            return typeParameter.ConstraintTypes.Any(constraint => !SymbolEqualityComparer.Default.Equals(constraint.OriginalDefinition, profileContract.OriginalDefinition));
        }

        /// <summary>
        /// Builds provider profile surface models using the root feature surfaces
        /// compatible with each discovered provider profile.
        /// </summary>
        /// <param name="compilation">
        /// Compilation being processed by the source generator.
        /// </param>
        /// <param name="profiles">
        /// Concrete provider profiles discovered by the generator.
        /// </param>
        /// <param name="surfaceDefinitions">
        /// Generic root feature surface definitions discovered by the generator.
        /// </param>
        /// <returns>
        /// Immutable collection containing the resolved surface model for every profile.
        /// </returns>
        private static ImmutableArray<ProfileSurfaceModel> BuildProfileSurfaceModels(Compilation compilation, ImmutableArray<INamedTypeSymbol> profiles, ImmutableArray<INamedTypeSymbol> surfaceDefinitions)
        {
            return profiles
                .Select(profile => new ProfileSurfaceModel(
                    profile,
                    ResolveProfileSurfaces(profile, surfaceDefinitions),
                    SymbolEqualityComparer.Default.Equals(profile.ContainingAssembly, compilation.Assembly)))
                .OrderBy(static model => model.Profile.ToDisplayString(), StringComparer.Ordinal)
                .ToImmutableArray();
        }

        /// <summary>
        /// Resolves the root query feature surfaces compatible with the specified provider profile.
        /// </summary>
        /// <param name="profile">
        /// Provider profile being resolved.
        /// </param>
        /// <param name="surfaceDefinitions">
        /// Generic root feature surface definitions available to the compilation.
        /// </param>
        /// <returns>
        /// Root feature surfaces compatible with the provider profile.
        /// </returns>
        private static IReadOnlyList<INamedTypeSymbol> ResolveProfileSurfaces(INamedTypeSymbol profile, ImmutableArray<INamedTypeSymbol> surfaceDefinitions)
        {
            var compatibleSurfaces = new List<INamedTypeSymbol>();

            foreach (var surfaceDefinition in surfaceDefinitions)
            {
                if (!SatisfiesSurfaceConstraints(profile, surfaceDefinition))
                    continue;

                compatibleSurfaces.Add(surfaceDefinition.Construct(profile));
            }

            return RemoveInheritedSurfaces(compatibleSurfaces);
        }

        /// <summary>
        /// Determines whether the specified provider profile satisfies the generic
        /// constraints declared by a root query feature surface.
        /// </summary>
        /// <param name="profile">
        /// Provider profile being evaluated.
        /// </param>
        /// <param name="surfaceDefinition">
        /// Generic root feature surface definition.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when every declared surface constraint is satisfied;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool SatisfiesSurfaceConstraints(INamedTypeSymbol profile, INamedTypeSymbol surfaceDefinition)
        {
            if (surfaceDefinition.TypeParameters.Length != 1)
                return false;

            var typeParameter = surfaceDefinition.TypeParameters[0];

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
        /// <param name="profile">
        /// Provider profile being inspected.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the profile exposes a public parameterless constructor;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool HasPublicParameterlessConstructor(INamedTypeSymbol profile)
        {
            return profile.InstanceConstructors.Any(constructor => constructor.Parameters.Length == 0 && constructor.DeclaredAccessibility == Accessibility.Public);
        }

        /// <summary>
        /// Determines whether a provider profile satisfies the specified type constraint.
        /// </summary>
        /// <param name="profile">
        /// Provider profile being evaluated.
        /// </param>
        /// <param name="constraint">
        /// Constraint type that must be satisfied.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the profile satisfies the constraint;
        /// otherwise, <see langword="false"/>.
        /// </returns>
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
        /// Removes root feature surfaces already inherited by another compatible surface.
        /// </summary>
        /// <param name="surfaces">
        /// Compatible root feature surfaces being reduced.
        /// </param>
        /// <returns>
        /// Minimal root feature surface set required by the generated engine.
        /// </returns>
        private static IReadOnlyList<INamedTypeSymbol> RemoveInheritedSurfaces(IReadOnlyList<INamedTypeSymbol> surfaces)
        {
            return surfaces
                .Where(candidate =>
                    !surfaces.Any(other =>
                        !SymbolEqualityComparer.Default.Equals(candidate, other) &&
                        other.AllInterfaces.Any(inheritedSurface => SymbolEqualityComparer.Default.Equals(inheritedSurface, candidate))))
                .OrderBy(static surface => surface.ToDisplayString(), StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>
        /// Generates manual root query builder extensions for locally declared provider profiles.
        /// </summary>
        /// <param name="context">
        /// Source production context used to add generated sources.
        /// </param>
        /// <param name="model">
        /// Complete generator model associated with the current compilation.
        /// </param>
        private static void GenerateManualQueryBuilderExtensions(SourceProductionContext context, GenerationModel model)
        {
            if (model.ProfileContract is null)
                return;

            foreach (var profileModel in model.Profiles.Where(static profileModel => profileModel.IsLocal))
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                if (profileModel.Surfaces.Count == 0)
                    continue;

                var source = GenerateManualQueryBuilderExtensions(profileModel, model.ProfileContract);

                context.AddSource($"{GetExtensionClassName(profileModel.Profile)}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        /// <summary>
        /// Generates the manual root query builder extensions associated with the specified profile.
        /// </summary>
        /// <param name="model">
        /// Provider profile surface model used to generate the extensions.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to delimit feature surface inheritance.
        /// </param>
        /// <returns>
        /// Generated source containing root query builder extensions.
        /// </returns>
        private static string GenerateManualQueryBuilderExtensions(ProfileSurfaceModel model, INamedTypeSymbol profileContract)
        {
            var profileType = model.Profile.ToDisplayString(GeneratedTypeDisplayFormat);

            var extensionClassName = GetExtensionClassName(model.Profile);

            var methods = GetManualSurfaceMethods(model.Surfaces, profileContract);

            var source = new StringBuilder();

            source.AppendLine("// <auto-generated />");
            source.AppendLine("#nullable enable");
            source.AppendLine();
            source.AppendLine($"namespace {GeneratedExtensionsNamespace}");
            source.AppendLine("{");
            source.AppendLine("    /// <summary>");
            source.AppendLine("    /// Provides generated root query feature extensions for");
            source.AppendLine($"    /// <see cref=\"{profileType}\"/>.");
            source.AppendLine("    /// </summary>");
            source.AppendLine($"    public static class {extensionClassName}");
            source.AppendLine("    {");

            for (var index = 0; index < methods.Count; index++)
            {
                AppendManualExtension(source, model, methods[index]);

                if (index < methods.Count - 1)
                    source.AppendLine();
            }

            source.AppendLine("    }");
            source.AppendLine("}");

            return source.ToString();
        }

        /// <summary>
        /// Appends a generated manual root query feature extension.
        /// </summary>
        /// <param name="source">
        /// Source builder receiving generated code.
        /// </param>
        /// <param name="model">
        /// Provider profile surface model associated with the generated extension.
        /// </param>
        /// <param name="method">
        /// Root feature surface method being exposed as an extension.
        /// </param>
        private static void AppendManualExtension(StringBuilder source, ProfileSurfaceModel model, IMethodSymbol method)
        {
            var profileType = model.Profile.ToDisplayString(GeneratedTypeDisplayFormat);

            var genericParameters = BuildGenericParameterList(method);

            var returnType = IsRootSurfaceReturn(method.ReturnType, model)
                ? $"global::TinyBlueWhale.EngineQuery.Abstractions.Interfaces.IQueryBuilder<{profileType}>"
                : method.ReturnType.ToDisplayString(GeneratedTypeDisplayFormat);

            var parameterDeclarations = method.Parameters.Select(BuildParameterDeclaration).ToList();

            parameterDeclarations.Insert(0, $"this global::TinyBlueWhale.EngineQuery.Abstractions.Interfaces.IQueryBuilder<{profileType}> queryBuilder");

            var parameters = string.Join(", ", parameterDeclarations);

            var arguments = string.Join(", ", method.Parameters.Select(BuildArgument));

            source.AppendLine("        /// <summary>");
            source.AppendLine($"        /// Provides the generated {method.Name} root query feature operation.");
            source.AppendLine("        /// </summary>");

            foreach (var typeParameter in method.TypeParameters)
            {
                source.AppendLine($"        /// <typeparam name=\"{typeParameter.Name}\">");
                source.AppendLine("        /// Generic type parameter associated with the feature operation.");
                source.AppendLine("        /// </typeparam>");
            }

            source.AppendLine("        /// <param name=\"queryBuilder\">");
            source.AppendLine("        /// Current root query builder instance.");
            source.AppendLine("        /// </param>");

            foreach (var parameter in method.Parameters)
            {
                source.AppendLine($"        /// <param name=\"{parameter.Name}\">");
                source.AppendLine("        /// Feature operation argument.");
                source.AppendLine("        /// </param>");
            }

            source.AppendLine("        /// <returns>");
            source.AppendLine("        /// Result produced by the root query feature operation.");
            source.AppendLine("        /// </returns>");
            source.AppendLine($"        public static {returnType} {method.Name}{genericParameters}({parameters})");

            AppendGenericConstraints(source, method, "            ");

            source.AppendLine("        {");
            source.AppendLine("            global::System.ArgumentNullException.ThrowIfNull(queryBuilder);");

            if (method.ReturnsVoid)
                source.AppendLine($"            queryBuilder.{method.Name}{genericParameters}({arguments});");
            else
                source.AppendLine($"            return queryBuilder.{method.Name}{genericParameters}({arguments});");

            source.AppendLine("        }");
        }

        /// <summary>
        /// Gets the root feature methods exposed by the resolved surface set for manual consumption.
        /// </summary>
        /// <param name="surfaces">
        /// Resolved root feature surfaces associated with a provider profile.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to delimit feature surface inheritance.
        /// </param>
        /// <returns>
        /// De-duplicated method collection used to generate manual extensions.
        /// </returns>
        private static IReadOnlyList<IMethodSymbol> GetManualSurfaceMethods(IReadOnlyList<INamedTypeSymbol> surfaces, INamedTypeSymbol profileContract)
        {
            var methods = new List<IMethodSymbol>();

            foreach (var surface in surfaces)
                AddSurfaceMethodsForManual(surface, profileContract, methods);

            return methods
                .GroupBy(BuildManualMethodIdentity, StringComparer.Ordinal)
                .Select(static group => group.First())
                .OrderBy(static method => method.Name, StringComparer.Ordinal)
                .ThenBy(static method => method.Arity)
                .ThenBy(static method => method.Parameters.Length)
                .ToList();
        }

        /// <summary>
        /// Adds methods declared by the specified surface and inherited root feature surfaces.
        /// </summary>
        /// <param name="surface">
        /// Root feature surface being inspected.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to delimit feature surface inheritance.
        /// </param>
        /// <param name="methods">
        /// Collection receiving discovered methods.
        /// </param>
        private static void AddSurfaceMethodsForManual(INamedTypeSymbol surface, INamedTypeSymbol profileContract, ICollection<IMethodSymbol> methods)
        {
            foreach (var method in GetDeclaredSurfaceMethods(surface))
                methods.Add(method);

            foreach (var inheritedSurface in surface.Interfaces)
            {
                if (!IsConstructedRootFeatureSurface(inheritedSurface, profileContract))
                    continue;

                AddSurfaceMethodsForManual(inheritedSurface, profileContract, methods);
            }
        }

        /// <summary>
        /// Generates the query engine surface and concrete wrapper associated with
        /// every discovered database provider profile.
        /// </summary>
        /// <param name="context">
        /// Source production context used to add generated sources.
        /// </param>
        /// <param name="model">
        /// Complete generator model associated with the dependency injection compilation.
        /// </param>
        private static void GenerateQueryEngineSurfaces(SourceProductionContext context, GenerationModel model)
        {
            if (model.ProfileContract is null)
                return;

            foreach (var profileModel in model.Profiles)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var source = GenerateQueryEngineSurface(profileModel, model.ProfileContract);

                context.AddSource($"{GetEngineName(profileModel.Profile)}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        /// <summary>
        /// Generates the query engine surface and implementation associated with
        /// the specified database provider profile.
        /// </summary>
        /// <param name="model">
        /// Provider profile surface model used to generate the engine.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to delimit feature surface inheritance.
        /// </param>
        /// <returns>
        /// Generated C# source containing the strongly typed query engine interface
        /// and concrete implementation.
        /// </returns>
        private static string GenerateQueryEngineSurface(ProfileSurfaceModel model, INamedTypeSymbol profileContract)
        {
            var profileType = model.Profile.ToDisplayString(GeneratedTypeDisplayFormat);

            var engineName = GetEngineName(model.Profile);

            var engineInterfaceName = $"I{engineName}";

            var source = new StringBuilder();

            source.AppendLine("// <auto-generated />");
            source.AppendLine("#nullable enable");
            source.AppendLine();
            source.AppendLine($"namespace {GeneratedNamespace}");
            source.AppendLine("{");

            AppendEngineInterface(source, profileType, engineInterfaceName, model.Surfaces);

            source.AppendLine();

            AppendEngineImplementation(source, model, profileContract, profileType, engineName, engineInterfaceName);

            source.AppendLine("}");

            return source.ToString();
        }

        /// <summary>
        /// Appends the generated public query engine interface associated with a provider profile.
        /// </summary>
        /// <param name="source">
        /// Source builder receiving generated code.
        /// </param>
        /// <param name="profileType">
        /// Fully qualified provider profile type.
        /// </param>
        /// <param name="engineInterfaceName">
        /// Generated query engine interface name.
        /// </param>
        /// <param name="featureSurfaces">
        /// Resolved root feature surfaces associated with the provider profile.
        /// </param>
        private static void AppendEngineInterface(StringBuilder source, string profileType, string engineInterfaceName, IReadOnlyList<INamedTypeSymbol> featureSurfaces)
        {
            var surfaces = new List<string>
            {
                $"global::{QueryEngineInterface}<{profileType}>"
            };

            surfaces.AddRange(featureSurfaces.Select(surface => surface.ToDisplayString(GeneratedTypeDisplayFormat)));            

            source.AppendLine("    /// <summary>");
            source.AppendLine("    /// Represents the generated query engine surface associated with");
            source.AppendLine($"    /// <see cref=\"{profileType}\"/>.");
            source.AppendLine("    /// </summary>");
            source.AppendLine($"    public interface {engineInterfaceName} :");

            for (var index = 0; index < surfaces.Count; index++)
            {
                source.Append("        ");
                source.Append(surfaces[index]);

                if (index < surfaces.Count - 1)
                    source.Append(',');

                source.AppendLine();
            }

            source.AppendLine("    {");
            source.AppendLine("    }");
        }

        /// <summary>
        /// Appends the generated query engine implementation associated with a provider profile.
        /// </summary>
        /// <param name="source">
        /// Source builder receiving generated code.
        /// </param>
        /// <param name="model">
        /// Provider profile surface model used to generate the implementation.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to delimit feature surface inheritance.
        /// </param>
        /// <param name="profileType">
        /// Fully qualified provider profile type.
        /// </param>
        /// <param name="engineName">
        /// Generated concrete query engine name.
        /// </param>
        /// <param name="engineInterfaceName">
        /// Generated query engine interface name.
        /// </param>
        private static void AppendEngineImplementation(
            StringBuilder source,
            ProfileSurfaceModel model,
            INamedTypeSymbol profileContract,
            string profileType,
            string engineName,
            string engineInterfaceName)
        {
            source.AppendLine("    /// <summary>");
            source.AppendLine("    /// Provides the generated query engine implementation associated with");
            source.AppendLine($"    /// <see cref=\"{profileType}\"/>.");
            source.AppendLine("    /// </summary>");
            source.AppendLine($"    internal sealed class {engineName}(global::TinyBlueWhale.EngineQuery.Core.QueryBuilding.QueryBuilder<{profileType}> queryBuilder) :");
            source.AppendLine($"        global::TinyBlueWhale.EngineQuery.DependencyInjection.QueryEngine<{profileType}>(queryBuilder),");
            source.AppendLine($"        {engineInterfaceName}");
            source.AppendLine("    {");

            var surfaceMethods = GetDependencyInjectionSurfaceMethods(model.Surfaces, profileContract);

            for (var index = 0; index < surfaceMethods.Count; index++)
            {
                AppendDependencyInjectionSurfaceMethod(source, model, surfaceMethods[index]);

                if (index < surfaceMethods.Count - 1)
                    source.AppendLine();
            }

            source.AppendLine("    }");
        }

        /// <summary>
        /// Gets every interface method that must be explicitly implemented by the generated
        /// query engine, including inherited root feature surface contracts.
        /// </summary>
        /// <param name="surfaces">
        /// Resolved root feature surfaces associated with a provider profile.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to delimit feature surface inheritance.
        /// </param>
        /// <returns>
        /// Surface method models requiring explicit forwarding implementations.
        /// </returns>
        private static IReadOnlyList<SurfaceMethodModel> GetDependencyInjectionSurfaceMethods(IReadOnlyList<INamedTypeSymbol> surfaces, INamedTypeSymbol profileContract)
        {
            var methods = new List<SurfaceMethodModel>();

            foreach (var surface in surfaces)
                AddDependencyInjectionSurfaceMethods(surface, profileContract, methods);

            return methods
                .GroupBy(BuildDependencyInjectionMethodIdentity, StringComparer.Ordinal)
                .Select(static group => group.First())
                .OrderBy(static method => method.Surface.ToDisplayString(), StringComparer.Ordinal)
                .ThenBy(static method => method.Method.Name, StringComparer.Ordinal)
                .ThenBy(static method => method.Method.Arity)
                .ThenBy(static method => method.Method.Parameters.Length)
                .ToList();
        }

        /// <summary>
        /// Adds methods declared by the specified root feature surface and inherited root feature contracts.
        /// </summary>
        /// <param name="surface">
        /// Root feature surface being inspected.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract used to delimit feature surface inheritance.
        /// </param>
        /// <param name="methods">
        /// Collection receiving interface method models.
        /// </param>
        private static void AddDependencyInjectionSurfaceMethods(INamedTypeSymbol surface, INamedTypeSymbol profileContract, ICollection<SurfaceMethodModel> methods)
        {
            foreach (var method in GetDeclaredSurfaceMethods(surface))
                methods.Add(new SurfaceMethodModel(surface, method));

            foreach (var inheritedSurface in surface.Interfaces)
            {
                if (!IsConstructedRootFeatureSurface(inheritedSurface, profileContract))
                    continue;

                AddDependencyInjectionSurfaceMethods(inheritedSurface, profileContract, methods);
            }
        }

        /// <summary>
        /// Appends an explicit root feature surface implementation to a generated query engine.
        /// </summary>
        /// <param name="source">
        /// Source builder receiving generated code.
        /// </param>
        /// <param name="model">
        /// Provider profile surface model associated with the generated engine.
        /// </param>
        /// <param name="surfaceMethod">
        /// Surface method being implemented.
        /// </param>
        private static void AppendDependencyInjectionSurfaceMethod(StringBuilder source, ProfileSurfaceModel model, SurfaceMethodModel surfaceMethod)
        {
            var surfaceType = surfaceMethod.Surface.ToDisplayString(GeneratedTypeDisplayFormat);

            var method = surfaceMethod.Method;

            var returnType = method.ReturnType.ToDisplayString(GeneratedTypeDisplayFormat);

            var genericParameters = BuildGenericParameterList(method);

            var parameters = string.Join(", ", method.Parameters.Select(BuildParameterDeclaration));

            var arguments = string.Join(", ", method.Parameters.Select(BuildArgument));

            var extensionType = $"global::{GeneratedExtensionsNamespace}.{GetExtensionClassName(model.Profile)}";

            source.AppendLine("        /// <inheritdoc />");
            source.AppendLine($"        {returnType} {surfaceType}.{method.Name}{genericParameters}({parameters})");

            AppendGenericConstraints(source, method, "            ");

            source.AppendLine("        {");

            if (IsRootSurfaceReturn(method.ReturnType, model))
            {
                source.AppendLine($"            {extensionType}.{method.Name}{genericParameters}(_innerQueryBuilder{BuildForwardedArgumentSuffix(arguments)});");
                source.AppendLine("            return this;");
            }
            else if (method.ReturnsVoid)
            {
                source.AppendLine($"            {extensionType}.{method.Name}{genericParameters}(_innerQueryBuilder{BuildForwardedArgumentSuffix(arguments)});");
            }
            else
            {
                source.AppendLine($"            return {extensionType}.{method.Name}{genericParameters}(_innerQueryBuilder{BuildForwardedArgumentSuffix(arguments)});");
            }

            source.AppendLine("        }");
        }

        /// <summary>
        /// Gets ordinary instance methods declared directly by the specified feature surface.
        /// </summary>
        /// <param name="surface">
        /// Feature surface being inspected.
        /// </param>
        /// <returns>
        /// Methods declared directly by the surface.
        /// </returns>
        private static IReadOnlyList<IMethodSymbol> GetDeclaredSurfaceMethods(INamedTypeSymbol surface)
        {
            return surface
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(static method => method.MethodKind == MethodKind.Ordinary && !method.IsStatic)
                .OrderBy(static method => method.Name, StringComparer.Ordinal)
                .ThenBy(static method => method.Arity)
                .ThenBy(static method => method.Parameters.Length)
                .ToList();
        }

        /// <summary>
        /// Determines whether the specified return type represents one of the root
        /// feature surfaces resolved for the provider profile.
        /// </summary>
        /// <param name="returnType">
        /// Method return type being inspected.
        /// </param>
        /// <param name="model">
        /// Provider profile surface model associated with the method.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the return type represents a root feature surface;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsRootSurfaceReturn(ITypeSymbol returnType, ProfileSurfaceModel model)
        {
            if (returnType is not INamedTypeSymbol namedReturnType)
                return false;

            foreach (var surface in model.Surfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(surface, namedReturnType))
                    return true;

                if (surface.AllInterfaces.Any(inheritedSurface => SymbolEqualityComparer.Default.Equals(inheritedSurface, namedReturnType)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Builds an identity used to de-duplicate manual extension methods.
        /// </summary>
        /// <param name="method">
        /// Method whose identity is being generated.
        /// </param>
        /// <returns>
        /// Stable method identity.
        /// </returns>
        private static string BuildManualMethodIdentity(IMethodSymbol method)
        {
            return $"{method.Name}`{method.Arity}({string.Join(",", method.Parameters.Select(parameter => $"{parameter.RefKind}:{parameter.Type.ToDisplayString(GeneratedTypeDisplayFormat)}"))})";
        }

        /// <summary>
        /// Builds an identity used to de-duplicate dependency injection surface implementations.
        /// </summary>
        /// <param name="model">
        /// Surface method model whose identity is being generated.
        /// </param>
        /// <returns>
        /// Stable surface method identity.
        /// </returns>
        private static string BuildDependencyInjectionMethodIdentity(SurfaceMethodModel model)
        {
            return $"{model.Surface.ToDisplayString(GeneratedTypeDisplayFormat)}::{BuildManualMethodIdentity(model.Method)}";
        }

        /// <summary>
        /// Builds the generic parameter list associated with the specified method.
        /// </summary>
        /// <param name="method">
        /// Method whose generic parameters are being rendered.
        /// </param>
        /// <returns>
        /// Generic parameter list or an empty string when the method is not generic.
        /// </returns>
        private static string BuildGenericParameterList(IMethodSymbol method)
        {
            if (method.TypeParameters.Length == 0)
                return string.Empty;

            return $"<{string.Join(", ", method.TypeParameters.Select(static parameter => parameter.Name))}>";
        }

        /// <summary>
        /// Appends generic type parameter constraints declared by the specified method.
        /// </summary>
        /// <param name="source">
        /// Source builder receiving generated constraints.
        /// </param>
        /// <param name="method">
        /// Method whose generic constraints are being rendered.
        /// </param>
        /// <param name="indentation">
        /// Indentation applied to generated constraint clauses.
        /// </param>
        private static void AppendGenericConstraints(StringBuilder source, IMethodSymbol method, string indentation)
        {
            foreach (var typeParameter in method.TypeParameters)
            {
                var constraints = BuildGenericConstraints(typeParameter);

                if (constraints.Count == 0)
                    continue;

                source.Append(indentation);
                source.Append("where ");
                source.Append(typeParameter.Name);
                source.Append(" : ");
                source.AppendLine(string.Join(", ", constraints));
            }
        }

        /// <summary>
        /// Builds the generic constraints associated with the specified type parameter.
        /// </summary>
        /// <param name="typeParameter">
        /// Generic type parameter being inspected.
        /// </param>
        /// <returns>
        /// Collection containing rendered generic constraints.
        /// </returns>
        private static IReadOnlyList<string> BuildGenericConstraints(ITypeParameterSymbol typeParameter)
        {
            var constraints = new List<string>();

            if (typeParameter.HasUnmanagedTypeConstraint)
                constraints.Add("unmanaged");
            else if (typeParameter.HasValueTypeConstraint)
                constraints.Add("struct");
            else if (typeParameter.HasReferenceTypeConstraint)
                constraints.Add(typeParameter.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated ? "class?" : "class");
            else if (typeParameter.HasNotNullConstraint)
                constraints.Add("notnull");

            constraints.AddRange(typeParameter.ConstraintTypes.Select(constraint => constraint.ToDisplayString(GeneratedTypeDisplayFormat)));

            if (typeParameter.HasConstructorConstraint)
                constraints.Add("new()");

            return constraints;
        }

        /// <summary>
        /// Builds the source representation of a generated method parameter.
        /// </summary>
        /// <param name="parameter">
        /// Parameter being rendered.
        /// </param>
        /// <returns>
        /// C# parameter declaration.
        /// </returns>
        private static string BuildParameterDeclaration(IParameterSymbol parameter)
        {
            var modifier = GetRefModifier(parameter.RefKind);

            var type = parameter.Type.ToDisplayString(GeneratedTypeDisplayFormat);

            var defaultValue = BuildDefaultValue(parameter);

            return $"{modifier}{type} {EscapeIdentifier(parameter.Name)}{defaultValue}";
        }

        /// <summary>
        /// Builds the forwarding invocation argument associated with a method parameter.
        /// </summary>
        /// <param name="parameter">
        /// Parameter being forwarded.
        /// </param>
        /// <returns>
        /// C# invocation argument.
        /// </returns>
        private static string BuildArgument(IParameterSymbol parameter)
        {
            return $"{GetRefModifier(parameter.RefKind)}{EscapeIdentifier(parameter.Name)}";
        }

        /// <summary>
        /// Builds the suffix used to append forwarded arguments after the root query builder argument.
        /// </summary>
        /// <param name="arguments">
        /// Rendered forwarded arguments.
        /// </param>
        /// <returns>
        /// Empty string when no arguments exist; otherwise a comma-prefixed argument list.
        /// </returns>
        private static string BuildForwardedArgumentSuffix(string arguments)
        {
            return string.IsNullOrWhiteSpace(arguments) ? string.Empty : $", {arguments}";
        }

        /// <summary>
        /// Gets the C# parameter modifier associated with the specified reference kind.
        /// </summary>
        /// <param name="refKind">
        /// Parameter reference kind.
        /// </param>
        /// <returns>
        /// C# reference modifier or an empty string.
        /// </returns>
        private static string GetRefModifier(RefKind refKind)
        {
            return refKind switch
            {
                RefKind.Ref => "ref ",
                RefKind.Out => "out ",
                RefKind.In => "in ",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Builds the default value declaration associated with an optional parameter.
        /// </summary>
        /// <param name="parameter">
        /// Parameter being inspected.
        /// </param>
        /// <returns>
        /// Default value declaration or an empty string when no explicit default exists.
        /// </returns>
        private static string BuildDefaultValue(IParameterSymbol parameter)
        {
            if (!parameter.HasExplicitDefaultValue)
                return string.Empty;

            if (parameter.ExplicitDefaultValue is null)
                return " = null";

            return parameter.ExplicitDefaultValue switch
            {
                bool value => value ? " = true" : " = false",
                string value => $" = \"{EscapeString(value)}\"",
                char value => $" = '{EscapeChar(value)}'",
                float value => $" = {value.ToString(System.Globalization.CultureInfo.InvariantCulture)}F",
                double value => $" = {value.ToString(System.Globalization.CultureInfo.InvariantCulture)}D",
                decimal value => $" = {value.ToString(System.Globalization.CultureInfo.InvariantCulture)}M",
                _ => $" = {Convert.ToString(parameter.ExplicitDefaultValue, System.Globalization.CultureInfo.InvariantCulture)}"
            };
        }

        /// <summary>
        /// Escapes a C# identifier when it conflicts with a language keyword.
        /// </summary>
        /// <param name="identifier">
        /// Identifier being escaped.
        /// </param>
        /// <returns>
        /// Escaped C# identifier.
        /// </returns>
        private static string EscapeIdentifier(string identifier)
        {
            return Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetKeywordKind(identifier) != Microsoft.CodeAnalysis.CSharp.SyntaxKind.None
                ? $"@{identifier}"
                : identifier;
        }

        /// <summary>
        /// Escapes a string value for inclusion in generated C# source.
        /// </summary>
        /// <param name="value">
        /// String value being escaped.
        /// </param>
        /// <returns>
        /// Escaped string value.
        /// </returns>
        private static string EscapeString(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }

        /// <summary>
        /// Escapes a character value for inclusion in generated C# source.
        /// </summary>
        /// <param name="value">
        /// Character value being escaped.
        /// </param>
        /// <returns>
        /// Escaped character value.
        /// </returns>
        private static string EscapeChar(char value)
        {
            return value switch
            {
                '\\' => "\\\\",
                '\'' => "\\'",
                '\r' => "\\r",
                '\n' => "\\n",
                '\t' => "\\t",
                _ => value.ToString()
            };
        }

        /// <summary>
        /// Generates dependency injection registration for all discovered provider profile factories.
        /// </summary>
        /// <param name="context">
        /// Source production context used to add generated source.
        /// </param>
        /// <param name="model">
        /// Complete generator model associated with the dependency injection compilation.
        /// </param>
        private static void GenerateDependencyInjectionRegistration(SourceProductionContext context, GenerationModel model)
        {
            var source = new StringBuilder();

            source.AppendLine("// <auto-generated />");
            source.AppendLine("#nullable enable");
            source.AppendLine();
            source.AppendLine("namespace TinyBlueWhale.EngineQuery.DependencyInjection.Extensions");
            source.AppendLine("{");
            source.AppendLine("    /// <summary>");
            source.AppendLine("    /// Provides generated EngineQuery dependency injection registrations.");
            source.AppendLine("    /// </summary>");
            source.AppendLine("    public static partial class ServiceCollectionExtensions");
            source.AppendLine("    {");
            source.AppendLine("        /// <summary>");
            source.AppendLine("        /// Registers generated strongly typed query engine factories.");
            source.AppendLine("        /// </summary>");
            source.AppendLine("        /// <param name=\"services\">");
            source.AppendLine("        /// Service collection receiving generated query engine factories.");
            source.AppendLine("        /// </param>");
            source.AppendLine("        static partial void RegisterGeneratedQueryEngineFactories(global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
            source.AppendLine("        {");

            foreach (var profileModel in model.Profiles)
                AppendFactoryRegistration(source, profileModel.Profile);

            source.AppendLine("        }");
            source.AppendLine("    }");
            source.AppendLine("}");

            context.AddSource("ServiceCollectionExtensions.QueryEngineFactories.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
        }

        /// <summary>
        /// Appends the dependency injection registrations associated with a provider profile.
        /// </summary>
        /// <param name="source">
        /// Source builder receiving generated registration code.
        /// </param>
        /// <param name="profile">
        /// Provider profile whose query engine factory and direct engine surface are being registered.
        /// </param>
        private static void AppendFactoryRegistration(StringBuilder source, INamedTypeSymbol profile)
        {
            var profileType = profile.ToDisplayString(GeneratedTypeDisplayFormat);

            var engineName = GetEngineName(profile);

            var engineInterfaceName = $"global::{GeneratedNamespace}.I{engineName}";

            var engineImplementationName = $"global::{GeneratedNamespace}.{engineName}";

            var factoryInterface = $"global::TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces.IQueryEngineFactory<{profileType}, {engineInterfaceName}>";

            var factoryImplementation = $"global::TinyBlueWhale.EngineQuery.DependencyInjection.Factories.QueryEngineFactory<{profileType}, {engineInterfaceName}>";

            source.AppendLine();
            source.AppendLine($"            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<{factoryInterface}>(services, serviceProvider => new {factoryImplementation}(serviceProvider, global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetServices<global::TinyBlueWhale.EngineQuery.DependencyInjection.Configuration.EngineQueryRegistration>(serviceProvider), queryBuilder => new {engineImplementationName}(queryBuilder)));");
            source.AppendLine();
            source.AppendLine($"            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<{engineInterfaceName}>(services, serviceProvider => global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<{factoryInterface}>(serviceProvider).Create());");
        }

        /// <summary>
        /// Gets the generated query engine name associated with the specified provider profile.
        /// </summary>
        /// <param name="profile">
        /// Provider profile whose engine name is being generated.
        /// </param>
        /// <returns>
        /// Generated query engine name without the profile suffix.
        /// </returns>
        private static string GetEngineName(INamedTypeSymbol profile)
        {
            return $"{GetProfileBaseName(profile)}QueryEngine";
        }

        /// <summary>
        /// Gets the generated root query builder extension class name associated with
        /// the specified provider profile.
        /// </summary>
        /// <param name="profile">
        /// Provider profile whose extension class name is being generated.
        /// </param>
        /// <returns>
        /// Generated extension class name.
        /// </returns>
        private static string GetExtensionClassName(INamedTypeSymbol profile)
        {
            return $"{GetProfileBaseName(profile)}QueryBuilderExtensions";
        }

        /// <summary>
        /// Gets the provider profile name without the conventional Profile suffix.
        /// </summary>
        /// <param name="profile">
        /// Provider profile whose base name is being generated.
        /// </param>
        /// <returns>
        /// Provider profile base name.
        /// </returns>
        private static string GetProfileBaseName(INamedTypeSymbol profile)
        {
            const string profileSuffix = "Profile";

            var profileName = profile.Name;

            if (profileName.EndsWith(profileSuffix, StringComparison.Ordinal))
                profileName = profileName.Substring(0, profileName.Length - profileSuffix.Length);

            return profileName;
        }

        /// <summary>
        /// Represents the complete generation state associated with a compilation.
        /// </summary>
        private sealed class GenerationModel
        {
            /// <summary>
            /// Initializes a new generation model.
            /// </summary>
            /// <param name="profiles">
            /// Provider profile surface models discovered by the generator.
            /// </param>
            /// <param name="profileContract">
            /// Database provider profile contract used during structural feature discovery.
            /// </param>
            /// <param name="supportsQueryEngineGeneration">
            /// Indicates whether the current compilation contains dependency injection query engine contracts.
            /// </param>
            public GenerationModel(ImmutableArray<ProfileSurfaceModel> profiles, INamedTypeSymbol? profileContract, bool supportsQueryEngineGeneration)
            {
                Profiles = profiles;
                ProfileContract = profileContract;
                SupportsQueryEngineGeneration = supportsQueryEngineGeneration;
            }

            /// <summary>
            /// Gets the discovered provider profile surface models.
            /// </summary>
            public ImmutableArray<ProfileSurfaceModel> Profiles { get; }

            /// <summary>
            /// Gets the database provider profile contract used during structural feature discovery.
            /// </summary>
            public INamedTypeSymbol? ProfileContract { get; }

            /// <summary>
            /// Gets whether the current compilation supports query engine and dependency injection generation.
            /// </summary>
            public bool SupportsQueryEngineGeneration { get; }
        }

        /// <summary>
        /// Represents a provider profile together with the root query feature surfaces
        /// compatible with the profile.
        /// </summary>
        private sealed class ProfileSurfaceModel
        {
            /// <summary>
            /// Initializes a new provider profile surface model.
            /// </summary>
            /// <param name="profile">
            /// Database provider profile represented by the model.
            /// </param>
            /// <param name="surfaces">
            /// Root query feature surfaces compatible with the profile.
            /// </param>
            /// <param name="isLocal">
            /// Indicates whether the profile is declared by the current compilation.
            /// </param>
            public ProfileSurfaceModel(INamedTypeSymbol profile, IReadOnlyList<INamedTypeSymbol> surfaces, bool isLocal)
            {
                Profile = profile ?? throw new ArgumentNullException(nameof(profile));
                Surfaces = surfaces ?? throw new ArgumentNullException(nameof(surfaces));
                IsLocal = isLocal;
            }

            /// <summary>
            /// Gets the database provider profile represented by the model.
            /// </summary>
            public INamedTypeSymbol Profile { get; }

            /// <summary>
            /// Gets the root query feature surfaces compatible with the profile.
            /// </summary>
            public IReadOnlyList<INamedTypeSymbol> Surfaces { get; }

            /// <summary>
            /// Gets whether the provider profile is declared by the current compilation.
            /// </summary>
            public bool IsLocal { get; }
        }

        /// <summary>
        /// Represents a method declared by a specific root feature surface.
        /// </summary>
        private sealed class SurfaceMethodModel
        {
            /// <summary>
            /// Initializes a new surface method model.
            /// </summary>
            /// <param name="surface">
            /// Root feature surface declaring the method.
            /// </param>
            /// <param name="method">
            /// Method declared by the root feature surface.
            /// </param>
            public SurfaceMethodModel(INamedTypeSymbol surface, IMethodSymbol method)
            {
                Surface = surface ?? throw new ArgumentNullException(nameof(surface));
                Method = method ?? throw new ArgumentNullException(nameof(method));
            }

            /// <summary>
            /// Gets the root feature surface declaring the method.
            /// </summary>
            public INamedTypeSymbol Surface { get; }

            /// <summary>
            /// Gets the method declared by the root feature surface.
            /// </summary>
            public IMethodSymbol Method { get; }
        }
    }
}
