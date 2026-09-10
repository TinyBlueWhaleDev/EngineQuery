
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Helpers
{

    /// <summary>
    /// Resolves provider-specific SQL function names from canonical query functions.
    /// </summary>
    public static class SqlFunctionNameResolver
    {
        /// <summary>
        /// Resolves a provider-specific scalar SQL function name.
        /// </summary>
        /// <param name="function">
        /// Scalar query function to resolve.
        /// </param>
        /// <param name="databaseDialect">
        /// Database dialect used to resolve provider-specific function naming.
        /// </param>
        /// <returns>
        /// Provider-specific SQL scalar function name.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="databaseDialect"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when <paramref name="function"/> is not supported.
        /// </exception>
        public static string ResolveScalarFunctionName(QueryScalarFunction function, ISqlDatabaseDialect databaseDialect)
        {
            ArgumentNullException.ThrowIfNull(databaseDialect);

            var canonicalFunctionName = function switch
            {
                QueryScalarFunction.Lower => "LOWER",
                QueryScalarFunction.Upper => "UPPER",
                QueryScalarFunction.Length => "LENGTH",
                QueryScalarFunction.Trim => "TRIM",
                QueryScalarFunction.Coalesce => "COALESCE",
                QueryScalarFunction.Concat => "CONCAT",
                _ => throw new NotSupportedException($"Scalar function '{function}' is not supported.")
            };

            return databaseDialect.ResolveScalarFunctionName(canonicalFunctionName);
        }

        /// <summary>
        /// Resolves a SQL aggregate function name.
        /// </summary>
        /// <param name="function">
        /// Aggregate query function to resolve.
        /// </param>
        /// <returns>
        /// SQL aggregate function name.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when <paramref name="function"/> is not supported.
        /// </exception>
        public static string ResolveAggregateFunctionName(QueryAggregateFunction function)
        {
            return function switch
            {
                QueryAggregateFunction.Count => "COUNT",
                QueryAggregateFunction.Sum => "SUM",
                QueryAggregateFunction.Average => "AVG",
                QueryAggregateFunction.Minimum => "MIN",
                QueryAggregateFunction.Maximum => "MAX",
                _ => throw new NotSupportedException($"Aggregate function '{function}' is not supported.")
            };
        }
    }
}
