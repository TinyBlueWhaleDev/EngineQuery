using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionParsing;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.MySql.Compilation
{
    /// <summary>
    /// Compiles query definitions into MySQL SQL command text.
    /// </summary>
    /// <remarks>
    /// This compiler generates MySQL-compatible SQL fragments and delegates
    /// provider-specific syntax rules to the configured database dialect.
    /// </remarks>
    public sealed class MySqlQueryCompiler(ISqlDatabaseDialect databaseDialect) : IQueryCompiler
    {
        private readonly ISqlDatabaseDialect _databaseDialect =
            databaseDialect ?? throw new ArgumentNullException(nameof(databaseDialect));

        /// <summary>
        /// Compiles the specified query definition into MySQL SQL command text and parameters.
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
            {
                return "SELECT *";
            }

            var selectedColumns = queryDefinition.SelectDefinitions
                .Select(selectDefinition => BuildSelectColumn(queryDefinition, selectDefinition));

            return $"SELECT {string.Join(", ", selectedColumns)}";
        }

        // Builds a SQL SELECT column fragment including optional alias projection.
        private string BuildSelectColumn(
            CompiledQueryDefinition queryDefinition,
            QuerySelectColumnDefinition selectDefinition)
        {
            var columnName = _databaseDialect.EscapeIdentifier(
                QueryColumnMappingHelper.ResolveColumnName(
                    queryDefinition,
                    selectDefinition.PropertyName));

            if (string.IsNullOrWhiteSpace(selectDefinition.Alias))
            {
                return columnName;
            }

            return $"{columnName} AS {_databaseDialect.EscapeIdentifier(selectDefinition.Alias)}";
        }

        // Adds SQL WHERE conditions when filters are defined.
        private void AddWhereClauseIfNeeded(
            CompiledQueryDefinition queryDefinition,
            List<QuerySqlParameter> sqlParameters,
            List<string> sqlLines)
        {
            if (queryDefinition.WhereDefinitions.Count == 0)
            {
                return;
            }

            var whereConditions = queryDefinition.WhereDefinitions
                .Select(whereDefinition =>
                {
                    var parser = new QueryWhereClauseExpressionParser(
                        _databaseDialect,
                        sqlParameters,
                        queryDefinition.ColumnMappings);

                    return parser.ParseToSqlCondition(whereDefinition.PredicateExpression.Body);
                })
                .ToList();

            sqlLines.Add("WHERE " + string.Join(" AND ", whereConditions));
        }

        // Adds SQL ORDER BY clauses preserving fluent ordering sequence.
        private void AddOrderByClauseIfNeeded(
            CompiledQueryDefinition queryDefinition,
            List<string> sqlLines)
        {
            if (queryDefinition.OrderingDefinitions.Count == 0)
            {
                return;
            }

            var orderingClauses = queryDefinition.OrderingDefinitions
                .Select(orderingDefinition =>
                {
                    var columnName = QueryColumnMappingHelper.ResolveColumnName(
                        queryDefinition,
                        orderingDefinition.PropertyName);

                    var sqlDirection = orderingDefinition.Direction == QueryOrderingDirection.Ascending
                        ? "ASC"
                        : "DESC";

                    return $"{_databaseDialect.EscapeIdentifier(columnName)} {sqlDirection}";
                });

            sqlLines.Add("ORDER BY " + string.Join(", ", orderingClauses));
        }

        // Adds provider-specific pagination syntax when pagination is enabled.
        private void AddPaginationClauseIfNeeded(
            CompiledQueryDefinition queryDefinition,
            List<string> sqlLines)
        {
            if (!queryDefinition.Pagination.HasPagination)
            {
                return;
            }

            if (queryDefinition.OrderingDefinitions.Count == 0)
            {
                throw new InvalidOperationException(
                    "Pagination requires at least one ORDER BY clause.");
            }

            var paginationClause = _databaseDialect.BuildPaginationClause(
                queryDefinition.Pagination.Skip,
                queryDefinition.Pagination.Take);

            sqlLines.Add(paginationClause);
        }
    }
}
