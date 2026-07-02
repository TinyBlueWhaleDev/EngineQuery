
namespace TinyBlueWhale.EngineQuery.Core.Parameters
{

    /// <summary>
    /// Generates deterministic SQL parameter names.
    /// </summary>
    public sealed class QueryParameterNameGenerator
    {
        private int _index;

        /// <summary>
        /// Creates the next SQL parameter name.
        /// </summary>
        /// <returns>
        /// Generated SQL parameter name.
        /// </returns>
        public string Next()
        {
            var name = $"@p{_index}";

            _index++;

            return name;
        }
    }
}
