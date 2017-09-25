using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using Wipcore.Library;

namespace Wipcore.Enova.Api.WebApi.Services
{
    /// <summary>
    /// A target for nlog to log to the memory, for quick reading without file buffering.
    /// </summary>
    [Target("array")]
    public class ArrayNlogTarget : TargetWithLayout
    {
        public ArrayNlogTarget()
        {
            MaxCount = 100;
        }
        private static readonly Dictionary<string, LinkedList<LogEventInfo>> LogByName = new Dictionary<string, LinkedList<LogEventInfo>>();
        private static readonly LinkedList<LogEventInfo> UnnamedLog = new LinkedList<LogEventInfo>();
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
        public int MaxCount { get; set; }


        public static IEnumerable<LogEventInfo> GetLog()
        {
            using (Lock.AcquireReadLock())
            {
                return UnnamedLog;
            }
        }

        public static IEnumerable<LogEventInfo> GetLog(string logName)
        {
            using (Lock.AcquireReadLock())
            {
                return LogByName.ContainsKey(logName) ? LogByName[logName] : new LinkedList<LogEventInfo>();
            }
        }
        public static void Clear(string logName = null)
        {
            using (Lock.AcquireWriteLock())
            {
                if (String.IsNullOrEmpty(logName))
                    UnnamedLog.Clear();
                else if (LogByName.ContainsKey(logName))
                    LogByName[logName].Clear();
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);
            logEvent.Message = logMessage;
            using (Lock.AcquireWriteLock())
            {
                var key = logEvent.LoggerName;

                if (!String.IsNullOrEmpty(key))
                {
                    if (!LogByName.ContainsKey(key))
                        LogByName[key] = new LinkedList<LogEventInfo>();
                    var namedLog = LogByName[key];
                    namedLog.AddLast(logEvent);
                    if (namedLog.Count > MaxCount)
                        namedLog.RemoveFirst();
                }

                UnnamedLog.AddLast(logEvent);

                if (UnnamedLog.Count > MaxCount)
                    UnnamedLog.RemoveFirst();
            }
        }
    }
}
