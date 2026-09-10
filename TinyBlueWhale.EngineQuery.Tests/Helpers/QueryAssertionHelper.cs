using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Helpers
{
    /// <summary>
    /// Provides shared assertions and normalization utilities for generated queries.
    /// </summary>
    internal static class QueryAssertionHelper
    {
        public static void AssertParameter(
            IReadOnlyList<QuerySqlParameter> parameters,
            int index,
            object expectedValue)
        {
            Assert.That(parameters, Has.Count.GreaterThan(index));

            Assert.Multiple(() =>
            {
                Assert.That(parameters[index].Name, Is.EqualTo($"@p{index}"));
                Assert.That(parameters[index].Value, Is.EqualTo(expectedValue));
            });
        }

        public static void AssertParameter(
            IReadOnlyList<QuerySqlParameter> parameters,
            int index,
            string name,
            object expectedValue)
        {
            Assert.That(parameters, Has.Count.GreaterThan(index));

            Assert.Multiple(() =>
            {
                Assert.That(parameters[index].Name, Is.EqualTo(name));
                Assert.That(parameters[index].Value, Is.EqualTo(expectedValue));
            });
        }

        public static string NormalizeSql(string commandText)
        {
            return commandText
                .Replace("[", string.Empty, StringComparison.Ordinal)
                .Replace("]", string.Empty, StringComparison.Ordinal)
                .Replace("\"", string.Empty, StringComparison.Ordinal)
                .Replace("`", string.Empty, StringComparison.Ordinal);
        }
    }
}
