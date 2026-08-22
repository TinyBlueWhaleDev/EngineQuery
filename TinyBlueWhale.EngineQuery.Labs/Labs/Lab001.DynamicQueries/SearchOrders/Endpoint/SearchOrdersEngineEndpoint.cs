using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TinyBlueWhale.EngineQuery.Labs.Domain.Enums;
using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Repositories.Interfaces;
using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.ViewModels;
using TinyBlueWhale.MinimalApi.Endpoints.Abstractions;
using TinyBlueWhale.MinimalApi.Endpoints.Attributes;
using TinyBlueWhale.MinimalApi.Versioning.Abstractions;

namespace TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Endpoint
{
    [Endpoint("Lab001.SearchOrders.EngineQuery")]
    public sealed class SearchOrdersEngineEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost(
                    "/api/labs/001/orders/engine-query",
                    async ([FromBody] SearchOrdersRequest request,
                        [FromKeyedServices(QueryImplementation.EngineQuery)] ISearchOrdersRepository repository,
                        [FromServices] IValidator<SearchOrdersRequest> validator,
                        CancellationToken cancellationToken) =>
                    {
                        var validation = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
                        if (!validation.IsValid)
                            return Results.ValidationProblem(validation.ToDictionary());

                        var response = await repository
                                            .SearchAsync(request, cancellationToken)
                                            .ConfigureAwait(false);

                        return Results.Ok(response);
                    })
                .HasApiVersion(ApiVersionRegistry.V1)
                .WithName("Lab001SearchOrdersEngineQuery")
                .WithTags("LAB-001")
                .Produces<SearchOrdersViewModel>()
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithSummary("Search Orders using Engine Query")
                .WithDescription("Searches orders using EngineQuery with the same query structure and result contract as Raw SQL.");
        }
    }
}
