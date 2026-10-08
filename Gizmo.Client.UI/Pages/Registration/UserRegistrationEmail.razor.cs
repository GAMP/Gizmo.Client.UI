using System.Threading.Tasks;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Microsoft.AspNetCore.Components.Web;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationEmailRoute)]
    public partial class UserRegistrationEmail : ShellComponentBase
    {
        private TextInput<string> _emailInput;

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserRegistrationEmailViewService RegistrationEmailViewService { get; set; }

        [Inject]
        UserRegistrationEmailViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public void OnCloseButtonClickHandler()
        {
            RegistrationEmailViewService.Reset();
        }

        private Task OnKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key != "Enter" || ViewState.IsLoading || ViewState.IsValid == false)
                return Task.CompletedTask;

            return RegistrationEmailViewService.SubmitAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && _emailInput is not null)
                await ElementFocus.TryAsync(() => _emailInput.FocusAsync());

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
