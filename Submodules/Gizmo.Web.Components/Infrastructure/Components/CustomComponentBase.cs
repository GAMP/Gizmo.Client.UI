using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Base implementation for custom components.
    /// </summary>
    public abstract class CustomComponentBase : ComponentBase, IDisposable
    {
        #region CONSTRUCTOR
        protected CustomComponentBase()
        { }
        #endregion

        #region FIELDS

        private bool _IsDisposed;
        private bool _IsRendered;
        private Queue<Func<Task>> _postRenderActions;

        #endregion

        #region IMPORTS

        /// <summary>
        /// Gets or sets JS Runtime instance.
        /// </summary>
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        #endregion

        #region PROPERTIES

        #region PUBLIC

        [Parameter]
        public ForwardRef RefBack { get; set; }

        #endregion

        #region PROTECTED

        /// <summary>
        /// Gets or sets if component is disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get { return _IsDisposed; }
            set { _IsDisposed = value; }
        }

        /// <summary>
        /// Gets or sets if component is rendered.
        /// </summary>
        protected bool IsRendered
        {
            get { return _IsRendered; }
            set { _IsRendered = value; }
        }

        /// <summary>
        /// Post render queue.
        /// </summary>
        /// <remarks>
        /// Stores an queue of functions to execute once render completes.
        /// </remarks>
        protected Queue<Func<Task>> PostRenderQueue
        {
            get
            {
                //lazy initialize value since its use is not common
                //since this will be called from single component UI thread there is no need for locking
                if (_postRenderActions == null)
                    _postRenderActions = new Queue<Func<Task>>();
                return _postRenderActions;
            }
        }

        /// <summary>
        /// Checks if there are post render queue items present.
        /// </summary>
        /// <remarks>
        /// The action uses the post render queue field to avoid creating queue instance in order to have less memory allocations.
        /// </remarks>
        protected bool HasPostRenderActions
        {
            get { return _postRenderActions?.Count > 0; }
        } 

        #endregion

        #endregion

        #region OVERRIDES

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            //execut base after render function
            await base.OnAfterRenderAsync(firstRender);

            //component is now considered rendered
            IsRendered = true;

            //execute first render function in case of initial rendering
            if (firstRender)
                await OnFirstAfterRenderAsync();

            //since the post render queue is lazy initialized
            if (HasPostRenderActions)
            {
                //create post render actions array
                var postRenderActions = PostRenderQueue.ToArray();

                //clear any queued post render actions
                //this will ensure that we dont keep post render actions allocated in case of one of them failing when executed
                PostRenderQueue.Clear();

                //execute each post render function
                foreach (var postRenderAction in postRenderActions)
                {
                    //break the loop in case of disposal
                    if (!IsDisposed)
                        break;

                    await postRenderAction();
                }
            }
        } 

        #endregion

        #region PUBLIC FUNCTIONS

        /// <summary>
        /// Asynchronosly invokes state has changed on UI thread.
        /// </summary>
        /// <remarks>
        /// <b>The method will not throw any exceptions in case of failure.</b>
        /// </remarks>
        public void DispatchStateHasChanged()
        {
            DispatchWorkflow(() =>
            {
                StateHasChanged();
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Runs <paramref name="workflow"/> on the renderer's dispatcher, absorbing the
        /// exceptions that happen purely because the host is tearing down.
        /// </summary>
        /// <param name="workflow">Work to run on the UI thread.</param>
        /// <remarks>
        /// <b>The method will not throw any exceptions in case of failure.</b>
        /// <para>
        /// This exists because of a logged production crash. An <c>async void</c> view state
        /// handler awaited <see cref="ComponentBase.InvokeAsync(Action)"/> while the WebView2
        /// browser process was going away; the dispatcher call faulted, and because an
        /// <c>async void</c> method rethrows on the thread pool instead of returning a faulted
        /// task, it reached the app domain handler and killed the whole client:
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
        /// Any handler subscribed to something that outlives the component (view states,
        /// singleton services, NavigationManager) must go through here or
        /// <see cref="DispatchStateHasChanged"/> instead of being written <c>async void</c>.
        /// </para>
        /// <para>
        /// The entire workflow runs as one dispatcher work item, so every <c>await</c> inside
        /// it resumes on the renderer's context. Multi step handlers (render, delay, render
        /// again) therefore cannot end up touching component state from a pool thread part
        /// way through.
        /// </para>
        /// </remarks>
        protected void DispatchWorkflow(Func<Task> workflow)
        {
            if (workflow == null || IsDisposed)
                return;

            try
            {
                Observe(InvokeAsync(async () =>
                {
                    try
                    {
                        //re-check on the UI thread: disposal can have happened while this was queued
                        if (!IsDisposed)
                            await workflow();
                    }
                    catch (Exception ex) when (IsTeardownException(ex))
                    {
                    }
                }));
            }
            catch (Exception ex) when (IsTeardownException(ex))
            {
                //InvokeAsync itself throws synchronously once the dispatcher is gone
            }
        }

        /// <summary>
        /// Checks whether an exception only means "the host is going away".
        /// </summary>
        /// <param name="exception">Exception to classify.</param>
        /// <returns>true if the exception is a teardown artifact, otherwise false.</returns>
        /// <remarks>
        /// Deliberately narrow. A genuine bug inside a handler must still surface as a crash
        /// during development rather than being silently swallowed here.
        /// </remarks>
        private static bool IsTeardownException(Exception exception)
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
            if (task == null || task.IsCompletedSuccessfully)
                return;

            task.ContinueWith(static faulted => { _ = faulted.Exception; },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        #endregion

        #region PROTECTED FUNCTIONS

        /// <summary>
        /// Queues a function for after render execution.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        protected void ExecuteAfterRender(Func<Task> action)
        {
            PostRenderQueue.Enqueue(action);
        }

        /// <summary>
        /// Creates .NET object reference.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value.</param>
        /// <returns>Dotnet object reference for value specified by <paramref name="value"/> parameter.</returns>
        ///<remarks>
        /// Official documentation <a href="https://docs.microsoft.com/en-us/aspnet/core/blazor/call-dotnet-from-javascript?view=aspnetcore-5.0"/>
        /// </remarks>
        protected DotNetObjectReference<T> CreateDotNetObjectReference<T>(T value) where T : class
        {
            return DotNetObjectReference.Create(value);
        }

        /// <summary>
        /// Invokes Javascript function.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="identifier">Function identifier.</param>
        /// <param name="args">Function arguments.</param>
        /// <returns>Associated task.</returns>
        protected ValueTask<T> JsInvokeAsync<T>(string identifier, params object[] args)
        {
            return JsRuntime.InvokeAsync<T>(identifier, args);
        }

        /// <summary>
        /// Invokes Javascript function without return parameters.
        /// </summary>
        /// <param name="identifier">Function identifier.</param>
        /// <param name="args">Function arguments.</param>
        /// <returns>Associated task.</returns>
        protected ValueTask InvokeVoidAsync(string identifier,params object[] args)
        {
            return JsRuntime.InvokeVoidAsync(identifier, args);
        }

        #endregion

        #region PROTECTED VIRTUAL FUNCTIONS

        /// <summary>
        /// This method will be called on first on first render.
        /// </summary>
        /// <returns>Associated task.</returns>
        protected virtual Task OnFirstAfterRenderAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the component.
        /// </summary>
        public virtual void Dispose()
        {
            //check if component is already disposed
            if (IsDisposed)
                return;

            try
            {
                //clear any post render functions
                if (HasPostRenderActions)
                    PostRenderQueue.Clear();
            }
            catch
            {
                //ignore any errors
            }

            //mark our class as disposed
            IsDisposed = true;
        }

        #endregion
    }
}
