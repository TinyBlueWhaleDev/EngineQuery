using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Features
{
    /// <summary>
    /// Dispatches internal query feature operations to the corresponding query components.
    /// </summary>
    internal static class QueryFeatureOperationDispatcher
    {
        /// <summary>
        /// Applies the specified feature operation to the current query components.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <param name="components">
        /// Components associated with the current query composition.
        /// </param>
        /// <param name="operation">
        /// Feature operation to apply.
        /// </param>
        internal static void Apply<TProfile>(QueryCommandBuilderComponents<TProfile> components, IQueryFeatureOperation operation)
            where TProfile : IDatabaseProviderProfile
        {
            ArgumentNullException.ThrowIfNull(components);
            ArgumentNullException.ThrowIfNull(operation);

            switch (operation)
            {
                case PaginationSkipOperation paginationSkip:
                    components.PaginationClauseBuilder.SetSkip(paginationSkip.Count);
                    return;

                case PaginationTakeOperation paginationTake:
                    components.PaginationClauseBuilder.SetTake(paginationTake.Count);
                    return;

                default:
                    throw new NotSupportedException($"Query feature operation '{operation.GetType().Name}' is not registered.");
            }
        }
    }
}
