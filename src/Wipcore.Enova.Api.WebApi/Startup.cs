﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using Swashbuckle.Swagger.Model;
using Wipcore.Core;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Connectivity;
using EnumerableExtensions = Wipcore.Library.EnumerableExtensions;

namespace Wipcore.Enova.Api.WebApi
{
    public class Startup
    {
        public const string ApiVersion = "1.0"; //TODO might specify somewhere else.
        private readonly string _configFolderPath;
        private readonly string _addInFolderPath;
        private string _swaggerDocsFolderPath;
        private IConfigurationRoot Configuration { get; }
        private IHostingEnvironment Env { get; }

        private IContainer Container { get; set; }


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
            EnumerableExtensions.ForEach(jsonConfigs, x => builder.AddJsonFile(Path.Combine(_configFolderPath, x.Key), x.Value, reloadOnChange: true));
            builder.AddEnvironmentVariables();

            Configuration = builder.Build();

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
                Console.WriteLine("Looking for controllers in assembly: {0}", assembly.FullName);
                builder.PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
            }
            builder.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
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
            Container = containerBuilder.Build();

            // add cmo properties
            var cmoProperties = Container.Resolve<IEnumerable<ICmoProperty>>();
            EnovaSystemFacade.Current.Connection.Kernel.AddCmoProperties(cmoProperties);

            return Container.Resolve<IServiceProvider>();
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
                ExpireTimeSpan = new TimeSpan(0, Configuration.GetValue<int>("Auth:ExpireTimeMinutes", 360), 0),
                SlidingExpiration = Configuration.GetValue<bool>("Auth:SlidingExpiration", true)
            };

            if (!String.IsNullOrEmpty(Configuration.GetValue<string>("Auth:CookieDomain", String.Empty)))
                cookieOptions.CookieDomain = Configuration.GetValue<string>("Auth:CookieDomain");
            if (!String.IsNullOrEmpty(Configuration.GetValue<string>("Auth:CookieName", String.Empty)))
                cookieOptions.CookieName = Configuration.GetValue<string>("Auth:CookieName");

            var jwtBearer = new JwtBearerOptions()
            {
                AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme,
                AutomaticAuthenticate = true,
                AutomaticChallenge = Configuration.GetValue<bool>("Auth:AutomaticChallenge", false),
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Auth:IssuerSigningKey", AuthService.DefaultSignKey))),
                    ValidateIssuer = Configuration.GetValue<bool>("Auth:ValidateIssuer", true),
                    ValidIssuer = AuthService.AuthenticationScheme,
                    ValidateAudience = Configuration.GetValue<bool>("Auth:ValidateAudience", true),
                    ValidAudience = Configuration.GetValue<string>("Auth:ValidAudience", "http://localhost:5000/"),
                    ValidateLifetime = Configuration.GetValue<bool>("Auth:ValidateLifetime", true),
                    ClockSkew = TimeSpan.Zero,
                    AuthenticationType = JwtBearerDefaults.AuthenticationScheme
                }
            };

            if(Configuration.GetValue<bool>("Auth:UseAuthTimeValidation", true))
                jwtBearer.TokenValidationParameters.LifetimeValidator = Container.Resolve<IAuthService>().ExpireValidator;

            app.UseJwtBearerAuthentication(jwtBearer);

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
            _swaggerDocsFolderPath = Configuration.GetValue<string>("ApiSettings:PathToSwaggerDocs", Path.GetFullPath(Env.ContentRootPath + @".\SwaggerDocs"));
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
                    Title = AuthService.AuthenticationScheme,
                    Description = "",
                    TermsOfService = ""
                });
                options.OperationFilter<ComplexModelFilter>(_swaggerDocsFolderPath);
                docFilePaths.ForEach(options.IncludeXmlComments);
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
            logger.LogInformation("Connecting to Enova with: {0}", EnovaSystemFacade.Current.Settings.DatabaseConnection);
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
                    var moduleTypes = assembly.GetTypes().Where(x => typeof (IEnovaApiModule).IsAssignableFrom(x)).ToList();
                    moduleTypes.ForEach(x => Console.WriteLine("Found addin IEnovaApiModule module in assembly: " + x.Assembly.FullName));
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
