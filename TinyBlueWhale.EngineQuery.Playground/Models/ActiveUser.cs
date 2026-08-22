namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    /// <summary>
    /// Represents an active user playground model.
    /// </summary>
    public sealed class ActiveUser
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents an archived user playground model.
    /// </summary>
    public sealed class ArchivedUser
    {
        /// <summary>
        /// Gets or sets the archived user identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the archived user email.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
