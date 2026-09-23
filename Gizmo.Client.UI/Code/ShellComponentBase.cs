using System;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Web.Components;

namespace Gizmo.Client.UI
{
    /// <summary>
    /// Runs a component's work on the renderer's dispatcher without <c>async void</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A handler subscribed to something that outlives its component - a view state, a
    /// static event, the navigation manager - is raised from the client's own threads.
    /// Written as <c>async void</c> awaiting <see cref="Microsoft.AspNetCore.Components.ComponentBase.InvokeAsync(Func{Task})"/>,
    /// such a handler took the whole client down in production: the WebView2 process was
    /// going away, the dispatcher call faulted, and because an <c>async void</c> method
    /// rethrows on the thread pool instead of returning a faulted task, it reached the app
    /// domain handler:
    /// </para>
    /// <code>
    /// [FTL] Client app domain unhandled exception. Client will exit.
    /// System.Threading.Tasks.TaskCanceledException: A task was canceled.
    ///    at Microsoft.AspNetCore.Components.WebView.Wpf.WpfDispatcher.InvokeAsync(Action workItem)
    ///    at Gizmo.Client.UI.Shared.MenuUserLinks.ViewState_OnChange(Object sender, EventArgs e)
    /// </code>
    /// <para>
    /// Losing the WebView is survivable on its own - the host recreates it - so a handler
    /// that cannot render must quietly do nothing rather than take the process with it.
    /// </para>
    /// <para>
    /// This lives in the shell rather than in <c>Gizmo.Web.Components</c> on purpose: the
    /// skin changes nothing outside this project. Components of this shell inherit
    /// <see cref="ShellComponentBase"/>; the few with a base of their own (a layout, for
    /// instance) call <see cref="Run"/> directly.
    /// </para>
    /// </remarks>
    public static class ShellDispatch
    {
        /// <summary>
        /// Queues <paramref name="workflow"/> on the renderer and observes it.
        /// </summary>
        /// <param name="invokeAsync">The component's <c>InvokeAsync</c>.</param>
        /// <param name="isDisposed">Whether the component is already gone.</param>
        /// <param name="workflow">Work to run on the UI thread.</param>
        /// <remarks>
        /// The whole workflow runs as one dispatcher work item, so every <c>await</c>
        /// inside it resumes on the renderer's context: a multi step handler (render,
        /// delay, render again) cannot end up touching component state from a pool thread
        /// part way through. <b>Never throws.</b>
        /// </remarks>
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
                        //Re-checked on the UI thread: disposal can have happened while this was queued.
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
                //InvokeAsync itself throws synchronously once the dispatcher is gone.
            }
        }

        /// <summary>
        /// Whether an exception only means "the host is going away".
        /// </summary>
        /// <param name="exception">Exception to classify.</param>
        /// <returns>true for a teardown artifact, otherwise false.</returns>
        /// <remarks>
        /// Deliberately narrow. A genuine bug inside a handler must still surface as a
        /// crash during development rather than being silently swallowed here.
        /// </remarks>
        private static bool IsTeardown(Exception exception)
        {
            return exception is OperationCanceledException  //TaskCanceledException derives from this
                or ObjectDisposedException
                or InvalidOperationException;               //renderer not initialized / already disposed
        }

        /// <summary>
        /// Observes a dispatcher task so a fault cannot resurface as an unobserved exception.
        /// </summary>
        /// <param name="task">Task to observe.</param>
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

    /// <summary>
    /// The base of this shell's components: <see cref="Gizmo.Web.Components.CustomDOMComponentBase"/>
    /// plus a safe way to render and to run work from another thread.
    /// </summary>
    /// <remarks>
    /// Anything subscribed to a view state, a static event or the navigation manager goes
    /// through <see cref="DispatchWorkflow"/> or <see cref="DispatchRender"/> - never
    /// <c>async void</c>. See <see cref="ShellDispatch"/> for what that prevents.
    /// </remarks>
    public abstract class ShellComponentBase : CustomDOMComponentBase
    {
        /// <summary>
        /// Runs a workflow on the renderer's dispatcher. Never throws.
        /// </summary>
        /// <param name="workflow">Work to run on the UI thread.</param>
        protected void DispatchWorkflow(Func<Task> workflow)
        {
            ShellDispatch.Run(InvokeAsync, () => IsDisposed, workflow);
        }

        /// <summary>
        /// Re-renders from any thread. Never throws.
        /// </summary>
        /// <remarks>
        /// The vendor's <see cref="Gizmo.Web.Components.CustomComponentBase.DispatchStateHasChanged"/>
        /// guards the render itself but leaves the dispatcher call unobserved, and that
        /// call throws synchronously once the renderer is gone. Same intent, whole path
        /// guarded.
        /// </remarks>
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
