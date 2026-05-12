using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.ExpressionsParsing;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation
{
    /// <summary>
    /// Provides a shared base implementation for SQL query compilers.
    /// </summary>
    /// <remarks>
    /// This compiler transforms query definitions into provider-specific SQL command text
    /// while delegating syntax differences to the configured database dialect.
    /// </remarks>
    public abstract class QueryCompilerBase(ISqlDatabaseDialect databaseDialect) : IQueryCompiler
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect ?? throw new ArgumentNullException(nameof(databaseDialect));

        /// <summary>
        /// Compiles the specified query definition into SQL command text and parameters.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition containing projections, filters, ordering and pagination metadata.
        /// </param>
        /// <returns>
        /// Generated SQL query command.
        /// </returns>
        public GeneratedSqlQuery Compile(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            var sqlParameters = new List<QuerySqlParameter>();

            var sqlLines = new List<string>
            {
                BuildSelectClause(queryDefinition),
                BuildFromClause(queryDefinition)
            };

            AddWhereClauseIfNeeded(queryDefinition, sqlParameters, sqlLines);

            AddOrderByClauseIfNeeded(queryDefinition, sqlLines);

            AddPaginationClauseIfNeeded(queryDefinition, sqlLines);

            return new GeneratedSqlQuery
            {
                CommandText = string.Join(
                    Environment.NewLine,
                    sqlLines),

                Parameters = sqlParameters
            };
        }

        // Builds the SQL FROM clause.
        protected virtual string BuildFromClause(CompiledQueryDefinition queryDefinition)
        {
            return $"FROM {_databaseDialect.EscapeIdentifier(queryDefinition.TableName)}";
        }

        // Builds the SQL SELECT clause from query projections.
        protected virtual string BuildSelectClause(CompiledQueryDefinition queryDefinition)
        {
            if (queryDefinition.SelectDefinitions.Count == 0)
                return "SELECT *";

            var selectedColumns = queryDefinition.SelectDefinitions
                .Select(selectDefinition => BuildSelectColumn(queryDefinition,selectDefinition));

            return $"SELECT {string.Join(", ", selectedColumns)}";
        }

        // Builds a SQL SELECT column fragment including optional alias projection.
        protected virtual string BuildSelectColumn(CompiledQueryDefinition queryDefinition, QuerySelectColumnDefinition selectDefinition)
        {
            var columnName = _databaseDialect.EscapeIdentifier(QueryColumnMappingHelper.ResolveColumnName(queryDefinition,selectDefinition.PropertyName));

            if (string.IsNullOrWhiteSpace(selectDefinition.Alias))
                return columnName;

            return $"{columnName} AS {_databaseDialect.EscapeIdentifier(selectDefinition.Alias)}";
        }

        // Adds SQL WHERE conditions when filters are defined.
        protected virtual void AddWhereClauseIfNeeded(CompiledQueryDefinition queryDefinition, List<QuerySqlParameter> sqlParameters, List<string> sqlLines)
        {
            if (queryDefinition.WhereDefinitions.Count == 0)
                return;

            var whereConditions = queryDefinition.WhereDefinitions
                .Select(whereDefinition =>
                {
                    var parser = CreateWhereClauseExpressionParser(sqlParameters,queryDefinition);

                    return parser.ParseToSqlCondition(whereDefinition.PredicateExpression.Body);
                });

            sqlLines.Add("WHERE " + string.Join(" AND ", whereConditions));
        }

        // Creates a SQL WHERE clause expression parser instance.
        protected virtual QueryWhereClauseExpressionParser CreateWhereClauseExpressionParser(List<QuerySqlParameter> sqlParameters,CompiledQueryDefinition queryDefinition)
        {
            return new QueryWhereClauseExpressionParser(_databaseDialect, sqlParameters, queryDefinition.ColumnMappings);
        }

        // Adds SQL ORDER BY clauses preserving fluent ordering sequence.
        protected virtual void AddOrderByClauseIfNeeded(CompiledQueryDefinition queryDefinition, List<string> sqlLines)
        {
            if (queryDefinition.OrderingDefinitions.Count == 0)
                return;

            var orderingClauses = queryDefinition.OrderingDefinitions
                .Select(orderingDefinition =>
                {
                    var columnName = QueryColumnMappingHelper.ResolveColumnName(queryDefinition, orderingDefinition.PropertyName);

                    var sqlDirection = orderingDefinition.Direction == QueryOrderingDirection.Ascending ? "ASC" : "DESC";

                    return $"{_databaseDialect.EscapeIdentifier(columnName)} {sqlDirection}";
                });

            sqlLines.Add("ORDER BY " + string.Join(", ", orderingClauses));
        }

        // Adds provider-specific pagination syntax when pagination is enabled.
        protected virtual void AddPaginationClauseIfNeeded(CompiledQueryDefinition queryDefinition, List<string> sqlLines)
        {
            if (!queryDefinition.Pagination.HasPagination)
                return;

            if (queryDefinition.OrderingDefinitions.Count == 0)
                throw new InvalidOperationException("Pagination requires at least one ORDER BY clause.");

            sqlLines.Add(_databaseDialect.BuildPaginationClause(queryDefinition.Pagination.Skip, queryDefinition.Pagination.Take));
        }
    }
}
