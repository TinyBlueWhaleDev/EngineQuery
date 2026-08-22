using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Playground.Shared
{
    internal static class ProviderQueryPrinter
    {
        /// <summary>
        /// Prints provider name, command text and parameters.
        /// </summary>
        public static void Print(string providerName, GeneratedSqlQuery sql)
        {
            Console.WriteLine($"--- {providerName} ---");
            Console.WriteLine(sql.CommandText);

            foreach (var parameter in sql.Parameters)
                Console.WriteLine($"{parameter.Name} = {parameter.Value}");

            Console.WriteLine();
        }
    }
}
