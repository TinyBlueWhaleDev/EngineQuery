using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Sql.Compilation.Models;
using TinyBlueWhale.EngineQuery.Sql.Dialects.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Enums;
using TinyBlueWhale.EngineQuery.Sql.ExpressionParsing;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation
{
    public sealed class QuerySqlCompiler(ISqlDatabaseDialect databaseDialect)
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect;

        public GeneratedSqlQuery CompileToSql(CompiledQueryDefinition queryDefinition)
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

        private string BuildSelectClause(CompiledQueryDefinition queryDefinition)
        {
            if (queryDefinition.SelectDefinitions.Count == 0)
                return "SELECT *";

            var selectedColumns = queryDefinition.SelectDefinitions
                .Select(selectDefinition => _databaseDialect.EscapeIdentifier(selectDefinition.PropertyName));

            return "SELECT " + string.Join(", ", selectedColumns);
        }
        private void AddWhereClauseIfNeeded(CompiledQueryDefinition queryDefinition, List<QuerySqlParameter> sqlParameters, List<string> sqlLines)
        {
            if (queryDefinition.WhereDefinitions.Count == 0)
                return;

            var whereConditions = queryDefinition.WhereDefinitions
                .Select(whereDefinition =>
                {
                    var parser = new QueryWhereClauseExpressionParser(_databaseDialect, sqlParameters);

                    return parser.ParseToSqlCondition(whereDefinition.PredicateExpression.Body);
                })
                .ToList();

            sqlLines.Add("WHERE " + string.Join(" AND ", whereConditions));
        }

        private void AddOrderByClauseIfNeeded(CompiledQueryDefinition queryDefinition,List<string> sqlLines)
        {
            if (queryDefinition.OrderingDefinitions.Count == 0)
                return;

            var orderingClauses = queryDefinition.OrderingDefinitions
                .Select(orderingDefinition =>
                {
                    var sqlDirection = orderingDefinition.Direction == QueryOrderingDirection.Ascending? "ASC" : "DESC";

                    return $"{_databaseDialect.EscapeIdentifier(orderingDefinition.PropertyName)} {sqlDirection}";
                });

            sqlLines.Add("ORDER BY " + string.Join(", ", orderingClauses));
        }

        private void AddPaginationClauseIfNeeded(CompiledQueryDefinition queryDefinition,List<string> sqlLines)
        {
            if (!queryDefinition.Pagination.HasPagination)
                return;

            if (queryDefinition.OrderingDefinitions.Count == 0)
                throw new InvalidOperationException("SQL Server pagination requires at least one ORDER BY clause.");

            var paginationClause = _databaseDialect.BuildPaginationClause(queryDefinition.Pagination.Skip, queryDefinition.Pagination.Take);

            sqlLines.Add(paginationClause);
        }
    }
}
