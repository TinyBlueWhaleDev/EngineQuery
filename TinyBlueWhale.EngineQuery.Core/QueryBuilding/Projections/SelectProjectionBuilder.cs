using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Projections
{

    /// <summary>
    /// Builds SELECT projection definitions.
    /// </summary>
    internal sealed class SelectProjectionBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        /// <summary>
        /// Adds selected properties for the root query entity.
        /// </summary>
        public void Add<T>(Expression<Func<T, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var selectedProperties = SelectedPropertyExpressionExtractor
                .ExtractSelectedProperties(selector);

            _context.QueryDefinition.SelectDefinitions.AddRange(selectedProperties);
        }

        /// <summary>
        /// Adds selected properties for an entity available in the current query scope.
        /// </summary>
        public void AddForSource<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

            var selectedColumns = SelectedPropertyExpressionExtractor
                .ExtractSelectedProperties(selector);

            foreach (var selectedColumn in selectedColumns)
            {
                _context.QueryDefinition.SelectDefinitions.Add(
                    selectedColumn with
                    {
                        Source = sourceDefinition
                    });
            }
        }

        /// <summary>
        /// Applies DISTINCT projection semantics.
        /// </summary>
        public void ApplyDistinct()
        {
            _context.QueryDefinition.IsDistinct = true;
        }
    }
}
