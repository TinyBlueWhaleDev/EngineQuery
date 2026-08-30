using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Subqueries
{

    /// <summary>
    /// Builds EXISTS and NOT EXISTS query definitions.
    /// </summary>
    internal sealed class ExistsClauseBuilder<TProfile>(QueryCommandBuilderContext context,
        TProfile profile)
        where TProfile : IDatabaseProviderProfile
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);
        private readonly NestedQueryCommandBuilderFactory<TProfile> _nestedFactory = new(context, profile);
        private readonly QuerySourceAliasResolver _aliasResolver = new(context);

        public void Add<TSubquery>(Func<IQueryBuilder<TProfile>,
            IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var nestedQueryBuilder = _nestedFactory.CreateQueryBuilder();
            var nestedCommandBuilder = subqueryBuilder(nestedQueryBuilder);

            var subqueryDefinition =
                NestedQueryCommandBuilderFactory<TProfile>.ExtractDefinition(
                    nestedCommandBuilder,
                    "The EXISTS subquery builder returned an unsupported query command builder instance.");

            subqueryDefinition.UseConstantSelectProjection = true;

            _context.QueryDefinition.ExistsDefinitions.Add(
                new QueryExistsDefinition
                {
                    Subquery = subqueryDefinition
                });
        }

        public void AddCorrelated<TOuter, TSubquery>(string? alias,
            Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder,
            bool isNegated)
        {
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var outerSource = _aliasResolver.EnsureAlias<TOuter>(_sourceResolver.Resolve<TOuter>());
            var nestedCommandBuilder = _nestedFactory.CreateMetadataBuilder<TSubquery>(alias);

            nestedCommandBuilder.RegisterOuterSources(
                new Dictionary<Type, QuerySourceDefinition>
                {
                    [typeof(TOuter)] = outerSource
                });

            var configuredBuilder = subqueryBuilder(nestedCommandBuilder);

            var subqueryDefinition =
                NestedQueryCommandBuilderFactory<TProfile>.ExtractDefinition(
                    configuredBuilder,
                    isNegated
                        ? "The NOT EXISTS subquery builder returned an unsupported query command builder instance."
                        : "The EXISTS subquery builder returned an unsupported query command builder instance.");

            subqueryDefinition.UseConstantSelectProjection = true;

            _context.QueryDefinition.ExistsDefinitions.Add(
                new QueryExistsDefinition
                {
                    Subquery = subqueryDefinition,
                    IsNegated = isNegated
                });
        }
    }
}
