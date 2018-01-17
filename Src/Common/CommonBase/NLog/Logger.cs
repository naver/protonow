using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Naver.Compass.Common.CommonBase
{
    public class NLogger
    {
        /// <summary>
        /// trace,debug(log),info,warn,error,fatal,off
        /// Level 0
        /// </summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="arg">The object to format. </param>
        public static void Trace(string format, params Object[] arg)
        {
            var logger = CreateLogger();
            var message = CreateMessage(format, arg);
            //var fullMessage = string.Format("[{0}][ThreadId:{1}][{2}][{3}] {4}", DateTime.Now.ToString(), GetThreadId(), "Trace", logger.Name, message);
            //System.Diagnostics.Debug.WriteLine(fullMessage);
            logger.Trace(message);
        }

        /// <summary>
        /// trace,DEBUG(LOG),info,warn,error,fatal,off
        /// Level 1
        /// </summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="arg">The object to format. </param>
        public static void Debug(string format, params Object[] arg)
        {
            var logger = CreateLogger();
            var message = CreateMessage(format, arg);
            //var fullMessage = string.Format("[{0}][ThreadId:{1}][{2}][{3}] {4}", DateTime.Now.ToString(), GetThreadId(), "Debug", logger.Name, message);
            //System.Diagnostics.Debug.WriteLine(fullMessage);
            logger.Debug(message);
        }

        /// <summary>
        /// trace,debug(log),INFO,warn,error,fatal,off
        /// Level 2
        /// </summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="arg">The object to format. </param>
        public static void Info(string format, params Object[] arg)
        {
            var logger = CreateLogger();
            var message = CreateMessage(format, arg);
            //var fullMessage = string.Format("[{0}][ThreadId:{1}][{2}][{3}] {4}", DateTime.Now.ToString(), GetThreadId(), "Info", logger.Name, message);
            //System.Diagnostics.Debug.WriteLine(fullMessage);
            logger.Info(message);
        }

        /// <summary>
        /// trace,debug(log),info,WARN,error,fatal,off
        /// Level 3
        /// </summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="arg">The object to format. </param>
        public static void Warn(string format, params Object[] arg)
        {
            var logger = CreateLogger();
            var message = CreateMessage(format, arg);
            //var fullMessage = string.Format("[{0}][ThreadId:{1}][{2}][{3}] {4}", DateTime.Now.ToString(), GetThreadId(), "Warn", logger.Name, message);
            //System.Diagnostics.Debug.WriteLine(fullMessage);
            logger.Warn(message);
        }

        /// <summary>
        /// trace,debug(log),info,warn,ERROR,fatal,off
        /// Level 4
        /// </summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="arg">The object to format. </param>
        public static void Error(string format, params Object[] arg)
        {
            var logger = CreateLogger();
            var message = CreateMessage(format, arg);
            //var fullMessage = string.Format("[{0}][ThreadId:{1}][{2}][{3}] {4}", DateTime.Now.ToString(), GetThreadId(), "Error", logger.Name, message);
            //System.Diagnostics.Debug.WriteLine(fullMessage);
            logger.Error(message);
        }

        /// <summary>
        /// trace,debug(log),info,warn,error,FATAL,off
        /// Level 5
        /// </summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="arg">The object to format. </param>
        public static void Fatal(string format, params Object[] arg)
        {
            var logger = CreateLogger();
            var message = CreateMessage(format, arg);
            //var fullMessage = string.Format("[{0}][ThreadId:{1}][{2}][{3}] {4}", DateTime.Now.ToString(), GetThreadId(), "Fatal", logger.Name, message);
            //System.Diagnostics.Debug.WriteLine(fullMessage);
            logger.Fatal(message);
        }

        /// <summary>
        /// trace,debug(log),info,warn,error,fatal,OFF
        /// Level 6
        /// </summary>
        /// <param name="format">A composite format string. </param>
        /// <param name="arg">The object to format. </param>
        public static void Off(string format, params Object[] arg)
        {
            var logger = CreateLogger();
            var message = CreateMessage(format, arg);
            //var fullMessage = string.Format("[{0}][ThreadId:{1}][{2}][{3}] {4}", DateTime.Now.ToString(), GetThreadId(), "Fatal", logger.Name, message);
            //System.Diagnostics.Debug.WriteLine(fullMessage);
            logger.Fatal(message);
        }

        private static string CreateMessage(string format, params Object[] arg)
        {
            try
            {
                return string.Format(format, arg);
            }
            catch
            {
                return format;
            }
        }

        private static Logger CreateLogger()
        {
            StackTrace stackTrace = new StackTrace();
            return LogManager.GetLogger(stackTrace.GetFrame(2).GetMethod().DeclaringType.FullName);
        }
    }
}
