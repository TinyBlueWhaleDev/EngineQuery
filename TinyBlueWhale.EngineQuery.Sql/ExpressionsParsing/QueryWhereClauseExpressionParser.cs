using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Parameters;

namespace TinyBlueWhale.EngineQuery.Sql.ExpressionsParsing
{
    /// <summary>
    /// Parses LINQ expression trees into SQL WHERE clause conditions.
    /// </summary>
    /// <remarks>
    /// This parser is responsible for converting supported expression patterns
    /// into provider-specific SQL predicate fragments and query parameters.
    /// </remarks>
    public sealed class QueryWhereClauseExpressionParser(ISqlDatabaseDialect databaseDialect,
        QueryParameterCollection sqlParameters, 
        IReadOnlyDictionary<string, string> columnMappings,
        string? tableAlias = null)
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect;
        private readonly QueryParameterCollection _sqlParameters = sqlParameters;
        private readonly IReadOnlyDictionary<string, string> _columnMappings = columnMappings;
        private readonly string? _tableAlias = tableAlias;

        /// <summary>
        /// Parses the specified expression into a SQL WHERE condition fragment.
        /// </summary>
        /// <param name="expression">
        /// Expression tree representing a query predicate.
        /// </param>
        /// <returns>
        /// SQL condition fragment generated from the expression.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the expression type is not supported by the parser.
        /// </exception>
        public string ParseToSqlCondition(Expression expression)
        {
            return expression switch
            {
                BinaryExpression binaryExpression => ParseBinaryExpressionToSqlCondition(binaryExpression),

                MemberExpression memberExpression => ParseBooleanPropertyToSqlCondition(memberExpression),

                UnaryExpression unaryExpression when unaryExpression.NodeType == ExpressionType.Not => ParseNegatedExpressionToSqlCondition(unaryExpression),

                MethodCallExpression methodCallExpression => ParseMethodCallExpressionToSqlCondition(methodCallExpression),

                _ => throw new NotSupportedException(
                    $"Expression '{expression}' is not supported in WHERE clauses.")
            };
        }

        // Resolves the SQL column reference associated with a CLR property.
        private string ResolveColumnReference(string propertyName)
        {
            var columnName = _columnMappings.TryGetValue(propertyName, out var mappedColumnName)
                ? mappedColumnName
                : propertyName;

            return string.IsNullOrWhiteSpace(_tableAlias)
                ? _databaseDialect.EscapeIdentifier(columnName)
                : _databaseDialect.BuildQualifiedIdentifier(_tableAlias, columnName);
        }

        // Parses a binary expression into a SQL condition.
        private string ParseBinaryExpressionToSqlCondition(BinaryExpression binaryExpression)
        {
            if (IsNullComparison(binaryExpression))
                return ParseNullComparisonToSqlCondition(binaryExpression);

            var leftOperand = ParseExpressionOperandToSql(
                binaryExpression.Left);

            var rightOperand = ParseExpressionOperandToSql(
                binaryExpression.Right);

            var sqlOperator = SqlComputedExpressionParser.ResolveSqlOperator(
                binaryExpression.NodeType);

            return $"({leftOperand} {sqlOperator} {rightOperand})";
        }

        // Determines whether the binary expression compares a value against null.
        private static bool IsNullComparison(
            BinaryExpression binaryExpression)
        {
            return binaryExpression.NodeType is ExpressionType.Equal or ExpressionType.NotEqual &&
                   (IsNullConstant(binaryExpression.Left) || IsNullConstant(binaryExpression.Right));
        }

        // Parses a null comparison into IS NULL or IS NOT NULL SQL.
        private string ParseNullComparisonToSqlCondition(
            BinaryExpression binaryExpression)
        {
            var nonNullExpression = IsNullConstant(binaryExpression.Left)
                ? binaryExpression.Right
                : binaryExpression.Left;

            var operand = ParseExpressionOperandToSql(
                nonNullExpression);

            var sqlOperator = binaryExpression.NodeType == ExpressionType.Equal
                ? "IS NULL"
                : "IS NOT NULL";

            return $"({operand} {sqlOperator})";
        }

        // Determines whether the expression represents a null constant.
        private static bool IsNullConstant(Expression expression)
        {
            if (expression is UnaryExpression unaryExpression &&
                (unaryExpression.NodeType == ExpressionType.Convert ||
                 unaryExpression.NodeType == ExpressionType.ConvertChecked))
            {
                expression = unaryExpression.Operand;
            }

            return expression is ConstantExpression constantExpression &&
                   constantExpression.Value is null;
        }

