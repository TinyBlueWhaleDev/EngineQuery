using FluentValidation;
using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.ViewModels;

namespace TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Validators;

public sealed class SearchOrdersRequestValidator : AbstractValidator<SearchOrdersRequest>
{
    private static readonly string[] SortFields =
        ["OrderDateUtc", "OrderNumber", "CustomerName", "Status", "TotalAmount"];

    public SearchOrdersRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.CreatedFromUtc).LessThanOrEqualTo(x => x.CreatedToUtc)
            .When(x => x.CreatedFromUtc.HasValue && x.CreatedToUtc.HasValue);
        RuleFor(x => x.MinimumTotal).GreaterThanOrEqualTo(0).When(x => x.MinimumTotal.HasValue);
        RuleFor(x => x.MaximumTotal).GreaterThanOrEqualTo(0).When(x => x.MaximumTotal.HasValue);
        RuleFor(x => x.MinimumTotal).LessThanOrEqualTo(x => x.MaximumTotal)
            .When(x => x.MinimumTotal.HasValue && x.MaximumTotal.HasValue);
        RuleFor(x => x.SortDirection)
            .Must(x => x.Equals("asc", StringComparison.OrdinalIgnoreCase)
                || x.Equals("desc", StringComparison.OrdinalIgnoreCase));
        RuleFor(x => x.SortBy)
            .Must(x => SortFields.Contains(x, StringComparer.OrdinalIgnoreCase));
    }
}
