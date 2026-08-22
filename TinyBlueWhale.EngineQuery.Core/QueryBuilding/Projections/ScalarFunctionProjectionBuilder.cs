using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

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
        public void Add<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, string alias)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

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
        public void Add<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object[]>> argumentsSelector, string alias)
        {
            ArgumentNullException.ThrowIfNull(argumentsSelector);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

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
