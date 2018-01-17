using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Naver.Compass.Service.Update
{
    public enum LogLevel
    {
        Trace = 0,
        Info = 1, 
        Warnning = 2,
        Error = 3,
        Fatal = 4,
    }

    public class Log : IDisposable
    {
        public Log(string logFile, LogLevel logLevel = LogLevel.Trace)
        {
            _logFile = logFile;
            _logLevel = logLevel;

            if(String.IsNullOrEmpty(_logFile) == false)
            {
                try
                {
                    _writer = new StreamWriter(_logFile, false);
                    GrantAccess(_logFile);
                }
                catch
                {
                    _writer = null;
                }
            }
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Flush();
                _writer.Close();
                _writer.Dispose();
                _writer = null;
            }
        }

        public void LogTrace(string format, params object[] args)
        {
            if (_writer != null && _logLevel >= LogLevel.Trace)
            {
                _writer.Write(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:fff") + @" - ");
                _writer.WriteLine(format, args);
            }
        }

        public void LogInfo(string format, params object[] args)
        {
            if (_writer != null && _logLevel >= LogLevel.Info)
            {
                _writer.Write(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:fff") + @" - ");
                _writer.WriteLine(format, args);
            }
        }

        public void LogWarnning(string format, params object[] args)
        {
            if (_writer != null && _logLevel >= LogLevel.Warnning)
            {
                _writer.Write(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:fff") + @" - ");
                _writer.WriteLine(format, args);
            }
        }

        public void LogError(string format, params object[] args)
        {
            if (_writer != null && _logLevel >= LogLevel.Error)
            {
                _writer.Write(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:fff") + @" - ");
                _writer.WriteLine(format, args);
            }
        }

        public void LogFatal(string format, params object[] args)
        {
            if (_writer != null && _logLevel >= LogLevel.Fatal)
            {
                _writer.Write(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:fff") + @" - ");
                _writer.WriteLine(format, args);
            }
        }

        public void Flush()
        {
            if (_writer != null)
            {
                _writer.Flush();
            }
        }

        private bool GrantAccess(string fullPath)
        {
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(fullPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private StreamWriter _writer;
        private LogLevel _logLevel = LogLevel.Trace;
        private string _logFile;
    }
}
