using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Subqueries
{

    /// <summary>
    /// Creates nested query command builders for subquery scenarios.
    /// </summary>
    internal sealed class NestedQueryCommandBuilderFactory(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;

        public QueryCommandBuilder<TSubquery> CreateMetadataBuilder<TSubquery>(string? alias)
        {
            var metadata = new QuerySourceResolver(_context).ResolveMetadata<TSubquery>();

            var columnMappings = QuerySourceResolver.BuildColumnMappings(metadata);

            return new QueryCommandBuilder<TSubquery>(_context.QueryCompiler, _context.MetadataResolver, metadata.TableName, alias, columnMappings);
        }

        public QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(_context.QueryCompiler, _context.MetadataResolver);
        }

        public static CompiledQueryDefinition ExtractDefinition<TSubquery>(IQueryCommandBuilder<TSubquery> commandBuilder, string errorMessage)
        {
            if (commandBuilder is not QueryCommandBuilder<TSubquery> concreteCommandBuilder)
                throw new InvalidOperationException(errorMessage);

            return concreteCommandBuilder.BuildDefinition();
        }
    }
}
