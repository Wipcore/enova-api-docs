using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.WebEncoders;
using Wipcore.Core;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi;
using Wipcore.Enova.Api.WebApi.Helpers;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("")]
    public class AboutController : Controller
    {
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
            sb.AppendLine("API documentation: " + baseurl + "/swagger/ui");
            

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

            return info;
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
