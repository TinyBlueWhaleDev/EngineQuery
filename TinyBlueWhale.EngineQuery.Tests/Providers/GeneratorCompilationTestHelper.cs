using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Tests.Providers
{
    /// <summary>
    /// Provides Roslyn compilation support for generated provider surface tests.
    /// </summary>
    internal static class GeneratorCompilationTestHelper
    {
        /// <summary>
        /// Compiles the specified consumer source against the generated EngineQuery
        /// provider surfaces and returns compilation errors.
        /// </summary>
        public static IReadOnlyList<Diagnostic> Compile(string source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                source,
                new CSharpParseOptions(LanguageVersion.Latest));

            var compilation = CSharpCompilation.Create(
                assemblyName: $"EngineQueryGeneratorContractTest_{Guid.NewGuid():N}",
                syntaxTrees: [syntaxTree],
                references: CreateMetadataReferences(),
                options: new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    nullableContextOptions: NullableContextOptions.Enable));

            return compilation
                .GetDiagnostics()
                .Where(diagnostic =>
                    diagnostic.Severity == DiagnosticSeverity.Error)
                .ToArray();
        }

        private static IEnumerable<MetadataReference> CreateMetadataReferences()
        {
            var referencePaths = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            AddTrustedPlatformAssemblies(referencePaths);

            AddAssembly(
                referencePaths,
                typeof(IDatabaseProviderProfile).Assembly);

            AddAssembly(
                referencePaths,
                typeof(QueryBuilder<>).Assembly);

            AddAssembly(
                referencePaths,
                typeof(SqlServer2008Profile).Assembly);

            AddAssembly(
                referencePaths,
                typeof(PostgreSql84Profile).Assembly);

            AddAssembly(
                referencePaths,
                typeof(MySql57Profile).Assembly);

            return referencePaths
                .Select(x => MetadataReference.CreateFromFile(x))
                .ToArray();
        }

        private static void AddTrustedPlatformAssemblies(
            ISet<string> referencePaths)
        {
            var trustedPlatformAssemblies =
                AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;

            if (string.IsNullOrWhiteSpace(trustedPlatformAssemblies))
                return;

            foreach (var assemblyPath in trustedPlatformAssemblies.Split(
                Path.PathSeparator,
                StringSplitOptions.RemoveEmptyEntries))
            {
                referencePaths.Add(assemblyPath);
            }
        }

        private static void AddAssembly(
            ISet<string> referencePaths,
            System.Reflection.Assembly assembly)
        {
            if (!string.IsNullOrWhiteSpace(assembly.Location))
                referencePaths.Add(assembly.Location);
        }
    }
}
