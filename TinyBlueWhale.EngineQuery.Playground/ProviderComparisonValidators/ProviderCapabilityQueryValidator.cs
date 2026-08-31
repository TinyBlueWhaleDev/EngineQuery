using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Composition;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates provider capability checks during SQL generation.
    /// </summary>
    public static class ProviderCapabilityQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            ProviderQueryPrinter.Print(
                "Provider Capability Supported Window Function",
                BuildSupportedQuery());

            PrintUnsupportedWindowFunction();
        }

        // Builds a query using supported provider capabilities.
        private static GeneratedSqlQuery BuildSupportedQuery()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            return SqlServerQueryCompiler.Factory.Create(metadataResolver)
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.Total
                })
                .SelectNtile(
                    buckets: 4,
                    alias: "Quartile",
                    windowBuilder: window => window
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        // Prints an unsupported provider capability validation result.
        private static void PrintUnsupportedWindowFunction()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            try
            {
                _ = SqlServerQueryCompiler.Factory.Create(metadataResolver)
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(o => new
                    {
                        OrderId = o.Id,
                        o.Total
                    })
                    .SelectNtile(
                        buckets: 4,
                        alias: "Quartile",
                        windowBuilder: window => window
                            .OrderByDescending<JoinOrder>(o => o.Total))
                    .Build();
            }
            catch (NotSupportedException exception)
            {
                Console.WriteLine("--- Provider Capability Unsupported Window Function ---");
                Console.WriteLine(exception.Message);
                Console.WriteLine();
            }
        }
    }
}
