
namespace TinyBlueWhale.EngineQuery.MySql.Profiles
{
    /// <summary>
    /// Represents the default MySQL provider profile used by EngineQuery.
    /// </summary>
    /// <remarks>
    /// The default profile targets the minimum supported MySQL version to expose
    /// only query functionality that is safe when no explicit version is configured.
    /// Consumers that require version-specific functionality should use the
    /// corresponding version profile.
    /// </remarks>
    public sealed class MySqlDefaultProfile : MySql57Profile
    {
    }
}
