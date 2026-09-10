using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Helpers
{

    /// <summary>
    /// Extracts scalar SQL function arguments from expression trees.
    /// </summary>
    internal static class ScalarFunctionArgumentExtractor
    {
        /// <summary>
        /// Extracts scalar function arguments from an array expression.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the scalar function arguments.
        /// </typeparam>
        /// <param name="expression">
        /// Expression containing the scalar function arguments.
        /// </param>
        /// <returns>
        /// Scalar function argument definitions extracted from the expression.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the expression cannot be represented as supported scalar function arguments.
        /// </exception>
        public static List<QueryScalarFunctionArgumentDefinition> Extract<TEntity>(Expression<Func<TEntity, object[]>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            return expression.Body switch
            {
                NewArrayExpression newArrayExpression =>
                [
                    .. newArrayExpression.Expressions.Select(Create)
                ],

                _ => throw new NotSupportedException($"Expression '{expression}' is not supported as a scalar function argument selector.")
            };
        }

        /// <summary>
        /// Creates a scalar function argument definition from an expression.
        /// </summary>
        private static QueryScalarFunctionArgumentDefinition Create(Expression expression)
        {
            if (expression is UnaryExpression unaryExpression)
                expression = unaryExpression.Operand;

            if (expression is MemberExpression memberExpression)
            {
                return new QueryScalarFunctionArgumentDefinition
                {
                    PropertyName = memberExpression.Member.Name
                };
            }

            if (expression is ConstantExpression constantExpression)
            {
                return new QueryScalarFunctionArgumentDefinition
                {
                    ConstantValue = constantExpression.Value
                };
            }

            throw new NotSupportedException($"Scalar function argument expression '{expression}' is not supported.");
        }
    }
}
