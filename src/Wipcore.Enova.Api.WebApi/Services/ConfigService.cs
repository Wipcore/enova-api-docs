using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.eNova.Api.WebApi.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IConfigurationRoot _configuration;

        public ConfigService(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public int DecimalsInAmountString() => _configuration.GetValue<int>("EnovaSettings:DecimalsInAmountString", 2);
    }
}
