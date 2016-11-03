using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Fasterflect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Connectivity;
using NLog.Extensions.Logging;
using Swashbuckle.SwaggerGen;
using Wipcore.Library;
using Wipcore.Core;
using Wipcore.Core.SystemMonitoring;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Api.OAuth;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Swashbuckle.Swagger.Model;

namespace Wipcore.Enova.Api.WebApi
{
    public class Startup
    {
        public const string ApiVersion = "1.0"; //TODO might specify somewhere else.
        private readonly string _configFolderPath;
        private readonly string _addInFolderPath;
        private string _swaggerDocsFolderPath;
        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Env { get; }

        public Startup(IHostingEnvironment env)
        {
            Env = env;
            
            Console.WriteLine("Webroot: "+env.WebRootPath);
            Console.WriteLine("Content root: " + env.ContentRootPath);

            // Set up configuration sources. Key = filename, Value = optional or not
            var jsonConfigs = new Dictionary<string, bool>() { { "appsettings.json" , false}, { "localappsettings.json", true },
                { "templateConfiguration.json" , false}, { "marketConfiguration.json", false}, { "customsettings.json", true} };
            _configFolderPath = Path.GetFullPath(Path.Combine(env.ContentRootPath, @".\Configs"));
            
            var builder = new ConfigurationBuilder();
            jsonConfigs.ForEach(x => builder.AddJsonFile(Path.Combine(_configFolderPath, x.Key), x.Value, reloadOnChange: true));
            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
            //jsonConfigs.ForEach(x => Configuration.ReloadOnChanged(_configFolderPath, x.Key));//monitor changes on all files

            _addInFolderPath = Configuration.GetValue<String>("ApiSettings:PathToAddins");
            if (String.IsNullOrEmpty(_addInFolderPath))
                _addInFolderPath = Path.GetFullPath(Path.Combine(env.ContentRootPath, @".\AddIn"));

            StartEnova();
        }

        private void StartEnova()
        {
            var settings = new InMemoryConnectionSettings
            {
                DatabaseConnection = Configuration.GetValue<String>("Enova:ConnectionString"),
                HistoryDatabaseConnection = Configuration.GetValue<String>("Enova:RevisionConnectionString"),
                CertificateKey = Configuration.GetValue<String>("Enova:CertificateKey"),
                CertificatePassword = Configuration.GetValue<String>("Enova:CertificatePassword"),
                UserName = Configuration.GetValue<String>("Enova:Alias"),
                Password = Configuration.GetValue<String>("Enova:Password"),
                LogPath = Configuration.GetValue<String>("Enova:LogPath"),
                LogLevel = (Wipcore.Library.Diagnostics.Log.LogLevel) Convert.ToInt32(Configuration.GetValue<String>("Enova:LogLevel")),
                PathToConfigurationFiles = _configFolderPath,
                PathToAddinDlls = _addInFolderPath
            };

            EnovaSystemFacade.Current.Settings = settings;
            EnovaSystemFacade.Current.Start();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance<IConfigurationRoot>(Configuration);

            var apiAssemblies = new List<Assembly>() {Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(AccountController))};
            var autofacModules = new List<IEnovaApiModule>() {new WebApiModule()};
            LoadAddinAssemblies(apiAssemblies, autofacModules);

            autofacModules.OrderBy(x => x.Priority).ToList().ForEach(x => containerBuilder.RegisterModule(x));

            // Add framework services.
            var builder = services.AddMvc();
            foreach (var assembly in apiAssemblies)
            {
                builder.PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
            }
            builder.AddControllersAsServices();

            //security
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(_configFolderPath)); //TODO: Not fully sure about this one
            services.AddAuthorization(options =>
            {
                options.AddPolicy(CustomerUrlIdentifierPolicy.Name, policy => policy.Requirements.Add(new CustomerUrlIdentifierPolicy()));
                options.AddPolicy(CustomerBodyIdentifierPolicy.Name, policy => policy.Requirements.Add(new CustomerBodyIdentifierPolicy()));
            });

            if (Configuration.GetValue<bool>("ApiSettings:UseSwagger", true))
                ConfigureSwagger(services);

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

            //app.UseIISPlatformHandler();  - Moved to Main-method

            app.UseStaticFiles();
            app.UseStatusCodePages();

