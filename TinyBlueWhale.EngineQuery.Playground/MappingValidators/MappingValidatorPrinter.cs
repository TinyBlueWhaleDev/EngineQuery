using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    internal static class MappingValidatorPrinter
    {
        public static void Print(string title, GeneratedSqlQuery sql)
        {
            Console.WriteLine($"--- {title} ---");
            Console.WriteLine(sql.CommandText);

            foreach (var parameter in sql.Parameters)
                Console.WriteLine($"{parameter.Name} = {parameter.Value}");

            Console.WriteLine();
        }
    }
}
