using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace AirportDistanceCalculator.CommonUtils.Tasks
{
    /// <summary>Some extension methods for <see cref="Task"/></summary>
    public static class TaskExtensions
    {
        /// <summary>Awaits for all tasks and returns their results as a tuple</summary>
        [SuppressMessage("AsyncUsage", "AsyncFixer02:MissingAsyncOpportunity", Justification = "Task.Result used after WhenAll()")]
        public static async Task<(T1, T2)> WhenAll<T1, T2>(
            this (Task<T1> t1, Task<T2> t2) tasks,
            bool continueOnCapturedContext = false)
        {
            await Task.WhenAll(tasks.t1, tasks.t2).ConfigureAwait(continueOnCapturedContext);
            return (tasks.t1.Result, tasks.t2.Result);
        }

        /// <summary>Awaits for all tasks and returns their results as a tuple</summary>
        [SuppressMessage("AsyncUsage", "AsyncFixer02:MissingAsyncOpportunity", Justification = "Task.Result used after WhenAll()")]
        public static async Task<(T1, T2, T3)> WhenAll<T1, T2, T3>(
            this (Task<T1> t1, Task<T2> t2, Task<T3> t3) tasks,
            bool continueOnCapturedContext = false)
        {
            await Task.WhenAll(tasks.t1, tasks.t2, tasks.t3).ConfigureAwait(continueOnCapturedContext);
            return (tasks.t1.Result, tasks.t2.Result, tasks.t3.Result);
        }
    }
}
