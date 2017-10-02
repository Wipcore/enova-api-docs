using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Wipcore.Enova.Api.Abstractions
{
    public static class ConfigurationExtension
    {
        public static T GetValue<T>(this IConfigurationRoot root, string key)
        {
            var value = root[key];
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T GetValue<T>(this IConfigurationRoot root, string key, T defaultValue)
        {
            var value = root[key];
            if (String.IsNullOrEmpty(value))
                return defaultValue;

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
