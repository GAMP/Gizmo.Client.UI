using System;
using System.Globalization;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
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
    /// Reads the accent palette the page resolved and publishes it for the
    /// notifications window, which has a document of its own - see <see cref="ShellTheme"/>.
    /// </summary>
    /// <remarks>
    /// A club's stylesheet can still be on its way at this point; grafitTheme applies it
    /// when it lands, and a change after this read only leaves the notifications window
    /// on the skin's default palette, which is not worth a watcher.
    /// </remarks>
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
            // An old bundle without the hook: the default palette stands.
        }
    }

    /// <summary>
    /// Declares the document's writing direction for the current language.
    /// </summary>
    /// <remarks>
    /// Every culture the client currently ships is left-to-right, so today this always
    /// writes "ltr". It is here because the direction has to be declared for any
    /// right-to-left support to have something to switch on - a mirrored stylesheet with
    /// no <c>dir</c> on the document would never apply. The stylesheet itself is not
    /// mirrored; see the RTL section of deploy/README.md for what that would take.
    /// </remarks>
    private Task ApplyDocumentDirectionAsync()
    {
        var direction = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft ? "rtl" : "ltr";

        return JSRuntime.InvokeVoidAsync("setDocumentDirection", direction).AsTask();
    }

    // The language menu switches culture at runtime, so the direction is re-applied rather
    // than only read at startup. Not async void: a fault here would come back on the pool.
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
                // The WebView is going away; there is no document left to mark.
            }
        });

    public void Dispose()
    {
        LocalizationViewState.OnChange -= OnCultureChanged;
    }
}
