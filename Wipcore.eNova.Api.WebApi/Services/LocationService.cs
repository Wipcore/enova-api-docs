using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.eNova.Api.WebApi.Services
{
    public class LocationService : ILocationService
    {
        private readonly IConfigurationRoot _configuration;

        public LocationService(IConfigurationRoot configuration)
        {
            _configuration = configuration;            
        }

        public IDictionary<string, string> GetParametersFromLocationConfiguration(string type, string location)
        {
            var config = _configuration.GetSection(type)?.GetSection(location);
            //var locationConfig = typeConfig[location];

            if (config == null)
                return null;

            var configDictionary = new Dictionary<string, string>();
            foreach(var setting in config.GetChildren())
            {
                configDictionary.Add(setting.Key, setting.Value);
            }


            return configDictionary;
        }
    }
}
