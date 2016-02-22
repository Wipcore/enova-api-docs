using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNet.Diagnostics;
using Wipcore.Enova.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Connectivity;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Api.WebApi.Mappers;
using Wipcore.Enova.Api.Models;

namespace Wipcore.Enova.Api.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("locationConfiguration.json")
                .AddJsonFile("marketConfiguration.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            StartEnova();
        }

        public IConfigurationRoot Configuration { get; set; }

        private void StartEnova()
        {
            EnovaSystemFacade.Current.LoadAllAssemblies();

            var settings = new InMemoryConnectionSettings();
            settings.DatabaseConnection = Configuration.Get<String>("Enova:ConnectionString");
            settings.HistoryDatabaseConnection = Configuration.Get<String>("Enova:RevisionConnectionString");
            settings.CertificateKey = Configuration.Get<String>("Enova:CertificateKey");
            settings.CertificatePassword = Configuration.Get<String>("Enova:CertificatePassword");
            settings.UserName = Configuration.Get<String>("Enova:Username");
            settings.Password = Configuration.Get<String>("Enova:Password");
            settings.LogPath = Configuration.Get<String>("Enova:LogPath");
            settings.LogLevel = (Wipcore.Library.Diagnostics.Log.LogLevel)Convert.ToInt32(Configuration.Get<String>("Enova:LogLevel"));

            EnovaSystemFacade.Current.Settings = settings;

            EnovaSystemFacade.Current.Start();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<WebApiModule>();
            containerBuilder.RegisterInstance<IConfigurationRoot>(Configuration);
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return container.Resolve<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
