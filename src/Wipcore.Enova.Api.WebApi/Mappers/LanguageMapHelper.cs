using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public static class LanguageMapHelper
    {
        public static Dictionary<string, object> MapLanguageProperty(this Dictionary<string, object> dictionary, string propertyName, List<EnovaLanguage> languages, Func<EnovaLanguage, object> mapFunc)
        {
            if (languages?.Any() == true)
            {
                foreach (var enovaLanguage in languages)
                {
                    dictionary.Add($"{propertyName}-{enovaLanguage.Identifier}", mapFunc.Invoke(enovaLanguage));
                }
            }
            else
            {
                dictionary.Add(propertyName, mapFunc.Invoke(null));
            }

            return dictionary;
        }

    }
}
