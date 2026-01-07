using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.FileProviders;
using Microsoft.Web.WebView2.Core;

namespace Gizmo.Client.UI.Host.WPF
{
    public class CustomBlazor : BlazorWebView
    {
        public CustomBlazor(params IFileProvider[] fileProviders)
        {
            _fileProviders = fileProviders;
        }

        public IFileProvider[] _fileProviders;

        public override IFileProvider CreateFileProvider(string contentRootDir)
        {
            var allProviders = _fileProviders.Append(base.CreateFileProvider(contentRootDir));
            return new CompositeFileProvider(allProviders);
        }
    }

    /// <summary>
    /// Interaction logic for NotificationsHost.xaml
    /// </summary>
    public partial class NotificationsHost : Window, INotificationsHost
    {
        public NotificationsHost(IUICompositionService uICompositionService,
            INotificationsService notificationsService,
            IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _uICompositionService = uICompositionService;
            _serviceProvider = serviceProvider;
            AllowsTransparency = true;
            Background = System.Windows.Media.Brushes.Transparent;
            WindowStyle = WindowStyle.None;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _notificationsService = notificationsService;
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
            windowInteropHelper.EnsureHandle();
        }

        private readonly IUICompositionService _uICompositionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationsService _notificationsService;

        private bool isOpen = false;

        public async Task HideAsync()
        {
            if (!isOpen)
            {
                return;
            }

            await Task.Delay(1000);
            Dispatcher.Invoke(Hide);


            isOpen = false;
            return;
        }

        public async Task ShowAsync()
        {
            if (isOpen)
            {
                return;
            }

            isOpen = true;

            var appAssembly = _uICompositionService.AppAssembly;
            var notificationsComponent = _uICompositionService.NotificationsComponentType;

            var manifestEmbeddedProvider = new ManifestEmbeddedFileProvider(appAssembly, @"wwwroot");

            var blazorWebView = await Dispatcher.InvokeAsync(() =>
            {
                var blazorWebView = new CustomBlazor([manifestEmbeddedProvider])
                {
                    Services = _serviceProvider,
                    MaxHeight = 600,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    BlazorWebViewInitialized = BlazorWebViewInit,
                };

                blazorWebView.RootComponents.Add(new RootComponent()
                {
                    ComponentType = _uICompositionService.NotificationsComponentType,
                    Selector = "#app",
                });

                blazorWebView.RootComponents.Add(new RootComponent()
                {
                    ComponentType = typeof(HeadOutlet),
                    Selector = "head::after",
                });

                blazorWebView.BlazorWebViewInitializing += (sender, args) =>
                {
                    if (sender is BlazorWebView view)
                    {
                        view.MinHeight = 400;
                        view.BlazorWebViewInitializing -= (sender, args) => { };
                        view.WebView.DefaultBackgroundColor = System.Drawing.Color.Transparent;
                    }
                };

                return blazorWebView;
            });

            Dispatcher.Invoke(() =>
            {
                _VIEW_HOST.Child = blazorWebView;
                blazorWebView.HostPage = @"wwwroot\notifications.html";
            });
            Dispatcher.Invoke(Show); 
        }

        private void BlazorWebViewInit(object sender, BlazorWebViewInitializedEventArgs e)
        {
            if (sender is BlazorWebView view)
            {
                var staticFiles = Path.Combine(Environment.CurrentDirectory, "static");
                if (Directory.Exists(staticFiles))
                {
                    //map static folder
                    view.WebView.CoreWebView2.SetVirtualHostNameToFolderMapping("static", staticFiles, CoreWebView2HostResourceAccessKind.Allow);
                }
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            //get window handle
            IntPtr handle = new WindowInteropHelper(this).Handle;

            //int exStyle = GetWindowLong(handle, GWL_EXSTYLE);
            //SetWindowLong(handle, GWL_EXSTYLE, exStyle | WS_EX_NOACTIVATE);

            //get hwndSource
            var source = HwndSource.FromHwnd(handle);

            //add window proc hook
            source.AddHook(new HwndSourceHook(WindowProc));

            _notificationsService.NotificationsChanged += _notificationsService_NotificationsChanged;
        }

        private async void _notificationsService_NotificationsChanged(object sender, NotificationsChangedArgs e)
        {
            var visibleCount = _notificationsService.GetVisible().Count();
            if (visibleCount > 0 && !isOpen)
            {
                await ShowAsync();
            }

            if (visibleCount <= 0 && isOpen)
            {
                await HideAsync();
            }
        }

        protected virtual IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }
    }
}
