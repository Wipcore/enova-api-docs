using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core;
using Wipcore.Core.SystemMonitoring;

namespace Wipcore.Enova.Api.WebApi.Services
{
    /// <summary>
    /// Very basic implementation of a system monitor for Enova.
    /// </summary>
    public class SystemMonitorService : ISystemMonitor
    {
        public static SystemMonitorService Current;

        public SystemMonitorService()
        {
            Current = this;
        }

        public bool IsStarted { get; private set; }
        public dynamic PerformanceCounterData { get; set; }
        public dynamic SystemHealthData { get; set; }

        public dynamic ClusterData { get; set; }


        public WipLog Log { get; set; }
        public string[] PerformanceCounterDataNames { get; set; }
        public string[] SystemHealthDataNames { get; set; }
        public string[] ClusterDataNames { get; set; }

        public void Start()
        {
            IsStarted = true;
        }

        public void Stop()
        {
            IsStarted = false;
        }
    }
}
