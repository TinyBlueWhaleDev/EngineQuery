using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Subqueries
{

    /// <summary>
    /// Builds IN subquery query definitions.
    /// </summary>
    internal sealed class InSubqueryClauseBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);
        private readonly NestedQueryCommandBuilderFactory _nestedFactory = new(context);

        public void Add<TOuter, TSubquery>(Expression<Func<TOuter, object>> outerSelector, string? alias, Func<IQueryCommandBuilder<TSubquery>, IQueryCommandBuilder<TSubquery>> subqueryBuilder)
        {
            ArgumentNullException.ThrowIfNull(outerSelector);
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var outerSource = _sourceResolver.Resolve<TOuter>();
            var nestedCommandBuilder = _nestedFactory.CreateMetadataBuilder<TSubquery>(alias);

            nestedCommandBuilder.RegisterOuterSources(
                new Dictionary<Type, QuerySourceDefinition>
                {
                    [typeof(TOuter)] = outerSource
                });

            var configuredBuilder = subqueryBuilder(nestedCommandBuilder);

            var subqueryDefinition = NestedQueryCommandBuilderFactory.ExtractDefinition(configuredBuilder,"The IN subquery builder returned an unsupported query command builder instance.");

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
