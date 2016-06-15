using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wipcore.Library;

namespace Wipcore.eNova.Api.WebApi.Helpers
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
    }
}
