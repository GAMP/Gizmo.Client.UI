using System.Threading.Tasks;
using Gizmo.Client;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI;

public partial class App : ComponentBase
{
    #region PROPERTIES

    /// <summary>
    /// Component discovery service.
    /// </summary>
    [Inject] public IUICompositionService ComponentDiscoveryService { get; protected set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private JSRuntimeService JSRuntimeService { get; set; }
    [Inject] private NavigationService NavigationService { get; set; }
    [Inject] private JSInteropService JSInteropService { get; set; }

    // Existing, already-registered types only (see AvatarService's own
    // remarks for why it can't be [Inject]-resolved directly). Constructing
    // it here, once, at app start, is what puts its LoginStateChange
    // subscription in place before the very first login on this station.
    [Inject] private IGizmoClient GizmoClient { get; set; }
    [Inject] private IOptionsMonitor<ClientNetworkOptions> ClientNetworkOptions { get; set; }
    [Inject] private UserViewState UserViewState { get; set; }
    [Inject] private ILogger<AvatarService> AvatarServiceLogger { get; set; }

    #endregion

    protected override void OnInitialized()
    {
        JSRuntimeService.AssociateJSRuntime(JSRuntime);
        NavigationService.AssociateNavigationManager(NavigationManager);

        AvatarService.Initialize(GizmoClient, NavigationService, ClientNetworkOptions, UserViewState, AvatarServiceLogger);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JSInteropService.InitializeAsync(default);

        await base.OnAfterRenderAsync(firstRender);
    }
}
