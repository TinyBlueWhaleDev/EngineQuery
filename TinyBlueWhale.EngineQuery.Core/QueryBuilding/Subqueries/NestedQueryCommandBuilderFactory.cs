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
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query builder.
    /// </typeparam>
    internal sealed class NestedQueryCommandBuilderFactory<TProfile>(QueryCommandBuilderContext context,
        TProfile profile)
        where TProfile : IDatabaseProviderProfile
    {
        private readonly QueryCommandBuilderContext _context = context ?? throw new ArgumentNullException(nameof(context));

        private readonly TProfile _profile = profile ?? throw new ArgumentNullException(nameof(profile));

        /// <summary>
        /// Creates a nested query command builder using metadata resolved
        /// for the specified subquery entity type.
        /// </summary>
        /// <typeparam name="TSubquery">
        /// Entity type associated with the nested query source.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the nested query source.
        /// </param>
        /// <returns>
        /// Nested query command builder configured from entity metadata.
        /// </returns>
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

        /// <summary>
        /// Creates a nested query builder using the current query compiler,
        /// metadata resolver and database provider profile.
        /// </summary>
        /// <returns>
        /// Query builder configured for nested query construction.
        /// </returns>
        public QueryBuilder<TProfile> CreateQueryBuilder()
        {
            return new QueryBuilder<TProfile>(_context.QueryCompiler, _context.MetadataResolver, _profile);
        }

        /// <summary>
        /// Extracts the compiled query definition from a supported nested
        /// query command builder.
        /// </summary>
        /// <typeparam name="TSubquery">
        /// Entity type associated with the nested query command.
        /// </typeparam>
        /// <param name="commandBuilder">
        /// Query command builder containing the nested query definition.
        /// </param>
        /// <param name="errorMessage">
        /// Error message used when the supplied builder is not supported.
        /// </param>
        /// <returns>
        /// Compiled nested query definition.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the supplied command builder is not a supported
        /// <see cref="QueryCommandBuilder{T, TProfile}"/> instance.
        /// </exception>
        public static CompiledQueryDefinition ExtractDefinition<TSubquery>(IQueryCommandBuilder<TSubquery, TProfile> commandBuilder, string errorMessage)
        {
            if (commandBuilder is not QueryCommandBuilder<TSubquery, TProfile> concreteCommandBuilder)
                throw new InvalidOperationException(errorMessage);

            return concreteCommandBuilder.BuildDefinition();
        }
    }
}
