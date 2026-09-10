using FluentValidation;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.DependencyInjection.Extensions;
using TinyBlueWhale.EngineQuery.Labs.Domain.Enums;
using TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence.FluentMappings;
using TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence.SqlServer;
using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Repositories.Interfaces;
using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Repositories.SearchOrdersRaw;
using TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.Validators;

namespace TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddLabPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SqlServerOptions>()
            .Bind(configuration.GetSection(SqlServerOptions.SectionName))
            .Validate(x => !string.IsNullOrWhiteSpace(x.ConnectionString), "SqlServer:ConnectionString is required.")
            .ValidateOnStart();

        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddEngineQuery(options => options.Add(
            QueryEngineProvider.SqlServer,
            metadata => metadata.UseFluentMetadata(EngineQueryMappings.CreateResolver)));
        services.AddValidatorsFromAssemblyContaining<SearchOrdersRequestValidator>();

        services.AddKeyedScoped<ISearchOrdersRepository, SearchOrdersRawRepository>(QueryImplementation.Raw);
        //services.AddKeyedScoped<ISearchOrdersRepository, SearchOrdersEngineRepository>(QueryImplementation.EngineQuery);

        return services;
    }
}
