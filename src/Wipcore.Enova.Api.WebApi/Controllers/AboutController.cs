using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Targets;
using Wipcore.Core;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Services;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("")]
    public class AboutController : EnovaApiController
    {
        private readonly IConfigurationRoot _configurationRoot;

        public AboutController(IConfigurationRoot configurationRoot, IExceptionService exceptionService) : base(exceptionService)
        {
            _configurationRoot = configurationRoot;
        }

        /// <summary>
        /// Basic welcome information about the API.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        [HttpGet("/api")]
        public ContentResult About()
        {
            var baseurl = HttpContext.Request.Host;
            var sb = new StringBuilder();
            
            sb.AppendLine("Welcome to Enova API. Version " + Startup.ApiVersion);
            sb.AppendLine("Enova is: " + (IsEnovaAlive() ? "Online" : "Offline"));
            sb.AppendLine();
            sb.AppendLine("API documentation: " + baseurl + "/swagger/");
            

            return Content(sb.ToString());
        }

        /// <summary>
        /// Returns true if Enova is running.
        /// </summary>
        [HttpGet("IsEnovaAlive")]
        public bool IsEnovaAlive()
        {
            var heartbeat = DateTime.MinValue;
            foreach (WipSystemMonitorDataItem column in SystemMonitorService.Current.ClusterData)
            {
                if (column.Name != "Heartbeat")
                    continue;
                heartbeat = Convert.ToDateTime(column.Value);
                break;
            }

            return heartbeat > DateTime.UtcNow.AddMinutes(-1);

        }

        /// <summary>
        /// Get information about this Enova node.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = AuthService.AdminRole)]
        [HttpGet("NodeInfo")]
        [HttpGet("api/NodeInfo")]
        public IDictionary<string, object> NodeInfo()
        {
            var info = new Dictionary<string, object>();

            foreach (WipSystemMonitorDataItem column in SystemMonitorService.Current.ClusterData)
            {
                if(column.Name == "HandshakeInfo")
                    continue;
                info.Add(column.Name, column.Value);
            }

            info.Add("IsAlive", IsEnovaAlive());
            info.Add("Database", _configurationRoot.GetValue<String>("Enova:ConnectionString"));
            info.Add("EnovaLogging", _configurationRoot.GetValue<String>("Enova:LogPath"));
            info.Add("NlogLogging", GetNlogLogFile());


            return info;
        }

        private string GetNlogLogFile()
        {
            var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("File");
            var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
            return fileTarget.FileName.Render(logEventInfo)?.Replace('/', '\\');
        }

        /// <summary>
        /// Get monitor information about the Enova system.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = AuthService.AdminRole)]
        [HttpGet("MonitorInfo")]
        public IDictionary<string, object> MonitorInfo()
        {
            var info = new Dictionary<string, object>();

            foreach (WipSystemMonitorDataItem column in SystemMonitorService.Current.PerformanceCounterData)
            {
                info.Add(column.Name, column.Value);
            }

            return info;
        }
    }
}
