using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Aggregates;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Aliases;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.CommonTableExpressions;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Cte;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Deletes;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.DependencyInjection;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Expressions;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Inserts;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Joins;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Pagination;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Predicates;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.ScalarFunctions;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Selects;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.SetOperations;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Subqueries;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Updates;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.WindowFunctions;

#region Selects

BasicSelectQueryValidator.Run();
DistinctQueryValidator.Run();

#endregion

#region Aliases

TableAliasQueryValidator.Run();

#endregion

#region Joins

JoinQueryValidator.Run();
JoinProjectionQueryValidator.Run();
MultiSourceWhereQueryValidator.Run();
MultiSourceWhereIfQueryValidator.Run();
MultiSourceOrderByQueryValidator.Run();
MultiSourceComputedWhereQueryValidator.Run();
ApplyLateralJoinQueryValidator.Run();

#endregion

#region Predicates

WhereCollectionQueryValidator.Run();
WhereLogicalOperatorQueryValidator.Run();
WhereStringContainsQueryValidator.Run();

#endregion

#region Expressions

LogicalExpressionQueryValidator.Run();
WhereComputedExpressionQueryValidator.Run();
CaseWhenQueryValidator.Run();

#endregion

#region Scalar Functions

ScalarFunctionQueryValidator.Run();
MultiArgumentScalarFunctionQueryValidator.Run();
WhereScalarFunctionQueryValidator.Run();

#endregion

#region Aggregates

AggregateQueryValidator.Run();
AggregateComputedExpressionQueryValidator.Run();
GroupByQueryValidator.Run();
HavingQueryValidator.Run();

#endregion

#region Pagination

PaginationQueryValidator.Run();
DynamicOrderingPaginationQueryValidator.Run();

#endregion

#region Subqueries

ExistsQueryValidator.Run();
CorrelatedExistsQueryValidator.Run();
NotExistsQueryValidator.Run();
InSubqueryQueryValidator.Run();
DerivedTableQueryValidator.Run();

#endregion

#region Common Table Expressions

CommonTableExpressionQueryValidator.Run();
RecursiveCommonTableExpressionQueryValidator.Run();

#endregion

#region Window Functions

RowNumberWindowFunctionQueryValidator.Run();
RankingWindowFunctionQueryValidator.Run();
LagLeadWindowFunctionQueryValidator.Run();
FirstLastValueWindowFunctionQueryValidator.Run();
NtileWindowFunctionQueryValidator.Run();

#endregion

#region Set Operations

UnionQueryValidator.Run();
UnionAllQueryValidator.Run();
IntersectExceptQueryValidator.Run();

#endregion

#region Inserts

InsertCommandQueryValidator.Run();

#endregion

#region Updates

UpdateCommandQueryValidator.Run();

#endregion

#region Deletes

DeleteCommandQueryValidator.Run();

#endregion

#region Dependency Injection

DependencyInjectionQueryValidator.Run();

#endregion
