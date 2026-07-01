using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.Helpers
{

    /// <summary>
    /// Provides helpers to classify query expressions.
    /// </summary>
    internal static class QueryExpressionClassificationHelper
    {
        /// <summary>
        /// Determines whether the specified expression represents a simple member access.
        /// </summary>
        /// <param name="expression">
        /// Expression to classify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the expression represents a simple member access; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsSimpleMemberAccess(Expression expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            return UnwrapConvertExpression(expression) is MemberExpression memberExpression &&
                memberExpression.Expression is ParameterExpression;
        }

        /// <summary>
        /// Determines whether the specified expression represents a computed expression.
        /// </summary>
        /// <param name="expression">
        /// Expression to classify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the expression represents a computed expression; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsComputedExpression(Expression expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            return !IsSimpleMemberAccess(expression);
        }

        /// <summary>
        /// Removes conversion wrappers from an expression.
        /// </summary>
        /// <param name="expression">
        /// Expression to unwrap.
        /// </param>
        /// <returns>
        /// Expression without conversion wrappers.
        /// </returns>
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
