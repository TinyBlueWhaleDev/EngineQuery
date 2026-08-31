using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.DependencyInjection.Extensions;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;
using TinyBlueWhale.EngineQuery.Generated;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework;
using TinyBlueWhale.EngineQuery.Metadata.Models;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.Playground.EntityFramework;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates EngineQuery dependency injection resolution using strongly typed
    /// database provider profiles and generated query engine surfaces.
    /// </summary>
    public static class DependencyInjectionQueryValidator
    {
        /// <summary>
        /// Runs the dependency injection validation scenarios.
        /// </summary>
        public static void Run()
        {
            ValidateSingleProviderResolution();
            ValidateProfileResolution();
            ValidateMultiProviderResolution();
            ValidateMultipleMetadataRequiresExplicitStrategy();
            ValidateEntityFrameworkMetadataResolution();
            ValidateInsertCommandResolution();
            ValidateUpdateCommandResolution();
            ValidateDeleteCommandResolution();
        }

        /// <summary>
        /// Validates query engine resolution when a single provider and metadata strategy are configured.
        /// </summary>
        private static void ValidateSingleProviderResolution()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseFluentMetadata(
                        ProviderMetadataFactory.CreateJoinMetadataResolver);
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine =
                serviceProvider.GetRequiredService<
                    ISqlServerDefaultQueryEngine>();

            ProviderQueryPrinter.Print(
                "DI SQL Server Single Provider",
                BuildQuery(queryEngine));
        }

        /// <summary>
        /// Validates that a single SQL Server provider registration can resolve
        /// the generated query engine surfaces associated with its provider profiles.
        /// </summary>
        private static void ValidateProfileResolution()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseFluentMetadata(
                        ProviderMetadataFactory.CreateJoinMetadataResolver);
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var defaultEngine =
                serviceProvider.GetRequiredService<
                    ISqlServerDefaultQueryEngine>();

            var sqlServer2008Engine =
                serviceProvider.GetRequiredService<
                    ISqlServer2008QueryEngine>();

            var sqlServer2012Engine =
                serviceProvider.GetRequiredService<
                    ISqlServer2012QueryEngine>();

            ProviderQueryPrinter.Print(
                "DI SQL Server Default Profile",
                BuildQuery(defaultEngine));

            ProviderQueryPrinter.Print(
                "DI SQL Server 2008 Profile",
                BuildQuery(sqlServer2008Engine));

            ProviderQueryPrinter.Print(
                "DI SQL Server 2012 Profile",
                BuildQuery(sqlServer2012Engine));
        }

        /// <summary>
        /// Validates direct strongly typed query engine resolution when multiple
        /// database providers are registered simultaneously.
        /// </summary>
        private static void ValidateMultiProviderResolution()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseFluentMetadata(
                        ProviderMetadataFactory.CreateJoinMetadataResolver);
                });

                options.Add(QueryEngineProvider.PostgreSql, metadata =>
                {
                    metadata.UseFluentMetadata(
                        ProviderMetadataFactory.CreateJoinMetadataResolver);
                });

                options.Add(QueryEngineProvider.MySql, metadata =>
                {
                    metadata.UseFluentMetadata(
                        ProviderMetadataFactory.CreateJoinMetadataResolver);
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var sqlServer =
                serviceProvider.GetRequiredService<
                    ISqlServerDefaultQueryEngine>();

            var postgreSql =
                serviceProvider.GetRequiredService<
                    IPostgreSqlDefaultQueryEngine>();

            var mySql =
                serviceProvider.GetRequiredService<
                    IMySqlDefaultQueryEngine>();

            ProviderQueryPrinter.Print(
                "DI SQL Server Direct Resolution",
                BuildQuery(sqlServer));

            ProviderQueryPrinter.Print(
                "DI PostgreSQL Direct Resolution",
                BuildQuery(postgreSql));

            ProviderQueryPrinter.Print(
                "DI MySQL Direct Resolution",
                BuildQuery(mySql));
        }

        /// <summary>
        /// Validates that an explicit metadata strategy is required when multiple
        /// strategies are configured for the same database provider.
        /// </summary>
        private static void ValidateMultipleMetadataRequiresExplicitStrategy()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseFluentMetadata(
                        ProviderMetadataFactory.CreateJoinMetadataResolver);

                    metadata.UseAttributeMetadata();
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var factory =
                serviceProvider.GetRequiredService<
                    IQueryEngineFactory<
                        SqlServerDefaultProfile,
                        ISqlServerDefaultQueryEngine>>();

            try
            {
                factory.Create();

                throw new InvalidOperationException(
                    "Expected metadata strategy ambiguity exception was not thrown.");
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine(exception.Message);
            }

            ProviderQueryPrinter.Print(
                "DI SQL Server Explicit Fluent Metadata",
                BuildQuery(
                    factory.Create(
                        MetadataStrategy.Fluent)));

            ProviderQueryPrinter.Print(
                "DI SQL Server Explicit Attribute Metadata",
                BuildQuery(
                    factory.Create(
                        MetadataStrategy.Attribute)));
        }

        /// <summary>
        /// Validates Entity Framework metadata resolution through direct strongly typed
        /// SQL Server query engine injection.
        /// </summary>
        private static void ValidateEntityFrameworkMetadataResolution()
        {
            var services = new ServiceCollection();

            services.AddDbContext<EngineQueryValidationDbContext>(options =>
            {
                options.UseInMemoryDatabase(
                    nameof(EngineQueryValidationDbContext));
            });

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseEntityFrameworkMetadata<
                        EngineQueryValidationDbContext>();
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine =
                serviceProvider.GetRequiredService<
                    ISqlServerDefaultQueryEngine>();

            ProviderQueryPrinter.Print(
                "DI SQL Server Entity Framework Metadata",
                BuildQuery(queryEngine));
        }

        /// <summary>
        /// Validates INSERT command generation through direct strongly typed
        /// SQL Server query engine injection.
        /// </summary>
        private static void ValidateInsertCommandResolution()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseFluentMetadata(
                        ProviderMetadataFactory.CreateJoinMetadataResolver);
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine =
                serviceProvider.GetRequiredService<
                    ISqlServerDefaultQueryEngine>();

            ProviderQueryPrinter.Print(
                "DI SQL Server Insert Command",
                BuildInsertCommand(queryEngine));
        }

        /// <summary>
        /// Validates UPDATE command generation through direct strongly typed
        /// SQL Server query engine injection.
        /// </summary>
        private static void ValidateUpdateCommandResolution()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseFluentMetadata(
                        ProviderMetadataFactory.CreateJoinMetadataResolver);
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine =
                serviceProvider.GetRequiredService<
                    ISqlServerDefaultQueryEngine>();

            ProviderQueryPrinter.Print(
                "DI SQL Server Update Command",
                BuildUpdateCommand(queryEngine));
        }

        /// <summary>
        /// Validates DELETE command generation through direct strongly typed
        /// SQL Server query engine injection.
        /// </summary>
        private static void ValidateDeleteCommandResolution()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseFluentMetadata(
                        ProviderMetadataFactory.CreateJoinMetadataResolver);
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine =
                serviceProvider.GetRequiredService<
                    ISqlServerDefaultQueryEngine>();

            ProviderQueryPrinter.Print(
                "DI SQL Server Delete Command",
                BuildDeleteCommand(queryEngine));
        }

        /// <summary>
        /// Builds a validation SELECT query using the specified strongly typed query builder.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile associated with the query builder.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query builder used to generate the validation query.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .InnerJoin<JoinOrder, JoinUser>(
                    alias: "u",
                    on: (order, user) => order.UserId == user.Id)
                .Select<JoinOrder>(order => new
                {
                    OrderId = order.Id,
                    order.UserId,
                    order.Total
                })
                .Select<JoinUser>(user => new
                {
                    UserEmail = user.Email
                })
                .Where<JoinOrder>(order => order.Total > 100)
                .OrderByDescending<JoinOrder>(order => order.Total)
                .Build();
        }

        /// <summary>
        /// Builds a validation INSERT command using the specified strongly typed query builder.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile associated with the query builder.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query builder used to generate the INSERT command.
        /// </param>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        private static GeneratedSqlQuery BuildInsertCommand<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .Build();
        }

        /// <summary>
        /// Builds a validation UPDATE command using the specified strongly typed query builder.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile associated with the query builder.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query builder used to generate the UPDATE command.
        /// </param>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        private static GeneratedSqlQuery BuildUpdateCommand<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .Update<JoinUser>()
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();
        }

        /// <summary>
        /// Builds a validation DELETE command using the specified strongly typed query builder.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile associated with the query builder.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query builder used to generate the DELETE command.
        /// </param>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        private static GeneratedSqlQuery BuildDeleteCommand<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .DeleteFrom<JoinUser>()
                .Where(user => user.Id == 10)
                .Build();
        }
    }
}
