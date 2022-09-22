using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ApplicationCard : CustomDOMComponentBase
    {
        private bool _warningDialogIsOpen;

        private bool _clickHandled = false;

        #region PROPERTIES

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Parameter]
        public ApplicationViewState Application { get; set; }

        [Parameter]
        public EventCallback<int> OnOpenExecutableSelector { get; set; }

        [Parameter]
        public bool ShowDateAdded { get; set; }

        #endregion

        #region Methods

        public void OpenDetails()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            NavigationManager.NavigateTo($"/appdetails/{Application.Id}");
        }

        private async Task Execute()
        {
            _clickHandled = true;

            await OnOpenExecutableSelector.InvokeAsync(Application.Id);
        }

        private void ShowWarning()
        {
            _warningDialogIsOpen = true;
        }

        #endregion
    }
}
