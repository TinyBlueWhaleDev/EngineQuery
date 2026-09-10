namespace TinyBlueWhale.EngineQuery.SqlServer.Profiles
{
    /// <summary>
    /// Represents the default SQL Server provider profile used by EngineQuery.
    /// </summary>
    /// <remarks>
    /// The default profile targets the minimum supported SQL Server version
    /// so that version-specific functionality is not exposed unless the
    /// consumer explicitly selects a compatible version profile.
    /// </remarks>
    public sealed class SqlServerDefaultProfile : SqlServer2008Profile
    {
    }
}
