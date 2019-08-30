using System;
using System.Diagnostics;

namespace AggieGlobal.WebApi.Common
{
    public sealed class ExecutionTimeLogger : Disposable
    {
        #region Data Types
        #endregion

        #region Member variales
        private readonly Stopwatch _stopwatch = null;
        private readonly string _operationName = null;
        #endregion

        #region Constructors
        public ExecutionTimeLogger(string operationName)
        {
            _operationName = operationName;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }
        public ExecutionTimeLogger(Action task, string operationName)
        {
            _stopwatch = new Stopwatch();
            _operationName = operationName;

            _stopwatch.Start();
            task();
            _stopwatch.Stop();

            Trace.TraceInformation("Time Taken: {0:0.000}ms. for Operation:\"{1}\".", _stopwatch.ElapsedMilliseconds, _operationName);
            _stopwatch = null;
        }
        #endregion

        #region Properties

        #endregion

        #region Overrides
        protected override void doCleanup()
        {
            if (_stopwatch == null)
            {
                return;
            }
            _stopwatch.Stop();
            Trace.TraceInformation("Time Taken: {0:0.000}ms. for Operation:\"{1}\".", _stopwatch.ElapsedMilliseconds, _operationName);
        }
        #endregion
    }

}