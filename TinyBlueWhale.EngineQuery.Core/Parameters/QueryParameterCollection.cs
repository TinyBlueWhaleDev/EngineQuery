using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Core.Parameters
{
    /// <summary>
    /// Stores SQL parameters generated during query compilation.
    /// </summary>
    public sealed class QueryParameterCollection
    {
        private readonly QueryParameterNameGenerator _nameGenerator = new();
        private readonly List<QuerySqlParameter> _parameters = [];

        /// <summary>
        /// Gets the generated SQL parameters.
        /// </summary>
        public IReadOnlyList<QuerySqlParameter> Parameters => _parameters;

        /// <summary>
        /// Adds a parameter value and returns its generated parameter name.
        /// </summary>
        /// <param name="value">
        /// Parameter value.
        /// </param>
        /// <returns>
        /// Generated SQL parameter name.
        /// </returns>
        public string Add(object? value)
        {
            var parameterName = _nameGenerator.Next();

            _parameters.Add(
                new QuerySqlParameter
                {
                    Name = parameterName,
                    Value = value
                });

            return parameterName;
        }

        /// <summary>
        /// Adds an existing SQL parameter using a newly generated parameter name.
        /// </summary>
        /// <param name="parameter">
        /// Existing SQL parameter.
        /// </param>
        /// <returns>
        /// Rewritten SQL parameter.
        /// </returns>
        public QuerySqlParameter AddRewritten(QuerySqlParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            var parameterName = _nameGenerator.Next();

            var rewrittenParameter = new QuerySqlParameter
            {
                Name = parameterName,
                Value = parameter.Value
            };

            _parameters.Add(rewrittenParameter);

            return rewrittenParameter;
        }

        /// <summary>
        /// Converts the generated parameters to a mutable list.
        /// </summary>
        /// <returns>
        /// Generated SQL parameter list.
        /// </returns>
        public List<QuerySqlParameter> ToList()
        {
            return [.. _parameters];
        }
    }
}
