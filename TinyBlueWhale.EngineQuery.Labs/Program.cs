using TinyBlueWhale.EngineQuery.Labs.Configuration;
using TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence;
using TinyBlueWhale.MinimalApi.Extensions;
using TinyBlueWhale.MinimalApi.Versioning.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTinyBlueWhaleMinimalApi(typeof(Program).Assembly)    
    .AddLabPersistence(builder.Configuration);

builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    var registry = app.Services
        .GetRequiredService<ApiVersionRegistry>();

    foreach (var version in registry.Versions)
    {
        var groupName = version.MinorVersion > 0
                ? $"v{version.MajorVersion}.{version.MinorVersion}"
                : $"v{version.MajorVersion}";

        options.SwaggerEndpoint(
            $"/swagger/{groupName}/swagger.json",
            $"TinyBlueWhale.EngineQuery Labs {groupName}");
    }

    options.RoutePrefix = string.Empty;
});

app.MapTinyBlueWhaleMinimalApi();

app.Run();
