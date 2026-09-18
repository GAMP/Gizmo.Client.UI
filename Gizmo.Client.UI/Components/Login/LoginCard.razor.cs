using Gizmo.Client.Options;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class LoginCard : CustomDOMComponentBase
    {
        private bool _previousIsIdle = false;
        private bool _slideIn = false;
        private bool _slideOut = false;
        private bool _locked = false;


        #region PROPERTIES

        [Inject()]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState { get; init; }

        [Inject]
        IOptions<UserLoginOptions> UserLoginOptions { get; set; }

        [Inject]
        UserIdleViewState UserIdleViewState { get; set; }

        [Parameter]
        public RenderFragment CardHeader { get; set; }

        [Parameter]
        public RenderFragment CardBody { get; set; }

        [Parameter]
        public RenderFragment CardFooter { get; set; }

        #endregion

        //Idle changes are raised off the UI thread. This used to be async void awaiting
        //InvokeAsync, which meant a dispatcher fault during WebView teardown was rethrown on
        //the thread pool and exited the whole client. The slide animation is also a multi step
        //workflow (render, wait, render), so it runs as a single dispatcher work item to keep
        //every step on the renderer's context.
        private void UserIdleViewState_OnChange(object sender, System.EventArgs e)
        {
            if (_locked) return;
            if (_previousIsIdle == UserIdleViewState.IsIdle) return;

            DispatchWorkflow(async () =>
            {
                if (UserIdleViewState.IsIdle)
                {
                    _slideOut = true;
                }
                else
                {
                    _slideIn = true;
                }

                StateHasChanged();
                await Task.Delay(1000);
                _previousIsIdle = UserIdleViewState.IsIdle;
                _slideIn = false;
                _slideOut = false;
                StateHasChanged();
            });
        }

        protected override void OnInitialized()
        {
            _previousIsIdle = UserIdleViewState.IsIdle;
            UserIdleViewState.OnChange += UserIdleViewState_OnChange;

            _locked = UserLoginOptions.Value.Disabled && !UserRegisterConfigurationViewState.IsEnabled;

            base.OnInitialized();
        }

        //UserIdleViewState is a singleton that outlives this card, and the card is rebuilt on
        //every login/logout cycle. Without this the station accumulates one dead subscriber per
        //cycle, each one still being invoked on every idle transition.
        public override void Dispose()
        {
            UserIdleViewState.OnChange -= UserIdleViewState_OnChange;

            base.Dispose();
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-login-card")
                .If("slide-in", () => _slideIn && !_locked)
                .If("slide-out", () => _slideOut && !_locked)
                .If("hidden", () => !_slideIn && !_slideOut && _previousIsIdle || _locked)
                .AsString();

        #endregion
    }
}
