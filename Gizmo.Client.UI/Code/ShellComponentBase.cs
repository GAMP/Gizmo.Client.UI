using System;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Web.Components;

namespace Gizmo.Client.UI
{
    public static class ShellDispatch
    {
        public static void Run(Func<Func<Task>, Task> invokeAsync, Func<bool> isDisposed, Func<Task> workflow)
        {
            if (invokeAsync is null || workflow is null || isDisposed?.Invoke() == true)
                return;

            try
            {
                Observe(invokeAsync(async () =>
                {
                    try
                    {
                        if (isDisposed?.Invoke() != true)
                            await workflow();
                    }
                    catch (Exception ex) when (IsTeardown(ex))
                    {
                    }
                }));
            }
            catch (Exception ex) when (IsTeardown(ex))
            {
            }
        }

        private static bool IsTeardown(Exception exception)
        {
            return exception is OperationCanceledException
                or ObjectDisposedException
                or InvalidOperationException;
        }

        private static void Observe(Task task)
        {
            if (task is null || task.IsCompletedSuccessfully)
                return;

            task.ContinueWith(static faulted => { _ = faulted.Exception; },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }

    public abstract class ShellComponentBase : CustomDOMComponentBase
    {
        protected void DispatchWorkflow(Func<Task> workflow)
        {
            ShellDispatch.Run(InvokeAsync, () => IsDisposed, workflow);
        }

        protected void DispatchRender()
        {
            DispatchWorkflow(() =>
            {
                StateHasChanged();
                return Task.CompletedTask;
            });
        }
    }
}
