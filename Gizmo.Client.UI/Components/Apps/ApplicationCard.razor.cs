using System.Threading.Tasks;

using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class ApplicationCard : CustomDOMComponentBase
    {
        private bool _isVisibleImageContent;
        private bool _clickHandled = false;

        #region PROPERTIES

        [Inject]
        AppDetailsPageViewState AppDetailsPageViewState { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Parameter]
        public AppViewState Application { get; set; }

        #endregion

        #region METHODS

        public void OpenDetails()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            if (!AppDetailsPageViewState.DisableAppDetails)
                NavigationService.NavigateTo(ClientRoutes.ApplicationDetailsRoute + $"?ApplicationId={Application.ApplicationId.ToString()}");
        }

        public void Ignore()
        {
            _clickHandled = true;
        }

        private Task OnVisibleImageContent(MouseEventArgs _)
        {
            if (!_isVisibleImageContent)
            {
                _isVisibleImageContent = true;
                StateHasChanged();
            }
            return Task.CompletedTask;
        }

        #endregion
    }
}
