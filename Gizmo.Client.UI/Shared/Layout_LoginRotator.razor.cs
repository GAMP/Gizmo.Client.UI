using System;
using System.Threading.Tasks;
using Gizmo.Client.Options;
using Gizmo.Client.UI.View.Constants;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Shared
{
    public partial class Layout_LoginRotator : ShellComponentBase
    {
        private LoginRotatorItemViewState _previousItem;
        private LoginRotatorItemViewState _currentItem;
        private double _shownPosition;
        private bool _animation = false;
        private string _nextAnimation = string.Empty;
        private static readonly string[] TransitionClasses = new string[] { "slide-in-left", "slide-in-top", "slide-in-right", "slide-in-bottom" };
        private int _sequenceIndex;

        [Inject]
        LoginRotatorViewState ViewState { get; set; }

        [Inject]
        IOptionsMonitor<LoginRotatorOptions> LoginRotatorOptions { get; set; }

        private double CurrentPosition => _currentItem?.PositionSeconds ?? 0;

        private async Task OnVideoEventHandle(VideoEventArgs args)
        {
            // The clock changes the slide. End of a file does not.
            if (args.VideoState == VideoStates.Ended)
                return;

            if (args.VideoState == VideoStates.Error)
            {
                await JsRuntime.InvokeVoidAsync("resetVideo", args.Id);
                return;
            }

            await JsRuntime.InvokeVoidAsync("playVideo", args.Id, CurrentPosition);
        }

        private void ViewState_OnChange(object sender, System.EventArgs e)
        {
            var next = ViewState.CurrentItem;
            if (next == null)
                return;

            var same = _currentItem != null
                && string.Equals(_currentItem.MediaPath, next.MediaPath, StringComparison.OrdinalIgnoreCase);

            if (same && Math.Abs(_shownPosition - next.PositionSeconds) < 0.15)
                return;

            _shownPosition = next.PositionSeconds;

            if (same)
            {
                _currentItem = next;
                DispatchWorkflow(() =>
                {
                    StateHasChanged();
                    return Task.CompletedTask;
                });
                return;
            }

            _previousItem = _currentItem;
            _currentItem = next;

            DispatchWorkflow(async () =>
            {
                _animation = true;
                _nextAnimation = TakeTransitionClass();
                StateHasChanged();
                await Task.Delay(1000);
                _animation = false;
                StateHasChanged();
            });
        }

        private string TakeTransitionClass()
        {
            var transition = LoginRotatorOptions.CurrentValue.Transition;
            if (transition == LoginRotatorTransition.Sequential)
            {
                var className = TransitionClasses[_sequenceIndex % TransitionClasses.Length];
                _sequenceIndex++;
                return className;
            }

            var index = (int)transition;
            if ((uint)index >= (uint)TransitionClasses.Length)
                index = 0;

            return TransitionClasses[index];
        }

        protected override void OnInitialized()
        {
            _currentItem = ViewState.CurrentItem;
            _shownPosition = _currentItem?.PositionSeconds ?? 0;

            ViewState.OnChange += ViewState_OnChange;

            base.OnInitialized();
        }

        public override void Dispose()
        {
            ViewState.OnChange -= ViewState_OnChange;

            base.Dispose();
        }

        #region CLASSMAPPERS

        protected string AnimatedClassName => new ClassMapper()
                .Add("giz-login-background-wrapper")
                .Add("current")
                .If(_nextAnimation, () => _animation)
                .AsString();

        #endregion
    }
}
