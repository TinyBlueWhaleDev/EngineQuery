using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using TinyBlueWhale.EngineQuery.Samples.EntityFramework;

namespace TinyBlueWhale.EngineQuery.Samples.Providers
{
    public sealed class SampleProviderContext
    {
        public required SampleProviderKind Kind { get; init; }

        public required string Name { get; init; }

        public required string ConnectionString { get; init; }

        //public required Func<IEntityMetadataResolver, QueryBuilder> BuildQueryBuilder { get; init; }

        public required Func<DbConnection> OpenConnection { get; init; }

        public required Func<string, object?, DbParameter> BuildParameter { get; init; }

        public required Func<DbContextOptions<SampleDbContext>> BuildDbContextOptions { get; init; }
    }
}
