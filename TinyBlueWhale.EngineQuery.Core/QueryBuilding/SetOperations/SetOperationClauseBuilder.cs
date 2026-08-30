using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.SetOperations
{
    /// <summary>
    /// Builds SQL set operation definitions.
    /// </summary>
    internal sealed class SetOperationClauseBuilder<TProfile>(QueryCommandBuilderContext context,
        TProfile profile)
        where TProfile : IDatabaseProviderProfile
    {
        private readonly QueryCommandBuilderContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly TProfile _profile = profile ?? throw new ArgumentNullException(nameof(profile));
        public void Add<TSet>(QuerySetOperation operation,
            Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            ArgumentNullException.ThrowIfNull(setBuilder);

            var nestedQueryBuilder = new QueryBuilder<TProfile>(
                _context.QueryCompiler,
                _context.MetadataResolver,
                _profile);

            var nestedCommandBuilder = setBuilder(nestedQueryBuilder);

            if (nestedCommandBuilder is not QueryCommandBuilder<TSet, TProfile> concreteNestedCommandBuilder)
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
