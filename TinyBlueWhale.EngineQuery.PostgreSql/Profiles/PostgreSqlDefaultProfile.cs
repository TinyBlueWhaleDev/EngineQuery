namespace TinyBlueWhale.EngineQuery.PostgreSql.Profiles
{
    /// <summary>
    /// Represents the default PostgreSQL provider profile used by EngineQuery.
    /// </summary>
    /// <remarks>
    /// The default profile targets the minimum supported PostgreSQL version so
    /// that version-specific functionality is not exposed unless the consumer
    /// explicitly selects a compatible version profile.
    /// </remarks>
    public sealed class PostgreSqlDefaultProfile : PostgreSql84Profile
    {
    }
}
