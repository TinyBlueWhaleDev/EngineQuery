using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Core.ExpressionsParsing
{
    /// <summary>
    /// Extracts selected property names from projection expressions.
    /// </summary>
    /// <remarks>
    /// Used to determine which entity properties should be included
    /// in generated SQL SELECT clauses.
    /// </remarks>
    public static class SelectedPropertyExpressionExtractor
    {
        /// <summary>
        /// Extracts selected property names from the specified projection expression.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the projection.
        /// </typeparam>
        /// <param name="selector">
        /// Projection expression that selects one or more entity properties.
        /// </param>
        /// <returns>
        /// Collection of selected property names.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the projection expression is not supported.
        /// </exception>
        public static IReadOnlyList<string> ExtractSelectedProperties<T>(
            Expression<Func<T, object>> selector)
        {
            return selector.Body switch
            {
                NewExpression newExpression => ExtractFromNewExpression(newExpression),

                MemberExpression memberExpression => [memberExpression.Member.Name],

                UnaryExpression unaryExpression when unaryExpression.Operand is MemberExpression memberExpression => [memberExpression.Member.Name],

                _ => throw new NotSupportedException($"Select expression '{selector}' is not supported.")
            };
        }

        // Extracts property names from anonymous object projections.
        private static IReadOnlyList<string> ExtractFromNewExpression(NewExpression newExpression)
        {
            return [.. newExpression.Arguments.OfType<MemberExpression>().Select(x => x.Member.Name)];
        }
    }
}
