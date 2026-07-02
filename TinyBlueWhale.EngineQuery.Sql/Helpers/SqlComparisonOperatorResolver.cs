using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Sql.Helpers
{
    /// <summary>
    /// Resolves SQL comparison operators from query comparison operator definitions.
    /// </summary>
    /// <remarks>
    /// This resolver centralizes comparison operator translation so WHERE and HAVING builders
    /// can share the same SQL operator mapping.
    /// </remarks>
    public static class SqlComparisonOperatorResolver
    {
        /// <summary>
        /// Resolves the SQL comparison operator keyword or symbol.
        /// </summary>
        /// <param name="comparisonOperator">
        /// Query comparison operator to resolve.
        /// </param>
        /// <returns>
        /// SQL comparison operator.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the comparison operator is not supported.
        /// </exception>
        public static string Resolve(QueryComparisonOperator comparisonOperator)
        {
            return comparisonOperator switch
            {
                QueryComparisonOperator.Equal => "=",
                QueryComparisonOperator.NotEqual => "<>",
                QueryComparisonOperator.GreaterThan => ">",
                QueryComparisonOperator.GreaterThanOrEqual => ">=",
                QueryComparisonOperator.LessThan => "<",
                QueryComparisonOperator.LessThanOrEqual => "<=",
                _ => throw new NotSupportedException($"Comparison operator '{comparisonOperator}' is not supported.")
            };
        }
    }
}
