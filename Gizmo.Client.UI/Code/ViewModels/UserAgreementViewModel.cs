using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class UserAgreementViewModel : ViewModelBase
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }
    }
}