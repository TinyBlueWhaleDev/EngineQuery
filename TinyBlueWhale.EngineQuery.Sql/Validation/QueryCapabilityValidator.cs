using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Sql.Validation
{
    /// <summary>
    /// Validates whether a compiled query definition can be executed by the configured database provider.
    /// </summary>
    /// <remarks>
    /// This validator checks provider capabilities for SQL features such as common table expressions,
    /// recursive common table expressions, window functions, lateral joins, pagination and set operations.
    /// Nested query definitions are validated recursively.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="QueryCapabilityValidator"/> class.
    /// </remarks>
    /// <param name="providerCapabilities">
    /// Provider capability definition used to validate query features.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="providerCapabilities"/> is <see langword="null"/>.
    /// </exception>
    public sealed class QueryCapabilityValidator(IDatabaseProviderCapabilities providerCapabilities)
    {
        private readonly IDatabaseProviderCapabilities _providerCapabilities = providerCapabilities ?? throw new ArgumentNullException(nameof(providerCapabilities));

        /// <summary>
        /// Validates whether the specified query definition uses only supported provider features.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to validate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when the query uses a feature unsupported by the current provider.
        /// </exception>
        public void Validate(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            ValidateCurrentQuery(queryDefinition);
            ValidateNestedQueries(queryDefinition);
        }

        /// <summary>
        /// Validates whether pagination is supported by the configured provider.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown when the provider does not support any configured pagination syntax.
        /// </exception>
        public void ValidatePaginationSupport()
        {
            if (!_providerCapabilities.SupportsOffsetFetchPagination && !_providerCapabilities.SupportsLimitOffsetPagination)
                throw new NotSupportedException("Pagination is not supported by the current provider.");
        }

        private void ValidateCurrentQuery(CompiledQueryDefinition queryDefinition)
        {
            if (queryDefinition.CteDefinitions.Count > 0 && !_providerCapabilities.SupportsCommonTableExpressions)
                throw new NotSupportedException("Common table expressions are not supported by the current provider.");

            if (queryDefinition.CteDefinitions.Any(cteDefinition => cteDefinition.IsRecursive) && !_providerCapabilities.SupportsRecursiveCommonTableExpressions)
                throw new NotSupportedException("Recursive common table expressions are not supported by the current provider.");

            if (queryDefinition.WindowFunctionDefinitions.Count > 0 && !_providerCapabilities.SupportsWindowFunctions)
                throw new NotSupportedException("Window functions are not supported by the current provider.");

            if (queryDefinition.ApplyDefinitions.Count > 0 && !_providerCapabilities.SupportsLateralJoins)
                throw new NotSupportedException("APPLY or LATERAL joins are not supported by the current provider.");

            if (queryDefinition.SetOperationDefinitions.Any(setOperation => setOperation.Operation == QuerySetOperation.Intersect) && !_providerCapabilities.SupportsIntersect)
                throw new NotSupportedException("INTERSECT set operations are not supported by the current provider.");

            if (queryDefinition.SetOperationDefinitions.Any(setOperation => setOperation.Operation == QuerySetOperation.Except) && !_providerCapabilities.SupportsExcept)
                throw new NotSupportedException("EXCEPT set operations are not supported by the current provider.");

            if (queryDefinition.Pagination.HasPagination)
                ValidatePaginationSupport();
        }

        private void ValidateNestedQueries(CompiledQueryDefinition queryDefinition)
        {
            foreach (var cteDefinition in queryDefinition.CteDefinitions)
                Validate(cteDefinition.Query);

            foreach (var setOperationDefinition in queryDefinition.SetOperationDefinitions)
                Validate(setOperationDefinition.Query);

            foreach (var applyDefinition in queryDefinition.ApplyDefinitions)
                Validate(applyDefinition.Subquery);

            foreach (var existsDefinition in queryDefinition.ExistsDefinitions)
                Validate(existsDefinition.Subquery);

            foreach (var inSubqueryDefinition in queryDefinition.InSubqueryDefinitions)
                Validate(inSubqueryDefinition.Subquery);

            foreach (var sourceDefinition in queryDefinition.SourceDefinitions.Values.Where(source => source.IsDerivedTable))
                Validate(sourceDefinition.Subquery!);
        }
    }
}
