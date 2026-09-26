using System;
using System.Globalization;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI;

public partial class App : ComponentBase, IDisposable
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
    [Inject] private ILocalizationService LocalizationService { get; set; }
    [Inject] private ClientLocalizationViewState LocalizationViewState { get; set; }
    [Inject] private IServiceProvider ServiceProvider { get; set; }
    [Inject] private IClientNotificationService NotificationService { get; set; }

    #endregion

    protected override void OnInitialized()
    {
        JSRuntimeService.AssociateJSRuntime(JSRuntime);
        NavigationService.AssociateNavigationManager(NavigationManager);
        ShellStringOverrides.Associate(LocalizationService);

        LocalizationViewState.OnChange += OnCultureChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSInteropService.InitializeAsync(default);
            await ApplyDocumentDirectionAsync();
            await PublishAccentAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// </summary>
    private async Task PublishAccentAsync()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("grafitTheme.sync");

            var accent = await JSRuntime.InvokeAsync<string>("grafitTheme.get");

            ShellTheme.Set(accent);
        }
        catch (Exception exception) when (exception is JSException or InvalidOperationException)
        {
        }
    }

    /// <summary>
    /// </summary>
    private Task ApplyDocumentDirectionAsync()
    {
        var direction = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft ? "rtl" : "ltr";

        return JSRuntime.InvokeVoidAsync("setDocumentDirection", direction).AsTask();
    }

    private void OnCultureChanged(object sender, EventArgs e) =>
        _ = InvokeAsync(async () =>
        {
            try
            {
                await ApplyDocumentDirectionAsync();
            }
            catch (Exception exception) when (exception is OperationCanceledException
                                                or ObjectDisposedException
                                                or InvalidOperationException)
            {
            }
        });

    public void Dispose()
    {
        LocalizationViewState.OnChange -= OnCultureChanged;
    }
}
