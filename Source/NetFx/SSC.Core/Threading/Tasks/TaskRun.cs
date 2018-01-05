#define NO_BREAK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SSC.Threading.Tasks
{
    public static class TaskRun
    {

        public static async Task<TaskRunResult<T>> Run<T>(Func<Task<TaskRunResult<T>>> taskToRun, TaskRunSettings settings = null, CancellationToken? cToken = null)
        {
            TaskRunResult<T> r = new TaskRunResult<T>(default(T), null, false);
            //
            if (settings == null)
                settings = new TaskRunSettings();
            for (int iTry = 0; iTry < settings.TryCount; iTry++)
            {
                bool isSuccess = false;
                try
                {
                    var task_Run = taskToRun();
                    Task task_Timeout = null;
                    Task task_Cancel = null;

                    List<Task> tasks = new List<Task>();
                    tasks.Add(task_Run);
                    if (settings.TimeOut != null)
                    {
                        //Func<Task<bool>> task_Timeout_Func = async () =>
                        //    {
                        //        await Task.Run(() => Task.Delay(settings.TimeOut.Value));
                        //        return true;
                        //    };
                        //task_Timeout = task_Timeout_Func();
                        task_Timeout = Task.Delay(settings.TimeOut.Value);
                        tasks.Add(task_Timeout);
                    }
                    if (cToken != null)
                    {
                        task_Cancel = Task.Run(async () =>
                        {
                            while (!cToken.Value.IsCancellationRequested)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(1));
                            }
                        });
                        tasks.Add(task_Cancel);
                    }

                    var task_R = await Task.WhenAny(tasks.ToArray());
                    if (task_R == task_Run)
                    {
                        r = task_Run.Result;
                        isSuccess = r.IsSuccess;
                    }
                    else if (task_R == task_Timeout)
                    {
                        r = new TaskRunResult<T>(default(T), new TimeoutException("RunTask timed out."), false);
                        isSuccess = false;
                    }
                    else if (task_R == task_Cancel)
                    {
                        r = new TaskRunResult<T>(default(T), null, true);
                        isSuccess = false;
                    }
                }
                catch (Exception ex1)
                {
                    r = new TaskRunResult<T>(default(T), ex1, false);
                    isSuccess = false;
                }
                if (!isSuccess && !r.Canceled)
                {
#if true//NETFX_CORE
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(settings.DelayBetweenTriesInSeconds));
#else
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(delayBetweenTriesInSeconds));
#endif
                }
                else
                    break;
            }
#if DEBUG && !NO_BREAK
            if (r.Exception != null && System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
#endif
            if (settings.ThrowExceptionOnFailure && r.Exception != null)
                throw new AggregateException(r.Exception);
            //
            return r;
        }
    }
}
