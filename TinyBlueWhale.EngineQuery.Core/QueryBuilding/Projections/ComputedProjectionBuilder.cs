using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Projections
{

    /// <summary>
    /// Builds computed SELECT expression definitions.
    /// </summary>
    internal sealed class ComputedProjectionBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        /// <summary>
        /// Adds a computed projection for an entity available in the current query scope.
        /// </summary>
        public void Add<TEntity>(Expression<Func<TEntity, object>> expression, string alias)
        {
            ArgumentNullException.ThrowIfNull(expression);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

            _context.QueryDefinition.ComputedExpressionDefinitions.Add(
                new QueryComputedExpressionDefinition
                {
                    Expression = expression,
                    Alias = alias,
                    Source = sourceDefinition
                });
        }
    }
}
