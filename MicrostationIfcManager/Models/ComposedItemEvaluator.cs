using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MicrostationIfcManager.Models
{
    public static class ComposedItemEvaluator
    {

        public static List<string> GetPropertyNames(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return new List<string>();

            // RegexOptions.Compiled is supported in .NET Framework 4.8
            Regex angleTokenRegex = new Regex(@"\<([^<>]+)\>", RegexOptions.Compiled);

            var matches = angleTokenRegex.Matches(expression);
            List<string> result = new List<string>();

            foreach (Match m in matches)
            {
                string value = m.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    result.Add(value);
                }
            }

            return result;
        }



        public static string Resolve(string expression, Dictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(expression))
                return expression;

            Regex placeholderRegex = new Regex(@"\<([^<>]+)\>", RegexOptions.Compiled);

            return placeholderRegex.Replace(expression, match =>
            {
                string key = match.Groups[1].Value;

                // Try to get the value (case-insensitive)
                if (values.TryGetValue(key, out string value))
                    return value;

                // Try case-insensitive matching
                foreach (var kv in values)
                {
                    if (string.Equals(kv.Key, key, System.StringComparison.OrdinalIgnoreCase))
                        return kv.Value;
                }

                // If not found, keep original <Property>
                return match.Value;
            });
        }

    }
}
