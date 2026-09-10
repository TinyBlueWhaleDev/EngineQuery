using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Metadata
{
    /// <summary>
    /// Validates metadata resolution behavior used by query builders.
    /// </summary>
    [TestFixture]
    internal sealed class MetadataResolutionTests
    {
        [Test]
        public void From_WhenMetadataIsNotRegistered_ShouldThrow()
        {
            var metadataResolver =
                TestMetadataFactory.CreateMetadataResolver();

            var queryBuilder =
                SqlServerQueryCompiler.Factory.Create(metadataResolver);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                queryBuilder.From<UnmappedEntity>("x"));

            Assert.That(exception, Is.Not.Null);
        }
    }
}
