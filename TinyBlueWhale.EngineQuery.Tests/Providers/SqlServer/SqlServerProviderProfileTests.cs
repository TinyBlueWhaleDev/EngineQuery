namespace TinyBlueWhale.EngineQuery.Tests.Providers.SqlServer
{
    ///// <summary>
    ///// Validates SQL Server provider version profiles and their
    ///// exposed query features.
    ///// </summary>
    //[TestFixture]
    //public sealed class SqlServerProviderProfileTests
    //{
    //    [Test]
    //    public void SqlServer2008Profile_ShouldExposeExpectedVersion()
    //    {
    //        var profile = new SqlServer2008Profile();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(profile.Version.Major, Is.EqualTo(10));
    //            Assert.That(profile.Version.Minor, Is.EqualTo(0));
    //            Assert.That(profile.Version.Patch, Is.EqualTo(0));
    //        });
    //    }

    //    [Test]
    //    public void SqlServer2008Profile_ShouldNotExposePaginationFeature()
    //    {
    //        var profile = new SqlServer2008Profile();

    //        Assert.That(profile, Is.Not.AssignableTo<IPaginationFeature>());
    //    }

    //    [Test]
    //    public void SqlServer2012Profile_ShouldExposeExpectedVersion()
    //    {
    //        var profile = new SqlServer2012Profile();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(profile.Version.Major, Is.EqualTo(11));
    //            Assert.That(profile.Version.Minor, Is.EqualTo(0));
    //            Assert.That(profile.Version.Patch, Is.EqualTo(0));
    //        });
    //    }

    //    [Test]
    //    public void SqlServer2012Profile_ShouldExposePaginationFeature()
    //    {
    //        var profile = new SqlServer2012Profile();

    //        Assert.That(profile, Is.AssignableTo<IPaginationFeature>());
    //    }

    //    [Test]
    //    public void SqlServer2012Profile_ShouldUseSqlServerPaginationStrategy()
    //    {
    //        var profile = new SqlServer2012Profile();

    //        var paginationFeature = (IPaginationFeature)profile;

    //        var strategy = paginationFeature.CreatePaginationStrategy();

    //        Assert.That(strategy, Is.TypeOf<SqlServer2012PaginationStrategy>());
    //    }

    //    [Test]
    //    public void SqlServerDefaultProfile_ShouldUseMinimumSupportedVersion()
    //    {
    //        var profile = new SqlServerDefaultProfile();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(profile, Is.AssignableTo<SqlServer2008Profile>());

    //            Assert.That(profile.Version.Major, Is.EqualTo(10));
    //            Assert.That(profile.Version.Minor, Is.EqualTo(0));
    //            Assert.That(profile.Version.Patch, Is.EqualTo(0));
    //        });
    //    }

    //    [Test]
    //    public void SqlServerDefaultProfile_ShouldNotExposePaginationFeature()
    //    {
    //        var profile = new SqlServerDefaultProfile();

    //        Assert.That(profile, Is.Not.AssignableTo<IPaginationFeature>());
    //    }
    //}
}
