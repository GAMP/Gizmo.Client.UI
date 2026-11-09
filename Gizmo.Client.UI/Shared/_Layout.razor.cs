using System;
using System.Globalization;
using System.Threading.Tasks;

using Gizmo.Client.Options;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Shared
{
    public partial class _Layout : LayoutComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        IOptionsMonitor<ClientInterfaceOptions> ClientInterfaceOptions { get; set; }

        [Inject]
        IJSRuntime JsRuntime { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        WallpaperViewState WallpaperViewState { get; set; }

        [Inject]
        HostOutOfOrderViewState HostOutOfOrderViewState { get; set; }

        [Inject]
        UserMenuViewService UserMenuViewService { get; set; }

        [Inject]
        UserBalanceViewState UserBalanceViewState { get; set; }

        [Inject]
        UserViewState UserViewState { get; set; }

        [Inject]
        ILogger<_Layout> Logger { get; set; }

        [CascadingParameter]
        GrafitLocalizationService GrafitLocalization { get; set; }

        private bool HasClubWallpaper => !string.IsNullOrEmpty(ClientInterfaceOptions.CurrentValue.Background);

        private bool HasTimeLimit => UserBalanceViewState.Time.HasValue;

        private string TimeLeftText
        {
            get
            {
                if (!UserBalanceViewState.Time.HasValue)
                    return string.Empty;

                var time = TimeLeft.NotNegative(UserBalanceViewState.Time);
                var hours = TimeUnitAbbreviation(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_HOUR_ABBREVIATED));
                var minutes = TimeUnitAbbreviation(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_MINUTE_ABBREVIATED));

                return $"{(int)time.TotalHours}{hours} {time.Minutes:00}{minutes}";
            }
        }

        private string PointsText => UserBalanceViewState.PointsBalance.ToString("N0", CultureInfo.CurrentCulture);

        private string BalanceText => UserBalanceViewState.Balance.ToString("C", CultureInfo.CurrentCulture);

        private string RootClass => "giz-root"
            + (_figuresHidden ? " giz-root--figures-hidden" : _figuresToggled ? " giz-root--figures-revealed" : string.Empty)
            + (_stage == AssemblyStage.Revealing ? " giz-root--assembling" : string.Empty);

        private string HudClass => _figuresHidden
            ? "giz-top-bar__hud giz-top-bar__hud--hidden"
            : _figuresToggled ? "giz-top-bar__hud giz-top-bar__hud--revealed" : "giz-top-bar__hud";

        private bool ShowBoot => _stage != AssemblyStage.Done;

        private bool HasUserName => !string.IsNullOrWhiteSpace(UserViewState.Username);

        private string BootTitle => HasUserName ? UserViewState.Username : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BOOT_HELLO);

        private string BootClass => _stage == AssemblyStage.Revealing ? "giz-boot giz-boot--leaving" : "giz-boot";

        private string HudTitle => _figuresHidden
            ? LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_MENU_BALANCE_IS_HIDDEN))
            : null;

        private string FiguresToggleLabel =>
            GrafitLocalization.GetString(_figuresHidden ? GrafitResourceKeys.SHELL_BALANCE_SHOW : GrafitResourceKeys.SHELL_BALANCE_HIDE);

        private string FiguresToggleIcon => _figuresHidden ? "ph-eye" : "ph-eye-slash";

        #endregion

        #region FIELDS

        private bool _playTransition;
        private readonly ProgressLoading _progress = new();
        private bool _figuresHidden;
        private bool _figuresToggled;
        private readonly ShellAssembly _assembly = new();
        private AssemblyStage _stage = AssemblyStage.Loading;

        private enum AssemblyStage { Loading, Revealing, Done }

        private static readonly TimeSpan BootShortest = TimeSpan.FromMilliseconds(650);
        private static readonly TimeSpan BootLongest = TimeSpan.FromSeconds(4);
        private static readonly TimeSpan AssemblyLength = TimeSpan.FromMilliseconds(1100);

        #endregion

        #region METHODS

        private void ToggleFigures()
        {
            _figuresHidden = !_figuresHidden;
            _figuresToggled = true;
        }

        private async Task AssembleAsync()
        {
            try
            {
                var released = Task.WhenAny(_assembly.WhenReleasedAsync(), Task.Delay(BootLongest));
                await Task.WhenAll(Task.Delay(BootShortest), released);

                _stage = AssemblyStage.Revealing;
                await InvokeAsync(StateHasChanged);

                await Task.Delay(AssemblyLength);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "The shell's assembly after sign-in failed.");
            }

            _stage = AssemblyStage.Done;
            await InvokeAsync(StateHasChanged);
        }

        private string TimeUnitAbbreviation(string resourceKey) =>
            LocalizationService.GetString(resourceKey)?.TrimEnd('.', ' ') ?? string.Empty;

        private async Task OnMainClick(MouseEventArgs e)
        {
            await JsRuntime.InvokeVoidAsync("closeOpenPopups", e);
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            _playTransition = true;
            _ = InvokeAsync(StateHasChanged);
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += OnLocationChanged;

            this.SubscribeChange(HostOutOfOrderViewState);
            this.SubscribeChange(UserBalanceViewState);
            this.SubscribeChange(UserViewState);

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                _ = AssembleAsync();

            if (firstRender || _playTransition)
            {
                _playTransition = false;
                await JsRuntime.InvokeVoidAsync("restartPageTransition");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;

            this.UnsubscribeChange(HostOutOfOrderViewState);
            this.UnsubscribeChange(UserBalanceViewState);
            this.UnsubscribeChange(UserViewState);
        }

        #endregion
    }
}
