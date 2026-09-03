using TinyBlueWhale.EngineQuery.Samples.Metadata;

namespace TinyBlueWhale.EngineQuery.Samples.Queries
{
    public sealed class SalesQueryScenario
    {
        public required string Name { get; init; }
        public required MetadataStrategy MetadataStrategy { get; init; }
        public required Type ResultType { get; init; }
        //public required Func<QueryBuilder, GeneratedSqlQuery> Build { get; init; }
    }
}
