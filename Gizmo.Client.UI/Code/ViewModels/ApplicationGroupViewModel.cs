using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class ApplicationGroupViewModel
    {
        public int Id { get; set; }
        public int? ParentGroupId { get; set; }
        public string Name { get; set; }
    }
}