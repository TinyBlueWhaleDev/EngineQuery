using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TinyBlueWhale.EngineQuery.Generators
{
    /// <summary>
    /// Generates strongly typed EngineQuery feature extensions, query engine
    /// surfaces, implementations and dependency injection registrations from
    /// concrete database provider profiles.
    /// </summary>
    /// <remarks>
    /// Provider profiles act as the single source of truth for provider version and
    /// feature availability. Feature surfaces declare their requirements through
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

        private const string QueryCommandBuilderInterfaceMetadataName =
            "TinyBlueWhale.EngineQuery.Abstractions.Interfaces.IQueryCommandBuilder`2";

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
        private static GenerationModel BuildGenerationModel(Compilation compilation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var profileContract = compilation.GetTypeByMetadataName(DatabaseProviderProfileInterface);

            if (profileContract is null)
                return new GenerationModel(ImmutableArray<ProfileSurfaceModel>.Empty, null, false);

            var profiles = DiscoverProfiles(compilation, profileContract, cancellationToken);
            var rootSurfaceDefinitions = DiscoverRootFeatureSurfaces(compilation, profileContract, cancellationToken);
            var compositionSurfaceDefinitions = DiscoverCompositionFeatureSurfaces(compilation, profileContract, cancellationToken);
            var profileModels = BuildProfileSurfaceModels(compilation, profiles, rootSurfaceDefinitions, compositionSurfaceDefinitions, profileContract);
            var supportsQueryEngineGeneration = compilation.GetTypeByMetadataName(QueryEngineInterfaceMetadataName) is not null;

            return new GenerationModel(profileModels, profileContract, supportsQueryEngineGeneration);
        }

        /// <summary>
        /// Discovers concrete database provider profiles available through the current
        /// compilation and referenced EngineQuery assemblies.
        /// </summary>
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
        private static bool IsConcreteProfile(INamedTypeSymbol type, INamedTypeSymbol profileContract)
        {
            if (type.TypeKind != TypeKind.Class || type.IsAbstract)
                return false;

            return type.AllInterfaces.Any(implementedInterface => SymbolEqualityComparer.Default.Equals(implementedInterface, profileContract));
        }

        /// <summary>
        /// Discovers generic root query feature surfaces.
        /// </summary>
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

            var implementsQueryBuilder = type.AllInterfaces.Any(implementedInterface => SymbolEqualityComparer.Default.Equals(implementedInterface.OriginalDefinition, queryBuilderContract.OriginalDefinition));

            return implementsQueryBuilder && GetProfileTypeParameterIndex(type.OriginalDefinition, profileContract) >= 0;
        }

        /// <summary>
        /// Determines whether the specified constructed interface represents a root feature surface.
        /// </summary>
        private static bool IsConstructedRootFeatureSurface(INamedTypeSymbol surface, INamedTypeSymbol profileContract)
        {
            return surface.TypeKind == TypeKind.Interface &&
                   surface.IsGenericType &&
                   surface.Arity == 1 &&
                   GetProfileTypeParameterIndex(surface.OriginalDefinition, profileContract) >= 0;
        }

        /// <summary>
        /// Discovers generic query composition feature surfaces.
        /// </summary>
        private static ImmutableArray<INamedTypeSymbol> DiscoverCompositionFeatureSurfaces(Compilation compilation, INamedTypeSymbol profileContract, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryCommandBuilderContract = compilation.GetTypeByMetadataName(QueryCommandBuilderInterfaceMetadataName);

            if (queryCommandBuilderContract is null)
                return ImmutableArray<INamedTypeSymbol>.Empty;

            var surfaces = ImmutableArray.CreateBuilder<INamedTypeSymbol>();

            CollectCompositionFeatureSurfaces(compilation.Assembly.GlobalNamespace, queryCommandBuilderContract, profileContract, surfaces, cancellationToken);

            foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!assembly.Name.StartsWith("TinyBlueWhale.EngineQuery", StringComparison.Ordinal))
                    continue;

                CollectCompositionFeatureSurfaces(assembly.GlobalNamespace, queryCommandBuilderContract, profileContract, surfaces, cancellationToken);
            }

            return surfaces
                .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                .OrderBy(static surface => surface.ToDisplayString(), StringComparer.Ordinal)
                .ToImmutableArray();
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

            var implementsQueryCommandBuilder = type.AllInterfaces.Any(implementedInterface => SymbolEqualityComparer.Default.Equals(implementedInterface.OriginalDefinition, queryCommandBuilderContract.OriginalDefinition));

            return implementsQueryCommandBuilder && GetProfileTypeParameterIndex(type.OriginalDefinition, profileContract) >= 0;
        }

        /// <summary>
        /// Gets the index of the provider profile type parameter declared by a feature surface.
        /// </summary>
        private static int GetProfileTypeParameterIndex(INamedTypeSymbol surfaceDefinition, INamedTypeSymbol profileContract)
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
        /// Builds provider profile models using compatible root and composition surfaces.
        /// </summary>
        private static ImmutableArray<ProfileSurfaceModel> BuildProfileSurfaceModels(Compilation compilation, ImmutableArray<INamedTypeSymbol> profiles, ImmutableArray<INamedTypeSymbol> rootSurfaceDefinitions, ImmutableArray<INamedTypeSymbol> compositionSurfaceDefinitions, INamedTypeSymbol profileContract)
        {
            return profiles
                .Select(profile => new ProfileSurfaceModel(
                    profile,
                    ResolveRootProfileSurfaces(profile, rootSurfaceDefinitions, profileContract),
                    ResolveCompositionProfileSurfaces(profile, compositionSurfaceDefinitions, profileContract),
                    SymbolEqualityComparer.Default.Equals(profile.ContainingAssembly, compilation.Assembly)))
                .OrderBy(static model => model.Profile.ToDisplayString(), StringComparer.Ordinal)
                .ToImmutableArray();
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
            return surfaceDefinitions
                .Where(surface => SatisfiesSurfaceConstraints(profile, surface, profileContract))
                .Where(candidate => !surfaceDefinitions.Any(other =>
                    !SymbolEqualityComparer.Default.Equals(candidate, other) &&
                    SatisfiesSurfaceConstraints(profile, other, profileContract) &&
                    other.AllInterfaces.Any(inherited => SymbolEqualityComparer.Default.Equals(inherited.OriginalDefinition, candidate.OriginalDefinition))))
                .OrderBy(static surface => surface.ToDisplayString(), StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>
        /// Determines whether the specified provider profile satisfies the profile
        /// type parameter constraints declared by a feature surface.
        /// </summary>
        private static bool SatisfiesSurfaceConstraints(INamedTypeSymbol profile, INamedTypeSymbol surfaceDefinition, INamedTypeSymbol profileContract)
        {
            var profileParameterIndex = GetProfileTypeParameterIndex(surfaceDefinition, profileContract);

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
            return profile.InstanceConstructors.Any(constructor => constructor.Parameters.Length == 0 && constructor.DeclaredAccessibility == Accessibility.Public);
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
            return surfaces
                .Where(candidate =>
                    !surfaces.Any(other =>
                        !SymbolEqualityComparer.Default.Equals(candidate, other) &&
                        other.AllInterfaces.Any(inheritedSurface => SymbolEqualityComparer.Default.Equals(inheritedSurface, candidate))))
                .OrderBy(static surface => surface.ToDisplayString(), StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>
        /// Generates manual root and composition feature extensions for locally declared profiles.
        /// </summary>
        private static void GenerateManualQueryBuilderExtensions(SourceProductionContext context, GenerationModel model)
        {
            if (model.ProfileContract is null)
                return;

            foreach (var profileModel in model.Profiles.Where(static profileModel => profileModel.IsLocal))
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                if (profileModel.RootSurfaces.Count == 0 && profileModel.CompositionSurfaces.Count == 0)
                    continue;

                var source = GenerateManualQueryBuilderExtensions(profileModel, model.ProfileContract);

                context.AddSource($"{GetExtensionClassName(profileModel.Profile)}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        /// <summary>
        /// Generates manual root and composition query feature extensions for the specified profile.
        /// </summary>
        private static string GenerateManualQueryBuilderExtensions(ProfileSurfaceModel model, INamedTypeSymbol profileContract)
        {
            var profileType = model.Profile.ToDisplayString(GeneratedTypeDisplayFormat);
            var extensionClassName = GetExtensionClassName(model.Profile);
            var rootMethods = GetManualSurfaceMethods(model.RootSurfaces, profileContract);
            var compositionMethods = GetCompositionSurfaceMethods(model.CompositionSurfaces, profileContract);
            var source = new StringBuilder();

            source.AppendLine("// <auto-generated />");
            source.AppendLine("#nullable enable");
            source.AppendLine();
            source.AppendLine($"namespace {GeneratedExtensionsNamespace}");
            source.AppendLine("{");
            source.AppendLine("    /// <summary>");
            source.AppendLine("    /// Provides generated query feature extensions for");
            source.AppendLine($"    /// <see cref=\"{profileType}\"/>.");
            source.AppendLine("    /// </summary>");
            source.AppendLine($"    public static class {extensionClassName}");
            source.AppendLine("    {");

            var hasPreviousMethod = false;

            foreach (var method in rootMethods)
            {
                if (hasPreviousMethod)
                    source.AppendLine();

                AppendManualExtension(source, model, method);
                hasPreviousMethod = true;
            }

            foreach (var surfaceMethod in compositionMethods)
            {
                if (hasPreviousMethod)
                    source.AppendLine();

                AppendCompositionExtension(source, model, surfaceMethod, profileContract);
                hasPreviousMethod = true;
            }

            source.AppendLine("    }");
            source.AppendLine("}");

            return source.ToString();
        }

        /// <summary>
        /// Appends a generated manual root query feature extension.
        /// </summary>
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
        /// Gets root feature methods exposed by the resolved root surface set.
        /// </summary>
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
        /// Adds methods declared by a root surface and inherited root feature surfaces.
        /// </summary>
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
        /// Gets composition feature methods exposed by compatible composition surfaces.
        /// </summary>
        private static IReadOnlyList<SurfaceMethodModel> GetCompositionSurfaceMethods(IReadOnlyList<INamedTypeSymbol> surfaces, INamedTypeSymbol profileContract)
        {
            var methods = new List<SurfaceMethodModel>();

            foreach (var surface in surfaces)
                AddCompositionSurfaceMethods(surface, profileContract, methods);

            return methods
                .GroupBy(BuildCompositionMethodIdentity, StringComparer.Ordinal)
                .Select(static group => group.First())
                .OrderBy(static model => model.Method.Name, StringComparer.Ordinal)
                .ThenBy(static model => model.Method.Arity)
                .ThenBy(static model => model.Method.Parameters.Length)
                .ToList();
        }

        /// <summary>
        /// Adds methods declared by a composition surface and inherited feature surfaces.
        /// </summary>
        private static void AddCompositionSurfaceMethods(INamedTypeSymbol surface, INamedTypeSymbol profileContract, ICollection<SurfaceMethodModel> methods)
        {
            foreach (var method in GetDeclaredSurfaceMethods(surface))
                methods.Add(new SurfaceMethodModel(surface, method));

            foreach (var inheritedSurface in surface.Interfaces)
            {
                if (GetProfileTypeParameterIndex(inheritedSurface.OriginalDefinition, profileContract) < 0)
                    continue;

                AddCompositionSurfaceMethods(inheritedSurface.OriginalDefinition, profileContract, methods);
            }
        }

        /// <summary>
        /// Appends a generated profile-closed composition feature extension.
        /// </summary>
        private static void AppendCompositionExtension(StringBuilder source, ProfileSurfaceModel model, SurfaceMethodModel surfaceMethod, INamedTypeSymbol profileContract)
        {
            var surface = surfaceMethod.Surface.OriginalDefinition;
            var method = surfaceMethod.Method;
            var profileParameterIndex = GetProfileTypeParameterIndex(surface, profileContract);

            if (profileParameterIndex < 0)
                return;

            var profileParameter = surface.TypeParameters[profileParameterIndex];
            var promotedSurfaceParameters = surface.TypeParameters.Where((_, index) => index != profileParameterIndex).ToList();
            var substitutions = new Dictionary<ITypeParameterSymbol, string>(SymbolEqualityComparer.Default);
            var profileType = model.Profile.ToDisplayString(GeneratedTypeDisplayFormat);

            substitutions[profileParameter] = profileType;

            foreach (var typeParameter in promotedSurfaceParameters)
                substitutions[typeParameter] = typeParameter.Name;

            foreach (var typeParameter in method.TypeParameters)
                substitutions[typeParameter] = typeParameter.Name;

            var genericTypeParameters = promotedSurfaceParameters
                .Select(static parameter => parameter.Name)
                .Concat(method.TypeParameters.Select(static parameter => parameter.Name))
                .ToList();

            var genericParameters = genericTypeParameters.Count == 0
                ? string.Empty
                : $"<{string.Join(", ", genericTypeParameters)}>";

            var queryCommandBuilder = FindQueryCommandBuilderSurface(surface);

            if (queryCommandBuilder is null)
                return;

            var receiverType = RenderType(queryCommandBuilder, substitutions);
            var returnType = RenderType(method.ReturnType, substitutions);
            var parameterDeclarations = method.Parameters.Select(parameter => BuildParameterDeclaration(parameter, substitutions)).ToList();

            parameterDeclarations.Insert(0, $"this {receiverType} queryBuilder");

            var parameters = string.Join(", ", parameterDeclarations);
            var arguments = string.Join(", ", method.Parameters.Select(BuildArgument));
            var hookName = $"Apply{method.Name}";
            var hookGenericParameters = method.TypeParameters.Length == 0
                ? string.Empty
                : $"<{string.Join(", ", method.TypeParameters.Select(static parameter => parameter.Name))}>";

            source.AppendLine("        /// <summary>");
            source.AppendLine($"        /// Provides the generated {method.Name} composition feature operation.");
            source.AppendLine("        /// </summary>");

            foreach (var typeParameter in promotedSurfaceParameters)
            {
                source.AppendLine($"        /// <typeparam name=\"{typeParameter.Name}\">");
                source.AppendLine("        /// Generic type parameter associated with the current query composition.");
                source.AppendLine("        /// </typeparam>");
            }

            foreach (var typeParameter in method.TypeParameters)
            {
                source.AppendLine($"        /// <typeparam name=\"{typeParameter.Name}\">");
                source.AppendLine("        /// Generic type parameter associated with the feature operation.");
                source.AppendLine("        /// </typeparam>");
            }

            source.AppendLine("        /// <param name=\"queryBuilder\">");
            source.AppendLine("        /// Current query command builder instance.");
            source.AppendLine("        /// </param>");

            foreach (var parameter in method.Parameters)
            {
                source.AppendLine($"        /// <param name=\"{parameter.Name}\">");
                source.AppendLine("        /// Feature operation argument.");
                source.AppendLine("        /// </param>");
            }

            source.AppendLine("        /// <returns>");
            source.AppendLine("        /// Result produced by the composition feature operation.");
            source.AppendLine("        /// </returns>");
            source.AppendLine($"        public static {returnType} {method.Name}{genericParameters}({parameters})");

            AppendGenericConstraints(source, promotedSurfaceParameters, substitutions, "            ");
            AppendGenericConstraints(source, method.TypeParameters, substitutions, "            ");

            source.AppendLine("        {");
            source.AppendLine("            global::System.ArgumentNullException.ThrowIfNull(queryBuilder);");

            foreach (var parameter in method.Parameters.Where(static parameter => parameter.Type.TypeKind == TypeKind.Delegate))
                source.AppendLine($"            global::System.ArgumentNullException.ThrowIfNull({EscapeIdentifier(parameter.Name)});");

            if (method.ReturnsVoid)
                source.AppendLine($"            queryBuilder.{hookName}{hookGenericParameters}({arguments});");
            else
                source.AppendLine($"            return queryBuilder.{hookName}{hookGenericParameters}({arguments});");

            source.AppendLine("        }");
        }

        /// <summary>
        /// Finds the IQueryCommandBuilder contract inherited by a composition surface.
        /// </summary>
        private static INamedTypeSymbol? FindQueryCommandBuilderSurface(INamedTypeSymbol surface)
        {
            return surface.AllInterfaces.FirstOrDefault(implementedInterface =>
                implementedInterface.OriginalDefinition.ToDisplayString() ==
                "TinyBlueWhale.EngineQuery.Abstractions.Interfaces.IQueryCommandBuilder<T, TProfile>");
        }

        /// <summary>
        /// Generates query engine surfaces for every discovered profile.
        /// </summary>
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
        /// Generates the query engine surface and implementation associated with a profile.
        /// </summary>
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

            AppendEngineInterface(source, profileType, engineInterfaceName, model.RootSurfaces);

            source.AppendLine();

            AppendEngineImplementation(source, model, profileContract, profileType, engineName, engineInterfaceName);

            source.AppendLine("}");

            return source.ToString();
        }

        /// <summary>
        /// Appends the generated public query engine interface associated with a provider profile.
        /// </summary>
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
        private static void AppendEngineImplementation(StringBuilder source, ProfileSurfaceModel model, INamedTypeSymbol profileContract, string profileType, string engineName, string engineInterfaceName)
        {
            source.AppendLine("    /// <summary>");
            source.AppendLine("    /// Provides the generated query engine implementation associated with");
            source.AppendLine($"    /// <see cref=\"{profileType}\"/>.");
            source.AppendLine("    /// </summary>");
            source.AppendLine($"    internal sealed class {engineName}(global::TinyBlueWhale.EngineQuery.Core.QueryBuilding.QueryBuilder<{profileType}> queryBuilder) :");
            source.AppendLine($"        global::TinyBlueWhale.EngineQuery.DependencyInjection.QueryEngine<{profileType}>(queryBuilder),");
            source.AppendLine($"        {engineInterfaceName}");
            source.AppendLine("    {");

            var surfaceMethods = GetDependencyInjectionSurfaceMethods(model.RootSurfaces, profileContract);

            for (var index = 0; index < surfaceMethods.Count; index++)
            {
                AppendDependencyInjectionSurfaceMethod(source, model, surfaceMethods[index]);

                if (index < surfaceMethods.Count - 1)
                    source.AppendLine();
            }

            source.AppendLine("    }");
        }

        /// <summary>
        /// Gets root surface methods requiring explicit DI forwarding implementations.
        /// </summary>
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
        /// Adds methods declared by a root feature surface and inherited root contracts.
        /// </summary>
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
        /// Appends an explicit root feature implementation to a generated query engine.
        /// </summary>
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
        /// Gets ordinary instance methods declared directly by a feature surface.
        /// </summary>
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
        /// Determines whether the specified return type represents a resolved root surface.
        /// </summary>
        private static bool IsRootSurfaceReturn(ITypeSymbol returnType, ProfileSurfaceModel model)
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
        /// Builds an identity used to de-duplicate manual root extension methods.
        /// </summary>
        private static string BuildManualMethodIdentity(IMethodSymbol method)
        {
            return $"{method.Name}`{method.Arity}({string.Join(",", method.Parameters.Select(parameter => $"{parameter.RefKind}:{parameter.Type.ToDisplayString(GeneratedTypeDisplayFormat)}"))})";
        }

        /// <summary>
        /// Builds an identity used to de-duplicate composition extension methods.
        /// </summary>
        private static string BuildCompositionMethodIdentity(SurfaceMethodModel model)
        {
            return $"{model.Surface.OriginalDefinition.ToDisplayString(GeneratedTypeDisplayFormat)}::{BuildManualMethodIdentity(model.Method)}";
        }

        /// <summary>
        /// Builds an identity used to de-duplicate DI surface implementations.
        /// </summary>
        private static string BuildDependencyInjectionMethodIdentity(SurfaceMethodModel model)
        {
            return $"{model.Surface.ToDisplayString(GeneratedTypeDisplayFormat)}::{BuildManualMethodIdentity(model.Method)}";
        }

        /// <summary>
        /// Builds the generic parameter list associated with the specified method.
        /// </summary>
        private static string BuildGenericParameterList(IMethodSymbol method)
        {
            if (method.TypeParameters.Length == 0)
                return string.Empty;

            return $"<{string.Join(", ", method.TypeParameters.Select(static parameter => parameter.Name))}>";
        }

        /// <summary>
        /// Appends generic constraints declared by the specified method.
        /// </summary>
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
        /// Appends generic constraints using the specified type substitutions.
        /// </summary>
        private static void AppendGenericConstraints(StringBuilder source, IEnumerable<ITypeParameterSymbol> typeParameters, IReadOnlyDictionary<ITypeParameterSymbol, string> substitutions, string indentation)
        {
            foreach (var typeParameter in typeParameters)
            {
                var constraints = BuildGenericConstraints(typeParameter, substitutions);

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
        /// Builds generic constraints associated with a type parameter.
        /// </summary>
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
        /// Builds generic constraints associated with a type parameter using substitutions.
        /// </summary>
        private static IReadOnlyList<string> BuildGenericConstraints(ITypeParameterSymbol typeParameter, IReadOnlyDictionary<ITypeParameterSymbol, string> substitutions)
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

            constraints.AddRange(typeParameter.ConstraintTypes.Select(constraint => RenderType(constraint, substitutions)));

            if (typeParameter.HasConstructorConstraint)
                constraints.Add("new()");

            return constraints;
        }

        /// <summary>
        /// Builds the source representation of a generated method parameter.
        /// </summary>
        private static string BuildParameterDeclaration(IParameterSymbol parameter)
        {
            var modifier = GetRefModifier(parameter.RefKind);
            var type = parameter.Type.ToDisplayString(GeneratedTypeDisplayFormat);
            var defaultValue = BuildDefaultValue(parameter);

            return $"{modifier}{type} {EscapeIdentifier(parameter.Name)}{defaultValue}";
        }

        /// <summary>
        /// Builds a parameter declaration using generic type substitutions.
        /// </summary>
        private static string BuildParameterDeclaration(IParameterSymbol parameter, IReadOnlyDictionary<ITypeParameterSymbol, string> substitutions)
        {
            var modifier = GetRefModifier(parameter.RefKind);
            var type = RenderType(parameter.Type, substitutions);
            var defaultValue = BuildDefaultValue(parameter);

            return $"{modifier}{type} {EscapeIdentifier(parameter.Name)}{defaultValue}";
        }

        /// <summary>
        /// Renders a type while replacing selected generic type parameters.
        /// </summary>
        private static string RenderType(ITypeSymbol type, IReadOnlyDictionary<ITypeParameterSymbol, string> substitutions)
        {
            var parts = type.ToDisplayParts(GeneratedTypeDisplayFormat);
            var source = new StringBuilder();

            foreach (var part in parts)
            {
                if (part.Symbol is ITypeParameterSymbol typeParameter && TryGetTypeSubstitution(typeParameter, substitutions, out var substitution))
                    source.Append(substitution);
                else
                    source.Append(part.ToString());
            }

            return source.ToString();
        }

        /// <summary>
        /// Attempts to resolve a generic type parameter substitution.
        /// </summary>
        private static bool TryGetTypeSubstitution(ITypeParameterSymbol typeParameter, IReadOnlyDictionary<ITypeParameterSymbol, string> substitutions, out string substitution)
        {
            foreach (var pair in substitutions)
            {
                if (!SymbolEqualityComparer.Default.Equals(pair.Key, typeParameter))
                    continue;

                substitution = pair.Value;
                return true;
            }

            substitution = string.Empty;
            return false;
        }

        /// <summary>
        /// Builds the forwarding invocation argument associated with a method parameter.
        /// </summary>
        private static string BuildArgument(IParameterSymbol parameter)
        {
            return $"{GetRefModifier(parameter.RefKind)}{EscapeIdentifier(parameter.Name)}";
        }

        /// <summary>
        /// Builds the suffix used to append forwarded arguments after the root builder argument.
        /// </summary>
        private static string BuildForwardedArgumentSuffix(string arguments)
        {
            return string.IsNullOrWhiteSpace(arguments) ? string.Empty : $", {arguments}";
        }

        /// <summary>
        /// Gets the C# parameter modifier associated with the specified reference kind.
        /// </summary>
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
        private static string EscapeIdentifier(string identifier)
        {
            return Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetKeywordKind(identifier) != Microsoft.CodeAnalysis.CSharp.SyntaxKind.None
                ? $"@{identifier}"
                : identifier;
        }

        /// <summary>
        /// Escapes a string value for inclusion in generated C# source.
        /// </summary>
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
        /// Appends dependency injection registrations associated with a provider profile.
        /// </summary>
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
        /// Gets the generated query engine name associated with the specified profile.
        /// </summary>
        private static string GetEngineName(INamedTypeSymbol profile)
        {
            return $"{GetProfileBaseName(profile)}QueryEngine";
        }

        /// <summary>
        /// Gets the generated query builder extension class name associated with the specified profile.
        /// </summary>
        private static string GetExtensionClassName(INamedTypeSymbol profile)
        {
            return $"{GetProfileBaseName(profile)}QueryBuilderExtensions";
        }

        /// <summary>
        /// Gets the provider profile name without the conventional Profile suffix.
        /// </summary>
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
        private sealed class GenerationModel(ImmutableArray<ProfileSurfaceModel> profiles, INamedTypeSymbol? profileContract, bool supportsQueryEngineGeneration)
        {
            /// <summary>
            /// Gets the discovered provider profile surface models.
            /// </summary>
            public ImmutableArray<ProfileSurfaceModel> Profiles { get; } = profiles;

            /// <summary>
            /// Gets the database provider profile contract used during structural feature discovery.
            /// </summary>
            public INamedTypeSymbol? ProfileContract { get; } = profileContract;

            /// <summary>
            /// Gets whether the current compilation supports query engine and dependency injection generation.
            /// </summary>
            public bool SupportsQueryEngineGeneration { get; } = supportsQueryEngineGeneration;
        }

        /// <summary>
        /// Represents a provider profile together with compatible root and composition feature surfaces.
        /// </summary>
        private sealed class ProfileSurfaceModel(INamedTypeSymbol profile, IReadOnlyList<INamedTypeSymbol> rootSurfaces, IReadOnlyList<INamedTypeSymbol> compositionSurfaces, bool isLocal)
        {
            /// <summary>
            /// Gets the database provider profile represented by the model.
            /// </summary>
            public INamedTypeSymbol Profile { get; } = profile ?? throw new ArgumentNullException(nameof(profile));

            /// <summary>
            /// Gets root query feature surfaces compatible with the profile.
            /// </summary>
            public IReadOnlyList<INamedTypeSymbol> RootSurfaces { get; } = rootSurfaces ?? throw new ArgumentNullException(nameof(rootSurfaces));

            /// <summary>
            /// Gets composition query feature surfaces compatible with the profile.
            /// </summary>
            public IReadOnlyList<INamedTypeSymbol> CompositionSurfaces { get; } = compositionSurfaces ?? throw new ArgumentNullException(nameof(compositionSurfaces));

            /// <summary>
            /// Gets whether the profile is declared by the current compilation.
            /// </summary>
            public bool IsLocal { get; } = isLocal;
        }

        /// <summary>
        /// Represents a method declared by a specific feature surface.
        /// </summary>
        private sealed class SurfaceMethodModel(INamedTypeSymbol surface, IMethodSymbol method)
        {
            /// <summary>
            /// Gets the feature surface declaring the method.
            /// </summary>
            public INamedTypeSymbol Surface { get; } = surface ?? throw new ArgumentNullException(nameof(surface));

            /// <summary>
            /// Gets the method declared by the feature surface.
            /// </summary>
            public IMethodSymbol Method { get; } = method ?? throw new ArgumentNullException(nameof(method));
        }
    }
}
