using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ApplicationCard : CustomDOMComponentBase
    {
        private bool _warningDialogIsOpen;

        #region PROPERTIES

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Parameter]
        public ApplicationViewModel Application { get; set; }

        [Parameter]
        public EventCallback<int> OnOpenDetails { get; set; }

        [Parameter]
        public bool ShowDateAdded { get; set; }

        #endregion

        #region Methods

        public void OpenDetails()
        {
            NavigationManager.NavigateTo($"/appdetails/{Application.Id}");
        }

        private void Execute()
        {
        }

        private void ShowWarning()
        {
            _warningDialogIsOpen = true;
        }

        #endregion
    }
}
