using System;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
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
        private bool _submitting;
        private PlayerCardData _submitted;
        private PlayerCardData _welcome;

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

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewState BasicFieldsViewState { get; set; }

        [Inject]
        UserRegistrationAdditionalFieldsViewState AdditionalFieldsViewState { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        private bool HasAboutStep =>
            RegistrationSession.RequiredUserInfo?.Country == true ||
            RegistrationSession.RequiredUserInfo?.Address == true ||
            RegistrationSession.RequiredUserInfo?.City == true ||
            RegistrationSession.RequiredUserInfo?.PostCode == true;

        private static readonly string[] VerificationRoutes =
        {
            ClientRoutes.RegistrationIndexRoute,
            ClientRoutes.RegistrationProvidersRoute,
            ClientRoutes.RegistrationPhoneRoute,
            ClientRoutes.RegistrationEmailRoute,
            ClientRoutes.RegistrationConfirmationRoute,
            ClientRoutes.RegistrationRedirectRoute,
            ClientRoutes.RegistrationErrorRoute,
        };

        private RegistrationStep? CurrentRegistrationStep
        {
            get
            {
                var path = "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('?', '#')[0].TrimEnd('/');

                if (string.Equals(path, ClientRoutes.RegistrationBasicFieldsRoute, StringComparison.OrdinalIgnoreCase))
                    return RegistrationStep.Account;

                if (string.Equals(path, ClientRoutes.RegistrationAdditionalFieldsRoute, StringComparison.OrdinalIgnoreCase))
                    return RegistrationStep.About;

                return VerificationRoutes.Any(route => string.Equals(path, route, StringComparison.OrdinalIgnoreCase))
                    ? RegistrationStep.Verify
                    : null;
            }
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e) => HandlePositionChanged();

        private void OnRegistrationFormChanged(object sender, EventArgs e)
        {
            _submitting = BasicFieldsViewState.IsLoading || AdditionalFieldsViewState.IsLoading;

            if (_submitting)
                _submitted = PlayerCardData.From(BasicFieldsViewState, AdditionalFieldsViewState, RegistrationSession);
        }

        private void OnRegistrationCleared(object sender, EventArgs e)
        {
            if (!_submitting || _submitted is null)
                return;

            _submitting = false;
            _welcome = _submitted;
            HandlePositionChanged();
        }

        private void CloseWelcome()
        {
            var nick = _welcome?.Nick;
            _welcome = null;

            if (string.IsNullOrEmpty(nick))
                return;

            UserLoginService.SetLoginMethod(View.UserLoginType.UsernameOrEmail);
            UserLoginService.SetLoginName(nick);
        }

        private void UserIdleViewState_OnChange(object sender, EventArgs e)
        {
            if (_previousIsIdle == UserIdleViewState.IsIdle)
                return;

            DispatchWorkflow(async () =>
            {
                if (UserIdleViewState.IsIdle)
                {
                    _slideOut = true;
                    _welcome = null;
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

        private void DispatchWorkflow(Func<Task> workflow)
        {
            ShellDispatch.Run(InvokeAsync, () => _disposed, workflow);
        }

        protected override void OnInitialized()
        {
            _previousIsIdle = UserIdleViewState.IsIdle;
            UserIdleViewState.OnChange += UserIdleViewState_OnChange;
            HostHumberViewService.OnPositionChanged += HandlePositionChanged;
            NavigationManager.LocationChanged += OnLocationChanged;
            BasicFieldsViewState.OnChange += OnRegistrationFormChanged;
            AdditionalFieldsViewState.OnChange += OnRegistrationFormChanged;
            RegistrationSession.Cleared += OnRegistrationCleared;
            
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

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            UserIdleViewState.OnChange -= UserIdleViewState_OnChange;
            HostHumberViewService.OnPositionChanged -= HandlePositionChanged;
            NavigationManager.LocationChanged -= OnLocationChanged;
            BasicFieldsViewState.OnChange -= OnRegistrationFormChanged;
            AdditionalFieldsViewState.OnChange -= OnRegistrationFormChanged;
            RegistrationSession.Cleared -= OnRegistrationCleared;

            this.UnsubscribeChange(LoginRotatorViewState);
            this.UnsubscribeChange(LogoViewState);
            this.UnsubscribeChange(HostReservationViewState);
            this.UnsubscribeChange(HostOutOfOrderViewState);
        }
    }
}
