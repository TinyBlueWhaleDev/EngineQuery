using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    //<summary>
    //Validates advanced window function generation across providers.
    //</summary>
    //public static class AdvancedWindowFunctionQueryValidator
    //{
    //    /// <summary>
    //    /// Runs the validator.
    //    /// </summary>
    //    public static void Run()
    //    {
    //        var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

    //        ProviderQueryPrinter.Print(
    //            "SQL Server Advanced Window Functions",
    //            BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

    //        ProviderQueryPrinter.Print(
    //            "PostgreSQL Advanced Window Functions",
    //            BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

    //        ProviderQueryPrinter.Print(
    //            "MySQL Advanced Window Functions",
    //            BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
    //    }

    //    // Builds an advanced window function query.
    //    private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //         where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .From<JoinOrder>(alias: "o")
    //            .Select<JoinOrder>(o => new
    //            {
    //                OrderId = o.Id,
    //                o.UserId,
    //                o.Total
    //            })
    //            .SelectLag<JoinOrder>(
    //                expression: o => o.Total,
    //                alias: "PreviousOrderTotal",
    //                windowBuilder: window => window
    //                    .PartitionBy<JoinOrder>(o => o.UserId)
    //                    .OrderBy<JoinOrder>(o => o.Id),
    //                offset: 1)
    //            .SelectLead<JoinOrder>(
    //                expression: o => o.Total,
    //                alias: "NextOrderTotal",
    //                windowBuilder: window => window
    //                    .PartitionBy<JoinOrder>(o => o.UserId)
    //                    .OrderBy<JoinOrder>(o => o.Id),
    //                offset: 1)
    //            .Build();
    //    }
    //}
}
