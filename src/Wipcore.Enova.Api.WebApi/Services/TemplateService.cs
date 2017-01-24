using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Fasterflect;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IContextService _contextService;
        private readonly IEnumerable<IPropertyMapper> _mappers;
        private readonly ObjectCache _cache;

        public TemplateService(IConfigurationRoot configuration, IContextService contextService, IEnumerable<IPropertyMapper> mappers, ObjectCache cache)
        {
            _configuration = configuration;
            _contextService = contextService;
            _mappers = mappers;
            _cache = cache;
        }

        /// <summary>
        /// Read template information from configuration - info of how queries should be processed, I.E default languages, properties etc.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters">Parameters direct from query.</param>
        /// <returns></returns>
        public IQueryModel GetQueryModelFromTemplateConfiguration(Type type, IQueryModel parameters)
        {
            parameters = parameters ?? new QueryModel();
            
            List<IConfigurationSection> settings = null;
            var configType = type;
            while (configType != typeof (object))//looping down in types until some setting is found
            {
                settings = _configuration.GetSection(configType.Name)?.GetSection(parameters.Template).GetChildren().ToList();
                if (settings.Any())
                    break;

                configType = configType.BaseType;
            }
            
            //if the parameter is not already set, retrive it from the settings
            parameters.Page = parameters.Page ?? Convert.ToInt32(settings?.FirstOrDefault(x => x.Key == "page")?.Value ?? "1");
            parameters.Size = parameters.Size ?? Convert.ToInt32(settings?.FirstOrDefault(x => x.Key == "size")?.Value ?? "20");

            parameters.Sort = SetValue(parameters.Sort, settings, "sort");
            parameters.Filter = SetValue(parameters.Filter, settings, "filter");

            parameters.Properties = "_all".Equals(parameters.Properties, StringComparison.CurrentCultureIgnoreCase) ? 
                GetAllProperties(type) ://if keyword "_all" is given, then find all possible properties
                "_all_custom".Equals(parameters.Properties, StringComparison.CurrentCultureIgnoreCase) ? //if all_custom then only get custom propeties
                GetAllProperties(type, true) : 
                SetValue(parameters.Properties, settings, "properties");

            return parameters;
        }

        private string SetValue(string parameterValue, List<IConfigurationSection> settings, string settingName)
        {
            var settingValue = settings?.FirstOrDefault(x => x.Key == settingName)?.Value;
            if(parameterValue == null)
                return settingValue;
            //combine given parameter with setting. + or space as +  can be modelbinded to space
            if (parameterValue.StartsWith("+") || parameterValue.StartsWith(" "))
                return settingValue + "," + parameterValue.Substring(1);

            return parameterValue;
        }

        /// <summary>
        /// Get all possible properties on a type.
        /// </summary>
        private string GetAllProperties(Type type, bool onlyCustom = false)
        {
            var key = "all_properties_" + type.Name + onlyCustom;
            var cacheValue = _cache.Get(key);

            if (cacheValue != null)
                return cacheValue.ToString();

            //first get any matching mappers
            var properties = _mappers.
                Where(x => x.MapType == MapType.MapFromAndToEnovaAllowed || x.MapType == MapType.MapFromEnovaAllowed).
                Where(x => x.Type == type || (x.InheritMapper && x.Type.IsAssignableFrom(type))).
                OrderBy(x => x.Priority).SelectMany(x => x.Names).ToList();

            if (!onlyCustom)
            {
                //then get any basic enova properties
                string tableName;
                var propertyNames = _contextService.GetContext().GetAllPropertyNames(type, out tableName);
                var dummy = (BaseObject)EnovaObjectCreationHelper.CreateNew(type, _contextService.GetContext());

                foreach (var propertyName in propertyNames)
                {
                    try
                    {
                        dummy.GetProperty(propertyName);//making sure none of these properties causes exceptions
                        properties.Add(propertyName);
                    }
                    catch
                    {
                        // ignored, just interested in getting those that work
                    }
                }
            }

            //make sure they are unique
            properties = properties.Distinct().ToList();
            
            var propertiesString = String.Join(",", properties);
            _cache.Set(key, propertiesString, DateTime.Now.AddDays(1));

            return propertiesString;
        }
    }
}
