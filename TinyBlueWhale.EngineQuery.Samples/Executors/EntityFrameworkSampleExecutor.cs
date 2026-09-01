using Microsoft.EntityFrameworkCore;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Samples.Metadata;
using TinyBlueWhale.EngineQuery.Samples.Providers;
using TinyBlueWhale.EngineQuery.Samples.Queries;
using TinyBlueWhale.EngineQuery.Samples.Results;

namespace TinyBlueWhale.EngineQuery.Samples.Executors
{
    //public sealed class EntityFrameworkSampleExecutor : ISampleExecutor
    //{
    //    public string Name => "EF Core";

    //    public async Task<SampleExecutionResult> ExecuteAsync(
    //        SampleProviderContext provider,
    //        SalesQueryScenario scenario,
    //        CancellationToken cancellationToken = default)
    //    {
    //        var metadataName = BuildMetadataResolver.GetDisplayName(scenario.MetadataStrategy);
    //        GeneratedSqlQuery? query = null;

    //        try
    //        {
    //            var metadataResolver = BuildMetadataResolver.Create(provider, scenario.MetadataStrategy);
    //            var queryBuilder = provider.BuildQueryBuilder(metadataResolver);

    //            query = scenario.Build(queryBuilder);

    //            if (string.IsNullOrWhiteSpace(provider.ConnectionString))
    //                return BuildSkipped(provider, scenario, metadataName, query);

    //            await using var dbContext = BuildEntityFrameworkMetadata.CreateDbContext(provider);

    //            var rows = await ExecuteKeylessQueryAsync(
    //                dbContext,
    //                scenario.ResultType,
    //                query,
    //                provider,
    //                cancellationToken);

    //            return new SampleExecutionResult
    //            {
    //                Provider = provider.Name,
    //                Executor = Name,
    //                Metadata = metadataName,
    //                Query = scenario.Name,
    //                CommandText = query.CommandText,
    //                Parameters = query.Parameters,
    //                Status = "Success",
    //                RowCount = rows.Count,
    //                ResultText = SerializeRowsAsCsv.FromObjects(rows)
    //            };
    //        }
    //        catch (Exception exception)
    //        {
    //            return BuildError(provider, scenario, metadataName, query, exception);
    //        }
    //    }

    //    private static async Task<IReadOnlyList<object>> ExecuteKeylessQueryAsync(
    //        DbContext dbContext,
    //        Type resultType,
    //        GeneratedSqlQuery query,
    //        SampleProviderContext provider,
    //        CancellationToken cancellationToken)
    //    {
    //        var parameters = query.Parameters
    //            .Select(parameter => provider.BuildParameter(parameter.Name, parameter.Value))
    //            .ToArray();

    //        var setMethod = typeof(DbContext)
    //            .GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
    //            .MakeGenericMethod(resultType);

    //        var dbSet = setMethod.Invoke(dbContext, null)!;

    //        var fromSqlRawMethod = typeof(RelationalQueryableExtensions)
    //            .GetMethods()
    //            .Single(method =>
    //                method.Name == nameof(RelationalQueryableExtensions.FromSqlRaw) &&
    //                method.GetParameters().Length == 3)
    //            .MakeGenericMethod(resultType);

    //        var queryable = fromSqlRawMethod.Invoke(
    //            null,
    //            [dbSet, query.CommandText, parameters])!;

    //        var asNoTrackingMethod = typeof(EntityFrameworkQueryableExtensions)
    //            .GetMethods()
    //            .Single(method =>
    //                method.Name == nameof(EntityFrameworkQueryableExtensions.AsNoTracking) &&
    //                method.GetParameters().Length == 1)
    //            .MakeGenericMethod(resultType);

    //        var noTrackingQuery = asNoTrackingMethod.Invoke(
    //            null,
    //            [queryable])!;

    //        var toListAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
    //            .GetMethods()
    //            .Where(method => method.Name == nameof(EntityFrameworkQueryableExtensions.ToListAsync))
    //            .Single(method => method.GetParameters().Length == 2)
    //            .MakeGenericMethod(resultType);

    //        var task = (Task)toListAsyncMethod.Invoke(
    //            null,
    //            [noTrackingQuery, cancellationToken])!;

    //        await task.ConfigureAwait(false);

    //        var resultProperty = task.GetType().GetProperty("Result")!;
    //        var typedRows = (System.Collections.IEnumerable)resultProperty.GetValue(task)!;

    //        return typedRows.Cast<object>().ToList();
    //    }

    //    private SampleExecutionResult BuildSkipped(
    //        SampleProviderContext provider,
    //        SalesQueryScenario scenario,
    //        string metadataName,
    //        GeneratedSqlQuery query)
    //    {
    //        return new SampleExecutionResult
    //        {
    //            Provider = provider.Name,
    //            Executor = Name,
    //            Metadata = metadataName,
    //            Query = scenario.Name,
    //            CommandText = query.CommandText,
    //            Parameters = query.Parameters,
    //            Status = "Skipped",
    //            RowCount = 0,
    //            ErrorMessage = "Connection string is not configured."
    //        };
    //    }

    //    private SampleExecutionResult BuildError(
    //        SampleProviderContext provider,
    //        SalesQueryScenario scenario,
    //        string metadataName,
    //        GeneratedSqlQuery? query,
    //        Exception exception)
    //    {
    //        return new SampleExecutionResult
    //        {
    //            Provider = provider.Name,
    //            Executor = Name,
    //            Metadata = metadataName,
    //            Query = scenario.Name,
    //            CommandText = query?.CommandText ?? "(SQL generation failed)",
    //            Parameters = query?.Parameters ?? [],
    //            Status = "Error",
    //            RowCount = 0,
    //            ErrorMessage = exception.ToString()
    //        };
    //    }
    //}
}
