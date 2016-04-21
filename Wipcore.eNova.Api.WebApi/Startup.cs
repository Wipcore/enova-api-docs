using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Connectivity;
using NLog.Extensions.Logging;
using Wipcore.Library;
using Wipcore.Core;

namespace Wipcore.Enova.Api.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            //key = filename, value = optional or not
            var jsonConfigs = new Dictionary<string, bool>() { { "appsettings.json" , false}, { "localappsettings.json", true },
                { "locationConfiguration.json" , false}, { "marketConfiguration.json", false} };
            
            var builder = new ConfigurationBuilder();
            jsonConfigs.ForEach(x => builder.AddJsonFile(x.Key, x.Value));
            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
            jsonConfigs.ForEach(x => Configuration.ReloadOnChanged(x.Key));//monitor changes on all files

            StartEnova();
        }

        public IConfigurationRoot Configuration { get; set; }

        private void StartEnova()
        {
            EnovaSystemFacade.Current.LoadAllAssemblies();

            var settings = new InMemoryConnectionSettings
            {
                DatabaseConnection = Configuration.Get<String>("Enova:ConnectionString"),
                HistoryDatabaseConnection = Configuration.Get<String>("Enova:RevisionConnectionString"),
                CertificateKey = Configuration.Get<String>("Enova:CertificateKey"),
                CertificatePassword = Configuration.Get<String>("Enova:CertificatePassword"),
                UserName = Configuration.Get<String>("Enova:Username"),
                Password = Configuration.Get<String>("Enova:Password"),
                LogPath = Configuration.Get<String>("Enova:LogPath"),
                LogLevel = (Wipcore.Library.Diagnostics.Log.LogLevel) Convert.ToInt32(Configuration.Get<String>("Enova:LogLevel"))
            };

            EnovaSystemFacade.Current.Settings = settings;

            EnovaSystemFacade.Current.Start();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance<IConfigurationRoot>(Configuration);

            var apiAssemblies = new List<Assembly>() {Assembly.GetExecutingAssembly()};
            var autofacModules = new List<IEnovaApiModule>() {new WebApiModule()};
            LoadAddinAssemblies(apiAssemblies, autofacModules);

            autofacModules.OrderBy(x => x.Priority).ToList().ForEach(x => containerBuilder.RegisterModule(x));

            // Add framework services.
            services.AddMvc().AddControllersAsServices(apiAssemblies);
            
            containerBuilder.Populate(services);

            var container = containerBuilder.Build();

            // add cmo properties
            var cmoProperties = container.Resolve<IEnumerable<ICmoProperty>>();
            EnovaSystemFacade.Current.Connection.Kernel.AddCmoProperties(cmoProperties);

            return container.Resolve<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            ConfigureNlog(env,loggerFactory);

            app.UseIISPlatformHandler();

            app.UseStaticFiles();
            app.UseStatusCodePages();

            app.UseMvc();
        }

        private void ConfigureNlog(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            if (File.Exists("NLog.config"))
            {
                env.ConfigureNLog("NLog.config");
            }
            else
            {
                //temp until nlog is found correctly in root
                env.ConfigureNLog(@"approot\packages\Wipcore.eNova.Api.WebApi\1.0.0\root\NLog.config");
            }
        }
        
        private void LoadAddinAssemblies(List<Assembly> assemblies, List<IEnovaApiModule> autofacModules)
        {
            /* Load all dlls/assemblies from the addin folder, where external mappers/services/controllers can be added. */
            var addinPath = Configuration.Get<String>("ApiSettings:PathToAddins");
            if(String.IsNullOrEmpty(addinPath))
                addinPath = Path.Combine(Environment.CurrentDirectory, @"..\AddIn");

            if (!Directory.Exists(addinPath))
                return;

            var dllFiles = Directory.GetFiles(addinPath, "*.dll", SearchOption.AllDirectories);
            foreach (var dllFile in dllFiles)
            {
                /* Load by byte assembly since load by name might not work if
                       the assembly has been loaded before */
                using (Stream stream = File.OpenRead(dllFile))
                {
                    var assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    var assembly = Assembly.Load(assemblyData);
                    assemblies.Add(assembly);

                    //extract module types and add them to be registered
                    var moduleTypes = assembly.GetTypes().Where(x => typeof (IEnovaApiModule).IsAssignableFrom(x));
                    autofacModules.AddRange(moduleTypes.Select(x => (IEnovaApiModule) Activator.CreateInstance(x)));
                }
            }
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
