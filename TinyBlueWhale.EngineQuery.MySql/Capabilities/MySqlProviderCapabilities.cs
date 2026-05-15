using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.MySql.Capabilities
{

    /// <summary>
    /// Defines MySQL provider capability support.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="MySqlProviderCapabilities"/> class.
    /// </remarks>
    public sealed class MySqlProviderCapabilities(DatabaseProviderVersion version) : IDatabaseProviderCapabilities
    {
        private static readonly DatabaseProviderVersion DefaultVersion = DatabaseProviderVersion.Create(8, 0, 31);

        private readonly DatabaseProviderVersion _version = version ?? throw new ArgumentNullException(nameof(version));

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlProviderCapabilities"/> class using the default modern MySQL version.
        /// </summary>
        public MySqlProviderCapabilities()
            : this(DefaultVersion)
        {
        }

        /// <inheritdoc />
        public bool SupportsCommonTableExpressions => _version.IsAtLeast(8, 0);

        /// <inheritdoc />
        public bool SupportsRecursiveCommonTableExpressions => _version.IsAtLeast(8, 0);

        /// <inheritdoc />
        public bool SupportsWindowFunctions => _version.IsAtLeast(8, 0);

        /// <inheritdoc />
        public bool SupportsLateralJoins => _version.IsAtLeast(8, 0, 14);

        /// <inheritdoc />
        public bool SupportsIntersect => _version.IsAtLeast(8, 0, 31);

        /// <inheritdoc />
        public bool SupportsExcept => _version.IsAtLeast(8, 0, 31);

        /// <inheritdoc />
        public bool SupportsOffsetFetchPagination => false;

        /// <inheritdoc />
        public bool SupportsLimitOffsetPagination => true;
    }
}
