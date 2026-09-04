using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Filtering;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Subqueries
{

    /// <summary>
    /// Builds EXISTS and NOT EXISTS query definitions.
    /// </summary>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query builder.
    /// </typeparam>
    internal sealed class ExistsClauseBuilder<TProfile>(QueryCommandBuilderContext context, TProfile profile)
        where TProfile : IDatabaseProviderProfile
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);
        private readonly NestedQueryCommandBuilderFactory<TProfile> _nestedFactory = new(context, profile);
        private readonly QuerySourceAliasResolver _aliasResolver = new(context);

        /// <summary>
        /// Adds a non-correlated EXISTS subquery definition.
        /// </summary>
        /// <typeparam name="TSubquery">
        /// Entity type associated with the subquery source.
        /// </typeparam>
        /// <param name="subqueryBuilder">
        /// Delegate used to configure the EXISTS subquery.
        /// </param>
        public void Add<TSubquery>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var nestedQueryBuilder = _nestedFactory.CreateQueryBuilder();
            var nestedCommandBuilder = subqueryBuilder(nestedQueryBuilder);

            var subqueryDefinition = NestedQueryCommandBuilderFactory<TProfile>.ExtractDefinition(
                nestedCommandBuilder,
                "The EXISTS subquery builder returned an unsupported query command builder instance.");

            subqueryDefinition.UseConstantSelectProjection = true;

            _context.QueryDefinition.ExistsDefinitions.Add(
                new QueryExistsDefinition
                {
                    Subquery = subqueryDefinition
                });
        }

        /// <summary>
        /// Adds a correlated EXISTS or NOT EXISTS subquery definition.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Entity type associated with the outer query source.
        /// </typeparam>
        /// <typeparam name="TSubquery">
        /// Entity type associated with the subquery source.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the subquery source.
        /// </param>
        /// <param name="subqueryBuilder">
        /// Delegate used to configure the correlated subquery.
        /// </param>
        /// <param name="isNegated">
        /// Indicates whether the generated definition represents NOT EXISTS.
        /// </param>
        public void AddCorrelated<TOuter, TSubquery>(string? alias, Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder, bool isNegated)
        {
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var outerSource = _aliasResolver.EnsureAlias(_sourceResolver.Resolve<TOuter>());
            var nestedCommandBuilder = _nestedFactory.CreateMetadataBuilder<TSubquery>(alias);

            nestedCommandBuilder.RegisterOuterSources(
                new[]
                {
                    outerSource
                });

            var configuredBuilder = subqueryBuilder(nestedCommandBuilder);

            var subqueryDefinition = NestedQueryCommandBuilderFactory<TProfile>.ExtractDefinition(
                configuredBuilder,
                isNegated
                    ? "The NOT EXISTS subquery builder returned an unsupported query command builder instance."
                    : "The EXISTS subquery builder returned an unsupported query command builder instance.");

            subqueryDefinition.UseConstantSelectProjection = true;

            _context.QueryDefinition.ExistsDefinitions.Add(
                new QueryExistsDefinition
                {
                    Subquery = subqueryDefinition,
                    IsNegated = isNegated
                });
        }
    }
}
