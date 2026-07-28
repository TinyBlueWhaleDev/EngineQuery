using Dapper;
using System.Text;
using TinyBlueWhale.EngineQuery.Labs.Domain.Enums;
using TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence.SqlServer;
using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Repositories.Interfaces;
using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.ViewModels;

namespace TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Repositories.SearchOrdersRaw;

public sealed class SearchOrdersRawRepository(
    ISqlConnectionFactory connectionFactory,
    ILogger<SearchOrdersRawRepository> logger)
    : ISearchOrdersRepository
{
    public async Task<SearchOrdersViewModel> SearchAsync(
        SearchOrdersRequest request,
        CancellationToken cancellationToken)
    {
        var dataSql = new StringBuilder();
        var dataParameters = new DynamicParameters();

        dataSql.AppendLine("""
            SELECT
                o.Id AS OrderId,
                o.OrderNumber,
                o.OrderDateUtc,
                o.Status,
                o.TotalAmount,
                o.CustomerId,
                c.FirstName AS CustomerFirstName,
                c.LastName AS CustomerLastName,
                c.Email AS CustomerEmail
            FROM dbo.Orders AS o
            INNER JOIN dbo.Customers AS c
                ON c.Id = o.CustomerId
            WHERE 1 = 1
            """);

        if (request.CustomerId is int customerId)
        {
            dataSql.AppendLine("""AND o.CustomerId = @CustomerId""");

            dataParameters.Add("CustomerId", customerId);
        }

        if (request.Status is OrderStatus status)
        {
            dataSql.AppendLine("""AND o.Status = @Status""");

            dataParameters.Add("Status", status);
        }

        if (request.CreatedFromUtc is DateTime createdFromUtc)
        {
            dataSql.AppendLine("""AND o.OrderDateUtc >= @CreatedFromUtc""");

            dataParameters.Add("CreatedFromUtc",createdFromUtc);
        }

        if (request.CreatedToUtc is DateTime createdToUtc)
        {
            dataSql.AppendLine("""AND o.OrderDateUtc <= @CreatedToUtc""");

            dataParameters.Add("CreatedToUtc",createdToUtc);
        }

        if (request.MinimumTotal is decimal minimumTotal)
        {
            dataSql.AppendLine("""AND o.TotalAmount >= @MinimumTotal""");

            dataParameters.Add("MinimumTotal", minimumTotal);
        }

        if (request.MaximumTotal is decimal maximumTotal)
        {
            dataSql.AppendLine("""AND o.TotalAmount <= @MaximumTotal""");

            dataParameters.Add("MaximumTotal",maximumTotal);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            dataSql.AppendLine("""
                AND
                (
                    o.OrderNumber LIKE @Search
                    OR c.FirstName LIKE @Search
                    OR c.LastName LIKE @Search
                    OR c.Email LIKE @Search
                )
                """);

            dataParameters.Add("Search",$"%{request.Search.Trim()}%");
        }

        var direction = request.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? "ASC"
                : "DESC";

        var orderBy = request.SortBy.ToUpperInvariant() switch
        {
            "ORDERNUMBER" =>
                $"o.OrderNumber {direction}",

            "CUSTOMERNAME" =>
                $"c.FirstName {direction}, c.LastName {direction}",

            "STATUS" =>
                $"o.Status {direction}",

            "TOTALAMOUNT" =>
                $"o.TotalAmount {direction}",

            _ =>
                $"o.OrderDateUtc {direction}"
        };

        dataSql.Append("ORDER BY ")
            .Append(orderBy)
            .AppendLine(", o.Id ASC");

        dataSql.AppendLine("""
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;
            """);

        dataParameters.Add("Offset", (request.Page - 1) * request.PageSize);

        dataParameters.Add("PageSize", request.PageSize);

        var countSql = new StringBuilder();
        var countParameters = new DynamicParameters();

        countSql.AppendLine("""
            SELECT COUNT_BIG(1)
            FROM dbo.Orders AS o
            INNER JOIN dbo.Customers AS c
                ON c.Id = o.CustomerId
            WHERE 1 = 1
            """);

        if (request.CustomerId is int countCustomerId)
        {
            countSql.AppendLine("""AND o.CustomerId = @CustomerId""");

            countParameters.Add("CustomerId", countCustomerId);
        }

        if (request.Status is OrderStatus countStatus)
        {
            countSql.AppendLine("""AND o.Status = @Status""");

            countParameters.Add("Status",countStatus);
        }

        if (request.CreatedFromUtc is DateTime countCreatedFromUtc)
        {
            countSql.AppendLine("""AND o.OrderDateUtc >= @CreatedFromUtc""");

            countParameters.Add("CreatedFromUtc",countCreatedFromUtc);
        }

        if (request.CreatedToUtc is DateTime countCreatedToUtc)
        {
            countSql.AppendLine("""AND o.OrderDateUtc <= @CreatedToUtc""");

            countParameters.Add("CreatedToUtc",countCreatedToUtc);
        }

        if (request.MinimumTotal is decimal countMinimumTotal)
        {
            countSql.AppendLine("""AND o.TotalAmount >= @MinimumTotal""");

            countParameters.Add("MinimumTotal",countMinimumTotal);
        }

        if (request.MaximumTotal is decimal countMaximumTotal)
        {
            countSql.AppendLine("""AND o.TotalAmount <= @MaximumTotal""");

            countParameters.Add("MaximumTotal",countMaximumTotal);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            countSql.AppendLine("""
                AND
                (
                    o.OrderNumber LIKE @Search
                    OR c.FirstName LIKE @Search
                    OR c.LastName LIKE @Search
                    OR c.Email LIKE @Search
                )
                """);

            countParameters.Add(
                "Search",
                $"%{request.Search.Trim()}%");
        }

        logger.LogDebug(
            """
            Lab001 Raw SQL - Data Query

            {DataSql}

            Parameters: {DataParameters}

            Lab001 Raw SQL - Count Query

            {CountSql}

            Parameters: {CountParameters}
            """,
            dataSql.ToString(),
            string.Join(", ", dataParameters.ParameterNames),
            countSql.ToString(),
            string.Join(", ", countParameters.ParameterNames));

        await using var connection = await connectionFactory
            .OpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var countCommand = new CommandDefinition(
            countSql.ToString(),
            countParameters,
            cancellationToken: cancellationToken);

        var totalCount = checked((int)await connection
            .ExecuteScalarAsync<long>(countCommand)
            .ConfigureAwait(false));

        var dataCommand = new CommandDefinition(
            dataSql.ToString(),
            dataParameters,
            cancellationToken: cancellationToken);

        var items = (await connection
            .QueryAsync<SearchOrderItemViewModel>(dataCommand)
            .ConfigureAwait(false))
            .AsList();

        return new SearchOrdersViewModel
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
