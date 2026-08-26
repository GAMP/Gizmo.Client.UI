using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
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

        #endregion

        // Scattered icons around the avatar. Deliberately hand-placed
        // rather than generated from even rings: equal angle-steps at a
        // shared radius reads as neat concentric lines/rows, not the
        // organic "constellation" scatter this is meant to look like.
        // Angle/radius/size are all irregular on purpose - no two icons
        // share a radius band or a clean angle interval. Radii stay
        // outside the avatar's own edge (9.6rem/96px across = 48px radius).
        public sealed record OrbitIcon(string IconClass, double AngleDeg, double RadiusPx, double SizePx, string Color, double TiltDeg);

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
                string color = $"rgba(180,166,214,{it.Opacity.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
                icons.Add(new OrbitIcon(it.Icon, it.Angle, it.Radius, it.Size, color, tilt));
            }

            return icons;
        }

        private string DisplayName =>
            ViewState.IsGuest || string.IsNullOrEmpty(ViewState.Username)
                ? LocalizationService.GetString("GIZ_GEN_GUEST")
                : ViewState.Username;

        protected string Picture => AvatarService.Current?.Picture;

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

        private Task OnClickChangePasswordButtonHandler()
        {
            _shouldRender = true;

            UserMenuViewService.CloseUserLinks();

            return DialogService.ShowChangePasswordDialogAsync(true);
        }

        private Task OnClickEditPictureButtonHandler()
        {
            _shouldRender = true;

            UserMenuViewService.CloseUserLinks();

            return DialogService.ShowChangePictureDialogAsync();
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

            if (AvatarService.Current != null)
                AvatarService.Current.Changed += OnAvatarChanged;

            base.OnInitialized();
        }

        //Both of these fire from threads this component does not own - the view state raises
        //from the client's network/dispatcher threads, the avatar service from an HTTP
        //continuation - and both used to be async void awaiting InvokeAsync directly. When the
        //WebView2 browser process died during login the dispatcher call faulted, and async void
        //rethrew it on the thread pool: "Client app domain unhandled exception. Client will
        //exit." with this exact handler on the stack. DispatchStateHasChanged absorbs teardown
        //faults, so a lost WebView is now just a reload instead of the whole client dying.
        private void ViewState_OnChange(object sender, System.EventArgs e)
        {
            _shouldRender = true;
            DispatchStateHasChanged();
        }

        private void OnAvatarChanged()
        {
            _shouldRender = true;
            DispatchStateHasChanged();
        }

        public override void Dispose()
        {
            UserMenuViewState.OnChange -= ViewState_OnChange;
            ViewState.OnChange -= ViewState_OnChange;

            if (AvatarService.Current != null)
                AvatarService.Current.Changed -= OnAvatarChanged;

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
