﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Voodoo.Infrastructure;
using Voodoo.Infrastructure.Notations;
#if (!PCL)
namespace Voodoo.Logging
{
    public class FallbackLogger : ILogger
    {
        private static readonly object locker = new object();

        public void Log(Exception ex)
        {
            var log = new StringBuilder();
            log.Append(ex.ToString());
            foreach (var item in ex.Data)
            {
                log.AppendLine("");
                log.AppendLine(item.ToString());
                log.AppendLine(ex.Data[item].ToString());
            }

            Log(ex.ToString(), null);
        }

        public void Log(string log)
        {
            Log(log, null);
        }

        public void Log(string log, string logFilePath)
        {
            var path = string.Empty;
            var appName = getAppName();
            try
            {
                path = getLogFilePath(logFilePath);

                deleteFileIfNeeded(path);

                var text = string.Concat(DateTime.Now.ToString("F"),
                    Environment.NewLine, log, Environment.NewLine,
                    "**********************************************************", Environment.NewLine);
                #if !DNXCORE50
                lock (locker)
                {
                #endif
                    File.AppendAllText(path, text);
#if !DNXCORE50
                }
#endif
            }
            catch (Exception ex)
            {
                handleFileWriteFailure(log, ex, appName, path);
            }
        }
        
        private static void handleFileWriteFailure(string actualError, Exception ex, string appName, string path)
        {
            
#if !DNXCORE50
            var failedToWriteMessage = "Fallback Logger Failed to write log file: " + path;
            var source = appName ?? "Application";
            const string logName = "Application";

            try
            {
                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, logName);
            }
            catch
            {
                source = "ASP.NET 4.0.30319.0";

                var command =
                    string.Format(
                        "eventcreate /ID 1 /L APPLICATION /T INFORMATION  /SO {0} /D \"Event Source Created\"", appName);
                var eventSourceDoesNotExistMessage =
                    string.Concat(
                        "Event source does not exist for this application, you can set v:appName in the config file to customize it and/or run the following command ",
                        Environment.NewLine, command, Environment.NewLine,
                        "You may have to change the /ID parameter if it already exists");
                EventLog.WriteEntry(source, eventSourceDoesNotExistMessage, EventLogEntryType.Warning);
            }

            var actualMessage = string.Format("{0} {1}", actualError, ex);
            if (actualMessage.Length > 32000)
                actualMessage = actualMessage.Substring(0,32000);
            EventLog.WriteEntry(source, failedToWriteMessage, EventLogEntryType.Warning);

            EventLog.WriteEntry(source, actualMessage, EventLogEntryType.Error);
#endif
        }

        private static void deleteFileIfNeeded(string path)
        {
            if (File.Exists(path))
            { 
                var now = DateTime.Now;
                var lastWrite = File.GetLastWriteTime(path);
                if (lastWrite.Year == now.Year && lastWrite.Month == now.Month && lastWrite.Day == now.Day)
                {
                }
                else
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                    File.Delete(path);
                }
            }
        }

        private static string getLogFilePath(string logFilePath)
        {
            var configuredPath = VoodooGlobalConfiguration.LogFilePath;
            if (string.IsNullOrEmpty(logFilePath))
                logFilePath = configuredPath;


            var today = DateTime.Now.DayOfWeek.ToString();

            if (string.IsNullOrEmpty(logFilePath))
            {
                logFilePath = IoNic.IsWebHosted ? IoNic.GetApplicationRootDirectory() : @"c:\Logs" ;
            }
            var fileName = string.Format("log.{0}.txt", today);
            var path = Path.Combine(logFilePath, fileName);
            return path;
        }

        private string getAppName()
        {
            var appName = string.Empty;
            try
            {
                appName = VoodooGlobalConfiguration.ApplicationName;
                if (string.IsNullOrEmpty(appName))
                {
#if !DNXCORE50 && !PCL
                    var assembly = Assembly.GetCallingAssembly() == null
                        ? AppDomain.CurrentDomain.FriendlyName
                        : Assembly.GetCallingAssembly().FullName;
                    appName = assembly.Split(',')[0];
                    appName="Unname App";
#endif
#if DNXCORE50
                     appName="Unnamed DotNetCoreApp";
#endif
#if PCL
                     appName="Unnamed PortableApp";
#endif

                }
            }
            catch
            {
                appName = "Unname App";
            }
            return appName;
        }
    }
}
#endif