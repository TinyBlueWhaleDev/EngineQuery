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
using TinyBlueWhale.EngineQuery.Playground.EntityFramework;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.DependencyInjection
{
    /// <summary>
    /// Validates dependency injection registration and resolution for strongly typed query engines,
    /// provider profiles, metadata strategies and command builders.
    ///
    /// Single SQL Server provider:
    /// SELECT [o].[order_id] AS [OrderId], [o].[user_id], [o].[total], [u].[email] AS [UserEmail]
    /// FROM [orders] AS [o]
    /// INNER JOIN [users] AS [u] ON ([o].[user_id] = [u].[user_id])
    /// WHERE ([o].[total] &gt; @p0)
    /// ORDER BY [o].[total] DESC
    /// @p0 = 100
    ///
    /// SQL Server default profile:
    /// SELECT [o].[order_id] AS [OrderId], [o].[user_id], [o].[total], [u].[email] AS [UserEmail]
    /// FROM [orders] AS [o]
    /// INNER JOIN [users] AS [u] ON ([o].[user_id] = [u].[user_id])
    /// WHERE ([o].[total] &gt; @p0)
    /// ORDER BY [o].[total] DESC
    /// @p0 = 100
    ///
    /// SQL Server 2008 profile:
    /// SELECT [o].[order_id] AS [OrderId], [o].[user_id], [o].[total], [u].[email] AS [UserEmail]
    /// FROM [orders] AS [o]
    /// INNER JOIN [users] AS [u] ON ([o].[user_id] = [u].[user_id])
    /// WHERE ([o].[total] &gt; @p0)
    /// ORDER BY [o].[total] DESC
    /// @p0 = 100
    ///
    /// SQL Server 2012 profile:
    /// SELECT [o].[order_id] AS [OrderId], [o].[user_id], [o].[total], [u].[email] AS [UserEmail]
    /// FROM [orders] AS [o]
    /// INNER JOIN [users] AS [u] ON ([o].[user_id] = [u].[user_id])
    /// WHERE ([o].[total] &gt; @p0)
    /// ORDER BY [o].[total] DESC
    /// @p0 = 100
    ///
    /// SQL Server direct resolution:
    /// SELECT [o].[order_id] AS [OrderId], [o].[user_id], [o].[total], [u].[email] AS [UserEmail]
    /// FROM [orders] AS [o]
    /// INNER JOIN [users] AS [u] ON ([o].[user_id] = [u].[user_id])
    /// WHERE ([o].[total] &gt; @p0)
    /// ORDER BY [o].[total] DESC
    /// @p0 = 100
    ///
    /// PostgreSQL direct resolution:
    /// SELECT "o"."order_id" AS "OrderId", "o"."user_id", "o"."total", "u"."email" AS "UserEmail"
    /// FROM "orders" AS "o"
    /// INNER JOIN "users" AS "u" ON ("o"."user_id" = "u"."user_id")
    /// WHERE ("o"."total" &gt; @p0)
    /// ORDER BY "o"."total" DESC
    /// @p0 = 100
    ///
    /// MySQL direct resolution:
    /// SELECT `o`.`order_id` AS `OrderId`, `o`.`user_id`, `o`.`total`, `u`.`email` AS `UserEmail`
    /// FROM `orders` AS `o`
    /// INNER JOIN `users` AS `u` ON (`o`.`user_id` = `u`.`user_id`)
    /// WHERE (`o`.`total` &gt; @p0)
    /// ORDER BY `o`.`total` DESC
    /// @p0 = 100
    ///
    /// Multiple metadata strategies without an explicit selection:
    /// Multiple registrations support profile 'SqlServerDefaultProfile'. Specify a metadata strategy.
    ///
    /// Explicit Fluent metadata:
    /// SELECT [o].[order_id] AS [OrderId], [o].[user_id], [o].[total], [u].[email] AS [UserEmail]
    /// FROM [orders] AS [o]
    /// INNER JOIN [users] AS [u] ON ([o].[user_id] = [u].[user_id])
    /// WHERE ([o].[total] &gt; @p0)
    /// ORDER BY [o].[total] DESC
    /// @p0 = 100
    ///
    /// Explicit Attribute metadata:
    /// SELECT [o].[Id] AS [OrderId], [o].[UserId], [o].[Total], [u].[Email] AS [UserEmail]
    /// FROM [JoinOrder] AS [o]
    /// INNER JOIN [JoinUser] AS [u] ON ([o].[UserId] = [u].[Id])
    /// WHERE ([o].[Total] &gt; @p0)
    /// ORDER BY [o].[Total] DESC
    /// @p0 = 100
    ///
    /// Entity Framework metadata:
    /// SELECT [o].[Id] AS [OrderId], [o].[UserId], [o].[Total], [u].[Email] AS [UserEmail]
    /// FROM [Orders] AS [o]
    /// INNER JOIN [Users] AS [u] ON ([o].[UserId] = [u].[Id])
    /// WHERE ([o].[Total] &gt; @p0)
    /// ORDER BY [o].[Total] DESC
    /// @p0 = 100
    ///
    /// SQL Server INSERT command:
    /// INSERT INTO [users] ([email])
    /// VALUES (@p0)
    /// @p0 = admin@test.com
    ///
    /// SQL Server UPDATE command:
    /// UPDATE [users]
    /// SET [email] = @p0
    /// WHERE ([user_id] = @p1)
    /// @p0 = updated@test.com
    /// @p1 = 10
    ///
    /// SQL Server DELETE command:
    /// DELETE
    /// FROM [users]
    /// WHERE ([user_id] = @p0)
    /// @p0 = 1
    /// </summary>
    public static class DependencyInjectionQueryValidator
    {
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

        // Validates single-provider resolution.
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

        // Validates generated SQL Server profile resolution.
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

        // Validates direct resolution when multiple providers are registered.
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

        // Validates explicit metadata selection when multiple strategies are registered.
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

        // Validates Entity Framework metadata resolution.
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

        // Validates INSERT command resolution through dependency injection.
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

        // Validates UPDATE command resolution through dependency injection.
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

        // Validates DELETE command resolution through dependency injection.
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

        // Builds the common SELECT validation query.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
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

        // Builds the INSERT validation command.
        private static GeneratedSqlQuery BuildInsertCommand<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .Build();
        }

        // Builds the UPDATE validation command.
        private static GeneratedSqlQuery BuildUpdateCommand<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .Update<JoinUser>()
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();
        }

        // Builds the DELETE validation command.
        private static GeneratedSqlQuery BuildDeleteCommand<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .DeleteFrom<JoinUser>()
                .Where(user => user.Id == 10)
                .Build();
        }
    }
}
