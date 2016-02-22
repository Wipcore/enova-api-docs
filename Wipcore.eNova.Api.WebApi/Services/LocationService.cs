using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class LocationService : ILocationService
    {
        private readonly IConfigurationRoot _configuration;

        public LocationService(IConfigurationRoot configuration)
        {
            _configuration = configuration;            
        }

        public IGetParametersModel GetParametersFromLocationConfiguration(string type, IGetParametersModel parameters)
        {
            parameters = parameters ?? new GetParametersModel();

            var config = _configuration.GetSection(type)?.GetSection(parameters.Location);
            
            if (config == null)
                return null;

            var settings = config.GetChildren().ToList();

            //if the parameter is not already set, retrive it from the settings
            parameters.Filter = parameters.Filter ?? settings.FirstOrDefault(x => x.Key == "filter")?.Value;
            parameters.Properties = parameters.Properties ?? settings.FirstOrDefault(x => x.Key == "properties")?.Value;
            parameters.Sort = parameters.Sort ?? settings.FirstOrDefault(x => x.Key == "sort")?.Value;
            parameters.Page = parameters.Page ?? Convert.ToInt32(settings.FirstOrDefault(x => x.Key == "page")?.Value ?? "0");
            parameters.Size = parameters.Size ?? Convert.ToInt32(settings.FirstOrDefault(x => x.Key == "size")?.Value ?? "20");
            
            return parameters;
        }
    }
}
