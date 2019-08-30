using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AggieGlobal.WebApi.Common.Threading
{
    /// <summary>
    /// Implements Producer-Consumer Pattern to fetch request for processing & generate result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ProducerConsumerFramework<T> : Disposable, IProducerConsumerFramework<T>
    {
        #region Member Variables
        private const int _timeOutForBlockingCollectionOpInMS = 250;
        private const int _waitTimeBeforeFetchingNextRequest = 1000;
        private const int _waitTimeBeforeTakingNextRequest = 1000;
        private const int _retryIntervalForPreConditionTaskInMS = 1000 * 30 * 1;//30s Interval
#if DEBUG
        private const int _frequencyOfShowingOpMetricsInSeconds = 45;//Every 45th second
#else
        private const int _frequencyOfShowingOpMetricsInSeconds = 120;//Every 2 Minutes
#endif

        private CancellationTokenSource _cancellationTokenForStartUpJob;
        private Task _initializationTask;
        private CancellationTokenSource _cancellationTokenForManager;
        private bool _isPreConditionTaskExecSuccessful;
        private readonly Func<bool> _preConditionTaskBeforeInitialization;
        private readonly Func<int, T[]> _requestFetcher;
        private readonly Func<T, bool> _requestExecutor;
        private readonly int _concurrencyThreshold;
        private BlockingCollection<T> _requests;
        private List<Task> _allTasks;
        private int _totalCountOfRequestsExecuted;
        private int _totalCountOfFailedExecution;
        private readonly object _lockForOpMetrics;
        private readonly Stopwatch _stopwatchForOperationMetricsLogging;
        private readonly Action _serviceStopper = null;
        private object _lockerActiveThreadCount;
        private int _activeThreadCount;
        private bool _isOperationMetricsLoggingEnabled;
        #endregion

        #region Constructor(s)
        public ProducerConsumerFramework(Func<int, T[]> requestFetcher, Func<T, bool> requestExecutor, int concurrencyThreshold)
            : this(null, requestFetcher, requestExecutor, concurrencyThreshold, null)
        {
        }

        public ProducerConsumerFramework(Func<bool> preConditionTaskBeforeInitialization, Func<int, T[]> requestFetcher, Func<T, bool> requestExecutor, int concurrencyThreshold)
            : this(preConditionTaskBeforeInitialization, requestFetcher, requestExecutor, concurrencyThreshold, null)
        {
        }

        public ProducerConsumerFramework(Func<bool> preConditionTaskBeforeInitialization, Func<int, T[]> requestFetcher, Func<T, bool> requestExecutor, int concurrencyThreshold, Action serviceStopper)
        {
            _totalCountOfRequestsExecuted = 0;
            _totalCountOfFailedExecution = 0;
            _preConditionTaskBeforeInitialization = preConditionTaskBeforeInitialization;
            _requestFetcher = requestFetcher;
            _requestExecutor = requestExecutor;
            _concurrencyThreshold = concurrencyThreshold;
            _requests = new BlockingCollection<T>(_concurrencyThreshold);
            _initializationTask = null;
            _cancellationTokenForStartUpJob = null;
            _lockForOpMetrics = new object();

            _stopwatchForOperationMetricsLogging = new Stopwatch();
            _stopwatchForOperationMetricsLogging.Start();

            _isPreConditionTaskExecSuccessful = false;
            _serviceStopper = serviceStopper;

            _lockerActiveThreadCount = new object();
            _activeThreadCount = 0;

            _isOperationMetricsLoggingEnabled = true;
            Trace.TraceInformation("Instantiated 'Producer-Consumer-Framework' [Hash:{0}], Concurrency:{1}", this.GetHashCode(), _concurrencyThreshold);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Dumps Internal State Info to Application Log
        /// </summary>
        public void LogInternalState()
        {
            Trace.TraceInformation("============================= PCF =============================");
            int activeThreadCount = -1;
            lock (_lockerActiveThreadCount)
            {
                activeThreadCount = _activeThreadCount;
            }
            Trace.TraceInformation("# of Requests in Queue:{0}, Active Thread Count:{1}", _requests.Count, activeThreadCount);
            Trace.TraceInformation("Dumping state of all threads owned by PCF (Hash:{0})", GetHashCode());
            Trace.TraceInformation("ID\tIsCompleted\tIsCanceled\tIsFaulted\tStatus\tException");
            foreach (var t in _allTasks)
            {
                Trace.TraceInformation(string.Join("\t", t.Id, t.IsCompleted, t.IsCanceled, t.IsFaulted, t.Status, t.Exception.Message));
            }
            Trace.TraceInformation("============================= --- =============================");
        }

        #region IOpMetrics
        public int RequestCount
        {
            get
            {
                lock (_lockForOpMetrics)
                {
                    return _totalCountOfRequestsExecuted;
                }
            }
        }

        public int FailureCount
        {
            get
            {
                lock (_lockForOpMetrics)
                {
                    return _totalCountOfFailedExecution;
                }
            }
        }

        /// <summary>
        /// Operation Metrics - Success Ratio in percentage
        /// </summary>
        public int SuccessCount
        {
            get
            {
                int totalCount = this.RequestCount;
                int failCount = this.FailureCount;
                return (totalCount - failCount);
            }
        }

        public float SuccessRatioInPercent
        {
            get
            {
                float totalCount = this.RequestCount;
                float failCount = this.FailureCount;

                if (totalCount <= 0.0)
                    return 0.0f;

                return (100.0f * (totalCount - failCount)) / totalCount;
            }
        }
        #endregion

        public bool EnableOperationMetricsLogging
        {
            get
            {
                return _isOperationMetricsLoggingEnabled;
            }
            set
            {
                _isOperationMetricsLoggingEnabled = value;
            }
        }

        public void WaitForCompletion()
        {
            wait(_cancellationTokenForStartUpJob);
            wait(_cancellationTokenForManager);
        }

        /// <summary>
        /// Starts Producer-Consumer Process
        /// </summary>
        /// <returns>true if successful; else false, when it's already running</returns>
        public bool Start()
        {
            if (_cancellationTokenForManager != null)
            {
                Trace.TraceError("'Producer-Consumer Framework' [Hash:{0}] is already running!", this.GetHashCode());
                return false;
            }
            if (_preConditionTaskBeforeInitialization != null)
            {
                if (_cancellationTokenForStartUpJob != null || _initializationTask != null)
                {
                    Trace.TraceError("Pre-Condition Task for 'Producer-Consumer Framework' [Hash:{0}] is already running!", this.GetHashCode());
                    return false;
                }

                _cancellationTokenForStartUpJob = new CancellationTokenSource();
                _initializationTask = Task.Factory.StartNew(() =>
                    {
                        Trace.TraceInformation("Executing 'Pre-Condition Task' [Retry Frequency:{0}ms] for 'Producer-Consumer-Framework' [Hash:{1}, Concurrency:{2}]", _retryIntervalForPreConditionTaskInMS, this.GetHashCode(), _concurrencyThreshold);
                        _isPreConditionTaskExecSuccessful = executePreConditionTaskTillSuccessful(_cancellationTokenForStartUpJob, _preConditionTaskBeforeInitialization);
                        if (_isPreConditionTaskExecSuccessful)
                        {
                            Trace.TraceInformation("Pre-Condition Task for 'Producer-Consumer Framework' [Hash:{0}] executed successfully!", this.GetHashCode());
                            return;
                        }
                        Trace.TraceError("Execution of Pre-Condition Task for 'Producer-Consumer Framework' [Hash:{0}] encountered error!", this.GetHashCode());
                        if (_serviceStopper != null)
                        {
                            _serviceStopper();
                        }
                    }
                );
                _initializationTask.ContinueWith((x) =>
                    {
                        if (_isPreConditionTaskExecSuccessful && !_cancellationTokenForStartUpJob.IsCancellationRequested)
                        {
                            Trace.TraceInformation("'Producer-Consumer Framework' [Hash:{0}] is being set-up post execution of Pre-Condition Task.", this.GetHashCode());
                            initializeProducerAndConsumer();
                            Trace.TraceInformation("'Producer-Consumer Framework' [Hash:{0}] is ready to serve requests.", this.GetHashCode());
                        }
                        _cancellationTokenForStartUpJob = null;
                    });
                return true;
            }
            return initializeProducerAndConsumer();
        }

        /// <summary>
        /// Stops Producer-Consumer Process
        /// </summary>
        public void Stop()
        {
            if (_cancellationTokenForStartUpJob != null)
            {
                Trace.TraceInformation("Stopping Pre-Condition Task for'Producer-Consumer-Framework' [Hash:{0}]...", this.GetHashCode());
                _cancellationTokenForStartUpJob.Cancel();
                return;
            }

            if (_cancellationTokenForManager != null)
            {
                Trace.TraceInformation("Stopping 'Producer-Consumer-Framework' Execution [Hash:{0}], Concurrency:{1}", this.GetHashCode(), _concurrencyThreshold);
                _cancellationTokenForManager.Cancel();
                SafeDispose<CancellationTokenSource>(ref _cancellationTokenForManager);
                Thread.Sleep(_waitTimeBeforeFetchingNextRequest);
                Trace.TraceInformation("'Producer-Consumer Framework' [Hash:{0}] has been stopped!", this.GetHashCode());
                return;
            }
            Trace.TraceWarning("'Producer-Consumer Framework' [Hash:{0}] is not in running state, so can't be terminated!", this.GetHashCode());
        }
        #endregion

        #region Private Methods
        public bool initializeProducerAndConsumer()
        {
            if (_cancellationTokenForManager != null)
            {
                Trace.TraceError("'Producer-Consumer Framework' [Hash:{0}] is already running!", this.GetHashCode());
                return false;
            }
            Task t = null;

            _cancellationTokenForManager = new CancellationTokenSource();
            _allTasks = new List<Task>(_concurrencyThreshold + 1);
            // Start the ONE & ONLY producer
            t = Task.Factory.StartNew(() => fetchRequests(_requests, _cancellationTokenForManager.Token));
            t.ContinueWith(cleanUpUponThreadExecutionCompletion);
            _allTasks.Add(t);

            // Start N # of consumers
            for (int i = 0; i < _concurrencyThreshold; i++)
            {
                t = Task.Factory.StartNew(() => executeNextRequest(_requests, _cancellationTokenForManager.Token));
                t.ContinueWith(cleanUpUponThreadExecutionCompletion);
                _allTasks.Add(t);
            }
            return true;
        }

        private void cleanUpUponThreadExecutionCompletion(Task task)
        {
            string reason = (task.IsCompleted) ? "Normal" : "Termination";
            //Debug.Assert(task.IsCompleted, "'Producer-Consumer Framework' terminated!");
            Trace.TraceWarning("'Producer-Consumer Framework' [Hash:{0}] Execution terminated! Reason:{1}", this.GetHashCode(), reason);
        }

        private void updateOperationMetrics(bool isLastRequestExecSuccessfully)
        {
            lock (_lockForOpMetrics)
            {
                if (_totalCountOfRequestsExecuted == int.MaxValue)
                {
                    Trace.TraceWarning("Resetting counter(s) for operation metrics [Current Success Ratio in Percent:{0.00}]", SuccessRatioInPercent);
                    _totalCountOfRequestsExecuted = 0;
                    _totalCountOfFailedExecution = 0;
                }
                _totalCountOfRequestsExecuted++;
                if (!isLastRequestExecSuccessfully)
                {
                    _totalCountOfFailedExecution++;
                }
            }
        }

        private bool safeExecuteRequest(T request)
        {
            try
            {
                Trace.TraceInformation("Executing request:[{0}]...", request);
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                //
                bool result = this._requestExecutor(request);

                updateOperationMetrics(result);//Update Operation Metrics

                stopWatch.Stop();
                if (result)
                {
                    Trace.TraceInformation("Executed request:[{0}] successfully; Time Taken:{1}ms.", request, stopWatch.Elapsed.TotalMilliseconds);
                    return true;
                }
                Trace.TraceError("Execution Failed for request:[{0}]!", request);
            }
            catch (Exception e)
            {
                Trace.TraceError("Execution of request:[{0}] failed! Exception Caught:{1}", request, e.Message);
                Trace.TraceError(e.StackTrace);
            }
            return false;
        }

        private void executeNextRequest(BlockingCollection<T> bc, CancellationToken ct)
        {
            while (!bc.IsCompleted)
            {
                try
                {
                    if (bc.Count <= 0)
                    {
                        Thread.Sleep(_waitTimeBeforeTakingNextRequest);
                        continue;
                    }
                    T nextRequest;
                    if (!bc.TryTake(out nextRequest, _timeOutForBlockingCollectionOpInMS, ct))
                    {
                        Trace.TraceWarning("Failed to dequeue next request, Current Request Count:{0}!", bc.Count);
                        continue;
                    }
                    //
                    using (AutoExecuteStartFinishTasks ae = new AutoExecuteStartFinishTasks(() => { lock (_lockerActiveThreadCount) { _activeThreadCount++; } }, () => { lock (_lockerActiveThreadCount) { _activeThreadCount--; } }))
                    {
                        safeExecuteRequest(nextRequest);
                    }
                }
                catch (OperationCanceledException)
                {
                    Trace.TraceInformation("'Request Fetching Process' has been terminated, terminating 'Request Execution Process', Current Request Count:{0}!", bc.Count);
                    break;
                }
            }
        }

        private bool enqueueRequests(BlockingCollection<T> bc, T[] requests, CancellationToken ct)
        {
            try
            {
                foreach (var request in requests)
                {
                    bool success = bc.TryAdd(request, _timeOutForBlockingCollectionOpInMS, ct);
                    if (!success)
                    {
                        Trace.TraceError("PCF: Failed to Queue new request:[{0}] [Current Request Count:{1}; Bounded Capacity:{2}]!", request, bc.Count, bc.BoundedCapacity);
                        //Debug.Assert(success, string.Format("Failed to Queue new request:{0}!", request));
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                bc.CompleteAdding();
                Trace.TraceInformation("'Request Fetching Process' has been terminated, terminating 'Request Execution Process', Current Request Count:{0}!", bc.Count);
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

        private void fetchRequests(BlockingCollection<T> bc, CancellationToken ct)
        {
            while (true)
            {
                int countOfAvailableSlots = bc.BoundedCapacity - (this.CountOfActiveCosnumers + bc.Count);
                //Trace.TraceInformation("CountOfActiveCosnumers:{0}, bc.Count:{1}, countOfAvailableSlots:{2}", this.CountOfActiveCosnumers, bc.Count, countOfAvailableSlots);

                bool isRequestPresent = (countOfAvailableSlots > 0);
                T[] newRequests = null;
                if (isRequestPresent)
                {
                    try
                    {
                        isRequestPresent = false;
                        newRequests = _requestFetcher(countOfAvailableSlots);
                        isRequestPresent = (newRequests != null && newRequests.Length > 0);

                        if (isRequestPresent)
                        {
                            if (newRequests.Length > countOfAvailableSlots)
                            {
                                Trace.TraceError("# of Available Slots: {0}; Requests received for processing: {1}!", countOfAvailableSlots, newRequests.Length);
                            }
                            else
                            {
                                Trace.TraceInformation("Fetched {0}# of requests against demand of {1}# of requests.", newRequests.Length, countOfAvailableSlots);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Failed to fetch next {0}# of Requests! Exception Caught:{1}", countOfAvailableSlots, e.Message);
                        Trace.TraceError(e.StackTrace);
                    }
                }
                if (_isOperationMetricsLoggingEnabled && _stopwatchForOperationMetricsLogging.Elapsed.TotalSeconds > _frequencyOfShowingOpMetricsInSeconds)
                {
                    float f = this.SuccessRatioInPercent;
                    string success = (f <= 0.0) ? "0" : f.ToString("#.##");
                    Trace.TraceInformation("Operation Metrics for PCF [Hash:{0}]: # of Executed Requests:{1}; Success Rate: {2}%;", GetHashCode(), this.RequestCount, success);
                    _stopwatchForOperationMetricsLogging.Restart();
                }
                //
                if (!isRequestPresent)
                {
                    Thread.Sleep(_waitTimeBeforeFetchingNextRequest);
                    continue;
                }
                if (!enqueueRequests(bc, newRequests, ct))
                {
                    break;
                }
            }
            bc.CompleteAdding();
        }

        private static void wait(CancellationTokenSource cts)
        {
            if (cts == null)
            {
                return;
            }

            try
            {
                WaitHandle.WaitAll(new WaitHandle[] { cts.Token.WaitHandle });
            }
            catch (Exception e)
            {
                Trace.TraceInformation("EXPECTED Behavior: Exception:\"{0}\" has been caught & ignored!", e.Message);
                //EXPECTED Behavior - WaitAll() will throw this exception under normal circumstances
            }
        }

        private bool executePreConditionTaskTillSuccessful(CancellationTokenSource cts, Func<bool> task)
        {
            int count = 0;
            while (true)
            {
                if (cts.IsCancellationRequested)
                {
                    return false;
                }
                if (count > 0)
                {
                    Trace.TraceWarning("Executing 'Pre-Condition' task [Retry Count: {0}].", count++);
                }
                else
                {
                    Trace.TraceInformation("Executing 'Pre-Condition' task...");
                }

                try
                {
                    if (task())
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Error occured in executing Pre-Condition-Task! Exception:{0}", e.Message);
                    Trace.TraceError(e.StackTrace);
                    return false;
                }

                Trace.TraceWarning("****************************************");
                Trace.TraceWarning("Retrying 'Pre-Condition' task [Retry Count: {0}], Waiting for {1}ms before next retry.", count, _retryIntervalForPreConditionTaskInMS);
                Trace.TraceWarning("****************************************");
                Thread.Sleep(_retryIntervalForPreConditionTaskInMS);
            }
        }

        private int CountOfActiveCosnumers
        {
            get
            {
                lock (_lockerActiveThreadCount)
                {
                    return _activeThreadCount;
                }
            }
        }

        protected override void doCleanup()
        {
            Stop();
            if (_allTasks != null)
            {
                _allTasks.Clear();
            }
            SafeDispose<BlockingCollection<T>>(ref this._requests);
            SafeDispose<Task>(ref this._initializationTask);
        }
        #endregion
    }
}
