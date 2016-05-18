using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class LocationService : ILocationService
    {
        private readonly IConfigurationRoot _configuration;

        public LocationService(IConfigurationRoot configuration)
        {
            _configuration = configuration;            
        }

        public IGetParametersModel GetParametersFromLocationConfiguration(Type type, IGetParametersModel parameters)
        {
            parameters = parameters ?? new GetParametersModel();

            IConfigurationSection config = null;
            while ((config == null || !config.GetChildren().Any()) && type != typeof (object))
            {
                config = _configuration.GetSection(type.Name)?.GetSection(parameters.Location);

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
