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

        [TestCase(null, null, "")]
        [TestCase(null, 10, "LIMIT 10")]
        [TestCase(20, 10, "LIMIT 10 OFFSET 20")]
        [TestCase(20, null, "LIMIT 18446744073709551615 OFFSET 20")]
        public void BuildPaginationClause_ShouldGenerateExpectedPagination(int? skip, int? take, string expected)
        {
            var dialect = new MySqlDatabaseDialect();

            var result = dialect.BuildPaginationClause(
                skip,
                take);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
