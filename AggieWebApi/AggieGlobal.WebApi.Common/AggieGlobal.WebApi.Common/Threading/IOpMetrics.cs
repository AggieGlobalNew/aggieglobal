using System;

namespace AggieGlobal.WebApi.Common.Threading
{
    public interface IOpMetrics
    {
        /// <summary>
        /// Total # of requests executed till now
        /// </summary>
        int RequestCount { get; }

        /// <summary>
        /// Total # of requests which were failed in execution
        /// </summary>
        int FailureCount { get; }

        /// <summary>
        /// Total # of successfully executed requests.
        /// </summary>
        int SuccessCount { get; }

        /// <summary>
        /// Operation Metrics - Success Ratio in percentage
        /// </summary>
        float SuccessRatioInPercent { get; }
    }

}
