using Gizmo.Client.Options;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class LoginCard : ShellComponentBase
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

        [Parameter]
        public RenderFragment Aside { get; set; }

        [Parameter]
        public string Modifier { get; set; }

        #endregion

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

        public override void Dispose()
        {
            UserIdleViewState.OnChange -= UserIdleViewState_OnChange;

            base.Dispose();
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-login-card")
                .If("giz-login-card--spread", () => Aside is not null)
                .If(Modifier, () => !string.IsNullOrEmpty(Modifier))
                .If("slide-in", () => _slideIn && !_locked)
                .If("slide-out", () => _slideOut && !_locked)
                .If("hidden", () => !_slideIn && !_slideOut && _previousIsIdle || _locked)
                .AsString();

        #endregion
    }
}
