
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
