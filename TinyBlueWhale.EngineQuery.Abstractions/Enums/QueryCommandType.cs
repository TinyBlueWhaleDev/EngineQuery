namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Defines the SQL command type represented by a compiled query definition.
    /// </summary>
    public enum QueryCommandType
    {
        /// <summary>
        /// Represents a SQL SELECT query.
        /// </summary>
        Select = 0,

        /// <summary>
        /// Represents a SQL INSERT command.
        /// </summary>
        Insert = 1,

        /// <summary>
        /// Represents a SQL UPDATE command.
        /// </summary>
        Update = 2,

        /// <summary>
        /// Represents a SQL DELETE command.
        /// </summary>
        Delete = 3
    }
}
