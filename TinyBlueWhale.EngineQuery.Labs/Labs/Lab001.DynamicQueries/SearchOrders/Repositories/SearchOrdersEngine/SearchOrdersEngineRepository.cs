using Dapper;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;
using TinyBlueWhale.EngineQuery.Labs.Domain.Entities;
using TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence.SqlServer;
using TinyBlueWhale.EngineQuery.Labs.Labs.Helpers;
using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Repositories.Interfaces;
using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.ViewModels;

namespace TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Repositories.SearchOrdersEngine;

public sealed class SearchOrdersEngineRepository(
    IQueryEngine queryBuilder,
    ISqlConnectionFactory connectionFactory,
    ILogger<SearchOrdersEngineRepository> logger)
    : ISearchOrdersRepository
{
    public async Task<SearchOrdersViewModel> SearchAsync(
        SearchOrdersRequest request,
        CancellationToken cancellationToken)
    {
        var customerId = request.CustomerId.GetValueOrDefault();
        var status = request.Status.GetValueOrDefault();
        var createdFromUtc = request.CreatedFromUtc.GetValueOrDefault();
        var createdToUtc = request.CreatedToUtc.GetValueOrDefault();
        var minimumTotal = request.MinimumTotal.GetValueOrDefault();
        var maximumTotal = request.MaximumTotal.GetValueOrDefault();

        var search = request.Search?.Trim() ?? string.Empty;
        var hasSearch = !string.IsNullOrWhiteSpace(search);

        var offset = (request.Page - 1) * request.PageSize;

        var dataQuery = queryBuilder
            .From<Order>(alias: "o")
            .InnerJoin<Order, Customer>(alias: "c", on: (order, customer) => order.CustomerId == customer.Id)
            .Select<Order>(order => new
            {
                OrderId = order.Id,
                order.OrderNumber,
                order.OrderDateUtc,
                order.Status,
                order.TotalAmount,
                order.CustomerId
            })
            .Select<Customer>(customer => new
            {
                CustomerFirstName = customer.FirstName,
                CustomerLastName = customer.LastName,
                CustomerEmail = customer.Email
            })
            .WhereIf(request.CustomerId.HasValue, order => order.CustomerId == customerId)
            .WhereIf(request.Status.HasValue, order => order.Status == status)
            .WhereIf(request.CreatedFromUtc.HasValue, order => order.OrderDateUtc >= createdFromUtc)
            .WhereIf(request.CreatedToUtc.HasValue, order => order.OrderDateUtc <= createdToUtc)
            .WhereIf(request.MinimumTotal.HasValue, order => order.TotalAmount >= minimumTotal)
            .WhereIf(request.MaximumTotal.HasValue, order => order.TotalAmount <= maximumTotal)
            .WhereIf(hasSearch, order => order.OrderNumber.Contains(search), QueryLogicalOperator.Or)
            .WhereIf<Customer>(hasSearch, customer => customer.FirstName.Contains(search) ||
                customer.LastName.Contains(search) ||
                customer.Email.Contains(search), QueryLogicalOperator.Or);            
        

        var descending = request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
        var sortBy = request.SortBy?.Trim().ToUpperInvariant() ?? "ORDERDATEUTC";

        IOrderedQueryCommandBuilder<Order> orderedDataQuery;

        switch (sortBy)
        {
            case "ORDERNUMBER":
                orderedDataQuery = descending
                    ? dataQuery.OrderByDescending(order => order.OrderNumber)
                    : dataQuery.OrderBy(order => order.OrderNumber);
                break;

            case "CUSTOMERNAME":
                orderedDataQuery = descending
                    ? dataQuery.OrderByDescending<Customer>(customer => customer.FirstName)
                    : dataQuery.OrderBy<Customer>(customer => customer.FirstName);

                orderedDataQuery = descending
                    ? orderedDataQuery.ThenByDescending<Customer>(customer => customer.LastName)
                    : orderedDataQuery.ThenBy<Customer>(customer => customer.LastName);

                break;

            case "STATUS":
                orderedDataQuery = descending
                    ? dataQuery.OrderByDescending(order => order.Status)
                    : dataQuery.OrderBy(order => order.Status);

                break;

            case "TOTALAMOUNT":
                orderedDataQuery = descending
                    ? dataQuery.OrderByDescending(order => order.TotalAmount)
                    : dataQuery.OrderBy(order => order.TotalAmount);

                break;

            case "ORDERDATEUTC":
            default:
                orderedDataQuery = descending
                    ? dataQuery.OrderByDescending(order => order.OrderDateUtc)
                    : dataQuery.OrderBy(order => order.OrderDateUtc);

                break;
        }

        var generatedDataQuery = orderedDataQuery
            .ThenBy<Order>(order => order.Id)
            .Skip(offset)
            .Take(request.PageSize)
            .Build();

        var countQuery = queryBuilder
            .From<Order>(alias: "o")            
            .InnerJoin<Order, Customer>(alias: "c", on: (order, customer) => order.CustomerId == customer.Id)
            .SelectAggregate<Order>(QueryAggregateFunction.Count, order => order.Id, alias: "TotalCount")
            .WhereIf(request.CustomerId.HasValue, order => order.CustomerId == customerId)
            .WhereIf(request.Status.HasValue, order => order.Status == status)
            .WhereIf(request.CreatedFromUtc.HasValue, order => order.OrderDateUtc >= createdFromUtc)
            .WhereIf(request.CreatedToUtc.HasValue, order => order.OrderDateUtc <= createdToUtc)
            .WhereIf(request.MinimumTotal.HasValue, order => order.TotalAmount >= minimumTotal)
            .WhereIf(request.MaximumTotal.HasValue, order => order.TotalAmount <= maximumTotal)
            .WhereIf(hasSearch, order => order.OrderNumber.Contains(search), QueryLogicalOperator.Or)
            .WhereIf<Customer>(hasSearch, customer => customer.FirstName.Contains(search) ||
                customer.LastName.Contains(search) ||
                customer.Email.Contains(search), QueryLogicalOperator.Or)
            .Build();

        var dataParameters = generatedDataQuery.ToDynamicParameters();

        var countParameters = countQuery.ToDynamicParameters();

        logger.LogDebug("""
            
            Lab001 EngineQuery - Data Query

            {DataSql}

            Parameters: {DataParameters}

            Lab001 EngineQuery - Count Query

            {CountSql}

            Parameters: {CountParameters}
            """,
            generatedDataQuery.CommandText,
            string.Join(Environment.NewLine, generatedDataQuery.Parameters.Select(parameter => $"{parameter.Name} = {parameter.Value}")),
            countQuery.CommandText,
            string.Join(Environment.NewLine,countQuery.Parameters.Select(parameter =>$"{parameter.Name} = {parameter.Value}")));

        await using var connection = await connectionFactory
            .OpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var countCommand = new CommandDefinition(
            countQuery.CommandText,
            countParameters,
            cancellationToken: cancellationToken);

        var totalCount = checked(
            (int)await connection
                .ExecuteScalarAsync<long>(countCommand)
                .ConfigureAwait(false));

        var dataCommand = new CommandDefinition(
            generatedDataQuery.CommandText,
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
