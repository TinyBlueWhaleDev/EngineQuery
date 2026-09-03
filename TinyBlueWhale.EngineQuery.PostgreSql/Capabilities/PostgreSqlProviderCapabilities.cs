using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Capabilities
{

    /// <summary>
    /// Defines PostgreSQL provider capability support.
    /// </summary>
    public sealed class PostgreSqlProviderCapabilities : IDatabaseProviderCapabilities
    {
        private static readonly DatabaseProviderVersion DefaultVersion = DatabaseProviderVersion.Create(16, 0);

        private readonly DatabaseProviderVersion _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlProviderCapabilities"/> class using the default modern PostgreSQL version.
        /// </summary>
        public PostgreSqlProviderCapabilities()
            : this(DefaultVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlProviderCapabilities"/> class.
        /// </summary>
        public PostgreSqlProviderCapabilities(DatabaseProviderVersion version)
        {
            _version = version ?? throw new ArgumentNullException(nameof(version));
        }        

        /// <inheritdoc />
        public bool SupportsLateralJoins => _version.IsAtLeast(9, 3);

        /// <inheritdoc />
        public bool SupportsIntersect => true;

        /// <inheritdoc />
        public bool SupportsExcept => true;
    }
}
