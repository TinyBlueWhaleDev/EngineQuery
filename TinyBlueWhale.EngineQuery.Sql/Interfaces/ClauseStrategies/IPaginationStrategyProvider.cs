namespace TinyBlueWhale.EngineQuery.Sql.Interfaces.ClauseStrategies
{
    /// <summary>
    /// Defines a provider profile capable of supplying SQL pagination behavior.
    /// </summary>
    public interface IPaginationStrategyProvider
    {
        /// <summary>
        /// Creates the pagination strategy associated with the provider profile.
        /// </summary>
        /// <returns>
        /// Pagination strategy used to generate provider-specific pagination SQL.
        /// </returns>
        IPaginationStrategy CreatePaginationStrategy();
    }
}
