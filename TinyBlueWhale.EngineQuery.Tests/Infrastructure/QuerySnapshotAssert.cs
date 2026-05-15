using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{    
    /// <summary>
    /// Provides snapshot assertions for generated SQL queries.
    /// </summary>
    internal static class QuerySnapshotAssert
    {
        private const string UpdateSnapshotsEnvironmentVariable = "ENGINEQUERY_UPDATE_SNAPSHOTS";

        /// <summary>
        /// Asserts that the generated SQL query matches its stored provider snapshot.
        /// </summary>
        public static void Matches(string providerName, string snapshotName, GeneratedSqlQuery query)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
            ArgumentException.ThrowIfNullOrWhiteSpace(snapshotName);
            ArgumentNullException.ThrowIfNull(query);

            var snapshotPath = BuildSnapshotPath(providerName, snapshotName);
            var actualSnapshot = QuerySnapshotSerializer.Serialize(query);

            if (ShouldUpdateSnapshots())
            {
                Directory.CreateDirectory(Path.GetDirectoryName(snapshotPath)!);
                File.WriteAllText(snapshotPath, actualSnapshot);
                return;
            }

            if (!File.Exists(snapshotPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(snapshotPath)!);
                File.WriteAllText(snapshotPath, actualSnapshot);

                Assert.Inconclusive($"Snapshot did not exist and was created: {snapshotPath}");
            }

            var expectedSnapshot = File.ReadAllText(snapshotPath).TrimEnd();

            Assert.That(actualSnapshot, Is.EqualTo(expectedSnapshot));
        }

        private static string BuildSnapshotPath(string providerName, string snapshotName)
        {
            var fileName = snapshotName.EndsWith(".sqlsnap", StringComparison.OrdinalIgnoreCase)
                ? snapshotName
                : $"{snapshotName}.sqlsnap";

            var projectDirectory = ResolveProjectDirectory();
            return Path.Combine(projectDirectory, "Snapshots", providerName, fileName);
        }

        private static string ResolveProjectDirectory()
        {
            var currentDirectory = TestContext.CurrentContext.TestDirectory;
            var directory = new DirectoryInfo(currentDirectory);

            while (directory is not null)
            {
                var csprojFile = directory
                    .GetFiles("*.csproj")
                    .FirstOrDefault();

                if (csprojFile is not null)
                    return directory.FullName;

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Unable to resolve the test project directory.");
        }

        private static bool ShouldUpdateSnapshots()
        {
            var value = Environment.GetEnvironmentVariable(UpdateSnapshotsEnvironmentVariable);

            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(value, "1", StringComparison.OrdinalIgnoreCase);
        }
    }
}
