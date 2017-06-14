using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Wipcore.Enova.Api.WebApi.Helpers
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Creates a log friendly string from a dictionary.
        /// </summary>
        public static string ToLog(this IDictionary<string, object> values)
        {
            if(values == null)
                return String.Empty;

            var valuesAsStrings = values.Select(x =>
            {
                if (x.Value is IDictionary<string, object>)
                    return $"{x.Key}={((IDictionary<string, object>) x.Value).ToLog()}";
                if (x.IsAdditionalValuesKey())
                {
                    var subValues = ((JObject) x.Value).ToObject<Dictionary<string, object>>();
                    return $"({x.Key}={subValues.ToLog()})";
                }
                return $"{x.Key}={x.Value}";

            });

            return String.Join(", ", valuesAsStrings);
        }

        /// <summary>
        /// True if the given value is an 'additionalvalue', which means it's a subdictionary.
        /// </summary>
        public static bool IsAdditionalValuesKey(this KeyValuePair<string, object> pair)
        {
            return pair.Key.ToLower() == "additionalvalues" && pair.Value is JObject;
        }

        public static T GetOrDefault<T>(this IDictionary<string, object> values, string key, object defaultValue = null)
        {
            object value;
            if (values.TryGetValue(key, out value))
                return (T)Convert.ChangeType(value, typeof(T));

            return defaultValue == null ? default(T) : (T)Convert.ChangeType(defaultValue, typeof(T));
        }

        public static bool ContainsKeyInsensitive(this IDictionary<string, object> dictionary, string key)
        {
            return dictionary.Keys.Any(x => String.Equals(key, x, StringComparison.InvariantCultureIgnoreCase));
        }

        public static T GetValueInsensitive<T>(this IDictionary<string, object> dictionary, string key)
        {
            var entry = dictionary.FirstOrDefault(x => String.Equals(key, x.Key, StringComparison.InvariantCultureIgnoreCase));
            if (entry.Value == null)
                return default(T);
            return (T)Convert.ChangeType(entry.Value, typeof(T));
        }

        public static T GetValueInsensitive<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            var entry = dictionary.FirstOrDefault(x => String.Equals(key, x.Key, StringComparison.InvariantCultureIgnoreCase));
            if (entry.Value == null)
                return defaultValue;
            return (T)entry.Value;
        }
    }
}
