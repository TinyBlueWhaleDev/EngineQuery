using System.Data.Common;
using System.Dynamic;

namespace TinyBlueWhale.EngineQuery.Samples.Results
{
    public static class SerializeRowsAsCsv
    {
        public static string FromObjects(IReadOnlyList<object> rows)
        {
            if (rows.Count == 0)
                return "(no rows)";

            var properties = rows[0]
                .GetType()
                .GetProperties();

            var lines = new List<string>
            {
                string.Join(",",properties.Select(property => property.Name))
            };

            lines.AddRange(rows.Select(row => string.Join(",", properties.Select(property => property.GetValue(row)))));

            return string.Join(Environment.NewLine, lines);
        }
        public static string FromDapperRows(IEnumerable<dynamic> rows)
        {
            var dictionaries = rows
                .Select(ToDictionary)
                .ToList();

            return FromDictionaries(dictionaries);
        }

        public static async Task<string> FromReaderAsync(DbDataReader reader)
        {
            var rows = new List<IDictionary<string, object?>>();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();

                for (var i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);

                rows.Add(row);
            }

            return FromDictionaries(rows);
        }

        private static string FromDictionaries(IReadOnlyList<IDictionary<string, object?>> rows)
        {
            if (rows.Count == 0)
                return "(no rows)";

            var columns = rows[0].Keys.ToList();
            var lines = new List<string>
        {
            string.Join(",",columns)
        };

            lines.AddRange(rows.Select(row => string.Join(",", columns.Select(column => row[column]))));

            return string.Join(Environment.NewLine, lines);
        }

        private static IDictionary<string, object?> ToDictionary(dynamic row)
        {
            if (row is IDictionary<string, object?> dictionary)
                return dictionary;

            if (row is IDictionary<string, object> objectDictionary)
                return objectDictionary.ToDictionary(
                    pair => pair.Key,
                    pair => (object?)pair.Value);

            if (row is ExpandoObject expando)
                return expando.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value);

            object obj = row;

            return obj.GetType()
                .GetProperties()
                .ToDictionary(
                    property => property.Name,
                    property => property.GetValue(obj));
        }
    }
}
