using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using TinyBlueWhale.MinimalApi.Versioning.Abstractions;

namespace TinyBlueWhale.EngineQuery.Labs.Configuration
{
    public sealed class ConfigureSwaggerOptions(
    ApiVersionRegistry registry)
    : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var version in registry.Versions)
            {
                var groupName = ApiVersionGroupNameFormatter.Format(version);

                options.SwaggerDoc(
                    groupName,
                    new OpenApiInfo
                    {
                        Title = "TinyBlueWhale.EngineQuery Labs",
                        Version = groupName
                    });
            }

            options.DocInclusionPredicate((documentName, apiDescription) =>
            {
                var apiVersion = apiDescription.GetApiVersion();

                if (apiVersion is null)
                    return documentName == ApiVersionGroupNameFormatter.Format(registry.DefaultVersion);

                var groupName = ApiVersionGroupNameFormatter.Format(apiVersion);

                return string.Equals(
                    documentName,
                    groupName,
                    StringComparison.OrdinalIgnoreCase);
            });
        }
    }
}
