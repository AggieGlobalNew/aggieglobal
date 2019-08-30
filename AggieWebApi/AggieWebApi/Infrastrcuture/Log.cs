//using AggieGlobal.WebApi.DataAccess.Common;
//using log4net;
//using log4net.Config;
//using Newtonsoft.Json;
//using System;
//using System.Collections;
//using System.Collections.Generic;

//namespace AggieGlobal.WebApi.Infrastructure
//{
//    public static class AggieGlobalLogManager
//    {
//        private static readonly ILog log = LogManager.GetLogger(typeof(AggieGlobalLogManager).ToString());
//        private static readonly ILog log = LogManager.GetLogger("SKYSITEWebApiLogger");
//        private static readonly ILog log = LogManager.GetLogger("RollingLogFileAppender");

//        static AggieGlobalLogManager()
//        {
//            // if log4net randomly stops logging - http://stackoverflow.com/questions/2263301/log4net-randomly-stops-logging
//            // process safety tips - http://hectorcorrea.com/blog/Log4net-Thread-Safe-but-not-Process-Safe

//            // use configure and watch
//            //XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(AggieGlobal.WebApi.Infrastructure.Utility.CurrentDirectory, "Web.config")));
//            XmlConfigurator.Configure();
//        }

//        public static void WriteLog(string message, Severity severity = Severity.Info)
//        {
//            writeToLog(message, severity);
//        }

//        #region Helpers - for Quick Calls
//        public static void Info(string messageFormat, params object[] args)
//        {
//            string message = string.Format(messageFormat, args);
//            writeToLog(message, Severity.Info);
//        }

//        public static void Debug(string messageFormat, params object[] args)
//        {
//            string message = string.Format(messageFormat, args);
//            writeToLog(message, Severity.Debug);
//        }

//        public static void Warn(Exception e, string messageFormat, params object[] args)
//        {
//            string message = string.Format(messageFormat, args);
//            Fatal(message, Severity.Warn);
//            logExceptionDetail(e, (x) => writeToLog(x, Severity.Warn));
//        }

//        public static void Warn(string messageFormat, params object[] args)
//        {
//            string message = string.Format(messageFormat, args);
//            writeToLog(message, Severity.Warn);
//        }

//        public static void Fatal(string messageFormat, params object[] args)
//        {
//            string message = string.Format(messageFormat, args);
//            writeToLog(message, Severity.Fatal);
//        }

//        public static void Fatal(Exception e, string messageFormat, params object[] args)
//        {
//            string message = string.Format(messageFormat, args);
//            Fatal(message, Severity.Fatal);
//            logAllExceptions(e, (x) => writeToLog(x, Severity.Fatal));
//        }
//        #endregion

//        #region Helpers to write Exception Detail by implementing 'IObjectRenderer' - if no ready-made class is available in log4Net logger

//        private static void logAllExceptions(Exception e, Action<string> logger)
//        {
//            logger(string.Format("Exception Detail..."));
//            while (e != null)
//            {
//                logExceptionDetail(e, logger);
//                e = e.InnerException;
//            }
//        }

//        private static void logExceptionDetail(Exception e, Action<string> logger)
//        {
//            logger(string.Format("Type: {0}", e.GetType().FullName));
//            logger(string.Format("Message: {0}", e.Message));
//            logger(string.Format("Source: {0}", e.Source));
//            logger(string.Format("TargetSite: {0}", e.TargetSite));
//            //Write Exception Data
//            logger(string.Format("Exception Data - {0}# of elements", e.Data.Count));
//            foreach (DictionaryEntry entry in e.Data)
//            {
//                logger(string.Format("{0}: {1}", entry.Key, entry.Value));
//            }
//            logger(string.Format("StackTrace: {0}", e.StackTrace));
//        }


//        private static void writeToLog(string message, Severity severity)
//        {
//            switch (severity)
//            {
//                case Severity.Info:
//                    log.Info(message);
//                    break;
//                case Severity.Warn:
//                    log.Warn(message);
//                    break;
//                case Severity.Debug:
//                    log.Debug(message);
//                    break;
//                case Severity.Error:
//                    log.Error(message);
//                    break;
//                case Severity.Fatal:
//                    log.Fatal(message);
//                    break;
//            }
//        }
//        #endregion

//        public static IEnumerable<Handler.WebApiLog> GetFileAppenderLogs(string type, int last, string ip)
//        {
//            var appender = LogManager.GetAllRepositories()[0].GetAppenders()[0] as log4net.Appender.RollingFileAppender;
//            var path = appender != null ? appender.File : string.Empty;

//            var logs = new List<WebApiLog>();

//            foreach (var line in new ReverseLineReader(path))
//            {
//                if (string.IsNullOrWhiteSpace(ip))
//                {
//                    if (line.StartsWith("{\"Uri\"") && type.ToLower().Contains("request"))
//                        logs.Add(JsonConvert.DeserializeObject<WebApiUsageRequest>(line));
//                    else if (line.Contains("{\"ContentType\"") && type.ToLower().Contains("response"))
//                        logs.Add(JsonConvert.DeserializeObject<WebApiUsageResponse>(line));
//                    else if (line.StartsWith("{\"Type\"") && type.ToLower().Contains("general"))
//                        logs.Add(JsonConvert.DeserializeObject<GeneralLog>(line));
//                }
//                else
//                {
//                    if (line.StartsWith("{\"Uri\"") && type.ToLower().Contains("request"))
//                    {
//                        var request = JsonConvert.DeserializeObject<WebApiUsageRequest>(line);
//                        if (request.IP.Contains(ip)) logs.Add(request);
//                    }
//                }

//                if (logs.Count >= last) break;
//            }

//            return logs;
//        }

//        public static IEnumerable<Handler.WebApiLog> GetAdoAppenderLogs(string type, int last, string ip)
//        {
//            var query = string.Format("SELECT TOP {0} [Message] FROM MI_ServiceLog WHERE LogType IN ('{1}') {2} ORDER BY [CreateDate] DESC", last, string.Join("','", type.ToLower().Split(',')), string.IsNullOrEmpty(ip) ? string.Empty : " AND IpAddress = " + ip);
//            var results = new DataRepository("DeviceManagerDB").GetQueryResult(query);

//            var logs = new List<WebApiLog>();

//            foreach (var result in results)
//            {
//                var line = result.Message;

//                try
//                {
//                    if (line.StartsWith("{\"Uri\"") && type.ToLower().Contains("request"))
//                        logs.Add(JsonConvert.DeserializeObject<WebApiUsageRequest>(line));
//                    else if (line.Contains("{\"ContentType\"") && type.ToLower().Contains("response"))
//                        logs.Add(JsonConvert.DeserializeObject<WebApiUsageResponse>(line));
//                    else if (line.StartsWith("{\"Type\"") && type.ToLower().Contains("general"))
//                        logs.Add(JsonConvert.DeserializeObject<GeneralLog>(line));
//                }
//                catch
//                {
//                    var log = new GeneralLog(typeof(GeneralLog), line);
//                    logs.Add(log);
//                }
//            }

//            return logs;
//        }
//    }

//    public enum Severity
//    {
//        Debug,
//        Info,
//        Warn,
//        Error,
//        Fatal
//    }
//}