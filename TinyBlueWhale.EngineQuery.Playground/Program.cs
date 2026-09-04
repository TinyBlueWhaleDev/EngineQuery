using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Inserts;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Joins;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Pagination;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.SetOperations;
using TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.WindowFunctions;

//BasicSelectQueryValidator.Run();
//TableAliasQueryValidator.Run();
//JoinQueryValidator.Run();
//JoinProjectionQueryValidator.Run();
//MultiSourceWhereQueryValidator.Run();
//MultiSourceWhereIfQueryValidator.Run();
//MultiSourceOrderByQueryValidator.Run();
//GroupByQueryValidator.Run();
//AggregateQueryValidator.Run();
//HavingQueryValidator.Run();
//ScalarFunctionQueryValidator.Run();
//WhereScalarFunctionQueryValidator.Run();
//MultiArgumentScalarFunctionQueryValidator.Run();
//ComputedExpressionQueryValidator.Run();
//WhereComputedExpressionQueryValidator.Run();
//LogicalExpressionQueryValidator.Run();
//CaseWhenQueryValidator.Run();
//ExistsQueryValidator.Run();
//MultiSourceComputedWhereQueryValidator.Run();
//CorrelatedExistsQueryValidator.Run();
//InSubqueryQueryValidator.Run();
//NotExistsQueryValidator.Run();
//DerivedTableQueryValidator.Run();
//UnionQueryValidator.Run();
//UnionAllQueryValidator.Run();
//DistinctQueryValidator.Run();
//AdvancedWindowFunctionQueryValidator.Run();
//ProviderCapabilityQueryValidator.Run();
//DependencyInjectionQueryValidator.Run();
//AggregateComputedExpressionQueryValidator.Run();
//DynamicOrderingPaginationQueryValidator.Run();
//WhereStringContainsQueryValidator.Run();
//WhereLogicalOperatorQueryValidator.Run();
//WhereCollectionQueryValidator.Run();
//InsertCommandQueryValidator.Run();
//UpdateCommandQueryValidator.Run();
//DeleteCommandQueryValidator.Run();

#region Pagination
PaginationQueryValidator.Run();
#endregion

#region Common Table Expressions

//CommonTableExpressionQueryValidator.Run();
//RecursiveCommonTableExpressionQueryValidator.Run();

#endregion

#region Window Functions

//RowNumberWindowFunctionQueryValidator.Run();
//RankingWindowFunctionQueryValidator.Run();
//LagLeadWindowFunctionQueryValidator.Run();
//FirstLastValueWindowFunctionQueryValidator.Run();
//NtileWindowFunctionQueryValidator.Run();

#endregion

#region SetOperations

//IntersectExceptQueryValidator.Run();

#endregion

#region Joins

//ApplyLateralJoinQueryValidator.Run();

#endregion

#region Inserts
//InsertCommandQueryValidator.Run();
#endregion
