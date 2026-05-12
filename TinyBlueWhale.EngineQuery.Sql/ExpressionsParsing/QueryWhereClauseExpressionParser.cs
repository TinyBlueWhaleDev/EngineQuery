using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.Interfaces;

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
        List<QuerySqlParameter> sqlParameters, 
        IReadOnlyDictionary<string, string> columnMappings)
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect;
        private readonly List<QuerySqlParameter> _sqlParameters = sqlParameters;
        private readonly IReadOnlyDictionary<string, string> _columnMappings = columnMappings;

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

        // Parses binary expressions such as ==, !=, >, <, && and ||.
        private string ParseBinaryExpressionToSqlCondition(
            BinaryExpression binaryExpression)
        {
            var leftOperand = ParseExpressionOperandToSql(binaryExpression.Left);
            var rightOperand = ParseExpressionOperandToSql(binaryExpression.Right);

            var sqlOperator = binaryExpression.NodeType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.AndAlso => "AND",
                ExpressionType.OrElse => "OR",

                _ => throw new NotSupportedException($"Binary operator '{binaryExpression.NodeType}' is not supported.")
            };

            return $"({leftOperand} {sqlOperator} {rightOperand})";
        }

        // Converts expression operands into SQL-compatible fragments or parameters.
        private string ParseExpressionOperandToSql(Expression expression)
        {
            return expression switch
            {
                MemberExpression memberExpression when memberExpression.Expression?.NodeType == ExpressionType.Parameter => 
                    memberExpression.Type == typeof(bool)
                        ? ParseBooleanPropertyToSqlCondition(memberExpression)
                        : _databaseDialect.EscapeIdentifier(ResolveColumnName(memberExpression.Member.Name)),

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

            var columnName = _databaseDialect.EscapeIdentifier(ResolveColumnName(memberExpression.Member.Name));

            var parameterName = AddSqlParameter(true);

            return $"({columnName} = {parameterName})";
        }

        // Parses negated expressions such as !IsActive.
        private string ParseNegatedExpressionToSqlCondition(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MemberExpression memberExpression &&memberExpression.Expression?.NodeType == ExpressionType.Parameter)
            {
                var columnName = _databaseDialect.EscapeIdentifier(ResolveColumnName(memberExpression.Member.Name));

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

            var columnName = _databaseDialect.EscapeIdentifier(ResolveColumnName(memberExpression.Member.Name));

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
            var parameterName = $"@p{_sqlParameters.Count}";

            _sqlParameters.Add(new QuerySqlParameter
            {
                Name = parameterName,
                Value = value
            });

            return parameterName;
        }

        // Resolves the database column name associated with a CLR property.
        private string ResolveColumnName(string propertyName)
        {
            return _columnMappings.TryGetValue(
                propertyName,
                out var columnName)
                    ? columnName
                    : propertyName;
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
