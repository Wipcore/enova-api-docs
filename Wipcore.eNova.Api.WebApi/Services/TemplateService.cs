﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly IConfigurationRoot _configuration;

        public TemplateService(IConfigurationRoot configuration)
        {
            _configuration = configuration;            
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

            IConfigurationSection config = null;
            while ((config == null || !config.GetChildren().Any()) && type != typeof (object))//looping down in types until some setting is found
            {
                config = _configuration.GetSection(type.Name)?.GetSection(parameters.Template);

                type = type.BaseType;
            }

            if (config == null)
                return null;

            var settings = config.GetChildren().ToList();

            //if the parameter is not already set, retrive it from the settings
            parameters.Page = parameters.Page ?? Convert.ToInt32(settings.FirstOrDefault(x => x.Key == "page")?.Value ?? "1");
            parameters.Size = parameters.Size ?? Convert.ToInt32(settings.FirstOrDefault(x => x.Key == "size")?.Value ?? "20");

            parameters.Sort = SetValue(parameters.Sort, settings, "sort");
            parameters.Filter = SetValue(parameters.Filter, settings, "filter");  
            parameters.Properties = SetValue(parameters.Properties, settings, "properties");

            return parameters;
        }

        private string SetValue(string parameterValue, List<IConfigurationSection> settings, string settingName)
        {
            var settingValue = settings.FirstOrDefault(x => x.Key == settingName)?.Value;
            if(parameterValue == null)
                return settingValue;
            //combine given parameter with setting. + or space as +  can be modelbinded to space
            if (parameterValue.StartsWith("+") || parameterValue.StartsWith(" "))
                return parameterValue.Substring(1) + ","+ settingValue;

            return parameterValue;
        }
    }
}
