using System;

namespace AggieGlobal.WebApi.Common.Threading
{
    /// <summary>
    /// Fetches request in one thread & processes them in async manner in N# of threads, to generate result
    /// following Producer-Consumer Pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProducerConsumerFramework<T> : IDisposable, IOpMetrics
    {
        /// <summary>
        /// Wait for Completion of all tasks upon calling of Stop method
        /// </summary>
        void WaitForCompletion();

        /// <summary>
        /// Starts Producer-Consumer Process
        /// </summary>
        /// <returns>true if successful; else false, when it's already running</returns>
        bool Start();

        /// <summary>
        /// Stops Producer-Consumer Process
        /// </summary>
        void Stop();

        /// <summary>
        /// Dumps Internal State Info to Application Log
        /// </summary>
        void LogInternalState();

        /// <summary>
        /// Control logging of Operation Metrics Info
        /// </summary>
        bool EnableOperationMetricsLogging { get; set; }
    }
}
