using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.SqlServer.Capabilities
{
    /// <summary>
    /// Defines SQL Server provider capability support.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SqlServerProviderCapabilities"/> class.
    /// </remarks>
    public sealed class SqlServerProviderCapabilities(DatabaseProviderVersion version) : IDatabaseProviderCapabilities
    {
        private static readonly DatabaseProviderVersion DefaultVersion = DatabaseProviderVersion.Create(16, 0);

        private readonly DatabaseProviderVersion _version = version ?? throw new ArgumentNullException(nameof(version));

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProviderCapabilities"/> class using the default modern SQL Server version.
        /// </summary>
        public SqlServerProviderCapabilities()
            : this(DefaultVersion)
        {
        }      

        /// <inheritdoc />
        public bool SupportsWindowFunctions => true;

        /// <inheritdoc />
        public bool SupportsLateralJoins => true;

        /// <inheritdoc />
        public bool SupportsIntersect => true;

        /// <inheritdoc />
        public bool SupportsExcept => true;

        /// <inheritdoc />
        public bool SupportsOffsetFetchPagination => _version.IsAtLeast(11, 0);

        /// <inheritdoc />
        public bool SupportsLimitOffsetPagination => false;
    }
}
