namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    /// <summary>
    /// Represents a derived table projection containing order summary information.
    /// </summary>
    public sealed class OrderSummary
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the order count.
        /// </summary>
        public int OrderCount { get; set; }
    }
}
