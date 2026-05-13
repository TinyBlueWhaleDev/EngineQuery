using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
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

            AddGroupByClauseIfNeeded(queryDefinition, sqlLines);

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


        #region Select Clause Building        
        // Builds the SQL SELECT clause from query projections and aggregate expressions.
        protected virtual string BuildSelectClause(CompiledQueryDefinition queryDefinition)
        {
            if (queryDefinition.SelectDefinitions.Count == 0 &&
                queryDefinition.AggregateDefinitions.Count == 0)
                return "SELECT *";

            var selectedColumns = queryDefinition.SelectDefinitions
                .Select(selectDefinition => BuildSelectColumn(queryDefinition, selectDefinition));

            var aggregateColumns = queryDefinition.AggregateDefinitions
                .Select(aggregateDefinition => BuildAggregateColumn(aggregateDefinition));

            return $"SELECT {string.Join(", ", selectedColumns.Concat(aggregateColumns))}";
        }

        // Builds a SQL SELECT column fragment including optional alias projection.
        protected virtual string BuildSelectColumn(CompiledQueryDefinition queryDefinition,QuerySelectColumnDefinition selectDefinition)
        {
            var columnReference = BuildSelectColumnReference(queryDefinition, selectDefinition);

            return string.IsNullOrWhiteSpace(selectDefinition.Alias)
                ? columnReference
                : $"{columnReference} AS {_databaseDialect.EscapeIdentifier(selectDefinition.Alias)}";
        }

        // Builds a SQL aggregate SELECT expression.
        protected virtual string BuildAggregateColumn(QueryAggregateDefinition aggregateDefinition)
        {
            var columnName = ResolveMappedColumnName(aggregateDefinition.SourceColumnMappings, aggregateDefinition.PropertyName);

            var columnReference = string.IsNullOrWhiteSpace(aggregateDefinition.SourceAlias)
                ? _databaseDialect.EscapeIdentifier(columnName)
                : _databaseDialect.BuildQualifiedIdentifier(aggregateDefinition.SourceAlias, columnName);

            return $"{ResolveAggregateFunctionName(aggregateDefinition.Function)}({columnReference}) AS {_databaseDialect.EscapeIdentifier(aggregateDefinition.Alias)}";
        }

        // Resolves the SQL aggregate function name.
        private static string ResolveAggregateFunctionName(
            QueryAggregateFunction function)
        {
            return function switch
            {
                QueryAggregateFunction.Count => "COUNT",
                QueryAggregateFunction.Sum => "SUM",
                QueryAggregateFunction.Average => "AVG",
                QueryAggregateFunction.Minimum => "MIN",
                QueryAggregateFunction.Maximum => "MAX",
                _ => throw new NotSupportedException($"Aggregate function '{function}' is not supported.")
            };
        }

        // Builds a SQL column reference for single-source and multi-source projections.
        private string BuildSelectColumnReference(CompiledQueryDefinition queryDefinition,QuerySelectColumnDefinition selectDefinition)
        {
            if (!string.IsNullOrWhiteSpace(selectDefinition.SourceAlias))
            {
                var columnName = ResolveMappedColumnName(selectDefinition.SourceColumnMappings,selectDefinition.PropertyName);

                return _databaseDialect.BuildQualifiedIdentifier(selectDefinition.SourceAlias,columnName);
            }

            return QueryColumnMappingHelper.ResolveColumnReference(queryDefinition,_databaseDialect,selectDefinition.PropertyName);
        }

        #endregion

        #region From Clause Building
        // Builds the SQL FROM clause.
        protected virtual string BuildFromClause(CompiledQueryDefinition queryDefinition)
        {
            var tableName = _databaseDialect.EscapeIdentifier(queryDefinition.TableName);

            return string.IsNullOrWhiteSpace(queryDefinition.TableAlias)
                ? $"FROM {tableName}"
                : $"FROM {tableName} AS {_databaseDialect.EscapeIdentifier(queryDefinition.TableAlias)}";
        }
        #endregion

        #region Join Clause Building
        // Adds SQL JOIN clauses when join definitions are configured.
        protected virtual void AddJoinClausesIfNeeded(CompiledQueryDefinition queryDefinition, List<string> sqlLines)
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

        #endregion

        #region Where Clause Building
        // Adds SQL WHERE conditions when filters are defined.
        protected virtual void AddWhereClauseIfNeeded(CompiledQueryDefinition queryDefinition, List<QuerySqlParameter> sqlParameters, List<string> sqlLines)
        {
            if (queryDefinition.WhereDefinitions.Count == 0)
                return;

            var whereConditions = queryDefinition.WhereDefinitions
                .Select(whereDefinition =>
                {
                    var parser = CreateWhereClauseExpressionParser(sqlParameters,queryDefinition, whereDefinition);

                    return parser.ParseToSqlCondition(whereDefinition.PredicateExpression.Body);
                });

            sqlLines.Add("WHERE " + string.Join(" AND ", whereConditions));
        }

        // Creates a SQL WHERE clause expression parser instance.
        protected virtual QueryWhereClauseExpressionParser CreateWhereClauseExpressionParser(List<QuerySqlParameter> sqlParameters, CompiledQueryDefinition queryDefinition, QueryWhereDefinition whereDefinition)
        {
            return new QueryWhereClauseExpressionParser(
                _databaseDialect,
                sqlParameters,
                whereDefinition.SourceColumnMappings ?? queryDefinition.ColumnMappings,
                whereDefinition.SourceAlias ?? queryDefinition.TableAlias);
        }

        #endregion

        #region Group By Clause Building

        // Adds SQL GROUP BY clauses when grouping definitions are configured.
        protected virtual void AddGroupByClauseIfNeeded(CompiledQueryDefinition queryDefinition,List<string> sqlLines)
        {
            if (queryDefinition.GroupByDefinitions.Count == 0)
                return;

            var groupByClauses = queryDefinition.GroupByDefinitions
                .SelectMany(groupByDefinition =>
                    BuildGroupByColumnReferences(queryDefinition, groupByDefinition));

            sqlLines.Add("GROUP BY " + string.Join(", ", groupByClauses));
        }

        // Builds SQL column references for all columns contained in a GROUP BY definition.
        private IEnumerable<string> BuildGroupByColumnReferences(CompiledQueryDefinition queryDefinition, QueryGroupByDefinition groupByDefinition)
        {
            foreach (var groupByColumn in groupByDefinition.Columns)
                yield return BuildGroupByColumnReference(queryDefinition, groupByDefinition, groupByColumn);
        }

        // Builds a SQL column reference for a single GROUP BY column.
        private string BuildGroupByColumnReference(CompiledQueryDefinition queryDefinition, QueryGroupByDefinition groupByDefinition, QueryColumnDefinition groupByColumn)
        {
            if (!string.IsNullOrWhiteSpace(groupByDefinition.SourceAlias))
            {
                var columnName = ResolveMappedColumnName(groupByDefinition.SourceColumnMappings, groupByColumn.PropertyName);

                return _databaseDialect.BuildQualifiedIdentifier(groupByDefinition.SourceAlias, columnName);
            }

            return QueryColumnMappingHelper.ResolveColumnReference(queryDefinition, _databaseDialect, groupByColumn.PropertyName);
        }
        #endregion


        #region Order By Clause Building

        // Adds SQL ORDER BY clauses preserving fluent ordering sequence.
        protected virtual void AddOrderByClauseIfNeeded(
            CompiledQueryDefinition queryDefinition,
            List<string> sqlLines)
        {
            if (queryDefinition.OrderingDefinitions.Count == 0)
                return;

            var orderingClauses = queryDefinition.OrderingDefinitions
                .SelectMany(orderingDefinition =>
                    BuildOrderingColumnReferences(queryDefinition, orderingDefinition)
                        .Select(columnReference =>
                            $"{columnReference} {ResolveSqlOrderingDirection(orderingDefinition.Direction)}"));

            sqlLines.Add("ORDER BY " + string.Join(", ", orderingClauses));
        }

        // Builds SQL column references for all columns contained in an ordering group.
        private IEnumerable<string> BuildOrderingColumnReferences(CompiledQueryDefinition queryDefinition, QueryOrderingDefinition orderingDefinition)
        {
            foreach (var orderingColumn in orderingDefinition.Columns)
                yield return BuildOrderingColumnReference(queryDefinition, orderingDefinition, orderingColumn);
        }

        // Builds a SQL column reference for a single ordering column.
        private string BuildOrderingColumnReference(CompiledQueryDefinition queryDefinition, QueryOrderingDefinition orderingDefinition, QueryColumnDefinition orderingColumn)
        {
            if (!string.IsNullOrWhiteSpace(orderingDefinition.SourceAlias))
            {
                var columnName = ResolveMappedColumnName(orderingDefinition.SourceColumnMappings, orderingColumn.PropertyName);

                return _databaseDialect.BuildQualifiedIdentifier(orderingDefinition.SourceAlias, columnName);
            }

            return QueryColumnMappingHelper.ResolveColumnReference(queryDefinition, _databaseDialect, orderingColumn.PropertyName);
        }

        // Resolves the SQL ordering direction keyword.
        private static string ResolveSqlOrderingDirection(QueryOrderingDirection direction)
        {
            return direction == QueryOrderingDirection.Ascending ? "ASC" : "DESC";
        }

        #endregion

        #region Pagination Clause Building

        // Adds provider-specific pagination syntax when pagination is enabled.
        protected virtual void AddPaginationClauseIfNeeded(CompiledQueryDefinition queryDefinition, List<string> sqlLines)
        {
            if (!queryDefinition.Pagination.HasPagination)
                return;

            if (queryDefinition.OrderingDefinitions.Count == 0)
                throw new InvalidOperationException("Pagination requires at least one ORDER BY clause.");

            sqlLines.Add(_databaseDialect.BuildPaginationClause(queryDefinition.Pagination.Skip, queryDefinition.Pagination.Take));
        }

        #endregion

        // Resolves mapped column names for select projections.
        private static string ResolveMappedColumnName(IReadOnlyDictionary<string, string>? columnMappings, string propertyName)
        {
            if (columnMappings is null)
                return propertyName;

            return columnMappings.TryGetValue(propertyName, out var columnName)
                ? columnName
                : propertyName;
        }
    }
}
