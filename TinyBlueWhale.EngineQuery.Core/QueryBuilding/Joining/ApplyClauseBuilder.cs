using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Subqueries;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;


namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Joining
{
    /// <summary>
    /// Builds APPLY query definitions.
    /// </summary>
    internal sealed class ApplyClauseBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);
        private readonly NestedQueryCommandBuilderFactory _nestedFactory = new(context);

        public void Add<TOuter, TApply>(
            QueryApplyType applyType,
            string alias,
            Func<IQueryCommandBuilder<TApply>, IQueryCommandBuilder<TApply>> applyBuilder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            ArgumentNullException.ThrowIfNull(applyBuilder);

            var outerSource = _sourceResolver.Resolve<TOuter>();

            _context.AliasRegistry.Register(alias);

            var nestedCommandBuilder = _nestedFactory.CreateMetadataBuilder<TApply>(alias);

            nestedCommandBuilder.RegisterOuterSources(
                new Dictionary<Type, QuerySourceDefinition>
                {
                    [typeof(TOuter)] = outerSource
                });

            var configuredBuilder = applyBuilder(nestedCommandBuilder);

            var subqueryDefinition =
                NestedQueryCommandBuilderFactory.ExtractDefinition(
                    configuredBuilder,
                    "The APPLY subquery builder returned an unsupported query command builder instance.");

            subqueryDefinition.ForceSelectAliases = true;

            _context.QueryDefinition.ApplyDefinitions.Add(
                new QueryApplyDefinition
                {
                    ApplyType = applyType,
                    Alias = alias,
                    Subquery = subqueryDefinition
                });
        }
    }
}
