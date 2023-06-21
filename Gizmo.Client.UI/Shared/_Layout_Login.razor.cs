using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Shared
{
    public partial class _Layout_Login : LayoutComponentBase
    {
        private bool _previousIsIdle = false;
        private bool _slideIn = false;
        private bool _slideOut = false;

        [Inject]
        UserIdleViewState UserIdleViewState { get; set; }

        /// <summary>
        /// Gets client version view state.
        /// </summary>
        [Inject()]
        private ClientVersionViewState ClientVersionViewState { get; set; }

        [Inject]
        IOptions<ClientInterfaceOptions> ClientUIOptions { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        LoginRotatorViewState LoginRotatorViewState { get; set; }

        [Inject()]
        HostReservationViewState HostReservationViewState { get; set; }

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
                .Add("giz-login-content")
                .If("shrink", () => _slideIn)
                .If("grow", () => _slideOut)
                .If("collapsed", () => !_slideIn && !_slideOut && !_previousIsIdle)
                .AsString();

        #endregion

        [Inject]
        UserIdleViewService UserIdleViewService { get; set; }

        private void test()
        {
            UserIdleViewService.Toggle();
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(HostReservationViewState);

            await base.OnInitializedAsync();
        }
    }
}
