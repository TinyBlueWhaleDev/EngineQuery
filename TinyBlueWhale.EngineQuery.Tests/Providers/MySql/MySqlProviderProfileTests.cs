using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.Sql.Clauses.Pagination;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.MySql
{
//    [TestFixture]
//    public sealed class MySqlProviderProfileTests
//    {
//        [Test]
//        public void MySql57Profile_ShouldExposeExpectedVersion()
//        {
//            var profile = new MySql57Profile();

//            Assert.Multiple(() =>
//            {
//                Assert.That(profile, Is.AssignableTo<IDatabaseProviderProfile>());
//                Assert.That(profile.Version.Major, Is.EqualTo(5));
//                Assert.That(profile.Version.Minor, Is.EqualTo(7));
//                Assert.That(profile.Version.Patch, Is.EqualTo(0));
//            });
//        }

//        [Test]
//        public void MySql80Profile_ShouldExposeExpectedVersion()
//        {
//            var profile = new MySql80Profile();

//            Assert.Multiple(() =>
//            {
//                Assert.That(profile.Version.Major, Is.EqualTo(8));
//                Assert.That(profile.Version.Minor, Is.EqualTo(0));
//                Assert.That(profile.Version.Patch, Is.EqualTo(0));
//            });
//        }

//        [Test]
//        public void MySql8014Profile_ShouldExposeExpectedVersion()
//        {
//            var profile = new MySql8014Profile();

//            Assert.Multiple(() =>
//            {
//                Assert.That(profile.Version.Major, Is.EqualTo(8));
//                Assert.That(profile.Version.Minor, Is.EqualTo(0));
//                Assert.That(profile.Version.Patch, Is.EqualTo(14));
//            });
//        }

//        [Test]
//        public void MySql8031Profile_ShouldExposeExpectedVersion()
//        {
//            var profile = new MySql8031Profile();

//            Assert.Multiple(() =>
//            {
//                Assert.That(profile.Version.Major, Is.EqualTo(8));
//                Assert.That(profile.Version.Minor, Is.EqualTo(0));
//                Assert.That(profile.Version.Patch, Is.EqualTo(31));
//            });
//        }

//        [Test]
//        public void MySqlDefaultProfile_ShouldUseMinimumSupportedVersion()
//        {
//            var profile = new MySqlDefaultProfile();

//            Assert.Multiple(() =>
//            {
//                Assert.That(profile, Is.AssignableTo<MySql57Profile>());
//                Assert.That(profile.Version.Major, Is.EqualTo(5));
//                Assert.That(profile.Version.Minor, Is.EqualTo(7));
//                Assert.That(profile.Version.Patch, Is.EqualTo(0));
//            });
//        }

//        [Test]
//        public void MySql57Profile_ShouldExposePaginationFeature()
//        {
//            var profile = new MySql57Profile();

//            Assert.That(profile, Is.AssignableTo<IPaginationFeature>());
//        }

//        [Test]
//        public void MySql57Profile_ShouldUseDefaultPaginationStrategy()
//        {
//            var profile = new MySql57Profile();

//            var paginationFeature = (IPaginationFeature)profile;

//            var strategy = paginationFeature.CreatePaginationStrategy();

//            Assert.That(strategy, Is.TypeOf<PaginationStrategy>());
//        }

//        [TestCase(typeof(MySql80Profile))]
//        [TestCase(typeof(MySql8014Profile))]
//        [TestCase(typeof(MySql8031Profile))]
//        [TestCase(typeof(MySqlDefaultProfile))]
//        public void MySqlProfiles_ShouldInheritPaginationFeature(Type profileType)
//        {
//            Assert.That(typeof(IPaginationFeature).IsAssignableFrom(profileType), Is.True);
//        }

//    }
}
