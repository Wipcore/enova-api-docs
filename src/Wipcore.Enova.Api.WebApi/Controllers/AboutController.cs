using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Targets;
using Wipcore.Core;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Api.Abstractions.Internal;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("")]
    public class AboutController : EnovaApiController
    {
        private readonly IConfigurationRoot _configurationRoot;

        public AboutController(IConfigurationRoot configurationRoot, EnovaApiControllerDependencies dependencies) : base(dependencies)
        {
            _configurationRoot = configurationRoot;
        }

        /// <summary>
        /// Basic welcome information about the API.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        [HttpGet("/api")]
        [AllowAnonymous]
        public ContentResult About()
        {
            var baseurl = HttpContext.Request.Host;
            var sb = new StringBuilder();

            var online = IsEnovaAlive();
            var color = online ? "green" : "red";
            var msg = online ? "Online" : "Offline";
            var url = baseurl + "/swagger/";

            sb.AppendLine("<html>");
            sb.AppendLine($"<h3>Welcome to Enova API. Version {Startup.ApiVersion}</h3>");
            sb.AppendLine($"<p>Enova is: <span style='color:{color}'>{msg}</span></p>");
            sb.AppendLine($"<p>API documentation: <a href='http://{url}'>{url}</a></p>");
            sb.AppendLine("</html>");
          
            return new ContentResult() {Content = sb.ToString(), ContentType = "text/html"};
        }

        /// <summary>
        /// API version.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/version")]
        [AllowAnonymous]
        public ContentResult Version() => Content(Startup.ApiVersion);
        

        /// <summary>
        /// Returns true if Enova is running.
        /// </summary>
        [HttpGet("IsEnovaAlive")]
        [AllowAnonymous]
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
        [HttpGet("NodeInfo")]
        [HttpGet("api/NodeInfo")]
        [Authorize(Roles = AuthService.AdminRole)]
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
            info.Add("Database", _configurationRoot["Enova:ConnectionString"]);
            info.Add("EnovaLogging", _configurationRoot["Enova:LogPath"]);
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
