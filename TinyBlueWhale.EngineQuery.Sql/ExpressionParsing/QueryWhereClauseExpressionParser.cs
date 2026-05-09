using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Sql.Dialects.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.ExpressionParsing
{
    public sealed class QueryWhereClauseExpressionParser(ISqlDatabaseDialect databaseDialect, List<QuerySqlParameter> sqlParameters)
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect;
        private readonly List<QuerySqlParameter> _sqlParameters = sqlParameters;

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

        private string ParseExpressionOperandToSql(Expression expression)
        {
            return expression switch
            {
                MemberExpression memberExpression when memberExpression.Expression?.NodeType == ExpressionType.Parameter => 
                    memberExpression.Type == typeof(bool)
                        ? ParseBooleanPropertyToSqlCondition(memberExpression)
                        : _databaseDialect.EscapeIdentifier(memberExpression.Member.Name),

                ConstantExpression constantExpression => AddSqlParameter(constantExpression.Value),

                MemberExpression memberExpression => AddSqlParameter(RuntimeExpressionValueExtractor.ExtractValue(memberExpression)),

                UnaryExpression unaryExpression when unaryExpression.NodeType == ExpressionType.Convert => ParseExpressionOperandToSql(unaryExpression.Operand),

                BinaryExpression nestedBinaryExpression => ParseBinaryExpressionToSqlCondition(nestedBinaryExpression),

                MethodCallExpression methodCallExpression => ParseMethodCallExpressionToSqlCondition(methodCallExpression),

                _ => throw new NotSupportedException($"Expression operand '{expression}' is not supported.")
            };
        }

        private string ParseBooleanPropertyToSqlCondition(MemberExpression memberExpression)
        {
            if (memberExpression.Expression?.NodeType != ExpressionType.Parameter)
                return AddSqlParameter(RuntimeExpressionValueExtractor.ExtractValue(memberExpression));

            var columnName = _databaseDialect.EscapeIdentifier(memberExpression.Member.Name);

            var parameterName = AddSqlParameter(true);

            return $"({columnName} = {parameterName})";
        }

        private string ParseNegatedExpressionToSqlCondition(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MemberExpression memberExpression &&memberExpression.Expression?.NodeType == ExpressionType.Parameter)
            {
                var columnName = _databaseDialect.EscapeIdentifier(memberExpression.Member.Name);

                var parameterName = AddSqlParameter(false);

                return $"({columnName} = {parameterName})";
            }

            return $"NOT ({ParseToSqlCondition(unaryExpression.Operand)})";
        }

        private string ParseMethodCallExpressionToSqlCondition(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Object is not MemberExpression memberExpression ||memberExpression.Expression?.NodeType != ExpressionType.Parameter)
                throw new NotSupportedException($"Method call '{methodCallExpression}' is not supported in WHERE clauses.");

            var columnName = _databaseDialect.EscapeIdentifier(memberExpression.Member.Name);

            return methodCallExpression.Method.Name switch
            {
                nameof(string.Contains) => BuildSqlLikeCondition(columnName, methodCallExpression.Arguments[0], SqlLikeSearchMode.Contains),

                nameof(string.StartsWith) => BuildSqlLikeCondition(columnName, methodCallExpression.Arguments[0], SqlLikeSearchMode.StartsWith),

                nameof(string.EndsWith) => BuildSqlLikeCondition(columnName, methodCallExpression.Arguments[0], SqlLikeSearchMode.EndsWith),

                _ => throw new NotSupportedException($"Method '{methodCallExpression.Method.Name}' is not supported in WHERE clauses.")
            };
        }

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

        private enum SqlLikeSearchMode
        {
            Contains,
            StartsWith,
            EndsWith
        }
    }
}
