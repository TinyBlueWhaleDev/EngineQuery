using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Sql.Helpers
{
    /// <summary>
    /// Extracts column metadata from lambda expressions used as column selectors.
    /// </summary>
    /// <remarks>
    /// This helper keeps expression selector parsing consistent across SQL clause builders.
    /// </remarks>
    public static class ExpressionColumnSelector
    {
        /// <summary>
        /// Extracts a single property name from a lambda expression.
        /// </summary>
        /// <param name="expression">
        /// Lambda expression that selects a single property.
        /// </param>
        /// <returns>
        /// Selected property name.
        /// </returns>
        public static string ExtractSinglePropertyName(LambdaExpression expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var body = expression.Body is UnaryExpression unaryExpression
                ? unaryExpression.Operand
                : expression.Body;

            if (body is not MemberExpression memberExpression)
                throw new NotSupportedException($"Expression '{expression}' is not supported as a column selector.");

            return memberExpression.Member.Name;
        }
    }
}
