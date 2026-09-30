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

                var time = UserBalanceViewState.Time.Value;
                var hours = TimeUnitAbbreviation(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_HOUR_ABBREVIATED));
                var minutes = TimeUnitAbbreviation(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_MINUTE_ABBREVIATED));

                return $"{(int)time.TotalHours}{hours} {time.Minutes:00}{minutes}";
            }
        }

        private string PointsText => UserBalanceViewState.PointsBalance.ToString("N0", CultureInfo.CurrentCulture);

        private string BalanceText => UserBalanceViewState.Balance.ToString("C", CultureInfo.CurrentCulture);

        private string RootClass => _figuresHidden ? "giz-root giz-root--figures-hidden" : "giz-root";

        private string HudClass => _figuresHidden ? "giz-top-bar__hud giz-top-bar__hud--hidden" : "giz-top-bar__hud";

        private string HudTitle => _figuresHidden
            ? LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_MENU_BALANCE_IS_HIDDEN))
            : null;

        private string FiguresToggleLabel =>
            GrafitLocalization.GetString(_figuresHidden ? GrafitResourceKeys.SHELL_BALANCE_SHOW : GrafitResourceKeys.SHELL_BALANCE_HIDE);

        private string FiguresToggleIcon => _figuresHidden ? "ph-eye" : "ph-eye-slash";

        #endregion

        #region FIELDS

        private bool _playTransition;
        private bool _figuresHidden;

        #endregion

        #region METHODS

        private void ToggleFigures() => _figuresHidden = !_figuresHidden;

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

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
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
        }

        #endregion
    }
}
