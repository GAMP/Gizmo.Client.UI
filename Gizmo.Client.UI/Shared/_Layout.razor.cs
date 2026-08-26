using Gizmo.Client.Options;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class _Layout : LayoutComponentBase
    {
        [Inject]
        IOptionsMonitor<ClientInterfaceOptions> ClientInterfaceOptions { get; set; }

        [Inject]
        IJSRuntime JsRuntime { get; set; }

        [Inject]
        WallpaperViewState WallpaperViewState { get; set; }

        [Inject]
        HostOutOfOrderViewState HostOutOfOrderViewState { get; set; }

        // Account chip (avatar + name) now renders directly in the top bar's
        // right pill instead of inside UserActionsBar, so its toggle needs
        // the same view service here.
        [Inject]
        UserMenuViewService UserMenuViewService { get; set; }

        // Tariff/time/points/balance stats render directly here as the HUD
        // capsule's segments (brief section 6.1) instead of through
        // TopBarInfoCluster.
        [Inject]
        UserBalanceViewState UserBalanceViewState { get; set; }

        private async Task OnMainClick(MouseEventArgs e)
        {
            await JsRuntime.InvokeVoidAsync("closeOpenPopups", e);
        }

        /// <summary>
        /// Localized hour/minute abbreviation without its trailing period.
        /// </summary>
        /// <remarks>
        /// The resource values carry a full stop ("h." / "ч."), which reads as
        /// noise in the top bar's compact time readout. Trimmed here rather
        /// than edited in the .resx so every other consumer of these keys
        /// keeps the punctuation it expects.
        /// </remarks>
        private string TimeUnitAbbreviation(string resourceKey) =>
            LocalizationService.GetString(resourceKey)?.TrimEnd('.', ' ') ?? string.Empty;

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(HostOutOfOrderViewState);
            this.SubscribeChange(UserBalanceViewState);

            await base.OnInitializedAsync();
        }
    }
}
