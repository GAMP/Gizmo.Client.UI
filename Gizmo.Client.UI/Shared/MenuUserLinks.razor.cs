using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuUserLinks : CustomDOMComponentBase
    {
        protected bool _shouldRender;

        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserViewService UserService { get; set; }

        [Inject]
        UserViewState ViewState { get; set; }

        [Inject]
        UserLockViewService UserLockService { get; set; }

        [Inject()]
        UserOnlineDepositViewService UserOnlineDepositViewStateService { get; set; }

        [Inject]
        UserMenuViewState UserMenuViewState { get; set; }

        [Inject]
        UserMenuViewService UserMenuViewService { get; set; }

        [Inject]
        IClientDialogService DialogService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        #endregion

        // Scattered icons around the avatar. Deliberately hand-placed
        // rather than generated from even rings: equal angle-steps at a
        // shared radius reads as neat concentric lines/rows, not the
        // organic "constellation" scatter this is meant to look like.
        // Angle/radius/size are all irregular on purpose - no two icons
        // share a radius band or a clean angle interval. Radii stay
        // outside the avatar's own edge (9.6rem/96px across = 48px radius).
        public sealed record OrbitIcon(string IconClass, double AngleDeg, double RadiusPx, double SizePx, string Opacity, double TiltDeg);

        public List<OrbitIcon> OrbitIcons { get; } = BuildOrbitIcons();

        private static List<OrbitIcon> BuildOrbitIcons()
        {
            var items = new (string Icon, double Angle, double Radius, double Size, double Opacity)[]
            {
                ("ph-game-controller",    12,  64, 15, .80),
                ("ph-headset",            58,  92, 10, .45),
                ("ph-desktop-tower",     104,  58, 13, .70),
                ("ph-keyboard",          146,  99,  9, .35),
                ("ph-crosshair-simple",  188,  70, 14, .75),
                ("ph-mouse",             221, 106,  8, .30),
                ("ph-joystick",          252,  61, 12, .65),
                ("ph-lightning",         283,  90, 11, .50),
                ("ph-cpu",               312,  73, 15, .78),
                ("ph-wifi-high",         341, 101,  9, .38),
            };

            var icons = new List<OrbitIcon>(items.Length);

            foreach (var it in items)
            {
                double tilt = Math.Sin(it.Angle * Math.PI / 180.0) * 15.0;
                //Colour comes from the stylesheet (the palette's ink), only the fade is per icon.
                string opacity = it.Opacity.ToString(System.Globalization.CultureInfo.InvariantCulture);
                icons.Add(new OrbitIcon(it.Icon, it.Angle, it.Radius, it.Size, opacity, tilt));
            }

            return icons;
        }

        private string DisplayName =>
            ViewState.IsGuest || string.IsNullOrEmpty(ViewState.Username)
                ? LocalizationService.GetString("GIZ_GEN_GUEST")
                : ViewState.Username;

        protected string Picture => ViewState.Picture;

        //The standing block: only for a customer of a club that runs any of the ladder,
        //achievements or challenges. A guest has no standing to show.
        protected bool ShowLoyalty => !ViewState.IsGuest && Loyalty.State.IsAvailable;

        /// <summary>The level's perks in words, joined; null when it has none the shell knows.</summary>
        protected static string LoyaltyPerks(Gizmo.Web.Api.Models.LadderStandingLevelModel level)
        {
            var perks = level?.Perks?.Select(LoyaltyText.Perk).Where(p => p is not null).ToList();
            return perks is { Count: > 0 } ? string.Join(" · ", perks) : null;
        }

        private void OnClickProgressHandler()
        {
            _shouldRender = true;

            UserMenuViewService.CloseUserLinks();

            NavigationService.NavigateTo(ClientRoutes.UserProfileRoute + "/progress");
        }

        private Task OnClickUserLockButtonHandler()
        {
            _shouldRender = true;

            UserMenuViewService.CloseUserLinks();

            return UserLockService.LockAsync();
        }

        private Task OnClickUserLogoutButtonHandler()
        {
            _shouldRender = true;

            UserMenuViewService.CloseUserLinks();

            return UserService.LogoutWithConfirmationAsync();
        }

        // The account page. A guest has no profile tab, so the Time tab is the front door.
        private void OnClickAccountButtonHandler()
        {
            _shouldRender = true;

            UserMenuViewService.CloseUserLinks();

            NavigationService.NavigateTo(ViewState.IsGuest ? ClientRoutes.UserProductsRoute : ClientRoutes.UserProfileRoute);
        }

        private Task OnClickChangePasswordButtonHandler()
        {
            _shouldRender = true;

            UserMenuViewService.CloseUserLinks();

            return DialogService.ShowChangePasswordDialogAsync(true);
        }

        #region OVERRIDES

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                await JsRuntime.InvokeVoidAsync("registerPopup", Ref);
                ClosePopupEventInterop = new ClosePopupEventInterop(JsRuntime);
                await ClosePopupEventInterop.SetupClosePopupEventCallback(args => ClosePopupHandler(args));
            }
        }

        #endregion

        private ClosePopupEventInterop ClosePopupEventInterop { get; set; }

        private Task ClosePopupHandler(string args)
        {
            if (args == Id)
            {
                _shouldRender = true;

                UserMenuViewService.CloseUserLinks();
            }

            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            ViewState.OnChange += ViewState_OnChange;
            UserMenuViewState.OnChange += ViewState_OnChange;
            Loyalty.Changed += OnLoyaltyChanged;

            base.OnInitialized();
        }

        // View states raise from the client's network and dispatcher threads, not this
        // component's. Written async void awaiting InvokeAsync, a dispatcher fault during
        // WebView teardown was rethrown on the thread pool and took the whole client down.
        // DispatchStateHasChanged absorbs those, so a lost WebView is only a reload.
        private void ViewState_OnChange(object sender, System.EventArgs e)
        {
            _shouldRender = true;
            DispatchStateHasChanged();
        }

        //The standing arrives and changes on the client's threads too.
        private void OnLoyaltyChanged()
        {
            _shouldRender = true;
            DispatchStateHasChanged();
        }

        public override void Dispose()
        {
            Loyalty.Changed -= OnLoyaltyChanged;
            UserMenuViewState.OnChange -= ViewState_OnChange;
            ViewState.OnChange -= ViewState_OnChange;

            ClosePopupEventInterop?.Dispose();

            base.Dispose();
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterPopup", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}
