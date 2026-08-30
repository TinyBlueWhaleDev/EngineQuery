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
    /// Groups internal builders used by query command builders.
    /// </summary>
    internal sealed class QueryCommandBuilderComponents<TProfile>
        where TProfile : IDatabaseProviderProfile
    {
        public required SelectProjectionBuilder SelectProjectionBuilder { get; init; }
        public required AggregateProjectionBuilder AggregateProjectionBuilder { get; init; }
        public required ScalarFunctionProjectionBuilder ScalarFunctionProjectionBuilder { get; init; }
        public required ComputedProjectionBuilder ComputedProjectionBuilder { get; init; }
        public required CaseWhenProjectionBuilder CaseWhenProjectionBuilder { get; init; }
        public required WindowFunctionProjectionBuilder WindowFunctionProjectionBuilder { get; init; }
        public required WhereClauseBuilder WhereClauseBuilder { get; init; }
        public required JoinClauseBuilder JoinClauseBuilder { get; init; }
        public required GroupByClauseBuilder GroupByClauseBuilder { get; init; }
        public required HavingClauseBuilder HavingClauseBuilder { get; init; }
        public required OrderByClauseBuilder OrderByClauseBuilder { get; init; }
        public required PaginationClauseBuilder PaginationClauseBuilder { get; init; }
        public required ApplyClauseBuilder<TProfile> ApplyClauseBuilder { get; init; }
        public required ExistsClauseBuilder<TProfile> ExistsClauseBuilder { get; init; }
        public required InSubqueryClauseBuilder<TProfile> InSubqueryClauseBuilder { get; init; }
        public required SetOperationClauseBuilder<TProfile> SetOperationClauseBuilder { get; init; }
    }
}
