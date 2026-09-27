using System;
using System.Globalization;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Services;
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

    protected GrafitLocalizationService GrafitLocalization { get; private set; }

    private DotNetObjectReference<App> _selfRef;

    #endregion

    protected override void OnInitialized()
    {
        JSRuntimeService.AssociateJSRuntime(JSRuntime);
        NavigationService.AssociateNavigationManager(NavigationManager);
        GrafitLocalization = new GrafitLocalizationService(LocalizationService);

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

    private async Task PublishAccentAsync()
    {
        try
        {
            _selfRef ??= DotNetObjectReference.Create(this);

            await JSRuntime.InvokeVoidAsync("grafitTheme.watch", _selfRef, nameof(OnAccentApplied));
            await JSRuntime.InvokeVoidAsync("grafitTheme.sync");

            var accent = await JSRuntime.InvokeAsync<string>("grafitTheme.get");

            ShellTheme.Set(accent);
        }
        catch (Exception exception) when (exception is JSException or InvalidOperationException)
        {
        }
    }

    [JSInvokable]
    public void OnAccentApplied(string accent) => ShellTheme.Set(accent);

    private async Task ApplyDocumentDirectionAsync()
    {
        var direction = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft ? "rtl" : "ltr";

        try
        {
            await JSRuntime.InvokeVoidAsync("setDocumentDirection", direction);
        }
        catch (Exception exception) when (exception is JSException
                                            or OperationCanceledException
                                            or ObjectDisposedException
                                            or InvalidOperationException)
        {
        }
    }

    private void OnCultureChanged(object sender, EventArgs e) =>
        _ = InvokeAsync(ApplyDocumentDirectionAsync);

    public void Dispose()
    {
        LocalizationViewState.OnChange -= OnCultureChanged;

        _selfRef?.Dispose();
    }
}
