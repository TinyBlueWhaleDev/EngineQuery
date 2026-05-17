using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Samples.Results
{
    public static class WriteSampleResult
    {
        public static void Write(SampleExecutionResult result)
        {
            Console.WriteLine(new string('=', 80));
            Console.WriteLine($"Provider : {result.Provider}");
            Console.WriteLine($"Executor : {result.Executor}");
            Console.WriteLine($"Metadata : {result.Metadata}");
            Console.WriteLine($"Query    : {result.Query}");
            Console.WriteLine($"Status   : {result.Status}");
            Console.WriteLine($"Rows     : {result.RowCount}");

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                Console.WriteLine($"Error    : {result.ErrorMessage}");

            Console.WriteLine();
            Console.WriteLine("SQL:");
            Console.WriteLine(result.CommandText);

            if (result.Parameters.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Parameters:");

                foreach (var parameter in result.Parameters)
                    Console.WriteLine($"{parameter.Name} = {parameter.Value}");
            }

            if (!string.IsNullOrWhiteSpace(result.ResultText))
            {
                Console.WriteLine();
                Console.WriteLine("Result:");
                Console.WriteLine(result.ResultText);
            }

            Console.WriteLine(new string('=', 80));
            Console.WriteLine();
        }
    }
}
