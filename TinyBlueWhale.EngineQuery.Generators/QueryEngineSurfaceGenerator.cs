using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TinyBlueWhale.EngineQuery.Generators.Discovery;
using TinyBlueWhale.EngineQuery.Generators.Generation;
using TinyBlueWhale.EngineQuery.Generators.Models;
using TinyBlueWhale.EngineQuery.Generators.Rendering;
using TinyBlueWhale.EngineQuery.Generators.Resolution;

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

        private const string QueryEngineInterfaceMetadataName =
            "TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces.IQueryEngine`1";

        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var generationModel = context.CompilationProvider.Select(static (compilation, cancellationToken) =>
                BuildGenerationModel(compilation, cancellationToken));

            context.RegisterSourceOutput(
                generationModel,
                static (productionContext, model) =>
                {
                    ManualExtensionGenerator.Generate(productionContext, model);

                    if (!model.SupportsQueryEngineGeneration)
                        return;

                    QueryEngineGenerator.Generate(productionContext, model);
                    DependencyInjectionGenerator.Generate(productionContext, model);
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
                return new GenerationModel(compilation, [], null, false);

            var profiles = ProviderProfileDiscovery.Discover(compilation, profileContract, cancellationToken);
            var rootSurfaceDefinitions = FeatureSurfaceDiscovery.DiscoverRootSurfaces(compilation, profileContract, cancellationToken);
            var compositionSurfaceDefinitions = FeatureSurfaceDiscovery.DiscoverCompositionSurfaces(compilation, profileContract, cancellationToken);
            var profileModels = FeatureSurfaceResolver.Resolve(compilation, profiles, rootSurfaceDefinitions, compositionSurfaceDefinitions, profileContract);
            var supportsQueryEngineGeneration = compilation.GetTypeByMetadataName(QueryEngineInterfaceMetadataName) is not null;

            return new GenerationModel(compilation, profileModels, profileContract, supportsQueryEngineGeneration);
        }
    }
}
