using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using System.Linq;
using Gizmo.Client.Options;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Pages
{
    [Route("/notfound")]
    public partial class NotFoundPage : ComponentBase
    {
        [Inject]
        UserLoginStatusViewState UserLoginStatusViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        IOptions<ClientHomeOptions> ClientHomeOptions { get; set; }

        [Inject]
        IUICompositionService IUICompositionService { get; set; }

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        protected string Title => GrafitLocalization?.GetString(GrafitResourceKeys.SHELL_STATE_NOT_FOUND_TITLE) ?? "Page not found";

        protected string Text => GrafitLocalization?.GetString(GrafitResourceKeys.SHELL_STATE_NOT_FOUND_TEXT) ?? "This section is unavailable or turned off";

        protected string Action => GrafitLocalization?.GetString(GrafitResourceKeys.SHELL_STATE_GO_HOME) ?? "Go to home";

        private void GoHome()
        {
            if (UserLoginStatusViewState.IsLoggedIn)
            {
                var firstCustomModule = IUICompositionService.PageModules.Where(a => a.DisplayOrder < 0).OrderBy(a => a.DisplayOrder).FirstOrDefault();

                if (firstCustomModule != null)
                {
                    NavigationService.NavigateTo(firstCustomModule.DefaultRoute);
                }
                else
                {
                    if (!ClientHomeOptions.Value.Disabled)
                    {
                        NavigationService.NavigateTo(ClientRoutes.HomeRoute);
                    }
                    else
                    {
                        NavigationService.NavigateTo(ClientRoutes.ApplicationsRoute);
                    }
                }
            }
            else
            {
                NavigationService.NavigateTo(ClientRoutes.LoginRoute);
            }
        }
    }
}
