using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class LoginCard : CustomDOMComponentBase
    {
        private bool _previousIsIdle = false;
        private bool _slideIn = false;
        private bool _slideOut = false;

        #region PROPERTIES

        [Inject]
        UserIdleViewState UserIdleViewState { get; set; }

        [Parameter]
        public RenderFragment CardHeader { get; set; }

        [Parameter]
        public RenderFragment CardBody { get; set; }

        [Parameter]
        public RenderFragment CardFooter { get; set; }

        #endregion

        private async void UserIdleViewState_OnChange(object sender, System.EventArgs e)
        {
            if (_previousIsIdle == UserIdleViewState.IsIdle)
                return;

            if (UserIdleViewState.IsIdle)
            {
                _slideOut = true;
            }
            else
            {
                _slideIn = true;
            }

            await InvokeAsync(StateHasChanged);
            await Task.Delay(1000);
            _previousIsIdle = UserIdleViewState.IsIdle;
            _slideIn = false;
            _slideOut = false;
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            _previousIsIdle = UserIdleViewState.IsIdle;
            UserIdleViewState.OnChange += UserIdleViewState_OnChange;

            base.OnInitialized();
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-login-card")
                .If("slide-in", () => _slideIn)
                .If("slide-out", () => _slideOut)
                .If("hidden", () => !_slideIn && !_slideOut && _previousIsIdle)
                .AsString();

        #endregion
    }
}
