using TinyBlueWhale.EngineQuery.MySql.Dialects;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.MySql
{
    /// <summary>
    /// Validates MySQL database dialect behavior.
    /// </summary>
    [TestFixture]
    public sealed class MySqlDatabaseDialectTests
    {
        [TestCase("users", "`users`")]
        [TestCase("order`history", "`order``history`")]
        [TestCase("schema`name", "`schema``name`")]
        public void EscapeIdentifier_Should_Escape_Backtick(string identifier, string expected)
        {
            var dialect = new MySqlDatabaseDialect();

            var result = dialect.EscapeIdentifier(identifier);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
