using TinyBlueWhale.EngineQuery.Abstractions.Attributes;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features
{
    /// <summary>
    /// Identifies a database provider profile that supports query pagination.
    /// </summary>
    [QueryFeatureSurface(typeof(IQueryPaginationBuilder<>))]
    public interface IPaginationFeature
    {
    }
}
