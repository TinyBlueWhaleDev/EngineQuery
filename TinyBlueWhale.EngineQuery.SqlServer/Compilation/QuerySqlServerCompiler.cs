using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionParsing;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.SqlServer.Compilation
{
    /// <summary>
    /// Compiles query definitions into provider-specific SQL command text.
    /// </summary>
    /// <remarks>
    /// The compiler is responsible only for SQL generation.
    /// It does not execute queries or manage database connections.
    /// </remarks>
    public sealed class QuerySqlServerCompiler(ISqlDatabaseDialect databaseDialect) : IQueryCompiler
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect;

        /// <summary>
        /// Compiles the specified query definition into SQL command text and parameters.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition containing projections, filters, ordering and pagination metadata.
        /// </param>
        /// <returns>
        /// Generated SQL query command.
        /// </returns>
        /// <remarks>
        /// SQL generation is deterministic:
        /// compiling the same query definition multiple times produces identical SQL and parameter ordering.
        /// </remarks>
        public GeneratedSqlQuery Compile(CompiledQueryDefinition queryDefinition)
        {
            var sqlParameters = new List<QuerySqlParameter>();

            var sqlLines = new List<string>
            {
                BuildSelectClause(queryDefinition),
                $"FROM {_databaseDialect.EscapeIdentifier(queryDefinition.TableName)}"
            };

            AddWhereClauseIfNeeded(queryDefinition, sqlParameters, sqlLines);

            AddOrderByClauseIfNeeded(queryDefinition, sqlLines);

            AddPaginationClauseIfNeeded(queryDefinition, sqlLines);

            return new GeneratedSqlQuery
            {
                CommandText = string.Join(Environment.NewLine, sqlLines),
                Parameters = sqlParameters
            };
        }

        // Builds the SQL SELECT clause from query projections.
        private string BuildSelectClause(CompiledQueryDefinition queryDefinition)
        {
            if (queryDefinition.SelectDefinitions.Count == 0)
                return "SELECT *";

            var selectedColumns = queryDefinition.SelectDefinitions
                .Select(selectDefinition => _databaseDialect.EscapeIdentifier(
                        ResolveColumnName(queryDefinition,selectDefinition.PropertyName)));

            return "SELECT " + string.Join(", ", selectedColumns);
        }

        // Adds SQL WHERE conditions when filters are defined.
        private void AddWhereClauseIfNeeded(CompiledQueryDefinition queryDefinition, List<QuerySqlParameter> sqlParameters, List<string> sqlLines)
        {
            if (queryDefinition.WhereDefinitions.Count == 0)
                return;

            var whereConditions = queryDefinition.WhereDefinitions
                .Select(whereDefinition =>
                {
                    var parser = new QueryWhereClauseExpressionParser(_databaseDialect, sqlParameters, queryDefinition.ColumnMappings);

                    return parser.ParseToSqlCondition(whereDefinition.PredicateExpression.Body);
                })
                .ToList();

            sqlLines.Add("WHERE " + string.Join(" AND ", whereConditions));
        }

        // Adds SQL ORDER BY clauses preserving fluent ordering sequence.
        private void AddOrderByClauseIfNeeded(CompiledQueryDefinition queryDefinition,List<string> sqlLines)
        {
            if (queryDefinition.OrderingDefinitions.Count == 0)
                return;

            var orderingClauses = queryDefinition.OrderingDefinitions
                .Select(orderingDefinition =>
                {
                    var sqlDirection = orderingDefinition.Direction == QueryOrderingDirection.Ascending? "ASC" : "DESC";

                    return $"{_databaseDialect.EscapeIdentifier(ResolveColumnName(queryDefinition, orderingDefinition.PropertyName))} {sqlDirection}";
                });

            sqlLines.Add("ORDER BY " + string.Join(", ", orderingClauses));
        }

        // Adds provider-specific pagination syntax when pagination is enabled. 
        private void AddPaginationClauseIfNeeded(CompiledQueryDefinition queryDefinition,List<string> sqlLines)
        {
            if (!queryDefinition.Pagination.HasPagination)
                return;

            if (queryDefinition.OrderingDefinitions.Count == 0)
                throw new InvalidOperationException("SQL Server pagination requires at least one ORDER BY clause.");

            var paginationClause = _databaseDialect.BuildPaginationClause(queryDefinition.Pagination.Skip, queryDefinition.Pagination.Take);

            sqlLines.Add(paginationClause);
        }

        // Resolves the database column name associated with a CLR property.
        private string ResolveColumnName(CompiledQueryDefinition queryDefinition,string propertyName)
        {
            return queryDefinition.ColumnMappings.TryGetValue(propertyName,out var columnName) ? columnName : propertyName;
        }
    }
}
