using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace TinyBlueWhale.EngineQuery.Generators
{

    /// <summary>
    /// Generates strongly typed EngineQuery surfaces, implementations and dependency
    /// injection registrations from concrete database provider profiles.
    /// </summary>
    /// <remarks>
    /// Provider profiles act as the single source of truth for provider version and
    /// feature availability. Feature contracts declare their associated query builder
    /// surfaces through QueryFeatureSurfaceAttribute.
    /// </remarks>
    [Generator]
    public sealed class QueryEngineSurfaceGenerator : IIncrementalGenerator
    {
        private const string DatabaseProviderProfileInterface =
            "TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers.IDatabaseProviderProfile";

        private const string QueryFeatureSurfaceAttribute =
            "TinyBlueWhale.EngineQuery.Abstractions.Attributes.QueryFeatureSurfaceAttribute";

        private const string QueryEngineInterface =
            "TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces.IQueryEngine";

        private const string GeneratedNamespace =
            "TinyBlueWhale.EngineQuery.Generated";

        /// <inheritdoc />
        public void Initialize(
            IncrementalGeneratorInitializationContext context)
        {
            var profiles = context.CompilationProvider
                .Select(static (compilation, cancellationToken) => DiscoverProfiles(compilation, cancellationToken));

            context.RegisterSourceOutput(
                profiles,
                static (productionContext, discoveredProfiles) =>
                {
                    GenerateQueryEngineSurfaces(productionContext, discoveredProfiles);

                    GenerateDependencyInjectionRegistration(productionContext, discoveredProfiles);
                });
        }

        /// <summary>
        /// Discovers concrete database provider profiles available through the current
        /// compilation and referenced EngineQuery assemblies.
        /// </summary>
        /// <param name="compilation">
        /// Compilation being processed by the source generator.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel profile discovery.
        /// </param>
        /// <returns>
        /// Immutable collection containing every discovered concrete provider profile.
        /// </returns>
        private static ImmutableArray<INamedTypeSymbol> DiscoverProfiles(Compilation compilation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var profileContract = compilation.GetTypeByMetadataName(DatabaseProviderProfileInterface);

            if (profileContract is null)
                return [];

            var profiles = ImmutableArray.CreateBuilder<INamedTypeSymbol>();

            CollectProfiles(compilation.Assembly.GlobalNamespace, profileContract, profiles, cancellationToken);

            foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!assembly.Name.StartsWith("TinyBlueWhale.EngineQuery", StringComparison.Ordinal))
                    continue;

                CollectProfiles(assembly.GlobalNamespace, profileContract, profiles, cancellationToken);
            }

            return [.. profiles.Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default).OrderBy(static profile => profile.ToDisplayString(), StringComparer.Ordinal)];
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
            {
                cancellationToken.ThrowIfCancellationRequested();

                CollectProfiles(nestedNamespace, profileContract, profiles, cancellationToken);
            }
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
        /// Discovers the query builder surfaces associated with the feature contracts
        /// implemented by the specified database provider profile.
        /// </summary>
        /// <param name="profile">
        /// Database provider profile being inspected.
        /// </param>
        /// <returns>
        /// Query builder surfaces exposed by the provider profile.
        /// </returns>
        private static IReadOnlyList<FeatureSurface> DiscoverFeatureSurfaces(INamedTypeSymbol profile)
        {
            var surfaces = new List<FeatureSurface>();

            foreach (var featureInterface in profile.AllInterfaces)
            {
                var attribute = featureInterface
                    .GetAttributes()
                    .FirstOrDefault(candidate =>
                        string.Equals(
                            candidate.AttributeClass?.ToDisplayString(),
                            QueryFeatureSurfaceAttribute,
                            StringComparison.Ordinal));

                if (attribute is null || attribute.ConstructorArguments.Length != 1)
                    continue;

                if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol surfaceDefinition)
                    continue;

                var surfaceOriginalDefinition = surfaceDefinition.OriginalDefinition;

                if (!surfaceOriginalDefinition.IsGenericType || surfaceOriginalDefinition.Arity != 1)
                    continue;

                var constructedSurface = surfaceOriginalDefinition.Construct(profile);

                surfaces.Add(new FeatureSurface(constructedSurface));
            }

            return surfaces
                .GroupBy(
                    surface => surface.InterfaceSymbol,
                    SymbolEqualityComparer.Default)
                .Select(static group => group.First())
                .OrderBy(
                    static surface =>
                        surface.InterfaceSymbol.ToDisplayString(),
                    StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>
        /// Generates the query engine surface and concrete wrapper associated with
        /// every discovered database provider profile.
        /// </summary>
        /// <param name="context">
        /// Source production context used to add generated sources.
        /// </param>
        /// <param name="profiles">
        /// Concrete provider profiles discovered during compilation.
        /// </param>
        private static void GenerateQueryEngineSurfaces(SourceProductionContext context, ImmutableArray<INamedTypeSymbol> profiles)
        {
            foreach (var profile in profiles)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var source =GenerateQueryEngineSurface(profile);

                context.AddSource($"{GetEngineName(profile)}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        /// <summary>
        /// Generates the query engine surface and implementation associated with
        /// the specified database provider profile.
        /// </summary>
        /// <param name="profile">
        /// Provider profile used to determine the generated engine surface.
        /// </param>
        /// <returns>
        /// Generated C# source containing the strongly typed query engine interface
        /// and concrete implementation.
        /// </returns>
        private static string GenerateQueryEngineSurface( INamedTypeSymbol profile)
        {
            var profileType = profile.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var engineName = GetEngineName(profile);

            var engineInterfaceName = $"I{engineName}";

            var featureSurfaces = DiscoverFeatureSurfaces(profile);
             
            var source = new StringBuilder();

            source.AppendLine("// <auto-generated />");
            source.AppendLine("#nullable enable");
            source.AppendLine();
            source.AppendLine($"namespace {GeneratedNamespace}");
            source.AppendLine("{");

            AppendEngineInterface(source, profileType, engineInterfaceName, featureSurfaces);

            source.AppendLine();

            AppendEngineImplementation(source, profileType, engineName, engineInterfaceName, featureSurfaces);

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
        /// Query builder surfaces exposed by the provider profile.
        /// </param>
        private static void AppendEngineInterface(StringBuilder source, string profileType, string engineInterfaceName, IReadOnlyList<FeatureSurface> featureSurfaces)
        {
            var surfaces = new List<string>
            {
                $"global::{QueryEngineInterface}<{profileType}>"
            };

            surfaces.AddRange(featureSurfaces.Select(feature => feature.InterfaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

            source.AppendLine("    /// <summary>");
            source.AppendLine("    /// Represents the generated query engine surface associated with");
            source.AppendLine($"    /// <see cref=\"{profileType}\"/>.");
            source.AppendLine("    /// </summary>");
            source.Append($"    public interface {engineInterfaceName} :");
            source.AppendLine();

            for (var index = 0; index < surfaces.Count;index++)
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
        /// <param name="profileType">
        /// Fully qualified provider profile type.
        /// </param>
        /// <param name="engineName">
        /// Generated concrete query engine name.
        /// </param>
        /// <param name="engineInterfaceName">
        /// Generated query engine interface name.
        /// </param>
        /// <param name="featureSurfaces">
        /// Query builder surfaces exposed by the provider profile.
        /// </param>
        private static void AppendEngineImplementation(
            StringBuilder source,
            string profileType,
            string engineName,
            string engineInterfaceName,
            IReadOnlyList<FeatureSurface> featureSurfaces)
        {
            source.AppendLine("    /// <summary>");
            source.AppendLine("    /// Provides the generated query engine implementation associated with");
            source.AppendLine($"    /// <see cref=\"{profileType}\"/>.");
            source.AppendLine("    /// </summary>");
            source.AppendLine("    /// <remarks>");
            source.AppendLine("    /// The generated implementation delegates query construction to the shared");
            source.AppendLine("    /// query engine while exposing only the feature surfaces supported by the profile.");
            source.AppendLine("    /// </remarks>");
            source.AppendLine($"    internal sealed class {engineName}(");
            source.AppendLine($"        global::TinyBlueWhale.EngineQuery.Core.QueryBuilding.QueryBuilder<{profileType}> queryBuilder) :");
            source.AppendLine($"        global::TinyBlueWhale.EngineQuery.DependencyInjection.QueryEngine<{profileType}>(queryBuilder),");
            source.AppendLine($"        {engineInterfaceName}");
            source.AppendLine("    {");

            foreach (var featureSurface in featureSurfaces)
            {
                AppendFeatureImplementation(source, featureSurface.InterfaceSymbol);
            }

            source.AppendLine("    }");
        }

        /// <summary>
        /// Appends explicit forwarding implementations for the methods exposed by
        /// the specified feature surface.
        /// </summary>
        /// <param name="source">
        /// Source builder receiving generated code.
        /// </param>
        /// <param name="surface">
        /// Constructed feature surface whose methods are being forwarded.
        /// </param>
        private static void AppendFeatureImplementation(StringBuilder source, INamedTypeSymbol surface)
        {
            var surfaceType = surface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            foreach (var method in GetSurfaceMethods(surface))
            {
                var returnType = method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                var genericParameters = BuildGenericParameterList(method);

                var parameters = string.Join(", ", method.Parameters.Select(BuildParameterDeclaration));

                var arguments = string.Join( ", ", method.Parameters.Select(BuildArgument));

                source.AppendLine("        /// <inheritdoc />");
                source.AppendLine($"        {returnType}");
                source.AppendLine($"            {surfaceType}.{method.Name}{genericParameters}({parameters})");

                AppendGenericConstraints(source, method, "            ");

                source.AppendLine("        {");
                source.Append("            ");

                if (!method.ReturnsVoid)
                    source.Append("return ");

                source.Append($"(({surfaceType})_innerQueryBuilder).{method.Name}{genericParameters}({arguments});");

                source.AppendLine();
                source.AppendLine("        }");
                source.AppendLine();
            }
        }

        /// <summary>
        /// Gets the ordinary instance methods declared directly by the specified
        /// feature surface.
        /// </summary>
        /// <param name="surface">
        /// Feature surface being inspected.
        /// </param>
        /// <returns>
        /// Methods declared directly by the feature surface and requiring explicit
        /// forwarding implementations.
        /// </returns>
        private static IReadOnlyList<IMethodSymbol> GetSurfaceMethods(INamedTypeSymbol surface)
        {
            return surface
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(static method =>
                    method.MethodKind == MethodKind.Ordinary &&
                    !method.IsStatic)
                .OrderBy(
                    static method => method.Name,
                    StringComparer.Ordinal)
                .ThenBy(
                    static method => method.Parameters.Length)
                .ToList();
        }

        /// <summary>
        /// Adds ordinary methods declared directly by the specified surface.
        /// </summary>
        /// <param name="methods">
        /// Collection receiving discovered methods.
        /// </param>
        /// <param name="surface">
        /// Surface whose directly declared methods are being inspected.
        /// </param>
        private static void AddSurfaceMethods(ICollection<IMethodSymbol> methods, INamedTypeSymbol surface)
        {
            foreach (var method in surface.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.MethodKind != MethodKind.Ordinary || method.IsStatic)
                    continue;

                methods.Add(method);
            }
        }

        /// <summary>
        /// Builds a deterministic signature used to remove duplicate inherited methods.
        /// </summary>
        /// <param name="method">
        /// Method whose signature is being generated.
        /// </param>
        /// <returns>
        /// Deterministic method signature.
        /// </returns>
        private static string GetMethodSignature(IMethodSymbol method)
        {
            var parameters = string.Join(
                "|",
                method.Parameters.Select(
                    parameter =>
                        $"{parameter.RefKind}:{parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}"));

            return $"{method.Name}`{method.Arity}({parameters})";
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
        private static void AppendGenericConstraints(
            StringBuilder source,
            IMethodSymbol method,
            string indentation)
        {
            foreach (var typeParameter in method.TypeParameters)
            {
                var constraints =
                    BuildGenericConstraints(typeParameter);

                if (constraints.Count == 0)
                    continue;

                source.Append(indentation);
                source.Append("where ");
                source.Append(typeParameter.Name);
                source.Append(" : ");
                source.AppendLine(
                    string.Join(", ", constraints));
            }
        }

        /// <summary>
        /// Builds the generic constraints associated with the specified type parameter.
        /// </summary>
        /// <param name="typeParameter">
        /// Generic type parameter being inspected.
        /// </param>
        /// <returns>
        /// Collection containing the rendered generic constraints.
        /// </returns>
        private static IReadOnlyList<string> BuildGenericConstraints(
            ITypeParameterSymbol typeParameter)
        {
            var constraints = new List<string>();

            if (typeParameter.HasUnmanagedTypeConstraint)
            {
                constraints.Add("unmanaged");
            }
            else if (typeParameter.HasValueTypeConstraint)
            {
                constraints.Add("struct");
            }
            else if (typeParameter.HasReferenceTypeConstraint)
            {
                constraints.Add(
                    typeParameter.ReferenceTypeConstraintNullableAnnotation ==
                    NullableAnnotation.Annotated
                        ? "class?"
                        : "class");
            }
            else if (typeParameter.HasNotNullConstraint)
            {
                constraints.Add("notnull");
            }

            constraints.AddRange(
                typeParameter.ConstraintTypes.Select(
                    constraint =>
                        constraint.ToDisplayString(
                            SymbolDisplayFormat.FullyQualifiedFormat)));

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
        private static string BuildParameterDeclaration(
            IParameterSymbol parameter)
        {
            var modifier =
                GetRefModifier(parameter.RefKind);

            var type =
                parameter.Type.ToDisplayString(
                    SymbolDisplayFormat.FullyQualifiedFormat);

            var defaultValue =
                BuildDefaultValue(parameter);

            return
                $"{modifier}{type} {EscapeIdentifier(parameter.Name)}{defaultValue}";
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
        private static string BuildArgument(
            IParameterSymbol parameter)
        {
            return
                $"{GetRefModifier(parameter.RefKind)}{EscapeIdentifier(parameter.Name)}";
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
        private static string GetRefModifier(
            RefKind refKind)
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
        private static string BuildDefaultValue(
            IParameterSymbol parameter)
        {
            if (!parameter.HasExplicitDefaultValue)
                return string.Empty;

            if (parameter.ExplicitDefaultValue is null)
                return " = null";

            return parameter.ExplicitDefaultValue switch
            {
                bool value =>
                    value ? " = true" : " = false",

                string value =>
                    $" = \"{EscapeString(value)}\"",

                char value =>
                    $" = '{EscapeChar(value)}'",

                float value =>
                    $" = {value.ToString(System.Globalization.CultureInfo.InvariantCulture)}F",

                double value =>
                    $" = {value.ToString(System.Globalization.CultureInfo.InvariantCulture)}D",

                decimal value =>
                    $" = {value.ToString(System.Globalization.CultureInfo.InvariantCulture)}M",

                _ =>
                    $" = {Convert.ToString(parameter.ExplicitDefaultValue, System.Globalization.CultureInfo.InvariantCulture)}"
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
        private static string EscapeIdentifier(
            string identifier)
        {
            return Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetKeywordKind(identifier) !=
                   Microsoft.CodeAnalysis.CSharp.SyntaxKind.None
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
        private static string EscapeString(
            string value)
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
        private static string EscapeChar(
            char value)
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
        /// <param name="profiles">
        /// Concrete provider profiles discovered during compilation.
        /// </param>
        private static void GenerateDependencyInjectionRegistration(
            SourceProductionContext context,
            ImmutableArray<INamedTypeSymbol> profiles)
        {
            var source = new StringBuilder();

            source.AppendLine("// <auto-generated />");
            source.AppendLine("#nullable enable");
            source.AppendLine();

            source.AppendLine(
                "namespace TinyBlueWhale.EngineQuery.DependencyInjection.Extensions");

            source.AppendLine("{");
            source.AppendLine("    /// <summary>");
            source.AppendLine(
                "    /// Provides generated EngineQuery dependency injection registrations.");
            source.AppendLine("    /// </summary>");
            source.AppendLine(
                "    public static partial class ServiceCollectionExtensions");
            source.AppendLine("    {");

            source.AppendLine("        /// <summary>");
            source.AppendLine(
                "        /// Registers generated strongly typed query engine factories.");
            source.AppendLine("        /// </summary>");
            source.AppendLine("        /// <param name=\"services\">");
            source.AppendLine(
                "        /// Service collection receiving generated query engine factories.");
            source.AppendLine("        /// </param>");

            source.AppendLine(
                "        static partial void RegisterGeneratedQueryEngineFactories(");

            source.AppendLine(
                "            global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");

            source.AppendLine("        {");

            foreach (var profile in profiles)
            {
                AppendFactoryRegistration(
                    source,
                    profile);
            }

            source.AppendLine("        }");
            source.AppendLine("    }");
            source.AppendLine("}");

            context.AddSource(
                "ServiceCollectionExtensions.QueryEngineFactories.g.cs",
                SourceText.From(
                    source.ToString(),
                    Encoding.UTF8));
        }

        /// <summary>
        /// Appends the dependency injection registration associated with a provider profile.
        /// </summary>
        /// <param name="source">
        /// Source builder receiving generated registration code.
        /// </param>
        /// <param name="profile">
        /// Provider profile whose query engine factory is being registered.
        /// </param>
        private static void AppendFactoryRegistration(
            StringBuilder source,
            INamedTypeSymbol profile)
        {
            var profileType =
                profile.ToDisplayString(
                    SymbolDisplayFormat.FullyQualifiedFormat);

            var engineName =
                GetEngineName(profile);

            var engineInterfaceName =
                $"global::{GeneratedNamespace}.I{engineName}";

            var engineImplementationName =
                $"global::{GeneratedNamespace}.{engineName}";

            var factoryInterface =
                $"global::TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces.IQueryEngineFactory<{profileType}, {engineInterfaceName}>";

            var factoryImplementation =
                $"global::TinyBlueWhale.EngineQuery.DependencyInjection.Factories.QueryEngineFactory<{profileType}, {engineInterfaceName}>";

            source.AppendLine();

            source.AppendLine(
                $"            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<{factoryInterface}>(");

            source.AppendLine(
                "                services,");

            source.AppendLine(
                "                serviceProvider =>");

            source.AppendLine(
                $"                    new {factoryImplementation}(");

            source.AppendLine(
                "                        serviceProvider,");

            source.AppendLine(
                "                        global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetServices<global::TinyBlueWhale.EngineQuery.DependencyInjection.Configuration.EngineQueryRegistration>(serviceProvider),");

            source.AppendLine(
                $"                        queryBuilder => new {engineImplementationName}(queryBuilder)));");
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
        private static string GetEngineName(
            INamedTypeSymbol profile)
        {
            const string profileSuffix = "Profile";

            var profileName =
                profile.Name;

            if (profileName.EndsWith(
                    profileSuffix,
                    StringComparison.Ordinal))
            {
                profileName = profileName.Substring(
                    0,
                    profileName.Length - profileSuffix.Length);
            }

            return $"{profileName}QueryEngine";
        }

        /// <summary>
        /// Represents a query builder surface associated with a provider feature contract.
        /// </summary>
        private sealed class FeatureSurface
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FeatureSurface"/> class.
            /// </summary>
            /// <param name="interfaceSymbol">
            /// Constructed query builder surface associated with the feature.
            /// </param>
            public FeatureSurface(
                INamedTypeSymbol interfaceSymbol)
            {
                InterfaceSymbol =
                    interfaceSymbol ??
                    throw new ArgumentNullException(
                        nameof(interfaceSymbol));
            }

            /// <summary>
            /// Gets the constructed query builder surface associated with the feature.
            /// </summary>
            public INamedTypeSymbol InterfaceSymbol { get; }
        }
    }

}
