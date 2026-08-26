using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Core.Helpers
{
    /// <summary>
    /// Provides utilities for resolving direct entity properties
    /// from strongly typed expressions.
    /// </summary>
    internal static class PropertyExpressionHelper
    {
        /// <summary>
        /// Resolves a direct entity property name from the specified selector.
        /// </summary>
        internal static string ResolvePropertyName<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, string parameterName, string errorMessage)
        {
            ArgumentNullException.ThrowIfNull(selector);

            return ResolvePropertyName(selector.Body, parameterName, errorMessage);
        }

        /// <summary>
        /// Resolves one or more direct entity property names
        /// from the specified selector.
        /// </summary>
        internal static IReadOnlyList<string> ResolvePropertyNames<TEntity>(Expression<Func<TEntity, object>> selector, string parameterName, string errorMessage)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var expression = UnwrapConvert(selector.Body);

            if (expression is MemberExpression memberExpression)
                return [ResolvePropertyName(memberExpression, parameterName, errorMessage)];

            if (expression is NewExpression newExpression)
                return [.. newExpression.Arguments.Select(argument => ResolvePropertyName(argument, parameterName, errorMessage))];

            throw new ArgumentException(errorMessage, parameterName);
        }

        private static string ResolvePropertyName(Expression expression, string parameterName, string errorMessage)
        {
            expression = UnwrapConvert(expression);

            if (expression is not MemberExpression memberExpression || memberExpression.Expression is not ParameterExpression)
                throw new ArgumentException(errorMessage, parameterName);

            return memberExpression.Member.Name;
        }

        private static Expression UnwrapConvert(Expression expression)
        {
            return expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert
                ? unaryExpression.Operand
                : expression;
        }
    }
}
