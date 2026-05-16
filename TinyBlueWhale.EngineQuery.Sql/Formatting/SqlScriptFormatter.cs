using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.Formatting
{

    /// <summary>
    /// Formats generated SQL scripts using deterministic top-level clause formatting.
    /// </summary>
    public sealed class SqlScriptFormatter
    {
        private static readonly string NewLine = Environment.NewLine;

        /// <summary>
        /// Formats a SQL script.
        /// </summary>
        /// <param name="sql">
        /// SQL script.
        /// </param>
        /// <returns>
        /// Formatted SQL script.
        /// </returns>
        public static string Format(string sql)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sql);

            var lines = sql
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var formattedLines = new List<string>();

            var currentLine = string.Empty;

            for (var i = 0; i < lines.Length; i++)
            {
                var token = lines[i];

                if (IsClauseStart(token, lines, i))
                {
                    if (!string.IsNullOrWhiteSpace(currentLine))
                        formattedLines.Add(currentLine.TrimEnd());

                    currentLine = token;
                }
                else
                {
                    currentLine += currentLine.Length == 0
                        ? token
                        : $" {token}";
                }
            }

            if (!string.IsNullOrWhiteSpace(currentLine))
                formattedLines.Add(currentLine.TrimEnd());

            return string.Join(NewLine, formattedLines);
        }

        // Determines whether the token starts a top-level SQL clause.
        private static bool IsClauseStart(
            string token,
            string[] tokens,
            int index)
        {
            if (index == 0)
                return true;

            if (token.Equals("FROM", StringComparison.Ordinal))
                return true;

            if (token.Equals("WHERE", StringComparison.Ordinal))
                return true;

            if (token.Equals("GROUP", StringComparison.Ordinal) &&
                NextTokenEquals(tokens, index, "BY"))
                return true;

            if (token.Equals("HAVING", StringComparison.Ordinal))
                return true;

            if (token.Equals("ORDER", StringComparison.Ordinal) &&
                NextTokenEquals(tokens, index, "BY"))
                return true;

            if (token.Equals("INNER", StringComparison.Ordinal) &&
                NextTokenEquals(tokens, index, "JOIN"))
                return true;

            if (token.Equals("LEFT", StringComparison.Ordinal) &&
                NextTokenEquals(tokens, index, "JOIN"))
                return true;

            if (token.Equals("RIGHT", StringComparison.Ordinal) &&
                NextTokenEquals(tokens, index, "JOIN"))
                return true;

            if (token.Equals("FULL", StringComparison.Ordinal) &&
                NextTokenEquals(tokens, index, "JOIN"))
                return true;

            if (token.Equals("CROSS", StringComparison.Ordinal) &&
                (NextTokenEquals(tokens, index, "JOIN") ||
                 NextTokenEquals(tokens, index, "APPLY")))
                return true;

            if (token.Equals("OUTER", StringComparison.Ordinal) &&
                NextTokenEquals(tokens, index, "APPLY"))
                return true;

            if (token.Equals("UNION", StringComparison.Ordinal))
                return true;

            if (token.Equals("INTERSECT", StringComparison.Ordinal))
                return true;

            if (token.Equals("EXCEPT", StringComparison.Ordinal))
                return true;

            return false;
        }

        // Determines whether the next token matches the expected value.
        private static bool NextTokenEquals(string[] tokens, int index, string expected)
        {
            if (index + 1 >= tokens.Length)
                return false;

            return tokens[index + 1].Equals(expected, StringComparison.Ordinal);
        }
    }
}
