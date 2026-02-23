using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RevitIfcManager.Models
{
    public static class ComposedItemEvaluator
    {
        public static List<string> GetPropertyNames(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return new List<string>();

            Regex angleTokenRegex = new(@"\<([^<>]+)\>", RegexOptions.Compiled);

            return angleTokenRegex
                .Matches(expression)
                .Select(m => m.Groups[1].Value.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }


        public static string Resolve(string expression, Dictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(expression))
                return expression;

            Regex placeholderRegex = new(@"\<([^<>]+)\>", RegexOptions.Compiled);

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
