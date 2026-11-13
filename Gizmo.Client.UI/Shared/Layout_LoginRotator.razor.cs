using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Shared
{
    public partial class Layout_LoginRotator : CustomDOMComponentBase
    {
        private LoginRotatorItemViewState _previousItem;
        private LoginRotatorItemViewState _currentItem;
        private double _shownPosition;
        private bool _animation = false;
        private string _nextAnimation = string.Empty;
        private string[] _animations = new string[] { "slide-in-top", "slide-in-bottom", "slide-in-left", "slide-in-right" };
        private Random random = new();

        [Inject]
        LoginRotatorViewState ViewState { get; set; }

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

        private async void ViewState_OnChange(object sender, System.EventArgs e)
        {
            var next = ViewState.CurrentItem;
            if (next == null)
                return;

            var same = _currentItem != null
                && string.Equals(_currentItem.MediaPath, next.MediaPath, StringComparison.OrdinalIgnoreCase);

            if (same && Math.Abs(_shownPosition - next.PositionSeconds) < 0.5)
                return;

            _shownPosition = next.PositionSeconds;

            if (same)
            {
                _currentItem = next;
                await InvokeAsync(StateHasChanged);
                return;
            }

            _previousItem = _currentItem;
            _currentItem = next;

            _animation = true;
            _nextAnimation = _animations[random.Next(0, _animations.Length)];
            await InvokeAsync(StateHasChanged);
            await Task.Delay(1000);
            _animation = false;
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            _currentItem = ViewState.CurrentItem;
            _shownPosition = _currentItem?.PositionSeconds ?? 0;

            ViewState.OnChange += ViewState_OnChange;

            base.OnInitialized();
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
