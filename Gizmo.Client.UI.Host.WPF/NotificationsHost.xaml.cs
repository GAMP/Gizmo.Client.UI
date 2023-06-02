using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.FileProviders;

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
    public partial class NotificationsHost : Window , INotificationsHost
    {
        public NotificationsHost(IUICompositionService uICompositionService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _uICompositionService = uICompositionService;
            _serviceProvider = serviceProvider;
            AllowsTransparency = true;
            Background =  System.Windows.Media.Brushes.Transparent;
            WindowStyle = WindowStyle.None;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private readonly IUICompositionService _uICompositionService;
        private readonly IServiceProvider _serviceProvider;

        private bool isOpen = false;

        public Task HideAsyc()
        {
            if(!isOpen)
            {
                return Task.CompletedTask;
            }

            Hide();


            isOpen = false;
            return Task.CompletedTask;
        }

        public Task ShowAsync()
        {
            if(isOpen)
            {
                return Task.CompletedTask;
            }

            isOpen = true;
            
            var appAssembly = _uICompositionService.AppAssembly;
            var notificationsComponent = _uICompositionService.NotificationsComponentType;

            var manifestEmbeddedProvider = new ManifestEmbeddedFileProvider(appAssembly, @"wwwroot");

            var blazorWebView = new CustomBlazor(manifestEmbeddedProvider)
            {
                Services = _serviceProvider,     
                MaxHeight= 600,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            
            blazorWebView.RootComponents.Add(new RootComponent()
            {               
                ComponentType = _uICompositionService.NotificationsComponentType,
                Selector = "#app",
            }); 

            blazorWebView.BlazorWebViewInitializing += (sender, args) =>
            {
                if (sender is BlazorWebView view)
                {                    
                    view.MinHeight = 400;
                    view.BlazorWebViewInitializing -= (sender, args) => { };
                    view.WebView.DefaultBackgroundColor = System.Drawing.Color.Yellow;                
                }
            };
        
      
            Show();
            _VIEW_HOST.Child = blazorWebView;
            blazorWebView.HostPage = @"wwwroot\notifications.html";
            return Task.CompletedTask;
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
        }

        protected virtual IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }
    }
}
