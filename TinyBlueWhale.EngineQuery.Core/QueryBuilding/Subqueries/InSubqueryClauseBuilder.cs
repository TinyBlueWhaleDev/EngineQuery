using System.Linq.Expressions;
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
    /// Builds IN subquery query definitions.
    /// </summary>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query builder.
    /// </typeparam>
    internal sealed class InSubqueryClauseBuilder<TProfile>(QueryCommandBuilderContext context, TProfile profile)
        where TProfile : IDatabaseProviderProfile
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);
        private readonly NestedQueryCommandBuilderFactory<TProfile> _nestedFactory = new(context, profile);
        private readonly QuerySourceAliasResolver _aliasResolver = new(context);

        /// <summary>
        /// Adds an IN subquery definition correlated with the specified outer query source.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Entity type associated with the outer query source.
        /// </typeparam>
        /// <typeparam name="TSubquery">
        /// Entity type associated with the subquery source.
        /// </typeparam>
        /// <param name="outerSelector">
        /// Expression selecting the outer value compared against the subquery.
        /// </param>
        /// <param name="alias">
        /// Optional alias assigned to the subquery source.
        /// </param>
        /// <param name="subqueryBuilder">
        /// Delegate used to configure the IN subquery.
        /// </param>
        public void Add<TOuter, TSubquery>(Expression<Func<TOuter, object>> outerSelector, string? alias, Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            ArgumentNullException.ThrowIfNull(outerSelector);
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
                "The IN subquery builder returned an unsupported query command builder instance.");

            _context.QueryDefinition.InSubqueryDefinitions.Add(
                new QueryInSubqueryDefinition
                {
                    OuterSelector = outerSelector,
                    OuterSource = outerSource,
                    Subquery = subqueryDefinition
                });
        }
    }
}
