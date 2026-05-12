using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            AddJoinClausesIfNeeded(queryDefinition, sqlLines);

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
            var tableName = _databaseDialect.EscapeIdentifier(queryDefinition.TableName);

            return string.IsNullOrWhiteSpace(queryDefinition.TableAlias)
                ? $"FROM {tableName}"
                : $"FROM {tableName} AS {_databaseDialect.EscapeIdentifier(queryDefinition.TableAlias)}";
        }

        // Adds SQL JOIN clauses when join definitions are configured.
        protected virtual void AddJoinClausesIfNeeded(CompiledQueryDefinition queryDefinition,List<string> sqlLines)
        {
            if (queryDefinition.JoinDefinitions.Count == 0)
                return;

            foreach (var joinDefinition in queryDefinition.JoinDefinitions)
                sqlLines.Add(BuildJoinClause(joinDefinition));
        }
        
        // Builds a SQL JOIN clause from a join definition.
        protected virtual string BuildJoinClause(QueryJoinDefinition joinDefinition)
        {
            var joinKeyword = joinDefinition.JoinType switch
            {
                QueryJoinType.Inner => "INNER JOIN",
                QueryJoinType.Left => "LEFT JOIN",
                _ => throw new NotSupportedException($"Join type '{joinDefinition.JoinType}' is not supported.")
            };

            var tableName = _databaseDialect.EscapeIdentifier(joinDefinition.TableName);
            var tableAlias = _databaseDialect.EscapeIdentifier(joinDefinition.TableAlias);
            var joinCondition = BuildJoinCondition(joinDefinition);

            return $"{joinKeyword} {tableName} AS {tableAlias} ON {joinCondition}";
        }

        // Builds the SQL ON condition associated with a JOIN clause.
        protected virtual string BuildJoinCondition(QueryJoinDefinition joinDefinition)
        {
            if (joinDefinition.JoinExpression.Body is not BinaryExpression binaryExpression)
                throw new NotSupportedException($"Join expression '{joinDefinition.JoinExpression}' is not supported.");

            if (binaryExpression.NodeType != ExpressionType.Equal)
                throw new NotSupportedException($"Join operator '{binaryExpression.NodeType}' is not supported.");

            var leftColumn = BuildJoinColumnReference(
                binaryExpression.Left,
                joinDefinition);

            var rightColumn = BuildJoinColumnReference(
                binaryExpression.Right,
                joinDefinition);

            return $"({leftColumn} = {rightColumn})";
        }

        // Builds a qualified SQL column reference from a join expression member access.
        private string BuildJoinColumnReference(Expression expression, QueryJoinDefinition joinDefinition)
        {
            if (expression is not MemberExpression memberExpression)
                throw new NotSupportedException($"Join expression member '{expression}' is not supported.");

            if (memberExpression.Expression is not ParameterExpression parameterExpression)
                throw new NotSupportedException($"Join expression source '{expression}' is not supported.");

            var propertyName = memberExpression.Member.Name;

            if (parameterExpression.Type == joinDefinition.SourceType)
            {
                var columnName = ResolveMappedColumnName(joinDefinition.SourceColumnMappings, propertyName);

                return _databaseDialect.BuildQualifiedIdentifier(joinDefinition.SourceAlias, columnName);
            }

            if (parameterExpression.Type == joinDefinition.JoinTypeEntity)
            {
                var columnName = ResolveMappedColumnName(joinDefinition.JoinColumnMappings, propertyName);

                return _databaseDialect.BuildQualifiedIdentifier(joinDefinition.TableAlias, columnName);
            }

            throw new NotSupportedException($"Join expression parameter type '{parameterExpression.Type.Name}' is not available in this join.");
        }

        // Resolves mapped column names for join expressions.
        private static string ResolveMappedColumnName(IReadOnlyDictionary<string, string> columnMappings, string propertyName)
        {
            return columnMappings.TryGetValue(propertyName, out var columnName)
                ? columnName
                : propertyName;
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
            var columnName = QueryColumnMappingHelper.ResolveColumnReference(queryDefinition,_databaseDialect,selectDefinition.PropertyName);

            return string.IsNullOrWhiteSpace(selectDefinition.Alias)
                ? columnName
                : $"{columnName} AS {_databaseDialect.EscapeIdentifier(selectDefinition.Alias)}";
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
            return new QueryWhereClauseExpressionParser(_databaseDialect, sqlParameters, queryDefinition.ColumnMappings, queryDefinition.TableAlias);
        }

        // Adds SQL ORDER BY clauses preserving fluent ordering sequence.
        protected virtual void AddOrderByClauseIfNeeded(CompiledQueryDefinition queryDefinition, List<string> sqlLines)
        {
            if (queryDefinition.OrderingDefinitions.Count == 0)
                return;

            var orderingClauses = queryDefinition.OrderingDefinitions
                .Select(orderingDefinition =>
                {
                    var columnReference = QueryColumnMappingHelper.ResolveColumnReference(queryDefinition, _databaseDialect, orderingDefinition.PropertyName);

                    var sqlDirection = orderingDefinition.Direction == QueryOrderingDirection.Ascending
                        ? "ASC"
                        : "DESC";

                    return $"{columnReference} {sqlDirection}";
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
