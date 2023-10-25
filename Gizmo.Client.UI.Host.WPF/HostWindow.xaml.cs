using Gizmo.Client.UI.Services;
using Gizmo.UI;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Gizmo.Client.UI.Host.WPF
{
    /// <summary>
    /// Interaction logic for HostWindow.xaml
    /// </summary>
    public partial class HostWindow : Window, IHostWindow
    {
        public HostWindow(DesktopUICompositionService componentDiscoveryService)
        {
            InitializeComponent();

            componentDiscoveryService.Initialized += ComponentDiscoveryService_Initialized;
            UpdateValues(componentDiscoveryService);
        }

        private void ComponentDiscoveryService_Initialized(object sender, System.EventArgs e)
        {
            UpdateValues((DesktopUICompositionService)sender);
        }

        private void UpdateValues(DesktopUICompositionService componentDiscoveryService)
        {
            var rootComponent = componentDiscoveryService.RootComponentType;
            if (rootComponent != null)
            {
                //set component type based on the settings found by discovery service
                _ROOT_COMPONENT.ComponentType = rootComponent;

                _BLAZOR_WEB_VIEW.HostPage = Path.Combine(componentDiscoveryService.BasePath, @"wwwroot\index.html");
            }
        }

        private void BlazorWebViewInit(object sender, BlazorWebViewInitializedEventArgs e)
        {
            var staticFiles = Path.Combine(Environment.CurrentDirectory, "static");
            if (Directory.Exists(staticFiles))
            {
                //map static folder
                _BLAZOR_WEB_VIEW.WebView.CoreWebView2.SetVirtualHostNameToFolderMapping("static", staticFiles, CoreWebView2HostResourceAccessKind.Allow);
            }
        }

        /// <summary>
        /// Gets web view process ids.
        /// </summary>
        /// <returns>Process ids, empty list if web view is not initialized.</returns>
        public IEnumerable<int> GetWebViewProcessIds() => Enumerable.Empty<int>();

        /// <summary>
        /// Gets window handle.
        /// </summary>
        public IntPtr Handle { get; } = IntPtr.Zero;
    }
}
