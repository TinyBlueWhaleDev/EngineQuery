
using TinyBlueWhale.EngineQuery.Abstractions.Enums;

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
        /// Gets or sets the source table name associated with the query.
        /// </summary>
        public required string TableName { get; set; }

        /// <summary>
        /// Gets or sets the optional table alias used to qualify generated SQL column references.
        /// </summary>
        public string? TableAlias { get; set; }

        /// <summary>
        /// Gets the SQL join definitions associated with the query.
        /// </summary>
        public List<QueryJoinDefinition> JoinDefinitions { get; } = [];

        /// <summary>
        /// Gets the APPLY join definitions associated with the query.
        /// </summary>
        public List<QueryApplyDefinition> ApplyDefinitions { get; } = [];

        /// <summary>
        /// Gets or sets the property-to-column mapping used during SQL generation.
        /// </summary>
        public IReadOnlyDictionary<string, string> ColumnMappings { get; set; } = new Dictionary<string, string>();

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

        /// /// <summary>
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
        /// Gets the query sources inherited from an outer query scope.
        /// </summary>
        public Dictionary<Type, QuerySourceDefinition> OuterSourceDefinitions { get; } = [];

        /// <summary>
        /// Gets the query sources available in the current SQL generation scope.
        /// </summary>
        public Dictionary<Type, QuerySourceDefinition> SourceDefinitions { get; } = [];

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
        /// Gets or sets the root entity type associated with the query.
        /// </summary>
        public required Type EntityType { get; set; }

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
