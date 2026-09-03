using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.Sql.Composition
{
    /// <summary>
    /// Represents SQL feature strategies resolved for a database provider profile.
    /// </summary>
    /// <remarks>
    /// This composition isolates provider profile resolution from the query compiler
    /// and allows additional feature strategies to be added without expanding
    /// provider compiler constructor signatures.
    /// </remarks>
    public sealed class QueryFeatureComposition
    {
        /// <summary>
        /// Gets or initializes the pagination strategy associated with the provider profile.
        /// </summary>
        /// <remarks>
        /// A <see langword="null"/> value indicates that the configured provider profile
        /// does not expose pagination support.
        /// </remarks>
        public IPaginationStrategy? PaginationStrategy { get; init; }

        /// <summary>
        /// Gets or initializes the common table expression strategy associated with the provider profile.
        /// </summary>
        /// <remarks>
        /// A <see langword="null"/> value indicates that the configured provider profile
        /// does not expose common table expression support.
        /// </remarks>
        public ICTEStrategy? CteStrategy { get; init; }
    }
}
