using System.Text;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{

    /// <summary>
    /// Serializes generated SQL queries into deterministic snapshot text.
    /// </summary>
    internal static class QuerySnapshotSerializer
    {
        /// <summary>
        /// Serializes a generated SQL query into snapshot format.
        /// </summary>
        public static string Serialize(GeneratedSqlQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var builder = new StringBuilder();

            builder.AppendLine(query.CommandText.Trim());

            if (query.Parameters.Count == 0)
                return builder.ToString().TrimEnd();

            builder.AppendLine();
            builder.AppendLine("-- Parameters");

            foreach (var parameter in query.Parameters)
                builder.AppendLine($"{parameter.Name} = {parameter.Value}");

            return builder.ToString().TrimEnd();
        }
    }
}
