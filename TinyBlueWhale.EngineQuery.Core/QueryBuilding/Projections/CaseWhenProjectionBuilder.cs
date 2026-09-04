using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Projections
{

    /// <summary>
    /// Builds CASE WHEN SELECT expression definitions.
    /// </summary>
    internal sealed class CaseWhenProjectionBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        /// <summary>
        /// Adds a CASE WHEN projection for an entity available in the current query scope.
        /// </summary>
        public void Add<TEntity>(Expression<Func<TEntity, bool>> condition, object? whenTrue, object? whenFalse, string alias)
        {
            ArgumentNullException.ThrowIfNull(condition);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

            _context.QueryDefinition.CaseWhenDefinitions.Add(
                new QueryCaseWhenDefinition
                {
                    ConditionExpression = condition,
                    WhenTrueValue = whenTrue,
                    WhenFalseValue = whenFalse,
                    Alias = alias,
                    Source = sourceDefinition
                });
        }
    }
}