        // Converts expression operands into SQL-compatible fragments or parameters.
        private string ParseExpressionOperandToSql(Expression expression)
        {
            return expression switch
            {
                MemberExpression memberExpression when memberExpression.Expression?.NodeType == ExpressionType.Parameter => 
                    memberExpression.Type == typeof(bool)
                        ? ParseBooleanPropertyToSqlCondition(memberExpression)
                        : ResolveColumnReference(memberExpression.Member.Name),

                ConstantExpression constantExpression => AddSqlParameter(constantExpression.Value),

                MemberExpression memberExpression => AddSqlParameter(RuntimeExpressionValueExtractor.ExtractValue(memberExpression)),

                UnaryExpression unaryExpression when unaryExpression.NodeType == ExpressionType.Convert => ParseExpressionOperandToSql(unaryExpression.Operand),

                BinaryExpression nestedBinaryExpression => ParseBinaryExpressionToSqlCondition(nestedBinaryExpression),

                MethodCallExpression methodCallExpression => ParseMethodCallExpressionToSqlCondition(methodCallExpression),

                _ => throw new NotSupportedException($"Expression operand '{expression}' is not supported.")
            };
        }

        // Converts boolean property expressions into SQL equality conditions.
        private string ParseBooleanPropertyToSqlCondition(MemberExpression memberExpression)
        {
            if (memberExpression.Expression?.NodeType != ExpressionType.Parameter)
                return AddSqlParameter(RuntimeExpressionValueExtractor.ExtractValue(memberExpression));

            var columnName = ResolveColumnReference(memberExpression.Member.Name);

            var parameterName = AddSqlParameter(true);

            return $"({columnName} = {parameterName})";
        }

        // Parses negated expressions such as !IsActive.
        private string ParseNegatedExpressionToSqlCondition(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MemberExpression memberExpression &&memberExpression.Expression?.NodeType == ExpressionType.Parameter)
            {
                var columnName = ResolveColumnReference(memberExpression.Member.Name);

                var parameterName = AddSqlParameter(false);

                return $"({columnName} = {parameterName})";
            }

            return $"NOT ({ParseToSqlCondition(unaryExpression.Operand)})";
        }

        // Parses supported string method calls such as Contains, StartsWith and EndsWith.
        private string ParseMethodCallExpressionToSqlCondition(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Object is not MemberExpression memberExpression ||memberExpression.Expression?.NodeType != ExpressionType.Parameter)
                throw new NotSupportedException($"Method call '{methodCallExpression}' is not supported in WHERE clauses.");

            var columnName = ResolveColumnReference(memberExpression.Member.Name);

            return methodCallExpression.Method.Name switch
            {
                nameof(string.Contains) => BuildSqlLikeCondition(columnName, methodCallExpression.Arguments[0], SqlLikeSearchMode.Contains),

                nameof(string.StartsWith) => BuildSqlLikeCondition(columnName, methodCallExpression.Arguments[0], SqlLikeSearchMode.StartsWith),

                nameof(string.EndsWith) => BuildSqlLikeCondition(columnName, methodCallExpression.Arguments[0], SqlLikeSearchMode.EndsWith),

                _ => throw new NotSupportedException($"Method '{methodCallExpression.Method.Name}' is not supported in WHERE clauses.")
            };
        }

        // Builds SQL LIKE conditions based on the selected search mode.
        private string BuildSqlLikeCondition(string columnName, Expression valueExpression, SqlLikeSearchMode searchMode)
        {
            var rawValue = RuntimeExpressionValueExtractor .ExtractValue(valueExpression);

            var value = rawValue?.ToString() ?? string.Empty;

            var searchPattern = searchMode switch
            {
                SqlLikeSearchMode.Contains => $"%{value}%",
                SqlLikeSearchMode.StartsWith => $"{value}%",
                SqlLikeSearchMode.EndsWith => $"%{value}",
                _ => value
            };

            var parameterName = AddSqlParameter(searchPattern);

            return $"({columnName} LIKE {parameterName})";
        }

        // Registers a SQL parameter preserving deterministic parameter ordering.
        private string AddSqlParameter(object? value)
        {
            return _sqlParameters.Add(value);
        }

        // Represents supported SQL LIKE search patterns.
        private enum SqlLikeSearchMode
        {
            Contains,
            StartsWith,
            EndsWith
        }
    }
}
