using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
using TinyBlueWhale.EngineQuery.DependencyInjection.Extensions;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.Playground.Models;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{        

    public static class DependencyInjectionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            ValidateSingleProviderDirectInjection();
            ValidateMultiProviderFactoryResolution();
            ValidateMultipleMetadataRequiresExplicitStrategy();
        }

        // Validates direct IQueryEngine injection when only one provider and metadata strategy are registered.
        private static void ValidateSingleProviderDirectInjection()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseFluentMetadata(ProviderMetadataFactory.CreateJoinMetadataResolver);
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

            ProviderQueryPrinter.Print(
                "DI SQL Server Single Provider",
                BuildQuery(queryEngine));
        }

        // Validates factory resolution for multiple providers.
        private static void ValidateMultiProviderFactoryResolution()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseFluentMetadata(ProviderMetadataFactory.CreateJoinMetadataResolver);
                });

                options.Add(QueryEngineProvider.PostgreSql, metadata =>
                {
                    metadata.UseFluentMetadata(ProviderMetadataFactory.CreateJoinMetadataResolver);
                });

                options.Add(QueryEngineProvider.MySql, metadata =>
                {
                    metadata.UseFluentMetadata(ProviderMetadataFactory.CreateJoinMetadataResolver);
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IQueryEngineFactory>();

            ProviderQueryPrinter.Print(
                "DI SQL Server Factory",
                BuildQuery(factory.Create(QueryEngineProvider.SqlServer)));

            ProviderQueryPrinter.Print(
                "DI PostgreSQL Factory",
                BuildQuery(factory.Create(QueryEngineProvider.PostgreSql)));

            ProviderQueryPrinter.Print(
                "DI MySQL Factory",
                BuildQuery(factory.Create(QueryEngineProvider.MySql)));
        }

        // Validates explicit metadata strategy selection when multiple strategies are registered.
        private static void ValidateMultipleMetadataRequiresExplicitStrategy()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseFluentMetadata(ProviderMetadataFactory.CreateJoinMetadataResolver);
                    metadata.UseAttributeMetadata();
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IQueryEngineFactory>();

            try
            {
                factory.Create(QueryEngineProvider.SqlServer);

                throw new InvalidOperationException(
                    "Expected metadata strategy ambiguity exception was not thrown.");
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine(exception.Message);
            }

            ProviderQueryPrinter.Print(
                "DI SQL Server Explicit Fluent Metadata",
                BuildQuery(factory.Create(
                    QueryEngineProvider.SqlServer,
                    MetadataStrategy.Fluent)));

            ProviderQueryPrinter.Print(
               "DI SQL Server Explicit Attribute Metadata",
               BuildQuery(factory.Create(
                   QueryEngineProvider.SqlServer,
                   MetadataStrategy.Attribute)));
        }

        // Builds a validation query.
        private static GeneratedSqlQuery BuildQuery(IQueryBuilder queryBuilder)
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
    }
}
