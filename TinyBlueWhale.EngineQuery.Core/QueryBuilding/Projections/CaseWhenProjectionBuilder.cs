using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
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
        /// <typeparam name="TEntity">
        /// Entity type associated with the CASE WHEN condition.
        /// </typeparam>
        /// <param name="condition">
        /// Predicate expression evaluated by the CASE WHEN expression.
        /// </param>
        /// <param name="whenTrue">
        /// Value returned when the condition evaluates to true.
        /// </param>
        /// <param name="whenFalse">
        /// Value returned when the condition evaluates to false.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the projected CASE WHEN expression.
        /// </param>
        public void Add<TEntity>(Expression<Func<TEntity, bool>> condition, object? whenTrue, object? whenFalse, string alias)
        {
            ArgumentNullException.ThrowIfNull(condition);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>(condition.Parameters.Single());

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
