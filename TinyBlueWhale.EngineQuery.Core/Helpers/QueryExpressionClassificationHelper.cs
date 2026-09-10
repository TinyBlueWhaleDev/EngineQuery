using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Core.Helpers
{

    /// <summary>
    /// Provides helpers to classify query expressions.
    /// </summary>
    internal static class QueryExpressionClassificationHelper
    {
        /// <summary>
        /// Determines whether the specified expression represents
        /// a direct member access over a query parameter.
        /// </summary>
        /// <param name="expression">
        /// Expression to classify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the expression represents
        /// a direct member access over a query parameter;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="expression"/> is null.
        /// </exception>
        public static bool IsSimpleMemberAccess(Expression expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            return UnwrapConvertExpression(expression) is MemberExpression memberExpression &&
                memberExpression.Expression is ParameterExpression;
        }

        /// <summary>
        /// Determines whether the specified expression represents
        /// a computed query expression.
        /// </summary>
        /// <param name="expression">
        /// Expression to classify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the expression does not represent
        /// a direct member access over a query parameter;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="expression"/> is null.
        /// </exception>
        public static bool IsComputedExpression(Expression expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            return !IsSimpleMemberAccess(expression);
        }

        // Removes conversion wrappers from the specified expression.
        private static Expression UnwrapConvertExpression(Expression expression)
        {
            while (expression is UnaryExpression unaryExpression &&
                unaryExpression.NodeType is ExpressionType.Convert or ExpressionType.ConvertChecked)
            {
                expression = unaryExpression.Operand;
            }

            return expression;
        }
    }
}
