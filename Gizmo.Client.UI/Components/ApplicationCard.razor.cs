using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ApplicationCard : CustomDOMComponentBase
    {
        private bool _clickHandled = false;

        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Parameter]
        public AppViewState Application { get; set; }

        [Parameter]
        public EventCallback<int> OnOpenExecutableSelector { get; set; }

        #endregion

        #region Methods

        public void OpenDetails()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            NavigationService.NavigateTo(ClientRoutes.ApplicationDetailsRoute + $"?ApplicationId={Application.ApplicationId.ToString()}");
        }

        public void Ignore()
        {
            _clickHandled = true;
        }

        #endregion
    }
}
