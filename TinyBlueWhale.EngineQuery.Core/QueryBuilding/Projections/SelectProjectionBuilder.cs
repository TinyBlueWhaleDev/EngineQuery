using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

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
        /// <remarks>
        /// When the requested entity type matches the root query source, the root source
        /// is selected explicitly. This preserves root projection semantics when multiple
        /// query sources use the same CLR entity type.
        /// </remarks>
        public void AddForSource<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = ResolveProjectionSource<TEntity>();

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
        /// Resolves the query source associated with a source-specific projection.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type represented by the projection.
        /// </typeparam>
        /// <returns>
        /// Query source associated with the projection.
        /// </returns>
        private QuerySourceDefinition ResolveProjectionSource<TEntity>()
        {
            var rootSource = _context.QueryDefinition.RootSource;

            if (rootSource.EntityType == typeof(TEntity))
                return rootSource;

            return _sourceResolver.Resolve<TEntity>();
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
