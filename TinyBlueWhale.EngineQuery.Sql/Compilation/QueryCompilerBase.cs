using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionScopes;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Sql.Helpers;

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
        /// Compiles the specified query definition into a generated SQL query.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to compile.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        public GeneratedSqlQuery Compile(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            var sqlParameters = new List<QuerySqlParameter>();

            var sqlLines = new List<string>
            {
                BuildSelectClause(queryDefinition, sqlParameters),
                BuildFromClause(queryDefinition, sqlParameters)
            };

            AddJoinClausesIfNeeded(queryDefinition, sqlLines);
            AddWhereClauseIfNeeded(queryDefinition, sqlParameters, sqlLines);
            AddGroupByClauseIfNeeded(queryDefinition, sqlLines);
            AddHavingClauseIfNeeded(queryDefinition, sqlParameters, sqlLines);
            AddOrderByClauseIfNeeded(queryDefinition, sqlLines);
            AddPaginationClauseIfNeeded(queryDefinition, sqlLines);

            var commandText = string.Join(Environment.NewLine, sqlLines);

            if (queryDefinition.UnionDefinitions.Count > 0)
                commandText = BuildUnionCommandText(queryDefinition, sqlParameters, commandText);

            if (queryDefinition.CteDefinitions.Count > 0)
                commandText = BuildCteClause(queryDefinition, sqlParameters) + Environment.NewLine + commandText;

            return new GeneratedSqlQuery
            {
                CommandText = commandText,
                Parameters = sqlParameters
            };
        }

        #region Builds the SQL WITH clause

        // Builds the SQL WITH clause for common table expressions.
        protected virtual string BuildCteClause(CompiledQueryDefinition queryDefinition, List<QuerySqlParameter> sqlParameters)
        {
            var cteClauses = queryDefinition.CteDefinitions
                .Select(cteDefinition =>
                {
                    var cteQuery = Compile(cteDefinition.Query);

                    var commandText = ReindexSubqueryParameters(
                        cteQuery,
                        sqlParameters);

                    return $"{_databaseDialect.EscapeIdentifier(cteDefinition.Name)} AS ({commandText})";
                });

            return "WITH " + string.Join(", ", cteClauses);
        }

        #endregion

        #region Union Clause building

        // Builds SQL command text including UNION clauses.
        protected virtual string BuildUnionCommandText(CompiledQueryDefinition queryDefinition,List<QuerySqlParameter> sqlParameters,
            string commandText)
        {
            foreach (var unionDefinition in queryDefinition.UnionDefinitions)
            {
                var unionQuery = Compile(unionDefinition.Query);

                var unionCommandText = ReindexSubqueryParameters(unionQuery,sqlParameters);

                var unionKeyword = unionDefinition.IncludeDuplicates? "UNION ALL" : "UNION";

                commandText += Environment.NewLine +
                               unionKeyword +
                               Environment.NewLine +
                               unionCommandText;
            }

            return commandText;
        }

        #endregion


        #region Select Clause Building        
        // Builds the SQL SELECT clause from query projections, aggregate expressions, scalar function expressions, computed expressions and CASE WHEN expressions.
        protected virtual string BuildSelectClause(CompiledQueryDefinition queryDefinition, List<QuerySqlParameter> sqlParameters)
        {
            if (queryDefinition.UseConstantSelectProjection)
                return "SELECT 1";

            if (queryDefinition.SelectDefinitions.Count == 0 &&
                queryDefinition.AggregateDefinitions.Count == 0 &&
                queryDefinition.ScalarFunctionDefinitions.Count == 0 &&
                queryDefinition.ComputedExpressionDefinitions.Count == 0 &&
                queryDefinition.CaseWhenDefinitions.Count == 0 &&
                queryDefinition.WindowFunctionDefinitions.Count == 0)
                return "SELECT *";            

            var selectedColumns = queryDefinition.SelectDefinitions
                .Select(selectDefinition => BuildSelectColumn(queryDefinition, selectDefinition));

            var aggregateColumns = queryDefinition.AggregateDefinitions
                .Select(BuildAggregateColumn);

            var scalarFunctionColumns = queryDefinition.ScalarFunctionDefinitions
                .Select(functionDefinition => BuildScalarFunctionColumn(functionDefinition, sqlParameters));

            var computedColumns = queryDefinition.ComputedExpressionDefinitions
                .Select(computedDefinition => BuildComputedExpressionColumn(computedDefinition, sqlParameters));

            var caseWhenColumns = queryDefinition.CaseWhenDefinitions
                .Select(caseWhenDefinition => BuildCaseWhenColumn(caseWhenDefinition, sqlParameters));

            var windowFunctionColumns = queryDefinition.WindowFunctionDefinitions
                .Select(BuildWindowFunctionColumn);

            var distinctKeyword = queryDefinition.IsDistinct ? "DISTINCT " : string.Empty;

            return $"SELECT {distinctKeyword}{string.Join(", ", selectedColumns
                .Concat(aggregateColumns)
                .Concat(scalarFunctionColumns)
                .Concat(computedColumns)
                .Concat(caseWhenColumns)
                .Concat(windowFunctionColumns))}";
        }

        // Builds a ROW_NUMBER window function projection.
        // Builds a SQL window function projection.
        protected virtual string BuildWindowFunctionColumn(
            QueryWindowFunctionDefinition windowFunctionDefinition)
        {
            var windowClauses = new List<string>();

            if (windowFunctionDefinition.Partitions.Count > 0)
            {
                var partitionColumns = windowFunctionDefinition.Partitions
                    .Select(BuildWindowPartitionColumn);

                windowClauses.Add("PARTITION BY " + string.Join(", ", partitionColumns));
            }

            var orderingColumns = windowFunctionDefinition.Orderings
                .Select(BuildWindowOrderingColumn);

            windowClauses.Add("ORDER BY " + string.Join(", ", orderingColumns));

            var functionName = ResolveWindowFunctionName(
                windowFunctionDefinition.Function);

            return $"{functionName}() OVER ({string.Join(" ", windowClauses)}) AS {_databaseDialect.EscapeIdentifier(windowFunctionDefinition.Alias)}";
        }

        // Resolves the SQL window function name.
        private static string ResolveWindowFunctionName(QueryWindowFunction function)
        {
            return function switch
            {
                QueryWindowFunction.RowNumber => "ROW_NUMBER",
                QueryWindowFunction.Rank => "RANK",
                QueryWindowFunction.DenseRank => "DENSE_RANK",
                _ => throw new NotSupportedException($"Window function '{function}' is not supported.")
            };
        }

        // Builds a PARTITION BY column reference for a window function.
        private string BuildWindowPartitionColumn(QueryWindowPartitionDefinition partitionDefinition)
        {
            var columnName = ResolveMappedColumnName(partitionDefinition.Source.ColumnMappings, partitionDefinition.Column.PropertyName);

            return _databaseDialect.BuildQualifiedIdentifier(partitionDefinition.Source.TableAlias, columnName);
        }

        // Builds an ORDER BY column reference for a window function.
        private string BuildWindowOrderingColumn(
            QueryWindowOrderingDefinition orderingDefinition)
        {
            var columnName = ResolveMappedColumnName(
                orderingDefinition.Source.ColumnMappings,
                orderingDefinition.Column.PropertyName);

            var columnReference = _databaseDialect.BuildQualifiedIdentifier(
                orderingDefinition.Source.TableAlias,
                columnName);

            var direction = orderingDefinition.Direction == QueryOrderingDirection.Ascending
                ? "ASC"
                : "DESC";

            return $"{columnReference} {direction}";
        }

        // Builds a CASE WHEN SQL projection.
        protected virtual string BuildCaseWhenColumn(QueryCaseWhenDefinition caseWhenDefinition, List<QuerySqlParameter> sqlParameters)
        {
            var expressionScope = new QueryExpressionScope();

            expressionScope.Register((ParameterExpression)caseWhenDefinition.ConditionExpression.Parameters.Single(), caseWhenDefinition.Source);

            var parser = new SqlComputedExpressionParser(
                _databaseDialect,
                sqlParameters,
                caseWhenDefinition.Source.ColumnMappings,
                caseWhenDefinition.Source.TableAlias,
                expressionScope);

            var conditionSql = parser.Parse(caseWhenDefinition.ConditionExpression.Body);
            var whenTrueParameter = AddSqlParameter(sqlParameters, caseWhenDefinition.WhenTrueValue);
            var whenFalseParameter = AddSqlParameter(sqlParameters, caseWhenDefinition.WhenFalseValue);

            return $"CASE WHEN {conditionSql} THEN {whenTrueParameter} ELSE {whenFalseParameter} END AS {_databaseDialect.EscapeIdentifier(caseWhenDefinition.Alias)}";
        }

        // Builds a computed SQL expression projection.
        protected virtual string BuildComputedExpressionColumn(QueryComputedExpressionDefinition computedDefinition, List<QuerySqlParameter> sqlParameters)
        {
            var expressionScope = new QueryExpressionScope();

            expressionScope.Register((ParameterExpression)computedDefinition.Expression.Parameters.Single(), computedDefinition.Source);

            var parser = new SqlComputedExpressionParser(_databaseDialect, sqlParameters, computedDefinition.Source.ColumnMappings, computedDefinition.Source.TableAlias, expressionScope);

            var sqlExpression = parser.Parse(computedDefinition.Expression.Body);

            return $"{sqlExpression} AS {_databaseDialect.EscapeIdentifier(computedDefinition.Alias)}";
        }

        // Builds a SQL SELECT column fragment including optional alias projection.       
        protected virtual string BuildSelectColumn(CompiledQueryDefinition queryDefinition, QuerySelectColumnDefinition selectDefinition)
        {
            var columnReference = BuildSelectColumnReference(queryDefinition, selectDefinition);

            var shouldApplyAlias = queryDefinition.ForceSelectAliases || !string.IsNullOrWhiteSpace(selectDefinition.Alias);

            if (!shouldApplyAlias)
                return columnReference;

            var alias = string.IsNullOrWhiteSpace(selectDefinition.Alias)
                ? selectDefinition.PropertyName
                : selectDefinition.Alias;

            return $"{columnReference} AS {_databaseDialect.EscapeIdentifier(alias)}";
        }

        // Builds a SQL aggregate SELECT expression.
        protected virtual string BuildAggregateColumn(QueryAggregateDefinition aggregateDefinition)
        {
            var columnName = ResolveMappedColumnName(aggregateDefinition.Source.ColumnMappings, aggregateDefinition.PropertyName);

            var columnReference = string.IsNullOrWhiteSpace(aggregateDefinition.Source.TableAlias)
                ? _databaseDialect.EscapeIdentifier(columnName)
                : _databaseDialect.BuildQualifiedIdentifier(aggregateDefinition.Source.TableAlias, columnName);

            var functionName = SqlFunctionNameResolver.ResolveAggregateFunctionName(aggregateDefinition.Function);

            return $"{functionName}({columnReference}) AS {_databaseDialect.EscapeIdentifier(aggregateDefinition.Alias)}";
        }

        // Builds a scalar SQL function projection.
        protected virtual string BuildScalarFunctionColumn(QueryScalarFunctionDefinition functionDefinition, List<QuerySqlParameter> sqlParameters)
        {
            var arguments = functionDefinition.Arguments.Count > 0
                ? BuildScalarFunctionArguments(functionDefinition, sqlParameters)
                : [BuildScalarFunctionSingleColumnArgument(functionDefinition)];

            var functionName = SqlFunctionNameResolver.ResolveScalarFunctionName(functionDefinition.Function, _databaseDialect);

            return $"{functionName}({string.Join(", ", arguments)}) AS {_databaseDialect.EscapeIdentifier(functionDefinition.Alias)}";
        }

        // Builds all scalar SQL function arguments.
        private List<string> BuildScalarFunctionArguments(QueryScalarFunctionDefinition functionDefinition, List<QuerySqlParameter> sqlParameters)
        {
            return [.. functionDefinition.Arguments.Select(argument => BuildScalarFunctionArgument(functionDefinition,argument,sqlParameters))];
        }

        // Builds a scalar SQL function argument.
        private string BuildScalarFunctionArgument(QueryScalarFunctionDefinition functionDefinition, QueryScalarFunctionArgumentDefinition argumentDefinition, List<QuerySqlParameter> sqlParameters)
        {
            if (argumentDefinition.IsColumn)
            {
                var columnName = ResolveMappedColumnName(functionDefinition.Source.ColumnMappings, argumentDefinition.PropertyName!);

                return string.IsNullOrWhiteSpace(functionDefinition.Source.TableAlias)
                    ? _databaseDialect.EscapeIdentifier(columnName)
                    : _databaseDialect.BuildQualifiedIdentifier(functionDefinition.Source.TableAlias, columnName);
            }

            return AddSqlParameter(sqlParameters, argumentDefinition.ConstantValue);
        }

        // Builds the single-column argument used by single-property scalar SQL function projections.
        private string BuildScalarFunctionSingleColumnArgument(QueryScalarFunctionDefinition functionDefinition)
        {
            if (string.IsNullOrWhiteSpace(functionDefinition.PropertyName))
                throw new InvalidOperationException("Scalar function property name is required for single-column function projections.");

            var columnName = ResolveMappedColumnName(functionDefinition.Source.ColumnMappings, functionDefinition.PropertyName);

            return string.IsNullOrWhiteSpace(functionDefinition.Source.TableAlias)
                ? _databaseDialect.EscapeIdentifier(columnName)
                : _databaseDialect.BuildQualifiedIdentifier(functionDefinition.Source.TableAlias,columnName);
        }


        // Builds a SQL column reference for single-source and multi-source projections.
        // Builds a SQL column reference for single-source and multi-source projections.
        private string BuildSelectColumnReference(
            CompiledQueryDefinition queryDefinition,
            QuerySelectColumnDefinition selectDefinition)
        {
            if (selectDefinition.Source is not null)
            {
                var columnName = ResolveMappedColumnName(selectDefinition.Source.ColumnMappings, selectDefinition.PropertyName);

                return _databaseDialect.BuildQualifiedIdentifier(selectDefinition.Source.TableAlias, columnName);
            }

            return QueryColumnMappingHelper.ResolveColumnReference(queryDefinition, _databaseDialect, selectDefinition.PropertyName);
        }

        #endregion

        #region From Clause Building
        // Builds the SQL FROM clause.
        protected virtual string BuildFromClause(CompiledQueryDefinition queryDefinition,List<QuerySqlParameter> sqlParameters)
        {
            var rootSource = queryDefinition.SourceDefinitions.TryGetValue(queryDefinition.EntityType, out var sourceDefinition)
                ? sourceDefinition
                : null;

            if (rootSource is not null)
                return $"FROM {BuildQuerySourceReference(rootSource, sqlParameters)}";

            var tableName = _databaseDialect.EscapeIdentifier(queryDefinition.TableName);

            return string.IsNullOrWhiteSpace(queryDefinition.TableAlias)
                ? $"FROM {tableName}"
                : $"FROM {tableName} AS {_databaseDialect.EscapeIdentifier(queryDefinition.TableAlias)}";
        }

        // Builds a SQL reference for a physical table or derived table query source.
        protected virtual string BuildQuerySourceReference(QuerySourceDefinition sourceDefinition, List<QuerySqlParameter> sqlParameters)
        {
            if (sourceDefinition.IsDerivedTable)
            {
                var subquery = Compile(sourceDefinition.Subquery!);

                var commandText = ReindexSubqueryParameters(subquery, sqlParameters);

                return $"({commandText}) AS {_databaseDialect.EscapeIdentifier(sourceDefinition.TableAlias)}";
            }

            if (sourceDefinition.IsTable)
            {
                return $"{_databaseDialect.EscapeIdentifier(sourceDefinition.TableName!)} AS {_databaseDialect.EscapeIdentifier(sourceDefinition.TableAlias)}";
            }

            throw new InvalidOperationException("Query source must define either a physical table or a derived table subquery.");
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
            if (queryDefinition.WhereDefinitions.Count == 0 &&
                queryDefinition.WhereScalarFunctionDefinitions.Count == 0 &&
                queryDefinition.WhereComputedExpressionDefinitions.Count == 0 &&
                queryDefinition.ExistsDefinitions.Count == 0 &&
                queryDefinition.InSubqueryDefinitions.Count == 0)
                return;

            var whereConditions = queryDefinition.WhereDefinitions
                .Select(whereDefinition =>
                {
                    var parser = CreateWhereClauseExpressionParser(sqlParameters, queryDefinition, whereDefinition);

                    return parser.ParseToSqlCondition(whereDefinition.PredicateExpression.Body);
                });

            var functionConditions = queryDefinition.WhereScalarFunctionDefinitions
                .Select(functionDefinition => BuildWhereScalarFunctionCondition(functionDefinition, sqlParameters));

            var computedConditions = queryDefinition.WhereComputedExpressionDefinitions
                .Select(computedDefinition => BuildWhereComputedExpressionCondition(computedDefinition, sqlParameters));

            var existsConditions = queryDefinition.ExistsDefinitions
                .Select(existsDefinition => BuildExistsCondition(existsDefinition, sqlParameters));

            var inConditions = queryDefinition.InSubqueryDefinitions
                .Select(inDefinition => BuildInSubqueryCondition(inDefinition, sqlParameters));

            sqlLines.Add("WHERE " + string.Join(" AND ",
                whereConditions
                    .Concat(functionConditions)
                    .Concat(computedConditions)
                    .Concat(existsConditions)
                    .Concat(inConditions)));
        }

        // Builds an IN subquery SQL condition.
        protected virtual string BuildInSubqueryCondition(QueryInSubqueryDefinition inDefinition, List<QuerySqlParameter> sqlParameters)
        {
            var outerColumnReference = BuildInSubqueryOuterColumnReference(inDefinition);
            var subquery = Compile(inDefinition.Subquery);
            var commandText = ReindexSubqueryParameters(subquery, sqlParameters);

            return $"{outerColumnReference} IN ({commandText})";
        }

        // Builds the outer column reference used by an IN subquery condition.
        private string BuildInSubqueryOuterColumnReference(QueryInSubqueryDefinition inDefinition)
        {
            var propertyName = ExtractSinglePropertyName(inDefinition.OuterSelector);

            var columnName = ResolveMappedColumnName(inDefinition.OuterSource.ColumnMappings,propertyName);

            return _databaseDialect.BuildQualifiedIdentifier(inDefinition.OuterSource.TableAlias, columnName);
        }

        // Extracts a single property name from a lambda expression.
        private static string ExtractSinglePropertyName(LambdaExpression expression)
        {
            var body = expression.Body is UnaryExpression unaryExpression
                ? unaryExpression.Operand
                : expression.Body;

            if (body is not MemberExpression memberExpression)
                throw new NotSupportedException($"Expression '{expression}' is not supported as a column selector.");

            return memberExpression.Member.Name;
        }

        // Reindexes subquery parameters and appends them to the parent parameter collection.
        private static string ReindexSubqueryParameters(GeneratedSqlQuery subquery, List<QuerySqlParameter> sqlParameters)
        {
            var offset = sqlParameters.Count;
            var commandText = subquery.CommandText;

            foreach (var parameter in subquery.Parameters)
            {
                var newName = $"@p{offset}";

                commandText = commandText.Replace(parameter.Name, newName);

                sqlParameters.Add(
                    new QuerySqlParameter
                    {
                        Name = newName,
                        Value = parameter.Value
                    });

                offset++;
            }

            return commandText;
        }

        // Builds an EXISTS SQL condition.
        protected virtual string BuildExistsCondition(QueryExistsDefinition existsDefinition, List<QuerySqlParameter> sqlParameters)
        {
            var existsQuery = Compile(existsDefinition.Subquery);
            var commandText = ReindexSubqueryParameters(existsQuery, sqlParameters);

            var existsKeyword = existsDefinition.IsNegated ? "NOT EXISTS": "EXISTS";

            return $"{existsKeyword} ({commandText})";
        }

        // Builds a SQL WHERE computed expression condition.
        protected virtual string BuildWhereComputedExpressionCondition(QueryWhereComputedExpressionDefinition computedDefinition, List<QuerySqlParameter> sqlParameters)
        {
            var expressionScope = new QueryExpressionScope();

            foreach (var source in computedDefinition.Sources)
                expressionScope.Register(source.Key, source.Value);

            var parser = new SqlComputedExpressionParser(
                _databaseDialect,
                sqlParameters,
                null,
                null,
                expressionScope);

            return parser.Parse(computedDefinition.Expression.Body);
        }

        // Creates a SQL WHERE clause expression parser instance.
        protected virtual QueryWhereClauseExpressionParser CreateWhereClauseExpressionParser(List<QuerySqlParameter> sqlParameters, CompiledQueryDefinition queryDefinition, QueryWhereDefinition whereDefinition)
        {
            return new QueryWhereClauseExpressionParser(
                _databaseDialect,
                sqlParameters,
                whereDefinition.Source.ColumnMappings ?? queryDefinition.ColumnMappings,
                whereDefinition.Source.TableAlias ?? queryDefinition.TableAlias);
        }

        // Builds a SQL WHERE scalar function condition.
        private string BuildWhereScalarFunctionCondition(QueryWhereScalarFunctionDefinition functionDefinition, List<QuerySqlParameter> sqlParameters)
        {
            var columnName = ResolveMappedColumnName(
                functionDefinition.Source.ColumnMappings,
                functionDefinition.PropertyName);

            var columnReference = string.IsNullOrWhiteSpace(functionDefinition.Source.TableAlias)
                ? _databaseDialect.EscapeIdentifier(columnName)
                : _databaseDialect.BuildQualifiedIdentifier(functionDefinition.Source.TableAlias, columnName);

            var parameterName = AddSqlParameter(sqlParameters, functionDefinition.Value);

            var functionName = SqlFunctionNameResolver.ResolveScalarFunctionName(functionDefinition.Function, _databaseDialect);

            return $"{functionName}({columnReference}) {ResolveComparisonOperator(functionDefinition.ComparisonOperator)} {parameterName}";
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
            if (!string.IsNullOrWhiteSpace(groupByDefinition.Source.TableAlias))
            {
                var columnName = ResolveMappedColumnName(groupByDefinition.Source.ColumnMappings, groupByColumn.PropertyName);

                return _databaseDialect.BuildQualifiedIdentifier(groupByDefinition.Source.TableAlias, columnName);
            }

            return QueryColumnMappingHelper.ResolveColumnReference(queryDefinition, _databaseDialect, groupByColumn.PropertyName);
        }
        #endregion

        #region Having Clause Building

        // Adds SQL HAVING conditions when aggregate filters are configured.
        protected virtual void AddHavingClauseIfNeeded(CompiledQueryDefinition queryDefinition, List<QuerySqlParameter> sqlParameters, List<string> sqlLines)
        {
            if (queryDefinition.HavingAggregateDefinitions.Count == 0)
                return;

            var havingConditions = queryDefinition.HavingAggregateDefinitions
                .Select(havingDefinition => BuildHavingAggregateCondition(havingDefinition, sqlParameters));

            sqlLines.Add("HAVING " + string.Join(" AND ", havingConditions));
        }

        // Builds a SQL HAVING aggregate condition.
        private string BuildHavingAggregateCondition(QueryHavingAggregateDefinition havingDefinition, List<QuerySqlParameter> sqlParameters)
        {
            var columnName = ResolveMappedColumnName(havingDefinition.Source.ColumnMappings, havingDefinition.PropertyName);

            var columnReference = string.IsNullOrWhiteSpace(havingDefinition.Source.TableAlias)
                ? _databaseDialect.EscapeIdentifier(columnName)
                : _databaseDialect.BuildQualifiedIdentifier(havingDefinition.Source.TableAlias, columnName);

            var parameterName = AddSqlParameter(sqlParameters, havingDefinition.Value);

            var functionName = SqlFunctionNameResolver.ResolveAggregateFunctionName(havingDefinition.Function);

            return $"{functionName}({columnReference}) {ResolveComparisonOperator(havingDefinition.ComparisonOperator)} {parameterName}";
        }

        // Resolves the SQL comparison operator.
        private static string ResolveComparisonOperator(
            QueryComparisonOperator comparisonOperator)
        {
            return comparisonOperator switch
            {
                QueryComparisonOperator.Equal => "=",
                QueryComparisonOperator.NotEqual => "<>",
                QueryComparisonOperator.GreaterThan => ">",
                QueryComparisonOperator.GreaterThanOrEqual => ">=",
                QueryComparisonOperator.LessThan => "<",
                QueryComparisonOperator.LessThanOrEqual => "<=",
                _ => throw new NotSupportedException($"Comparison operator '{comparisonOperator}' is not supported.")
            };
        }

        // Adds a SQL parameter and returns the generated parameter name.
        private static string AddSqlParameter(List<QuerySqlParameter> sqlParameters,object? value)
        {
            var parameterName = $"@p{sqlParameters.Count}";

            sqlParameters.Add(
                new QuerySqlParameter
                {
                    Name = parameterName,
                    Value = value
                });

            return parameterName;
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
            if (!string.IsNullOrWhiteSpace(orderingDefinition.Source.TableAlias))
            {
                var columnName = ResolveMappedColumnName(orderingDefinition.Source.ColumnMappings, orderingColumn.PropertyName);

                return _databaseDialect.BuildQualifiedIdentifier(orderingDefinition.Source.TableAlias, columnName);
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
