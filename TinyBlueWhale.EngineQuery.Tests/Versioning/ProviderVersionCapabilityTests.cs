using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.MySql.Capabilities;
using TinyBlueWhale.EngineQuery.PostgreSql.Capabilities;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;

namespace TinyBlueWhale.EngineQuery.Tests.Versioning
{

    /// <summary>
    /// Validates version-based provider capabilities.
    /// </summary>
    [TestFixture]
    public sealed class ProviderVersionCapabilityTests
    {
        [Test]
        public void MySql57_Should_Not_Support_Modern_Query_Features()
        {
            var capabilities = new MySqlProviderCapabilities(
                DatabaseProviderVersion.Create(5, 7));

            Assert.Multiple(() =>
            {
                Assert.That(capabilities.SupportsCommonTableExpressions, Is.False);
                Assert.That(capabilities.SupportsRecursiveCommonTableExpressions, Is.False);
                Assert.That(capabilities.SupportsWindowFunctions, Is.False);
                Assert.That(capabilities.SupportsLateralJoins, Is.False);
                Assert.That(capabilities.SupportsIntersect, Is.False);
                Assert.That(capabilities.SupportsExcept, Is.False);
                Assert.That(capabilities.SupportsLimitOffsetPagination, Is.True);
            });
        }

        [Test]
        public void MySql8031_Should_Support_Modern_Query_Features()
        {
            var capabilities = new MySqlProviderCapabilities(
                DatabaseProviderVersion.Create(8, 0, 31));

            Assert.Multiple(() =>
            {
                Assert.That(capabilities.SupportsCommonTableExpressions, Is.True);
                Assert.That(capabilities.SupportsRecursiveCommonTableExpressions, Is.True);
                Assert.That(capabilities.SupportsWindowFunctions, Is.True);
                Assert.That(capabilities.SupportsLateralJoins, Is.True);
                Assert.That(capabilities.SupportsIntersect, Is.True);
                Assert.That(capabilities.SupportsExcept, Is.True);
                Assert.That(capabilities.SupportsLimitOffsetPagination, Is.True);
            });
        }

        [Test]
        public void SqlServer2008_Should_Not_Support_OffsetFetch_Pagination()
        {
            var capabilities = new SqlServerProviderCapabilities(
                DatabaseProviderVersion.Create(10, 0));

            Assert.Multiple(() =>
            {
                Assert.That(capabilities.SupportsOffsetFetchPagination, Is.False);
                Assert.That(capabilities.SupportsLimitOffsetPagination, Is.False);
            });
        }

        [Test]
        public void SqlServer2012_Should_Support_OffsetFetch_Pagination()
        {
            var capabilities = new SqlServerProviderCapabilities(
                DatabaseProviderVersion.Create(11, 0));

            Assert.Multiple(() =>
            {
                Assert.That(capabilities.SupportsOffsetFetchPagination, Is.True);
                Assert.That(capabilities.SupportsLimitOffsetPagination, Is.False);
            });
        }

        [Test]
        public void PostgreSql92_Should_Not_Support_Lateral_Joins()
        {
            var capabilities = new PostgreSqlProviderCapabilities(
                DatabaseProviderVersion.Create(9, 2));

            Assert.That(capabilities.SupportsLateralJoins, Is.False);
        }

        [Test]
        public void PostgreSql93_Should_Support_Lateral_Joins()
        {
            var capabilities = new PostgreSqlProviderCapabilities(
                DatabaseProviderVersion.Create(9, 3));

            Assert.That(capabilities.SupportsLateralJoins, Is.True);
        }
    }
}
