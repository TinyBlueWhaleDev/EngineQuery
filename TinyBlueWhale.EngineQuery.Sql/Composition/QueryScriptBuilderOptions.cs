using TinyBlueWhale.EngineQuery.Sql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Clauses.LateralJoins;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.Sql.Composition
{
    /// <summary>
    /// Defines provider-specific options used while creating a query script builder.
    /// </summary>
    /// <remarks>
    /// These options allow providers to override only the SQL clause builders that require
    /// provider-specific behavior while reusing the default query compilation pipeline.
    /// </remarks>
    public sealed class QueryScriptBuilderOptions
    {

        /// <summary>
        /// Gets or initializes the optional pagination strategy used by the query pipeline.
        /// </summary>
        /// <remarks>
        /// When this value is <see langword="null"/>, the pagination clause builder
        /// is not included in the query compilation pipeline.
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

        /// <summary>
        /// Gets or initializes the lateral join strategy associated with the provider profile.
        /// </summary>
        /// <remarks>
        /// A <see langword="null"/> value indicates that the configured provider profile
        /// does not expose lateral join support.
        /// </remarks>
        public ILateralJoinStrategy? LateralJoinStrategy { get; init; }

        /// <summary>
        /// Gets or initializes the optional factory used to create the INSERT clause builder.
        /// </summary>
        /// <remarks>
        /// When this value is <see langword="null"/>, the default
        /// <see cref="InsertClauseBuilder"/> is used.
        /// </remarks>
        public Func<InsertClauseBuilder>? InsertClauseBuilderFactory { get; init; }
    }
}
