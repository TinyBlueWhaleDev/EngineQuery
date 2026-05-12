using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

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
        public static IReadOnlyList<QuerySelectColumnDefinition> ExtractSelectedProperties<T>(
            Expression<Func<T, object>> selector)
        {
            return selector.Body switch
            {
                NewExpression newExpression => ExtractFromNewExpression(newExpression),

                MemberExpression memberExpression => [
                    new QuerySelectColumnDefinition
                    {
                        PropertyName = memberExpression.Member.Name
                    }
                ],

                UnaryExpression unaryExpression when unaryExpression.Operand is MemberExpression memberExpression => 
                [
                    new QuerySelectColumnDefinition
                    {
                        PropertyName = memberExpression.Member.Name
                    }
                ],

                _ => throw new NotSupportedException($"Select expression '{selector}' is not supported.")
            };
        }
        // Extracts selected properties and aliases from anonymous object projections.
        private static IReadOnlyList<QuerySelectColumnDefinition> ExtractFromNewExpression(NewExpression newExpression)
        {
            return [.. newExpression.Arguments
                .Select((argument, index) =>
                    CreateSelectColumnDefinition(
                        argument,
                        newExpression.Members?[index].Name))];
        }

        // Creates a selected column definition from a projection argument.
        private static QuerySelectColumnDefinition CreateSelectColumnDefinition(Expression argument, string? projectedMemberName)
        {
            if (argument is not MemberExpression memberExpression)
                throw new NotSupportedException($"Select argument '{argument}' is not supported.");

            return new QuerySelectColumnDefinition
            {
                PropertyName = memberExpression.Member.Name,
                Alias = projectedMemberName == memberExpression.Member.Name
                    ? null
                    : projectedMemberName
            };
        }
    }
}
