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
    /// Extracts query column definitions from expressions that select one or more entity properties.
    /// </summary>
    public static class QueryColumnExpressionExtractor
    {
        /// <summary>
        /// Extracts query column definitions from a single-property or anonymous object expression.
        /// </summary>
        public static IReadOnlyList<QueryColumnDefinition> ExtractColumns<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            return expression.Body switch
            {
                MemberExpression memberExpression =>
                [
                    CreateColumnDefinition(memberExpression)
                ],

                UnaryExpression unaryExpression when unaryExpression.Operand is MemberExpression memberExpression =>
                [
                    CreateColumnDefinition(memberExpression)
                ],

                NewExpression newExpression =>
                    newExpression.Arguments
                        .Select(CreateColumnDefinition)
                        .ToList(),

                _ => throw new NotSupportedException(
                    $"Expression '{expression}' is not supported as a column selector.")
            };
        }

        // Creates a query column definition from a member access expression.
        private static QueryColumnDefinition CreateColumnDefinition(
            Expression expression)
        {
            if (expression is UnaryExpression unaryExpression)
                expression = unaryExpression.Operand;

            if (expression is not MemberExpression memberExpression)
                throw new NotSupportedException($"Column expression '{expression}' is not supported.");

            return new QueryColumnDefinition
            {
                PropertyName = memberExpression.Member.Name
            };
        }
    }
}
