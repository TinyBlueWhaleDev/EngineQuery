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
    /// feature availability. Generated query engine surfaces preserve the concrete
    /// provider profile type used by the fluent query contracts.
    /// </remarks>
    [Generator]
    public sealed class QueryEngineSurfaceGenerator : IIncrementalGenerator
    {
        private const string DatabaseProviderProfileInterface =
            "TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers.IDatabaseProviderProfile";

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
             
            var source = new StringBuilder();

            source.AppendLine("// <auto-generated />");
            source.AppendLine("#nullable enable");
            source.AppendLine();
            source.AppendLine($"namespace {GeneratedNamespace}");
            source.AppendLine("{");

            AppendEngineInterface(source, profileType, engineInterfaceName);

            source.AppendLine();

            AppendEngineImplementation(source, profileType, engineName, engineInterfaceName);

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
        private static void AppendEngineInterface(StringBuilder source, string profileType, string engineInterfaceName)
        {
            source.AppendLine("    /// <summary>");
            source.AppendLine("    /// Represents the generated query engine surface associated with");
            source.AppendLine($"    /// <see cref=\"{profileType}\"/>.");
            source.AppendLine("    /// </summary>");
            source.AppendLine($"    public interface {engineInterfaceName} :");
            source.AppendLine($"        global::{QueryEngineInterface}<{profileType}>");
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
        private static void AppendEngineImplementation(StringBuilder source, string profileType, string engineName, string engineInterfaceName)
        {
            source.AppendLine("    /// <summary>");
            source.AppendLine("    /// Provides the generated query engine implementation associated with");
            source.AppendLine($"    /// <see cref=\"{profileType}\"/>.");
            source.AppendLine("    /// </summary>");
            source.AppendLine($"    internal sealed class {engineName}(");
            source.AppendLine($"        global::TinyBlueWhale.EngineQuery.Core.QueryBuilding.QueryBuilder<{profileType}> queryBuilder) :");
            source.AppendLine($"        global::TinyBlueWhale.EngineQuery.DependencyInjection.QueryEngine<{profileType}>(queryBuilder),");
            source.AppendLine($"        {engineInterfaceName}");
            source.AppendLine("    {");
            source.AppendLine("    }");
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
        private static void GenerateDependencyInjectionRegistration(SourceProductionContext context, ImmutableArray<INamedTypeSymbol> profiles)
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
            source.AppendLine(                "    public static partial class ServiceCollectionExtensions");
            source.AppendLine("    {");
            source.AppendLine("        /// <summary>");
            source.AppendLine("        /// Registers generated strongly typed query engine factories.");
            source.AppendLine("        /// </summary>");
            source.AppendLine("        /// <param name=\"services\">");
            source.AppendLine("        /// Service collection receiving generated query engine factories.");
            source.AppendLine("        /// </param>");
            source.AppendLine("        static partial void RegisterGeneratedQueryEngineFactories(");
            source.AppendLine("            global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
            source.AppendLine("        {");

            foreach (var profile in profiles)
                AppendFactoryRegistration(source, profile);

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
            var profileType = profile.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var engineName = GetEngineName(profile);

            var engineInterfaceName = $"global::{GeneratedNamespace}.I{engineName}";
            var engineImplementationName = $"global::{GeneratedNamespace}.{engineName}";
            var factoryInterface = $"global::TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces.IQueryEngineFactory<{profileType}, {engineInterfaceName}>";
            var factoryImplementation = $"global::TinyBlueWhale.EngineQuery.DependencyInjection.Factories.QueryEngineFactory<{profileType}, {engineInterfaceName}>";

            source.AppendLine();
            source.AppendLine($"            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<{factoryInterface}>(");
            source.AppendLine("                services,");
            source.AppendLine("                serviceProvider =>");
            source.AppendLine($"                    new {factoryImplementation}(");
            source.AppendLine("                        serviceProvider,");
            source.AppendLine("                        global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetServices<global::TinyBlueWhale.EngineQuery.DependencyInjection.Configuration.EngineQueryRegistration>(serviceProvider),");
            source.AppendLine($"                        queryBuilder => new {engineImplementationName}(queryBuilder)));");
            source.AppendLine();
            source.AppendLine($"            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<{engineInterfaceName}>(");
            source.AppendLine("                services,");
            source.AppendLine("                serviceProvider =>");
            source.AppendLine($"                    global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<{factoryInterface}>(serviceProvider).Create());");
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
            const string profileSuffix = "Profile";

            var profileName = profile.Name;

            if (profileName.EndsWith(profileSuffix, StringComparison.Ordinal))
                profileName = profileName.Substring(0, profileName.Length - profileSuffix.Length);

            return $"{profileName}QueryEngine";
        }
       
    }
}
