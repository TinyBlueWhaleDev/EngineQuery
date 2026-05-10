using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Core.ExpressionsParsing
{
    /// <summary>
    /// Extracts runtime values from expression tree nodes.
    /// </summary>
    /// <remarks>
    /// Used when query expressions contain constants, captured variables,
    /// or member access expressions that must be converted into SQL parameters.
    /// </remarks>
    public static class RuntimeExpressionValueExtractor
    {
        /// <summary>
        /// Extracts the runtime value represented by the specified expression.
        /// </summary>
        /// <param name="expression">
        /// Expression tree node containing the value to extract.
        /// </param>
        /// <returns>
        /// Runtime value produced by evaluating the expression.
        /// </returns>
        public static object? ExtractValue(Expression expression)
        {
            var lambda = Expression.Lambda(expression);
            var compiledExpression = lambda.Compile();

            return compiledExpression.DynamicInvoke();
        }
    }
}
