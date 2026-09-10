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
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected property.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Property type returned by the selector.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the entity property.
        /// </param>
        /// <param name="parameterName">
        /// Parameter name used when reporting an invalid selector.
        /// </param>
        /// <param name="errorMessage">
        /// Error message used when the selector does not represent
        /// a direct entity property access.
        /// </param>
        /// <returns>
        /// Resolved entity property name.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="selector"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the selector does not represent a direct entity property access.
        /// </exception>
        internal static string ResolvePropertyName<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, string parameterName, string errorMessage)
        {
            ArgumentNullException.ThrowIfNull(selector);

            return ResolvePropertyName(selector.Body, parameterName, errorMessage);
        }

        /// <summary>
        /// Resolves one or more direct entity property names
        /// from the specified selector.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected properties.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects one or more entity properties.
        /// </param>
        /// <param name="parameterName">
        /// Parameter name used when reporting an invalid selector.
        /// </param>
        /// <param name="errorMessage">
        /// Error message used when the selector does not represent
        /// supported direct entity property access.
        /// </param>
        /// <returns>
        /// Resolved entity property names.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="selector"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the selector does not represent one or more
        /// supported direct entity property accesses.
        /// </exception>
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

        // Resolves a direct property name from an expression node.
        private static string ResolvePropertyName(Expression expression, string parameterName, string errorMessage)
        {
            expression = UnwrapConvert(expression);

            if (expression is not MemberExpression memberExpression || memberExpression.Expression is not ParameterExpression)
                throw new ArgumentException(errorMessage, parameterName);

            return memberExpression.Member.Name;
        }

        // Removes a conversion wrapper from the specified expression.
        private static Expression UnwrapConvert(Expression expression)
        {
            return expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert
                ? unaryExpression.Operand
                : expression;
        }
    }
}
