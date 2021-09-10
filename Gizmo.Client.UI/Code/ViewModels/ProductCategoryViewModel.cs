using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class ProductCategoryViewModel : ViewModelBase
    {
        public int Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int Products
        {
            get; set;
        }
    }
}