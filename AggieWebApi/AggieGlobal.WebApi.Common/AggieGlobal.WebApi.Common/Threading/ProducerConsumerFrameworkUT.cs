using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AggieGlobal.WebApi.Common.Threading
{
#if DEBUG
    /// <summary>
    /// Utility Class for performing UT for ProducerConsumerFramework class
    /// </summary>
    public static class ProducerConsumerFrameworkUT
    {
        #region Member Variables
        private static Func<int, int> Factorial = x => x < 0 ? -1 : x == 1 || x == 0 ? 1 : x * Factorial(x - 1);
        #endregion

        #region Public Methods
        public static void Test()
        {
            IProducerConsumerFramework<int> pcf = new ProducerConsumerFramework<int>(generateRequests, executeRequests, 5);
            pcf.Start();
            pcf.WaitForCompletion();
        }
        #endregion

        #region Private Methods
        private static int[] generateRequests(int countOfRequestsToGenerate)
        {
            List<int> requests = new List<int>();
            Random r = new Random();
            int count = r.Next(1, countOfRequestsToGenerate);
            for (int i = 0; i < count; i++)
            {
                requests.Add(r.Next(5, 15));
            }
            return (requests.Count > 0) ? requests.ToArray() : null;
        }

        private static bool executeRequests(int number)
        {
            try
            {
                int result = Factorial(number);
                Trace.TraceInformation("{0}!={1}", number, result);
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
            return false;
        }
        #endregion
    }
#endif
}
