using log4net;
using log4net.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace AggieGlobal.WebApi.Common
{
    public enum Severity
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public delegate void ExceptionLogged(DateTime exceptionRaisedAt, Exception e, string errorMessage, string[] exceptionDetail);

    public static class AggieGlobalLogManager
    {
        private static readonly ILog log = log4net.LogManager.GetLogger("RollingLogFileAppender");
        private static ExceptionLogged _onExceptionLogged;

        static AggieGlobalLogManager()
        {
            XmlConfigurator.Configure();
        }

        #region Helpers - for Quick Calls
        public static void SolrInfo(string messageFormat, params object[] args)
        {
                string message = safeFormatString(messageFormat, args);
                writeToLog(message, Severity.Info);
        }

        public static void Info(string messageFormat, params object[] args)
        {
            if (ConfigurationManager.AppSettings["EnableInfoLog"] != null && ConfigurationManager.AppSettings["EnableInfoLog"].ToString() == "true")
            {
                string message = safeFormatString(messageFormat, args);
                writeToLog(message, Severity.Info);
            }
        }

        public static void Debug(Exception e, string messageFormat, params object[] args)
        {
            if (ConfigurationManager.AppSettings["EnableDebugLog"] != null && ConfigurationManager.AppSettings["EnableDebugLog"].ToString() == "true")
            {
                string message = safeFormatString(messageFormat, args);
                Fatal(message, Severity.Debug);
                logExceptionDetail(e, (x) => writeToLog(x, Severity.Debug));
            }
        }

        public static void Debug(string messageFormat, params object[] args)
        {
            if (ConfigurationManager.AppSettings["EnableDebugLog"] != null && ConfigurationManager.AppSettings["EnableDebugLog"].ToString() == "true")
            {
                string message = safeFormatString(messageFormat, args);
                writeToLog(message, Severity.Debug);
            }
        }

        public static void Warn(Exception e, string messageFormat, params object[] args)
        {
            string message = safeFormatString(messageFormat, args);
            Warn(message, Severity.Warn);
            logExceptionDetail(e, (x) => writeToLog(x, Severity.Warn));
        }

        public static void Warn(string messageFormat, params object[] args)
        {
            string message = safeFormatString(messageFormat, args);
            writeToLog(message, Severity.Warn);
        }

        public static void Fatal(string messageFormat, params object[] args)
        {
            string message = safeFormatString(messageFormat, args);
            writeToLog(message, Severity.Fatal);

            //updateListeners(DateTime.Now, message);
        }

        public static void Fatal(Exception e, string messageFormat, params object[] args)
        {
            string message = safeFormatString(messageFormat, args);
            Fatal(message, Severity.Fatal);
            List<string> exceptionDetail = new List<string>();
            logAllExceptions(e, (x) =>
            {
                writeToLog(x, Severity.Fatal);
                exceptionDetail.Add(x);
            });
            updateListeners(e, DateTime.Now, message, exceptionDetail.ToArray());
        }
        #endregion

        public static event ExceptionLogged OnExceptionLogged
        {
            add
            {
                _onExceptionLogged += value;
            }
            remove
            {
                _onExceptionLogged -= value;
            }
        }

        #region Helpers to write Exception Detail

        private static void logAllExceptions(Exception e, Action<string> logger)
        {
            logger("Exception Detail...");
            while (e != null)
            {
                logExceptionDetail(e, logger);
                e = e.InnerException;
            }
        }

        private static void logExceptionDetail(Exception e, Action<string> logger)
        {
            logger(safeFormatString("Type: {0}", e.GetType().FullName));
            logger(safeFormatString("Message: {0}", e.Message));
            logger(safeFormatString("Source: {0}", e.Source));
            logger(safeFormatString("TargetSite: {0}", e.TargetSite));
            //Write Exception Data
            logger(safeFormatString("Exception Data - {0}# of elements", e.Data.Count));
            foreach (DictionaryEntry entry in e.Data)
            {
                logger(safeFormatString("{0}: {1}", entry.Key, entry.Value));
            }
            logger(safeFormatString("StackTrace: {0}", e.StackTrace));
        }

        private static void writeToLog(string message, Severity severity)
        {
            switch (severity)
            {
                case Severity.Info:
                    log.Info(message);
                    break;
                case Severity.Warn:
                    log.Warn(message);
                    break;
                case Severity.Debug:
                    log.Debug(message);
                    break;
                case Severity.Error:
                    log.Error(message);
                    break;
                case Severity.Fatal:
                    log.Fatal(message);
                    break;
            }
        }

        private static void updateListeners(Exception e, DateTime exceptionRaisedAt, string errorMessage, string[] exceptionDetail)
        {
            if (_onExceptionLogged == null)
                return;

            _onExceptionLogged(exceptionRaisedAt, e, errorMessage, exceptionDetail);
        }

        private static void updateListeners(DateTime exceptionRaisedAt, string errorMessage)
        {
            if (_onExceptionLogged == null)
                return;

            _onExceptionLogged(exceptionRaisedAt, null, errorMessage, null);
        }

        private static string safeFormatString(string messageFormat, params object[] args)
        {
            if (args == null || args.Length <= 0)
            {
                return messageFormat;
            }

            try
            {
                return string.Format(messageFormat, args);
            }
            catch (FormatException e)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder("Failed to prepare formatted string with Format Specification: [");
                sb.Append(messageFormat);
                sb.Append("] Arguments: [");
                sb.Append(string.Join(",", args));
                sb.Append("] Exception Handled: [");
                sb.Append(e.Message);
                //sb.Append(e.StackTrace);
                sb.Append("]");

                return sb.ToString();
            }
        }
        #endregion
    }
}