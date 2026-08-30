using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.MySql.Profiles
{
    /// <summary>
    /// Represents the EngineQuery provider profile for MySQL 8.0.31.
    /// </summary>
    /// <remarks>
    /// This profile represents the MySQL version boundary at which EngineQuery
    /// may expose the complete currently supported MySQL query feature set.
    /// </remarks>
    public class MySql8031Profile : MySql8014Profile
    {
        /// <inheritdoc />
        public override DatabaseProviderVersion Version { get; } = DatabaseProviderVersion.Create(8, 0, 31);
    }
}
