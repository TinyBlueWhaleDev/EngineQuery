using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.ExpressionsParsing
{
    /// <summary>
    /// Extracts selected property definitions from multi-source projection expressions.
    /// </summary>
    /// <remarks>
    /// Used by JOIN projections where selected properties can belong to different query sources.
    /// </remarks>
    public static class MultiSourceSelectedPropertyExpressionExtractor
    {
        /// <summary>
        /// Extracts selected property definitions from the specified multi-source projection expression.
        /// </summary>
        /// <typeparam name="TResult">
        /// Projection result type.
        /// </typeparam>
        /// <param name="selector">
        /// Projection expression that selects properties from multiple query sources.
        /// </param>
        /// <returns>
        /// Collection of selected property definitions.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the projection expression is not supported.
        /// </exception>
        public static IReadOnlyList<QuerySelectColumnDefinition> ExtractSelectedProperties<TResult>(LambdaExpression selector)
        {
            return selector.Body switch
            {
                NewExpression newExpression => ExtractFromNewExpression(newExpression),
                _ => throw new NotSupportedException($"Select expression '{selector}' is not supported.")
            };
        }

        // Extracts selected properties and aliases from multi-source anonymous object projections.
        private static IReadOnlyList<QuerySelectColumnDefinition> ExtractFromNewExpression(
            NewExpression newExpression)
        {
            return [.. newExpression.Arguments.Select((argument, index) => CreateSelectColumnDefinition(argument, newExpression.Members?[index].Name))];
        }

        // Creates a selected column definition from a multi-source projection argument.
        private static QuerySelectColumnDefinition CreateSelectColumnDefinition(Expression argument, string? projectedMemberName)
        {
            if (argument is not MemberExpression memberExpression)
                throw new NotSupportedException($"Select argument '{argument}' is not supported.");

            if (memberExpression.Expression is not ParameterExpression parameterExpression)
                throw new NotSupportedException($"Select source '{argument}' is not supported.");

            return new QuerySelectColumnDefinition
            {
                PropertyName = memberExpression.Member.Name,
                Alias = projectedMemberName == memberExpression.Member.Name ? null : projectedMemberName,
                SourceType = parameterExpression.Type
            };
        }
    }
}
