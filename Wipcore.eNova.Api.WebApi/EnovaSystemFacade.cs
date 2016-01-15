using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using Wipcore.Core;
using Wipcore.Enova.Connectivity;

namespace Wipcore.eNova.Api.WebApi
{
    /// <summary>
    /// Provides an easy way to start up an Enova system.
    /// </summary>
    public class EnovaSystemFacade
    {
        private InMemoryConnectionSettings _settings;
        private InMemoryConnection _connection;

        private static readonly EnovaSystemFacade m_systemFacade = new EnovaSystemFacade();

        private EnovaSystemFacade()
        {
            //_settings = new InMemoryConnectionSettings();
            //_settings.DatabaseConnection = WebConfigurationManager.AppSettings["Enova.LocalSystem.ConnectionString"];
            //_settings.HistoryDatabaseConnection = WebConfigurationManager.AppSettings["Enova.LocalSystem.RevisionConnectionString"];
            //_settings.CertificateKey = WebConfigurationManager.AppSettings["Enova.LocalSystem.CertificateKey"];
            //_settings.CertificatePassword = WebConfigurationManager.AppSettings["Enova.LocalSystem.CertificatePassword"];
            //_settings.UserName = WebConfigurationManager.AppSettings["Enova.Admin.Username"];
            //_settings.Password = WebConfigurationManager.AppSettings["Enova.Admin.Password"];
            //_settings.LogPath = WebConfigurationManager.AppSettings["Enova.LocalSystem.LogPath"];
            //_settings.LogLevel = (Wipcore.Library.Diagnostics.Log.LogLevel)Convert.ToInt32(WebConfigurationManager.AppSettings["Enova.LocalSystem.LogLevel"]);
        }
        
        public InMemoryConnectionSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        /// <summary>
        /// The Enova connection
        /// Use the set only in unit tests
        /// </summary>
        public InMemoryConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        /// <summary>
        /// Get the singleton instance of the Enova system facade. You can use this
        /// property in cases where you cannot use constructor injection.
        /// </summary>
        public static EnovaSystemFacade Current
        {
            get { return m_systemFacade; }
        }


        /// <summary>
        /// Starts the Enova system using the settings found in web.config.
        /// </summary>
        public void Start()
        {
            try
            {

                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();

                //LogWriter.Current.Info("EnovaSystemFacade start");
                //LoadAllAssemblies();
                
                // Start a system in memory.
                _connection = StartSystemInMemory();
                
                //stopwatch.Stop();
                //LogWriter.Current.Info("EnovaSystemFacade start end elapsed: {0}", stopwatch.Elapsed);
            }
            catch (Exception e)
            {
                //LogWriter.Current.Info("EnovaSystemFacade start exception: {0}", e);
                throw;
            }

        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
            _connection = null;
        }

        
        /// <summary>
        /// Starts a local Wipcore eNova system.
        /// </summary>
        private InMemoryConnection StartSystemInMemory()
        {
            //LogWriter.Current.Info("Starting system in memory.");
            
            InMemoryConnection connection = new InMemoryConnection(_settings);
            
            //m_fileLogLevelWatcher = new LogConfigFileWatcher();
            //m_fileLogLevelWatcher.Notify(logConfig =>
            //{
            //    connection.Kernel.LogLevel = (int)logConfig.LogLevel;
            //    if (logConfig.LogPath != null && logConfig.LogPath != connection.Kernel.LogPath)
            //        connection.Kernel.LogPath = logConfig.LogPath;
            //});

            //LogWriter.Current.Info("Local system started.");

            return connection;
        }

        /// <summary>
        /// Loads all assemblies so webpaymentprovuders etc can find them
        /// </summary>
        public void LoadAllAssemblies()
        {
            //string binPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin"); // if web application
            string binPath = System.AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(binPath))
            {
                throw new Exception("cant find bin folder!");
            }

            string[] dllPaths = Directory.GetFiles(binPath, "*.dll");
            foreach (string dllPath in dllPaths)
            {
                try
                {
                    Assembly a = Assembly.LoadFrom(dllPath);
                }
                catch (Exception ex)
                {
                    //LogWriter.Current.Info("Failed to load assembly:" + ex);
                }
            }

        }
    }
}