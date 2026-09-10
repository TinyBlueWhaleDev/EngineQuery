using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.SqlServer
{
    /// <summary>
    /// Validates SQL Server database dialect behavior.
    /// </summary>
    [TestFixture]
    public sealed class SqlServerDatabaseDialectTests
    {
        [TestCase("users", "[users]")]
        [TestCase("order]history", "[order]]history]")]
        [TestCase("schema]name", "[schema]]name]")]
        public void EscapeIdentifier_Should_Escape_Closing_Bracket(string identifier, string expected)
        {
            var dialect = new SqlServerDatabaseDialect();

            var result = dialect.EscapeIdentifier(identifier);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
