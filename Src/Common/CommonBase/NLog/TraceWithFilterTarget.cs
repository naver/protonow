using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Naver.Compass.Common.CommonBase.NLog
{
    [Target("TraceWithFilter")]
    public class TraceWithFilterTarget : TargetWithLayout
    {
        private Regex _filterRegex;
        private string _filter;

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        this._filterRegex = new Regex(value);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Filter is a invalid regex string.{0}", ex.ToString());
                    }
                }
            }
        }

        protected override void Write(global::NLog.LogEventInfo logEvent)
        {
#if DEBUG
            var message = Layout.Render(logEvent);
            if (_filterRegex == null || (_filterRegex != null && _filterRegex.Match(message).Success))
            {
                this.WriteToTrace(logEvent.Level, message);
            }
#endif
        }

        private void WriteToTrace(global::NLog.LogLevel logLevel, string message)
        {
            if (logLevel <= LogLevel.Debug)
            {
                Trace.WriteLine(message);
            }
            else if (logLevel == LogLevel.Info)
            {
                Trace.TraceInformation(message);
            }
            else if (logLevel == LogLevel.Warn)
            {
                Trace.TraceWarning(message);
            }
            else if (logLevel == LogLevel.Error)
            {
                Trace.TraceError(message);
            }
            else if (logLevel >= LogLevel.Fatal)
            {
                Trace.Fail(message);
            }
            else
            {
                Trace.WriteLine(message);
            }
        }
    }
}
