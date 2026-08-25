using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{
    /// <summary>
    /// Builds strongly typed SQL INSERT command definitions using a fluent API.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target INSERT table.
    /// </typeparam>
    /// <remarks>
    /// This builder does not execute database commands.
    /// It only captures INSERT command intent and delegates SQL generation to the query compiler.
    /// </remarks>
    public sealed class InsertCommandBuilder<T> : QueryCompositionCommandBuilderBase<T, IInsertSelectCommandBuilder<T>>,
        IInsertCommandBuilder<T>,
        IInsertValuesCommandBuilder<T>,
        IInsertSelectCommandBuilder<T>
    {
        private readonly IQueryCompiler _queryCompiler;
        private readonly CompiledQueryDefinition _queryDefinition;
        private readonly IEntityMetadataResolver _metadataResolver;
        private readonly QueryCommandBuilderContext _context;
        private readonly QueryCommandBuilderComponents _components;

        private protected override QueryCommandBuilderComponents Components => _components;
        protected override IInsertSelectCommandBuilder<T> Current => this;



        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryCompiler">
        /// Query compiler used to generate provider-specific command output.
        /// </param>
        /// <param name="tableName">
        /// Database table name associated with the INSERT command.
        /// </param>
        /// <param name="schemaName">
        /// Optional database schema name associated with the target INSERT table.
        /// </param>
        /// <param name="columnMappings">
        /// Optional property-to-column mappings used during SQL generation.
        /// </param>
        /// <param name="metadataResolver">
        /// Optional entity metadata resolver used for metadata-driven query composition.
        /// </param>
        internal InsertCommandBuilder(IQueryCompiler queryCompiler, IEntityMetadataResolver metadataResolver, string tableName, string? schemaName = null, IReadOnlyDictionary<string, string>? columnMappings = null)
        {
            ArgumentNullException.ThrowIfNull(queryCompiler);
            ArgumentNullException.ThrowIfNull(metadataResolver);
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            _queryCompiler = queryCompiler;
            _metadataResolver = metadataResolver;

            _queryDefinition = new CompiledQueryDefinition
            {
                CommandType = QueryCommandType.Insert,
                SchemaName = schemaName,
                TableName = tableName,
                ColumnMappings = columnMappings ?? new Dictionary<string, string>(),
                EntityType = typeof(T),
                InsertDefinition = new QueryInsertDefinition()
            };

            _context = new QueryCommandBuilderContext
            {
                QueryCompiler = _queryCompiler,
                QueryDefinition = _queryDefinition,
                MetadataResolver = _metadataResolver,
                AliasRegistry = new QueryAliasRegistry()
            };

            _components = QueryCommandBuilderComponentFactory.Create(_context);
        }

        /// <summary>
        /// Defines the target columns associated with the INSERT command.
        /// </summary>
        /// <param name="selector">
        /// Expression used to determine which target entity properties should be included in the generated SQL INSERT clause.
        /// </param>
        /// <returns>
        /// Current INSERT command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="selector"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the selector does not reference one or more direct entity properties.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when INSERT value assignments were already configured or when a selected target column was already added.
        /// </exception>
        public IInsertCommandBuilder<T> Columns(Expression<Func<T, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (_queryDefinition.InsertDefinition!.ValueDefinitions.Count > 0)
                throw new InvalidOperationException("INSERT SELECT columns cannot be combined with INSERT value assignments.");

            foreach (var propertyName in ResolvePropertyNames(selector))
            {
                var columnName = ResolveColumnName(propertyName);

                if (_queryDefinition.InsertDefinition.ColumnDefinitions.Any(definition => definition.ColumnName.Equals(columnName, StringComparison.Ordinal)))
                    throw new InvalidOperationException($"Property '{propertyName}' is already configured as an INSERT target column.");

                _queryDefinition.InsertDefinition.ColumnDefinitions.Add(
                    new QueryInsertColumnDefinition
                    {
                        ColumnName = columnName
                    });
            }

            return this;
        }


        /// <summary>
        /// Adds a value assignment for the selected entity property.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property type associated with the inserted value.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the target entity property.
        /// </param>
        /// <param name="value">
        /// Value assigned to the selected property.
        /// </param>
        /// <returns>
        /// Current INSERT command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="selector"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the selector does not reference a direct entity property.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the selected property was already assigned.
        /// </exception>
        public IInsertValuesCommandBuilder<T> Set<TProperty>(Expression<Func<T, TProperty>> selector, TProperty value)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (_queryDefinition.InsertDefinition!.SourceDefinition is not null)
                throw new InvalidOperationException("INSERT value assignments cannot be combined with an INSERT SELECT source.");

            if (_queryDefinition.InsertDefinition.ColumnDefinitions.Count > 0)
                throw new InvalidOperationException("INSERT value assignments cannot be combined with explicitly configured INSERT SELECT columns.");

            var propertyName = ResolvePropertyName(selector);
            var columnName = ResolveColumnName(propertyName);

            if (_queryDefinition.InsertDefinition.ValueDefinitions.Any(definition => definition.ColumnName.Equals(columnName, StringComparison.Ordinal)))
                throw new InvalidOperationException($"Property '{propertyName}' already has an INSERT value assignment.");

            _queryDefinition.InsertDefinition.ValueDefinitions.Add(
                new QueryInsertValueDefinition
                {
                    ColumnName = columnName,
                    Value = value
                });

            return this;
        }

        /// <summary>
        /// Configures provider-specific retrieval of the identity generated by the INSERT command.
        /// </summary>
        /// <returns>
        /// Current INSERT VALUES command builder instance.
        /// </returns>
        IInsertValuesCommandBuilder<T> IInsertValuesCommandBuilder<T>.ReturnIdentity()
        {
            ConfigureIdentityRetrieval(columnName: null);

            return this;
        }

        /// <summary>
        /// Configures retrieval of the generated identity using the selected target column.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property type associated with the generated identity.
        /// </typeparam>
        /// <param name="identitySelector">
        /// Expression that selects the target identity property.
        /// </param>
        /// <returns>
        /// Current INSERT VALUES command builder instance.
        /// </returns>
        IInsertValuesCommandBuilder<T> IInsertValuesCommandBuilder<T>.ReturnIdentity<TProperty>(Expression<Func<T, TProperty>> identitySelector)
        {
            ArgumentNullException.ThrowIfNull(identitySelector);

            var propertyName = ResolvePropertyName(identitySelector);
            var columnName = ResolveColumnName(propertyName);

            ConfigureIdentityRetrieval(columnName);

            return this;
        }

        /// <summary>
        /// Configures an INSERT SELECT source using an explicit table name.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type used as the source of the INSERT SELECT command.
        /// </typeparam>
        /// <param name="tableName">
        /// Database table name associated with the INSERT SELECT source.
        /// </param>
        /// <param name="alias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <returns>
        /// Current INSERT command builder instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="tableName"/> or <paramref name="alias"/> contains an invalid value.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the INSERT command already contains value assignments or when the source entity is already registered.
        /// </exception>
        public IInsertSelectCommandBuilder<T> From<TSource>(string tableName, string? alias = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            if (alias is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            EnsureInsertSelectMode();

            var metadata = EntityMetadataHelper.Resolve<TSource>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            RegisterSource<TSource>(tableName, metadata.SchemaName, alias, columnMappings);

            return this;
        }

        /// <summary>
        /// Configures an INSERT SELECT source using resolved entity metadata.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type used as the source of the INSERT SELECT command.
        /// </typeparam>
        /// <param name="alias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <returns>
        /// Current INSERT command builder instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="alias"/> contains an invalid value.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when metadata cannot be resolved, when INSERT value assignments already exist or when the source entity is already registered.
        /// </exception>
        public IInsertSelectCommandBuilder<T> From<TSource>(string? alias = null)
        {
            if (alias is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            EnsureInsertSelectMode();

            var metadata = EntityMetadataHelper.Resolve<TSource>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            RegisterSource<TSource>(metadata.TableName, metadata.SchemaName, alias, columnMappings);

            return this;
        }

        /// <summary>
        /// Compiles the current INSERT definition into SQL command text and parameters.
        /// </summary>
        /// <remarks>
        /// This method only compiles the captured INSERT definition.
        /// It does not execute the generated command against a database.
        /// </remarks>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no INSERT value assignments or INSERT SELECT source were configured.
        /// </exception>
        public GeneratedSqlQuery Build()
        {
            if (_queryDefinition.InsertDefinition!.ValueDefinitions.Count == 0 && _queryDefinition.InsertDefinition.SourceDefinition is null)
                throw new InvalidOperationException("At least one value or SELECT source must be configured before building an INSERT command.");

            ResolveInsertSelectColumns();

            return _queryCompiler.Compile(_queryDefinition);
        }

        // Resolves INSERT target columns from the SELECT projection when explicit target columns were not configured.
        private void ResolveInsertSelectColumns()
        {
            var insertDefinition = _queryDefinition.InsertDefinition!;

            if (insertDefinition.SourceDefinition is null)
                return;

            if (insertDefinition.ColumnDefinitions.Count > 0)
                return;

            var targetColumnNames = ResolveInsertSelectProjectionNames();

            if (targetColumnNames.Count == 0)
                throw new InvalidOperationException("At least one SELECT projection must be configured when INSERT target columns are not explicitly configured.");

            foreach (var targetColumnName in targetColumnNames)
                AddInferredInsertColumn(insertDefinition, targetColumnName);
        }

        // Resolves inferred INSERT target column names using the same projection order generated by the SELECT pipeline.
        private List<string> ResolveInsertSelectProjectionNames()
        {
            var targetColumnNames = new List<string>(
                _queryDefinition.SelectDefinitions.Count +
                _queryDefinition.AggregateDefinitions.Count +
                _queryDefinition.ScalarFunctionDefinitions.Count +
                _queryDefinition.ComputedExpressionDefinitions.Count +
                _queryDefinition.CaseWhenDefinitions.Count +
                _queryDefinition.WindowFunctionDefinitions.Count);

            targetColumnNames.AddRange(
                _queryDefinition.SelectDefinitions.Select(selectDefinition =>
                    string.IsNullOrWhiteSpace(selectDefinition.Alias)
                        ? selectDefinition.PropertyName
                        : selectDefinition.Alias));

            targetColumnNames.AddRange(
                _queryDefinition.AggregateDefinitions.Select(aggregateDefinition => aggregateDefinition.Alias));

            targetColumnNames.AddRange(
                _queryDefinition.ScalarFunctionDefinitions.Select(functionDefinition => functionDefinition.Alias));

            targetColumnNames.AddRange(
                _queryDefinition.ComputedExpressionDefinitions.Select(computedDefinition => computedDefinition.Alias));

            targetColumnNames.AddRange(
                _queryDefinition.CaseWhenDefinitions.Select(caseWhenDefinition => caseWhenDefinition.Alias));

            targetColumnNames.AddRange(
                _queryDefinition.WindowFunctionDefinitions.Select(windowFunctionDefinition => windowFunctionDefinition.Alias));

            return targetColumnNames;
        }

        // Adds an inferred INSERT target column while preventing duplicate projection names.
        private static void AddInferredInsertColumn(QueryInsertDefinition insertDefinition, string targetColumnName)
        {
            if (insertDefinition.ColumnDefinitions.Any(definition => definition.ColumnName.Equals(targetColumnName, StringComparison.Ordinal)))
                throw new InvalidOperationException($"Target INSERT column '{targetColumnName}' was resolved more than once from the SELECT projection.");

            insertDefinition.ColumnDefinitions.Add(
                new QueryInsertColumnDefinition
                {
                    ColumnName = targetColumnName
                });
        }

        // Ensures the current INSERT command can transition to INSERT SELECT composition.
        private void EnsureInsertSelectMode()
        {
            if (_queryDefinition.InsertDefinition!.ValueDefinitions.Count > 0)
                throw new InvalidOperationException("An INSERT SELECT source cannot be combined with INSERT value assignments.");
        }



        // Registers the root query source associated with the current INSERT SELECT command.
        private void RegisterSource<TSource>(string tableName, string? schemaName, string? alias, IReadOnlyDictionary<string, string> columnMappings)
        {
            if (_queryDefinition.InsertDefinition!.SourceDefinition is not null)
                throw new InvalidOperationException("The INSERT SELECT source is already configured.");

            if (_queryDefinition.SourceDefinitions.ContainsKey(typeof(TSource)))
                throw new InvalidOperationException($"Entity type '{typeof(TSource).Name}' is already registered in the current INSERT SELECT query scope.");

            var resolvedAlias = string.IsNullOrWhiteSpace(alias)
                ? QueryAliasGeneratorHelper.Generate(_queryDefinition.SourceDefinitions.Count)
                : alias;

            var sourceDefinition = new QuerySourceDefinition
            {
                EntityType = typeof(TSource),
                SchemaName = schemaName,
                TableName = tableName,
                TableAlias = resolvedAlias,
                ColumnMappings = columnMappings
            };

            _queryDefinition.InsertDefinition.SourceDefinition = sourceDefinition;
            _queryDefinition.SourceDefinitions[typeof(TSource)] = sourceDefinition;
            _queryDefinition.EntityType = typeof(TSource);

            _context.AliasRegistry.Register(resolvedAlias);
        }

        // Resolves the mapped database column associated with an INSERT target property.
        private string ResolveColumnName(string propertyName)
        {
            return _queryDefinition.ColumnMappings.TryGetValue(propertyName, out var mappedColumnName)
                ? mappedColumnName
                : propertyName;
        }

        // Resolves the selected entity property name from an INSERT value assignment expression.
        private static string ResolvePropertyName<TProperty>(Expression<Func<T, TProperty>> selector)
        {
            Expression expression = selector.Body;

            if (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
                expression = unaryExpression.Operand;

            if (expression is not MemberExpression memberExpression || memberExpression.Expression is not ParameterExpression)
                throw new ArgumentException("The INSERT selector must reference a direct entity property.", nameof(selector));

            return memberExpression.Member.Name;
        }

        // Resolves the selected entity property names from an INSERT target column expression.
        private static List<string> ResolvePropertyNames(Expression<Func<T, object>> selector)
        {
            Expression expression = selector.Body;

            if (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
                expression = unaryExpression.Operand;

            if (expression is MemberExpression memberExpression)
                return [ResolvePropertyName(memberExpression, nameof(selector))];

            if (expression is NewExpression newExpression)
                return newExpression.Arguments.Select(argument => ResolvePropertyName(argument, nameof(selector))).ToList();

            throw new ArgumentException("The INSERT columns selector must reference one or more direct entity properties.", nameof(selector));
        }

        // Resolves a direct entity property name from an INSERT target column expression.
        private static string ResolvePropertyName(Expression expression, string parameterName)
        {
            if (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
                expression = unaryExpression.Operand;

            if (expression is not MemberExpression memberExpression || memberExpression.Expression is not ParameterExpression)
                throw new ArgumentException("The INSERT columns selector must reference direct entity properties.", parameterName);

            return memberExpression.Member.Name;
        }

        // Configures identity retrieval for a direct INSERT VALUES command.
        private void ConfigureIdentityRetrieval(string? columnName)
        {
            var insertDefinition = _queryDefinition.InsertDefinition!;

            if (insertDefinition.ValueDefinitions.Count == 0)
                throw new InvalidOperationException("Identity retrieval requires at least one INSERT value assignment.");

            if (insertDefinition.SourceDefinition is not null)
                throw new InvalidOperationException("Identity retrieval cannot be combined with an INSERT SELECT source.");

            if (insertDefinition.IdentityDefinition is not null)
                throw new InvalidOperationException("Identity retrieval is already configured for the current INSERT command.");

            insertDefinition.IdentityDefinition = new QueryInsertIdentityDefinition
            {
                ColumnName = columnName
            };
        }
    }
}
