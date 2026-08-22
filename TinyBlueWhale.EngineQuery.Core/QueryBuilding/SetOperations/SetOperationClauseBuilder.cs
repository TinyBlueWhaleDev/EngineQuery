using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.SetOperations
{
    /// <summary>
    /// Builds SQL set operation definitions.
    /// </summary>
    internal sealed class SetOperationClauseBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;

        public void Add<TSet>(QuerySetOperation operation, Func<IQueryBuilder, IQueryCommandBuilder<TSet>> setBuilder)
        {
            ArgumentNullException.ThrowIfNull(setBuilder);

            var nestedQueryBuilder = new QueryBuilder(
                _context.QueryCompiler,
                _context.MetadataResolver);

            var nestedCommandBuilder = setBuilder(nestedQueryBuilder);

            if (nestedCommandBuilder is not QueryCommandBuilder<TSet> concreteNestedCommandBuilder)
                throw new InvalidOperationException("The set operation builder returned an unsupported query command builder instance.");


            _context.QueryDefinition.SetOperationDefinitions.Add(
                new QuerySetOperationDefinition
                {
                    Operation = operation,
                    Query = concreteNestedCommandBuilder.BuildDefinition()
                });
        }
    }
}
