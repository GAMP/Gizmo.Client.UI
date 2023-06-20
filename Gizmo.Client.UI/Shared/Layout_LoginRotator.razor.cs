using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
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
        private bool _animation = false;
        private string _nextAnimation = string.Empty;
        private string[] _animations = new string[] { "slide-in-top", "slide-in-bottom", "slide-in-left", "slide-in-right" }; //, "slide-out-top", "slide-out-bottom" };
        private Random random = new();

        [Inject]
        LoginRotatorViewService LoginRotatorViewService { get; set; }

        [Inject]
        LoginRotatorViewState ViewState { get; set; }

        private async Task OnVideoEventHandle(VideoEventArgs args)
        {
            if (args.VideoState == VideoStates.Ended)
            {
                await LoginRotatorViewService.PlayNext();
            }
            else
            {
                await JsRuntime.InvokeVoidAsync("playVideo", args.Id);
            }
        }

        private async void ViewState_OnChange(object sender, System.EventArgs e)
        {
            _previousItem = _currentItem;
            _currentItem = ViewState.CurrentItem;
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
