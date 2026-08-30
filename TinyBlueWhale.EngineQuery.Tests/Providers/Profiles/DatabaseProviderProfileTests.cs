using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Sql.Profiles;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.Profiles
{
    [TestFixture]
    public sealed class DatabaseProviderProfileTests
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

        private sealed class TestProviderProfile : DatabaseProviderProfile
        {
            public override DatabaseProviderVersion Version { get; } =
                DatabaseProviderVersion.Create(8, 0, 31);
        }
    }
}
