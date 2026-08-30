using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Subqueries
{

    /// <summary>
    /// Builds IN subquery query definitions.
    /// </summary>
    internal sealed class InSubqueryClauseBuilder<TProfile>(QueryCommandBuilderContext context,
        TProfile profile)
        where TProfile : IDatabaseProviderProfile
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);
        private readonly NestedQueryCommandBuilderFactory<TProfile> _nestedFactory = new(context, profile);
        private readonly QuerySourceAliasResolver _aliasResolver = new(context);

        public void Add<TOuter, TSubquery>(Expression<Func<TOuter, object>> outerSelector,
            string? alias,
            Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            ArgumentNullException.ThrowIfNull(outerSelector);
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var outerSource = _aliasResolver.EnsureAlias<TOuter>(_sourceResolver.Resolve<TOuter>());
            var nestedCommandBuilder = _nestedFactory.CreateMetadataBuilder<TSubquery>(alias);

            nestedCommandBuilder.RegisterOuterSources(
                new Dictionary<Type, QuerySourceDefinition>
                {
                    [typeof(TOuter)] = outerSource
                });

            var configuredBuilder = subqueryBuilder(nestedCommandBuilder);

            var subqueryDefinition = NestedQueryCommandBuilderFactory<TProfile>.ExtractDefinition(configuredBuilder, "The IN subquery builder returned an unsupported query command builder instance.");

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
