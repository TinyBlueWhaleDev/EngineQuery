using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Subqueries
{

    /// <summary>
    /// Creates nested query command builders for subquery scenarios.
    /// </summary>
    internal sealed class NestedQueryCommandBuilderFactory<TProfile>(QueryCommandBuilderContext context,
        TProfile profile)
        where TProfile : IDatabaseProviderProfile
    {
        private readonly QueryCommandBuilderContext _context = context ?? throw new ArgumentNullException(nameof(context));

        private readonly TProfile _profile = profile ?? throw new ArgumentNullException(nameof(profile));

        public QueryCommandBuilder<TSubquery, TProfile> CreateMetadataBuilder<TSubquery>(string? alias)
        {
            var metadata = new QuerySourceResolver(_context).ResolveMetadata<TSubquery>();

            var columnMappings = QuerySourceResolver.BuildColumnMappings(metadata);

            return new QueryCommandBuilder<TSubquery, TProfile>(_context.QueryCompiler,
                _context.MetadataResolver,
                _profile,
                metadata.TableName,
                metadata.SchemaName,
                alias,
                columnMappings);
        }

        public QueryBuilder<TProfile> CreateQueryBuilder()
        {
            return new QueryBuilder<TProfile>(_context.QueryCompiler, _context.MetadataResolver, _profile);
        }

        public static CompiledQueryDefinition ExtractDefinition<TSubquery>(IQueryCommandBuilder<TSubquery, TProfile> commandBuilder,
            string errorMessage)
        {
            if (commandBuilder is not QueryCommandBuilder<TSubquery, TProfile> concreteCommandBuilder)
                throw new InvalidOperationException(errorMessage);

            return concreteCommandBuilder.BuildDefinition();
        }
    }
}
