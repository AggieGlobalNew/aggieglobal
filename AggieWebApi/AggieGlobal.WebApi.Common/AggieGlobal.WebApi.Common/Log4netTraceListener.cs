using log4net;
using System.Collections.Generic;
using System.Diagnostics;


namespace AggieGlobal.WebApi.Common
{
    public sealed class Log4netTraceListener : TraceListener
    {
        #region Constants
        /// <summary>
        /// Trace message for the ItemsSource timing issue. This line should be ignored.
        /// </summary>
        private const string ItemsSourceTimingIssueTrace = "ContentAlignment; DataItem=null;";
        private readonly string _loggerName = "RollingLogFileAppender";
        #endregion

        #region Variables
        System.Diagnostics.TraceSwitch _generalTraceSwitch;
        #endregion

        #region Constructor & destructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Log4netTraceListener"/> class.
        /// </summary>
        public Log4netTraceListener()
        {
            this._generalTraceSwitch = new System.Diagnostics.TraceSwitch("TraceLevelSwitch", "Entire application");

            this._loggerName = (System.Configuration.ConfigurationManager.AppSettings["log4netLoggerName"]!=null)?System.Configuration.ConfigurationManager.AppSettings["log4netLoggerName"].ToString():string.Empty;
            // Set default values

            this.Name = "Log4net Trace Listener";

            // Create additional trace sources list
            TraceSourceCollection = new List<TraceSource>();

            // Add additional trace sources - .NET framework
            //..
            // Subscribe to all trace sources
            foreach (TraceSource traceSource in TraceSourceCollection)
            {
                traceSource.Listeners.Add(this);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the trace source collection.
        /// </summary>
        /// <value>The trace source collection.</value>
        private List<TraceSource> TraceSourceCollection { get; set; }

        /// <summary>
        /// Gets or sets the active trace type.
        /// </summary>
        public TraceLevel ActiveTraceLevel
        {
            get
            {
                return this._generalTraceSwitch.Level;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Writes trace information, a formatted array of objects and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the <paramref name="args"/> array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            // Call overload
            TraceEvent(eventCache, source, eventType, id, string.Format(format, args));
        }

        /// <summary>
        /// Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            // Should we ignore this line?
            if (message.Contains(ItemsSourceTimingIssueTrace))
                return;
            ILog _log = log4net.LogManager.GetLogger(this._loggerName);
            writeToLogger(_log, eventType, message);
        }

        /// <summary>
        /// Writes text to the output window.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public override void Write(string message)
        {
            // Call write line
            WriteLine(message);
        }

        /// <summary>
        /// Writes a line of text to the output window.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public override void WriteLine(string message)
        {
            // Invoke the event (but only when output is set to verbose)
            if (ActiveTraceLevel == TraceLevel.Verbose)
            {
                ILog _log = log4net.LogManager.GetLogger(this._loggerName);
              
                _log.Debug(message);
            }

        }

        private void writeToLogger(ILog logger, TraceEventType eventType, string message)
        {
            switch (eventType)
            {
                case TraceEventType.Error:
                    if ((ActiveTraceLevel == TraceLevel.Error) ||
                         (ActiveTraceLevel == TraceLevel.Warning) ||
                         (ActiveTraceLevel == TraceLevel.Info) ||
                         (ActiveTraceLevel == TraceLevel.Verbose))
                    {
                        logger.Error(message);
                    }
                    break;

                case TraceEventType.Warning:
                    if ((ActiveTraceLevel == TraceLevel.Warning) ||
                         (ActiveTraceLevel == TraceLevel.Info) ||
                         (ActiveTraceLevel == TraceLevel.Verbose))
                    {
                        logger.Warn(message);
                    }
                    break;

                case TraceEventType.Information:
                    if ((ActiveTraceLevel == TraceLevel.Info) ||
                         (ActiveTraceLevel == TraceLevel.Verbose))
                    {
                        logger.Info(message);
                    }
                    break;

                // Everything else is verbose
                //case TraceEventType.Verbose:
                default:
                    if (ActiveTraceLevel == TraceLevel.Verbose)
                    {
                        logger.Debug(message);
                    }
                    break;
            }
        }
        #endregion
    }
}


