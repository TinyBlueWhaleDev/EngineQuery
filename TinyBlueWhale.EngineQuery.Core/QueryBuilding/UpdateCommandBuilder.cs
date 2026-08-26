using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Filtering;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{
    /// <summary>
    /// Builds strongly typed SQL UPDATE command definitions using a fluent API.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target UPDATE table.
    /// </typeparam>
    /// <remarks>
    /// This builder does not execute database commands.
    /// It only captures UPDATE command intent and delegates SQL generation to the query compiler.
    /// </remarks>
    public sealed class UpdateCommandBuilder<T> : IUpdateCommandBuilder<T>
    {
        private readonly IQueryCompiler _queryCompiler;
        private readonly CompiledQueryDefinition _queryDefinition;
        private readonly WhereClauseBuilder _whereClauseBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryCompiler">
        /// Query compiler used to generate provider-specific command output.
        /// </param>
        /// <param name="tableName">
        /// Database table name associated with the UPDATE command.
        /// </param>
        /// <param name="schemaName">
        /// Optional database schema name associated with the target INSERT table.
        /// </param>
        /// <param name="columnMappings">
        /// Optional property-to-column mappings used during SQL generation.
        /// </param>
        /// <param name="metadataResolver">
        /// Optional entity metadata resolver associated with the command.
        /// </param>
        internal UpdateCommandBuilder(IQueryCompiler queryCompiler, IEntityMetadataResolver metadataResolver, string tableName, string? schemaName = null, IReadOnlyDictionary<string, string>? columnMappings = null)
        {
            ArgumentNullException.ThrowIfNull(queryCompiler);
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            _queryCompiler = queryCompiler;

            _queryDefinition = new CompiledQueryDefinition
            {
                CommandType = QueryCommandType.Update,
                SchemaName = schemaName,
                TableName = tableName,
                TableAlias = null,
                ColumnMappings = columnMappings ?? new Dictionary<string, string>(),
                EntityType = typeof(T),
                UpdateDefinition = new QueryUpdateDefinition()
            };

            _queryDefinition.SourceDefinitions[typeof(T)] =
                new QuerySourceDefinition
                {
                    EntityType = typeof(T),
                    SchemaName = schemaName,
                    TableName = tableName,
                    TableAlias = null,
                    ColumnMappings = _queryDefinition.ColumnMappings
                };

            var context = new QueryCommandBuilderContext
            {
                QueryCompiler = _queryCompiler,
                QueryDefinition = _queryDefinition,
                MetadataResolver = metadataResolver,
                AliasRegistry = new QueryAliasRegistry()
            };

            _whereClauseBuilder = new WhereClauseBuilder(context);
        }

        /// <summary>
        /// Adds a value assignment for the selected entity property.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property type associated with the assigned value.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the target entity property.
        /// </param>
        /// <param name="value">
        /// Value assigned to the selected property.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        public IUpdateCommandBuilder<T> Set<TProperty>(Expression<Func<T, TProperty>> selector, TProperty value)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var updateDefinition = _queryDefinition.UpdateDefinition
                ?? throw new InvalidOperationException("The UPDATE command definition is not initialized.");

            var propertyName = ResolvePropertyName(selector);

            var columnName = _queryDefinition.ColumnMappings.TryGetValue(
                propertyName,
                out var mappedColumnName)
                    ? mappedColumnName
                    : propertyName;

            if (updateDefinition.AssignmentDefinitions.Any(definition => definition.ColumnName.Equals(columnName, StringComparison.Ordinal)))
                throw new InvalidOperationException($"Column '{columnName}' already has an UPDATE value assignment.");

            updateDefinition.AssignmentDefinitions.Add(
                new QueryUpdateAssignmentDefinition
                {
                    ColumnName = columnName,
                    Value = value
                });

            return this;
        }

        /// <summary>
        /// Adds a WHERE predicate for the target entity.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        public IUpdateCommandBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            _whereClauseBuilder.Add(predicate);
            return this;
        }

        /// <summary>
        /// Adds a WHERE predicate for the target entity using the specified logical operator.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to connect the predicate with the preceding WHERE predicate.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        public IUpdateCommandBuilder<T> Where(Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            _whereClauseBuilder.Add(predicate, logicalOperator);
            return this;
        }

        /// <summary>
        /// Adds an IN collection condition for the target entity.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property and collection element type.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the IN condition.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        public IUpdateCommandBuilder<T> WhereIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values)
        {
            _whereClauseBuilder.AddCollection(selector, values, isNegated: false);

            return this;
        }

        /// <summary>
        /// Adds a NOT IN collection condition for the target entity.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property and collection element type.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the NOT IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the NOT IN condition.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        public IUpdateCommandBuilder<T> WhereNotIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values)
        {
            _whereClauseBuilder.AddCollection(selector, values, isNegated: true);

            return this;
        }

        /// <summary>
        /// Adds a filtering expression only when the specified condition is true.
        /// </summary>
        /// <param name="condition">
        /// Determines whether the predicate should be added to the UPDATE command.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        public IUpdateCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            _whereClauseBuilder.AddIf(condition, predicate);
            return this;
        }

        /// <summary>
        /// Adds a conditional WHERE predicate using the specified logical operator.
        /// </summary>
        /// <param name="condition">
        /// Determines whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to connect the predicate with the preceding WHERE predicate.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        public IUpdateCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            _whereClauseBuilder.AddIf(
                condition,
                predicate,
                logicalOperator);

            return this;
        }

        /// <summary>
        /// Compiles the current UPDATE definition into SQL command text and parameters.
        /// </summary>
        /// <remarks>
        /// This method only compiles the captured UPDATE definition.
        /// It does not execute the generated command against a database.
        /// </remarks>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no value assignments or WHERE predicates were configured.
        /// </exception>
        public GeneratedSqlQuery Build()
        {
            var updateDefinition = _queryDefinition.UpdateDefinition ?? throw new InvalidOperationException("The UPDATE command definition is not initialized.");

            if (updateDefinition.AssignmentDefinitions.Count == 0)
                throw new InvalidOperationException("At least one value must be configured before building an UPDATE command.");

            if (_queryDefinition.WhereDefinitions.Count == 0 && _queryDefinition.WhereCollectionDefinitions.Count == 0)
            {
                throw new InvalidOperationException(
                    "At least one WHERE predicate must be configured before building an UPDATE command.");
            }

            return _queryCompiler.Compile(_queryDefinition);
        }

        // Resolves the selected entity property name from an UPDATE value assignment expression.
        private static string ResolvePropertyName<TProperty>(Expression<Func<T, TProperty>> selector)
        {
            Expression expression = selector.Body;

            if (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
                expression = unaryExpression.Operand;

            if (expression is not MemberExpression memberExpression || memberExpression.Expression is not ParameterExpression)
                throw new ArgumentException("The UPDATE selector must reference a direct entity property.", nameof(selector));

            return memberExpression.Member.Name;
        }
    }
}
