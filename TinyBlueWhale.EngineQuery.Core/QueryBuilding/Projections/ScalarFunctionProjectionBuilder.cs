using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Projections
{

    /// <summary>
    /// Builds scalar SQL function projection definitions.
    /// </summary>
    internal sealed class ScalarFunctionProjectionBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        /// <summary>
        /// Adds a scalar function projection for a single selected property.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the scalar function expression.
        /// </typeparam>
        /// <param name="function">
        /// Scalar SQL function applied to the selected property.
        /// </param>
        /// <param name="selector">
        /// Expression that selects the property used by the scalar function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the scalar function projection.
        /// </param>
        public void Add<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, string alias)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>(selector.Parameters.Single());

            var propertyName = QueryColumnExpressionExtractor
                .ExtractColumns(selector)
                .Single()
                .PropertyName;

            _context.QueryDefinition.ScalarFunctionDefinitions.Add(
                new QueryScalarFunctionDefinition
                {
                    Function = function,
                    PropertyName = propertyName,
                    Alias = alias,
                    Source = sourceDefinition
                });
        }

        /// <summary>
        /// Adds a scalar function projection using multiple arguments.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the scalar function arguments.
        /// </typeparam>
        /// <param name="function">
        /// Scalar SQL function applied to the specified arguments.
        /// </param>
        /// <param name="argumentsSelector">
        /// Expression that selects the arguments supplied to the scalar function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the scalar function projection.
        /// </param>
        public void Add<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object[]>> argumentsSelector, string alias)
        {
            ArgumentNullException.ThrowIfNull(argumentsSelector);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>(argumentsSelector.Parameters.Single());

            _context.QueryDefinition.ScalarFunctionDefinitions.Add(
                new QueryScalarFunctionDefinition
                {
                    Function = function,
                    Arguments = ScalarFunctionArgumentExtractor.Extract(argumentsSelector),
                    Alias = alias,
                    Source = sourceDefinition
                });
        }
    }
}
