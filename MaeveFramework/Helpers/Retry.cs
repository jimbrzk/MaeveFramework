using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MaeveFramework.Helpers
{
    /// <summary>
    /// Retry Action and catch exceptions
    /// </summary>
    // Source: https://stackoverflow.com/a/1563234/12482583
    public static class Retry
    {
        /// <summary>
        /// Run Action and catch exceptions
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryInterval"></param>
        /// <param name="maxAttemptCount">Throw aggregated exceptions after retries</param>
        public static void Do(
            Action action,
            TimeSpan retryInterval,
            int maxAttemptCount = 3)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, maxAttemptCount);
        }

        /// <summary>
        /// Run Action and catch exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="retryInterval"></param>
        /// <param name="maxAttemptCount">Throw aggregated exceptions after retries</param>
        /// <returns></returns>
        public static T Do<T>(
            Func<T> action,
            TimeSpan retryInterval,
            int maxAttemptCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }
    }
}
