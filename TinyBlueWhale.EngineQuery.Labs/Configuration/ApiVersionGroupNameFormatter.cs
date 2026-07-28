using Asp.Versioning;

namespace TinyBlueWhale.EngineQuery.Labs.Configuration
{
    internal static class ApiVersionGroupNameFormatter
    {
        public static string Format(ApiVersion version)
        {
            ArgumentNullException.ThrowIfNull(version);

            return version.MinorVersion > 0
                ? $"v{version.MajorVersion}.{version.MinorVersion}"
                : $"v{version.MajorVersion}";
        }
    }
}
