
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Commands;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Cte;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Filtering;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Grouping;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Ordering;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Pagination;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.SetOperations;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents the internal query definition used by the SQL compiler.
    /// </summary>
    /// <remarks>
    /// This model captures query intent before SQL text is generated.
    /// It is not exposed to consumers of the public API.
    /// </remarks>
    public sealed class CompiledQueryDefinition
    {
        /// <summary>
        /// Gets or sets the SQL command type represented by the query definition.
        /// </summary>
        public QueryCommandType CommandType { get; set; } = QueryCommandType.Select;

        /// <summary>
        /// Gets the SQL join definitions associated with the query.
        /// </summary>
        public List<QueryJoinDefinition> JoinDefinitions { get; } = [];

        /// <summary>
        /// Gets the APPLY join definitions associated with the query.
        /// </summary>
        public List<QueryApplyDefinition> ApplyDefinitions { get; } = [];

        /// <summary>
        /// Gets the selected columns included in the query projection.
        /// </summary>
        public List<QuerySelectColumnDefinition> SelectDefinitions { get; } = [];

        /// <summary>
        /// Gets the common table expressions associated with the query.
        /// </summary>
        public List<QueryCteDefinition> CteDefinitions { get; } = [];

        /// <summary>
        /// Gets CASE WHEN SELECT expressions associated with the query.
        /// </summary>
        public List<QueryCaseWhenDefinition> CaseWhenDefinitions { get; } = [];

        /// <summary>
        /// Gets the EXISTS conditions associated with the query.
        /// </summary>
        public List<QueryExistsDefinition> ExistsDefinitions { get; } = [];

        /// <summary>
        /// Gets or sets whether the query should generate a constant SELECT projection.
        /// </summary>
        public bool UseConstantSelectProjection { get; set; }

        /// <summary>
        /// Gets or sets whether the query should apply DISTINCT projection semantics.
        /// </summary>
        public bool IsDistinct { get; set; }

        /// <summary>
        /// Gets SQL window function projections associated with the query.
        /// </summary>
        public List<QueryWindowFunctionDefinition> WindowFunctionDefinitions { get; } = [];

        /// <summary>
        /// Gets the SQL set operation definitions associated with the query.
        /// </summary>
        public List<QuerySetOperationDefinition> SetOperationDefinitions { get; } = [];

        /// <summary>
        /// Gets the IN subquery conditions associated with the query.
        /// </summary>
        public List<QueryInSubqueryDefinition> InSubqueryDefinitions { get; } = [];

        /// <summary>
        /// Gets the IN and NOT IN collection conditions associated with the query.
        /// </summary>
        public List<QueryWhereCollectionDefinition> WhereCollectionDefinitions { get; } = [];

        /// <summary>
        /// Gets or sets the root query source associated with the current query scope.
        /// </summary>
        public required QuerySourceDefinition RootSource { get; set; }

        /// <summary>
        /// Gets the query sources available in the current query scope.
        /// </summary>
        public List<QuerySourceDefinition> Sources { get; } = [];

        /// <summary>
        /// Gets the query sources inherited from the outer query scope.
        /// </summary>
        public List<QuerySourceDefinition> OuterSources { get; } = [];

        /// <summary>
        /// Gets the SQL aggregate SELECT definitions associated with the query.
        /// </summary>
        public List<QueryAggregateDefinition> AggregateDefinitions { get; } = [];

        /// <summary>
        /// Gets scalar SQL function projections associated with the query.
        /// </summary>
        public List<QueryScalarFunctionDefinition> ScalarFunctionDefinitions { get; } = [];

        /// <summary>
        /// Gets computed SELECT expressions associated with the query.
        /// </summary>
        public List<QueryComputedExpressionDefinition> ComputedExpressionDefinitions { get; } = [];

        /// <summary>
        /// Gets the filtering definitions used to generate SQL WHERE clauses.
        /// </summary>
        public List<QueryWhereDefinition> WhereDefinitions { get; } = [];

        /// <summary>
        /// Gets the SQL WHERE scalar function definitions associated with the query.
        /// </summary>
        public List<QueryWhereScalarFunctionDefinition> WhereScalarFunctionDefinitions { get; } = [];

        /// <summary>
        /// Gets computed SQL WHERE expressions associated with the query.
        /// </summary>
        public List<QueryWhereComputedExpressionDefinition> WhereComputedExpressionDefinitions { get; } = [];

        /// <summary>
        /// Gets the SQL GROUP BY definitions associated with the query.
        /// </summary>
        public List<QueryGroupByDefinition> GroupByDefinitions { get; } = [];

        /// <summary>
        /// Gets the SQL HAVING aggregate definitions associated with the query.
        /// </summary>
        public List<QueryHavingAggregateDefinition> HavingAggregateDefinitions { get; } = [];

        /// <summary>
        /// Gets the ordering definitions used to generate SQL ORDER BY clauses.
        /// </summary>
        public List<QueryOrderingDefinition> OrderingDefinitions { get; } = [];

        /// <summary>
        /// Gets or sets the pagination definition used to generate SQL paging syntax.
        /// </summary>
        public QueryPaginationDefinition Pagination { get; set; } = new();

        /// <summary>
        /// Gets or sets whether selected columns should always be projected using CLR property aliases.
        /// </summary>
        public bool ForceSelectAliases { get; set; }

        /// <summary>
        /// Gets or sets the INSERT-specific command definition.
        /// </summary>
        public QueryInsertDefinition? InsertDefinition { get; set; }

        /// <summary>
        /// Gets or sets the UPDATE-specific intent associated with the compiled query definition.
        /// </summary>
        public QueryUpdateDefinition? UpdateDefinition { get; set; }

    }
}
