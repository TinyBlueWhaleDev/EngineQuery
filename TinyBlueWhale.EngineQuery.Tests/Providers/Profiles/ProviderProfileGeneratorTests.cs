using Microsoft.CodeAnalysis;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Sql.Profiles;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.Profiles
{
    /// <summary>
    /// Validates compile-time provider feature surfaces generated from profiles.
    /// </summary>
    [TestFixture]
    public sealed class ProviderProfileGeneratorTests
    {
        [Test]
        public void ProviderProfile_ShouldImplementDatabaseProviderProfileContract()
        {
            var profile = new TestProviderProfile();

            Assert.That(profile, Is.AssignableTo<IDatabaseProviderProfile>());
        }

        [Test]
        public void ProviderProfile_ShouldExposeConfiguredDatabaseVersion()
        {
            var profile = new TestProviderProfile();

            Assert.Multiple(() =>
            {
                Assert.That(profile.Version.Major, Is.EqualTo(8));
                Assert.That(profile.Version.Minor, Is.EqualTo(0));
                Assert.That(profile.Version.Patch, Is.EqualTo(31));
            });
        }

        [Test]
        public void SqlServer2012_Pagination_ShouldCompile()
        {
            var errors = GeneratorCompilationTestHelper.Compile(
                QueryTestProviderSource.SqlServer2012Pagination);

            Assert.That(
                errors,
                Is.Empty,
                FormatDiagnostics(errors));
        }

        [Test]
        public void SqlServer2008_Pagination_ShouldNotCompile()
        {
            var errors = GeneratorCompilationTestHelper.Compile(
                QueryTestProviderSource.SqlServer2008Pagination);

            Assert.That(errors, Is.Not.Empty);

            Assert.That(
                errors.Any(error =>
                    error.GetMessage().Contains(
                        "Skip",
                        StringComparison.Ordinal)),
                Is.True,
                FormatDiagnostics(errors));
        }

        [Test]
        public void PostgreSql93_Lateral_ShouldCompile()
        {
            var errors = GeneratorCompilationTestHelper.Compile(
                QueryTestProviderSource.PostgreSql93Lateral);

            Assert.That(
                errors,
                Is.Empty,
                FormatDiagnostics(errors));
        }

        [Test]
        public void PostgreSql84_Lateral_ShouldNotCompile()
        {
            var errors = GeneratorCompilationTestHelper.Compile(
                QueryTestProviderSource.PostgreSql84Lateral);

            Assert.That(errors, Is.Not.Empty);

            Assert.That(
                errors.Any(error =>
                    error.GetMessage().Contains(
                        "CrossApply",
                        StringComparison.Ordinal)),
                Is.True,
                FormatDiagnostics(errors));
        }

        [Test]
        public void SqlServer_ScalarIdentity_ShouldCompile()
        {
            var errors = GeneratorCompilationTestHelper.Compile(
                QueryTestProviderSource.SqlServerScalarIdentity);

            Assert.That(
                errors,
                Is.Empty,
                FormatDiagnostics(errors));
        }

        [Test]
        public void SqlServer_ReturningIdentity_ShouldNotCompile()
        {
            var errors = GeneratorCompilationTestHelper.Compile(
                QueryTestProviderSource.SqlServerReturningIdentity);

            AssertIdentitySurfaceShouldNotCompile(errors);
        }

        [Test]
        public void PostgreSql_ScalarIdentity_ShouldNotCompile()
        {
            var errors = GeneratorCompilationTestHelper.Compile(
                QueryTestProviderSource.PostgreSqlScalarIdentity);

            AssertIdentitySurfaceShouldNotCompile(errors);
        }

        [Test]
        public void PostgreSql_ReturningIdentity_ShouldCompile()
        {
            var errors = GeneratorCompilationTestHelper.Compile(
                QueryTestProviderSource.PostgreSqlReturningIdentity);

            Assert.That(
                errors,
                Is.Empty,
                FormatDiagnostics(errors));
        }

        [Test]
        public void MySql_ScalarIdentity_ShouldCompile()
        {
            var errors = GeneratorCompilationTestHelper.Compile(
                QueryTestProviderSource.MySqlScalarIdentity);

            Assert.That(
                errors,
                Is.Empty,
                FormatDiagnostics(errors));
        }

        [Test]
        public void MySql_ReturningIdentity_ShouldNotCompile()
        {
            var errors = GeneratorCompilationTestHelper.Compile(
                QueryTestProviderSource.MySqlReturningIdentity);

            AssertIdentitySurfaceShouldNotCompile(errors);
        }

        private static void AssertIdentitySurfaceShouldNotCompile(
            IReadOnlyList<Diagnostic> errors)
        {
            Assert.That(errors, Is.Not.Empty);

            Assert.That(
                errors.Any(error =>
                    error.GetMessage().Contains(
                        "ReturnIdentity",
                        StringComparison.Ordinal)),
                Is.True,
                FormatDiagnostics(errors));
        }

        private static string FormatDiagnostics(
            IEnumerable<Diagnostic> diagnostics)
        {
            return string.Join(
                Environment.NewLine,
                diagnostics.Select(diagnostic =>
                    $"{diagnostic.Id}: {diagnostic.GetMessage()}"));
        }

        private sealed class TestProviderProfile : DatabaseProviderProfile
        {
            public override DatabaseProviderVersion Version { get; } =
                DatabaseProviderVersion.Create(8, 0, 31);
        }
    }
}

