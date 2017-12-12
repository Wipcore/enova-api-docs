using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product;
using Wipcore.Enova.Api.NetClient;

namespace Wipcore.Enova.Api.NetClient
{
    /// <summary>
    /// This class registers all components that are needed to use NetClient. NOTE: only use this module if you do not want to setup all registrations yourself,
    /// as this can otherwise overwrite better registrations.
    /// </summary>
    public class NetClientDependenciesModule : Module
    {
       
        protected override void Load(ContainerBuilder builder)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("netclient.json");
            var configRoot = configBuilder.Build();

            var servicecollection = new ServiceCollection();
            servicecollection.AddDataProtection();
            var serviceProvider = servicecollection.BuildServiceProvider();
            
            builder.RegisterInstance<IConfigurationRoot>(configRoot).SingleInstance();
            builder.RegisterInstance<ILoggerFactory>(new LoggerFactory()).SingleInstance();
            builder.RegisterInstance<IServiceProvider>(serviceProvider).SingleInstance();
            builder.RegisterType<HttpContextAccessor>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterInstance<IDataProtectionProvider>(serviceProvider.GetDataProtectionProvider()).SingleInstance();
        }
    }
}
