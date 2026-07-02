
namespace TinyBlueWhale.EngineQuery.Metadata.Models
{

    /// <summary>
    /// Represents a metadata resolution strategy.
    /// </summary>
    /// <param name="Name">
    /// Metadata strategy name.
    /// </param>
    public readonly record struct MetadataStrategy(string Name)
    {
        /// <summary>
        /// Fluent metadata strategy.
        /// </summary>
        public static readonly MetadataStrategy Fluent = new("Fluent");

        /// <summary>
        /// Attribute metadata strategy.
        /// </summary>
        public static readonly MetadataStrategy Attribute = new("Attribute");

        /// <inheritdoc />
        public override string ToString() => Name;
    }
}
