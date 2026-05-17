using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Samples.Metadata;
using TinyBlueWhale.EngineQuery.Samples.Queries.Scenarios;

namespace TinyBlueWhale.EngineQuery.Samples.Queries
{
    public static class BuildSalesQueryScenarios
    {
        public static IReadOnlyList<SalesQueryScenario> Create(MetadataStrategy strategy)
        {
            return strategy switch
            {
                MetadataStrategy.Fluent => CreateFluent(),
                MetadataStrategy.Attribute => CreateAttribute(),
                MetadataStrategy.EntityFramework => CreateEntityFramework(),
                _ => throw new NotSupportedException($"Metadata strategy '{strategy}' is not supported.")
            };
        }

        private static IReadOnlyList<SalesQueryScenario> CreateFluent()
        {
            return
            [
                ..BasicQueries.CreateForFluent(),
                ..AggregationQueries.CreateForFluent(),
                ..CteQueries.CreateForFluent(),
                ..SetOperationQueries.CreateForFluent(),
                ..WindowQueries.CreateForFluent()
            ];
        }

        private static IReadOnlyList<SalesQueryScenario> CreateAttribute()
        {
            return
            [
                ..BasicQueries.CreateForAttribute(),
                ..AggregationQueries.CreateForAttribute(),
                ..CteQueries.CreateForAttribute(),
                ..SetOperationQueries.CreateForAttribute(),
                ..WindowQueries.CreateForAttribute()
            ];
        }

        private static IReadOnlyList<SalesQueryScenario> CreateEntityFramework()
        {
            return
            [
                ..BasicQueries.CreateForEntityFramework(),
                ..AggregationQueries.CreateForEntityFramework(),
                ..CteQueries.CreateForEntityFramework(),
                ..SetOperationQueries.CreateForEntityFramework(),
                ..WindowQueries.CreateForEntityFramework()
            ];
        }
    }
}
