using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.PostgreSql
{
    /// <summary>
    /// Validates PostgreSQL database dialect behavior.
    /// </summary>
    [TestFixture]
    public sealed class PostgreSqlDatabaseDialectTests
    {
        [TestCase("users", "\"users\"")]
        [TestCase("order\"history", "\"order\"\"history\"")]
        [TestCase("schema\"name", "\"schema\"\"name\"")]
        public void EscapeIdentifier_Should_Escape_Double_Quote(string identifier, string expected)
        {
            var dialect = new PostgreSqlDatabaseDialect();

            var result = dialect.EscapeIdentifier(identifier);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
