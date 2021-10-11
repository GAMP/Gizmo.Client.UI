using Gizmo.Shared.ViewModels;
using Gizmo.Web.Components;

namespace Gizmo.Client.UI.ViewModels
{
    public class PaymentMethodViewModel : ViewModelBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Icons Icon { get; set; }

    }
}