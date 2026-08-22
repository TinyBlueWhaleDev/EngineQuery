using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

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
        public static List<QueryScalarFunctionArgumentDefinition> Extract<TEntity>(Expression<Func<TEntity, object[]>> expression)
        {
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
