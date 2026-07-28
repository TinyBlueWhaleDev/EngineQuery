using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.ViewModels;

namespace TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Repositories.Interfaces
{
    public interface ISearchOrdersRepository
    {
        Task<SearchOrdersViewModel> SearchAsync(SearchOrdersRequest request, CancellationToken cancellationToken);
    }
}
