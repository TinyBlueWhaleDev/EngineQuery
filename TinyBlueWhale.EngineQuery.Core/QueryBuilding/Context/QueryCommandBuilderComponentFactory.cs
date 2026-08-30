using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Filtering;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Grouping;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Joining;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Ordering;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Projections;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.SetOperations;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Subqueries;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context
{
    /// <summary>
    /// Creates internal query command builder components.
    /// </summary>
    internal static class QueryCommandBuilderComponentFactory
    {
        public static QueryCommandBuilderComponents<TProfile> Create<TProfile>(QueryCommandBuilderContext context,
            TProfile profile)
            where TProfile : IDatabaseProviderProfile
        {

            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(profile);

            return new QueryCommandBuilderComponents<TProfile>
            {
                SelectProjectionBuilder = new SelectProjectionBuilder(context),
                AggregateProjectionBuilder = new AggregateProjectionBuilder(context),
                ScalarFunctionProjectionBuilder = new ScalarFunctionProjectionBuilder(context),
                ComputedProjectionBuilder = new ComputedProjectionBuilder(context),
                CaseWhenProjectionBuilder = new CaseWhenProjectionBuilder(context),
                WindowFunctionProjectionBuilder = new WindowFunctionProjectionBuilder(context),
                WhereClauseBuilder = new WhereClauseBuilder(context),
                JoinClauseBuilder = new JoinClauseBuilder(context),
                ApplyClauseBuilder = new ApplyClauseBuilder<TProfile>(context),
                GroupByClauseBuilder = new GroupByClauseBuilder(context),
                HavingClauseBuilder = new HavingClauseBuilder(context),
                OrderByClauseBuilder = new OrderByClauseBuilder(context),
                PaginationClauseBuilder = new PaginationClauseBuilder(context),
                ExistsClauseBuilder = new ExistsClauseBuilder<TProfile>(context),
                InSubqueryClauseBuilder = new InSubqueryClauseBuilder<TProfile>(context),
                SetOperationClauseBuilder = new SetOperationClauseBuilder<TProfile>(context)
            };
        }
    }
}
