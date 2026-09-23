using System;
using System.Threading.Tasks;
using Gizmo.Client.Options;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Shared
{
    public partial class _Layout_Login : LayoutComponentBase, IDisposable
    {
        private bool _previousIsIdle = false;
        private bool _slideIn = false;
        private bool _slideOut = false;
        private bool _locked = false;
        private bool _disposed = false;

        [Inject()]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState { get; init; }

        [Inject]
        IOptions<UserLoginOptions> UserLoginOptions { get; set; }

        [Inject]
        HostOutOfOrderViewState HostOutOfOrderViewState { get; set; }

        [Inject]
        UserIdleViewState UserIdleViewState { get; set; }

        /// <summary>
        /// Gets client version view state.
        /// </summary>
        [Inject()]
        private ClientVersionViewState ClientVersionViewState { get; set; }

        [Inject()]
        private LogoViewState LogoViewState { get; set; }

        [Inject]
        IOptions<ClientInterfaceOptions> ClientUIOptions { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        LoginRotatorViewState LoginRotatorViewState { get; set; }

        [Inject()]
        HostReservationViewState HostReservationViewState { get; set; }
        
        [Inject]
        HostNumberViewState HostNumberViewState { get; set; }
        
        [Inject]
        HostNumberViewService HostHumberViewService { get; set; }

        //Idle transitions arrive off the UI thread. As async void, a dispatcher fault while the
        //WebView was being torn down got rethrown on the thread pool and took the whole client
        //with it ("Client app domain unhandled exception. Client will exit."). This layout is a
        //LayoutComponentBase, so it cannot inherit ShellComponentBase; it calls the same helper
        //directly instead.
        private void UserIdleViewState_OnChange(object sender, EventArgs e)
        {
            if (_previousIsIdle == UserIdleViewState.IsIdle)
                return;

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

        /// <summary>
        /// Runs a workflow on the renderer's dispatcher, swallowing host teardown faults.
        /// </summary>
        /// <param name="workflow">Work to run on the UI thread.</param>
        /// <remarks>
        /// The shell's components get this from <see cref="ShellComponentBase"/>; a layout
        /// has a base of its own, so it hands <see cref="ShellDispatch"/> what it needs.
        /// </remarks>
        private void DispatchWorkflow(Func<Task> workflow)
        {
            ShellDispatch.Run(InvokeAsync, () => _disposed, workflow);
        }

        protected override void OnInitialized()
        {
            _previousIsIdle = UserIdleViewState.IsIdle;
            UserIdleViewState.OnChange += UserIdleViewState_OnChange;
            HostHumberViewService.OnPositionChanged += HandlePositionChanged;
            
            _locked = UserLoginOptions.Value.Disabled && !UserRegisterConfigurationViewState.IsEnabled;

            base.OnInitialized();
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .If("shrink", () => _slideIn)
                .If("grow", () => _slideOut || _locked)
                .If("collapsed", () => !_slideIn && !_slideOut && !_previousIsIdle)
                .If("giz-login-content--own-bg", () => !HasClubBackground)
                .AsString();

        /// <summary>
        /// The club has put a picture (or a rotator) behind the sign-in screen.
        /// </summary>
        /// <remarks>
        /// With nothing configured the vendor falls back to a stock photograph; this shell
        /// does not. The idle screen then shows the same gradient and icons that appear
        /// behind the sign-in card, in the accent's colours, so a club that sets nothing
        /// gets the shell's own look rather than a picture that belongs to no club. A
        /// configured picture stays the idle background and gives way to the gradient only
        /// while the card is up.
        /// </remarks>
        private bool HasClubBackground =>
            !string.IsNullOrEmpty(ClientUIOptions.Value.LoginBackground) || LoginRotatorViewState.IsEnabled;

        #endregion

        [Inject]
        UserIdleViewService UserIdleViewService { get; set; }

        private void test()
        {
            //UserIdleViewService.Toggle();
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(LoginRotatorViewState);
            this.SubscribeChange(LogoViewState);
            this.SubscribeChange(HostReservationViewState);
            this.SubscribeChange(HostOutOfOrderViewState);

            await base.OnInitializedAsync();
        }
        
        private void HandlePositionChanged()
        {
            DispatchWorkflow(() =>
            {
                StateHasChanged();
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Detaches every subscription this layout made.
        /// </summary>
        /// <remarks>
        /// This layout previously had no disposal at all, while subscribing to six sources that
        /// all outlive it (view state singletons plus the host number service). The login layout
        /// is torn down and rebuilt on every login and logout, so each cycle left another dead
        /// instance attached and still being invoked - exactly the population of stale handlers
        /// that turned a WebView hiccup into a client exit.
        /// </remarks>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            UserIdleViewState.OnChange -= UserIdleViewState_OnChange;
            HostHumberViewService.OnPositionChanged -= HandlePositionChanged;

            this.UnsubscribeChange(LoginRotatorViewState);
            this.UnsubscribeChange(LogoViewState);
            this.UnsubscribeChange(HostReservationViewState);
            this.UnsubscribeChange(HostOutOfOrderViewState);
        }
    }
}
