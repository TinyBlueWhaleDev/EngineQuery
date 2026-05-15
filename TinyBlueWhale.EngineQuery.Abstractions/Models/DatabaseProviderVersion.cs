using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Models
{
    /// <summary>
    /// Represents a database provider version.
    /// </summary>
    public sealed record DatabaseProviderVersion
    {
        /// <summary>
        /// Gets the major version.
        /// </summary>
        public required int Major { get; init; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        public int Minor { get; init; }

        /// <summary>
        /// Gets the patch version.
        /// </summary>
        public int Patch { get; init; }

        /// <summary>
        /// Creates a database provider version from major, minor and patch values.
        /// </summary>
        public static DatabaseProviderVersion Create(int major, int minor = 0, int patch = 0)
        {
            return new DatabaseProviderVersion
            {
                Major = major,
                Minor = minor,
                Patch = patch
            };
        }

        /// <summary>
        /// Determines whether the current version is greater than or equal to the specified version.
        /// </summary>
        public bool IsAtLeast(int major, int minor = 0, int patch = 0)
        {
            if (Major != major)
                return Major > major;

            if (Minor != minor)
                return Minor > minor;

            return Patch >= patch;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }
    }
}