            var dataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(_configFolderPath), configuration =>
                {
                    configuration.SetApplicationName(AuthService.AuthenticationScheme + ApiVersion);
                    if (Configuration.GetValue<bool>("Auth:UseDpapiProtection", true))//turn off if having problems in clustered systems
                        configuration.ProtectKeysWithDpapiNG();
                });
            
            var cookieOptions = new CookieAuthenticationOptions()
            {
                DataProtectionProvider = dataProtectionProvider,
                AuthenticationScheme = AuthService.AuthenticationScheme,
                AutomaticAuthenticate = true,
                AutomaticChallenge = Configuration.GetValue<bool>("Auth:AutomaticChallenge", false),
                LoginPath = Configuration.GetValue<string>("Auth:LoginPath", String.Empty),
                LogoutPath = Configuration.GetValue<string>("Auth:LogoutPath", String.Empty),
                ReturnUrlParameter = Configuration.GetValue<string>("Auth:ReturnUrlParameter", String.Empty),
                CookieHttpOnly = Configuration.GetValue<bool>("Auth:CookieHttpOnly", false),
                CookieSecure = Configuration.GetValue<bool>("Auth:CookieSecure", false) ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest,
                CookiePath = Configuration.GetValue<string>("Auth:CookiePath", "/"),
                ExpireTimeSpan = new TimeSpan(0, Configuration.GetValue<int>("Auth:ExpireTimeMinutes", 120), 0),
                SlidingExpiration = Configuration.GetValue<bool>("Auth:SlidingExpiration", false)
            };

            if (!String.IsNullOrEmpty(Configuration.GetValue<string>("Auth:CookieDomain", String.Empty)))
                cookieOptions.CookieDomain = Configuration.GetValue<string>("Auth:CookieDomain");
            if (!String.IsNullOrEmpty(Configuration.GetValue<string>("Auth:CookieName", String.Empty)))
                cookieOptions.CookieName = Configuration.GetValue<string>("Auth:CookieName");


            app.UseCookieAuthentication(cookieOptions);

            app.UseMvc();
            
            if (Configuration.GetValue<bool>("ApiSettings:UseSwagger", true))
            {
                app.UseSwagger();
                app.UseSwaggerUi();
            }
        }
        
        private void ConfigureSwagger(IServiceCollection services)
        {
            _swaggerDocsFolderPath = Configuration.GetValue<string>("ApiSettings:PathToSwaggerDocs", Path.GetFullPath(Env.WebRootPath + @"\..\SwaggerDocs"));
            var docFilePaths = Directory.Exists(_swaggerDocsFolderPath) ? Directory.GetFiles(_swaggerDocsFolderPath, "*.xml").ToList() : new List<string>();
            
            if(!docFilePaths.Any())
            {
                //if no files found, add default path to xml for just this assembly. Path depends on if this is deployed or not
                var defaultPath = Env.IsDevelopment() ? Path.GetFullPath(Env.WebRootPath + @"\..\..\artifacts\bin\Wipcore.Enova.Api.WebApi\Debug\dnx451\Wipcore.Enova.Api.WebApi.xml")
                    : Path.GetFullPath(Env.WebRootPath + @"\..\approot\packages\Wipcore.Enova.Api.WebApi\1.0.0\lib\dnx451\Wipcore.Enova.Api.WebApi.xml");//TODO versioning
                docFilePaths.Add(defaultPath);
            }

            services.AddSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Enova API",
                    Description = "",
                    TermsOfService = ""
                });
                options.OperationFilter<ComplexModelFilter>(_swaggerDocsFolderPath);
                docFilePaths.ForEach(x => options.IncludeXmlComments(x));
                options.DescribeAllEnumsAsStrings();

            });

        }

        private void ConfigureNlog(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            env.ConfigureNLog(Path.Combine(_configFolderPath, "NLog.config"));

            //print some useful information, ensuring it's setup correctly
            var logger = loggerFactory.CreateLogger("Startup folders");
            logger.LogInformation("Reading configuration files from: {0}", _configFolderPath);
            logger.LogInformation("Reading addin files from: {0}", _addInFolderPath);
            if (Configuration.GetValue<bool>("ApiSettings:UseSwagger", true))
                logger.LogInformation("Reading swagger docs from: {0}", _swaggerDocsFolderPath);
        }
        
        private void LoadAddinAssemblies(List<Assembly> assemblies, List<IEnovaApiModule> autofacModules)
        {
            /* Load all dlls/assemblies from the addin folder, where external mappers/services/controllers can be added. */
            if (!Directory.Exists(_addInFolderPath))
                return;

            var dllFiles = Directory.GetFiles(_addInFolderPath, "*.dll", SearchOption.AllDirectories);
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
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
