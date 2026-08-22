namespace TinyBlueWhale.EngineQuery.Playground.Models
{

    /// <summary>
    /// Represents a latest order projection used by APPLY examples.
    /// </summary>
    public sealed class LatestOrder
    {
        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the order total.
        /// </summary>
        public decimal Total { get; set; }
    }
}
