using System.Windows;

namespace Gizmo.Client.UI.Host.WPF
{
    /// <summary>
    /// Interaction logic for HostWindow.xaml
    /// </summary>
    public partial class HostWindow : Window , IHostWindow
    {
        public HostWindow(IComponentDiscoveryService componentDiscoveryService)
        {
            InitializeComponent();
            _ROOT_COMPONENT.ComponentType = componentDiscoveryService.RootComponentType;
        }        
    }
}
