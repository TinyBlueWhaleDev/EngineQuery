
namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents an argument used by a scalar SQL function.
    /// </summary>
    public sealed record QueryScalarFunctionArgumentDefinition
    {
        /// <summary>
        /// Gets the entity property name when the argument represents a column.
        /// </summary>
        public string? PropertyName { get; init; }

        /// <summary>
        /// Gets the constant value when the argument represents a parameterized value.
        /// </summary>
        public object? ConstantValue { get; init; }

        /// <summary>
        /// Gets a value indicating whether the argument represents a column reference.
        /// </summary>
        public bool IsColumn => !string.IsNullOrWhiteSpace(PropertyName);
    }
}
