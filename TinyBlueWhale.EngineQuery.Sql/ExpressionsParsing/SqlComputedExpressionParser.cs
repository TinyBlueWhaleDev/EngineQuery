using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.ExpressionScopes;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Parameters;
using TinyBlueWhale.EngineQuery.Sql.Helpers;


namespace TinyBlueWhale.EngineQuery.Sql.ExpressionsParsing
{
    /// <summary>
    /// Parses computed expression trees into SQL expressions.
    /// </summary>
    public sealed class SqlComputedExpressionParser(ISqlDatabaseDialect databaseDialect,
        QueryParameterCollection sqlParameters, 
        IReadOnlyDictionary<string, string>? columnMappings, 
        string? sourceAlias,
        QueryExpressionScope? expressionScope = null)
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect;
        private readonly QueryParameterCollection  _sqlParameters = sqlParameters;
        private readonly IReadOnlyDictionary<string, string>? _columnMappings = columnMappings;
        private readonly string? _sourceAlias = sourceAlias;
        private readonly QueryExpressionScope? _expressionScope = expressionScope;

        /// <summary>
        /// Parses the specified expression into a SQL computed expression.
        /// </summary>
        public string Parse(Expression expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            return ParseExpression(UnwrapConvertExpression(expression));
        }

        // Parses supported computed SQL expressions.
        private string ParseExpression(Expression expression)
        {
            return expression switch
            {
                BinaryExpression binaryExpression => ParseBinaryExpression(binaryExpression),
                MemberExpression memberExpression => ParseMemberExpression(memberExpression),
                ConstantExpression constantExpression => AddSqlParameter(constantExpression.Value),
                _ => throw new NotSupportedException($"Computed expression '{expression}' is not supported.")
            };
        }

        // Parses binary expressions such as addition, subtraction, multiplication and division.
        private string ParseBinaryExpression(BinaryExpression binaryExpression)
        {
            ArgumentNullException.ThrowIfNull(binaryExpression);

            if (binaryExpression.NodeType == ExpressionType.Coalesce)
                return ParseCoalesceExpression(binaryExpression);

            var left = ParseExpression(UnwrapConvertExpression(binaryExpression.Left));
            var right = ParseExpression(UnwrapConvertExpression(binaryExpression.Right));
            var sqlOperator = ResolveSqlOperator(binaryExpression.NodeType);

            return $"({left} {sqlOperator} {right})";
        }

        // Parses SQL COALESCE expressions.
        private string ParseCoalesceExpression(BinaryExpression binaryExpression)
        {
            var left = ParseExpression(binaryExpression.Left);
            var right = ParseExpression(binaryExpression.Right);

            return $"COALESCE({left}, {right})";
        }

        // Parses member access expressions as SQL column references or captured values.
        private string ParseMemberExpression(MemberExpression memberExpression)
        {
            if (_expressionScope is not null && memberExpression.Expression is ParameterExpression)
            {
                var resolver = new ScopedColumnReferenceResolver(_databaseDialect, _expressionScope);

                return resolver.Resolve(memberExpression);
            }

            if (memberExpression.Expression is not ParameterExpression)
                return AddSqlParameter(Expression.Lambda(memberExpression).Compile().DynamicInvoke());

            var propertyName = memberExpression.Member.Name;
            var columnName = ResolveMappedColumnName(propertyName);

            return string.IsNullOrWhiteSpace(_sourceAlias)
                ? _databaseDialect.EscapeIdentifier(columnName)
                : _databaseDialect.BuildQualifiedIdentifier(_sourceAlias, columnName);
        }


        // Resolves the SQL operator associated with the expression type.
        public static string ResolveSqlOperator(ExpressionType expressionType)
        {
            return expressionType switch
            {
                ExpressionType.Add => "+",
                ExpressionType.Subtract => "-",
                ExpressionType.Multiply => "*",
                ExpressionType.Divide => "/",
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.AndAlso => "AND",
                ExpressionType.OrElse => "OR",
                _ => throw new NotSupportedException($"Computed expression operator '{expressionType}' is not supported.")
            };
        }

        // Resolves mapped column names.
        private string ResolveMappedColumnName(string propertyName)
        {
            if (_columnMappings is null)
                return propertyName;

            return _columnMappings.TryGetValue(propertyName, out var columnName)
                ? columnName
                : propertyName;
        }

        // Adds a SQL parameter and returns the generated parameter name.
        
        private string AddSqlParameter(object? value)
        {           
            return _sqlParameters.Add(value);
        }

        // Removes boxing conversions generated by object-returning expression selectors.
        public static Expression UnwrapConvertExpression(Expression expression)
        {
            return expression is UnaryExpression unaryExpression &&
                   unaryExpression.NodeType == ExpressionType.Convert
                ? unaryExpression.Operand
                : expression;
        }
    }
}
