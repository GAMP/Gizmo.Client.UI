using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class ClientLanguageMenu : CustomDOMComponentBase
    {
        [Inject]
        public ClientLanguagesViewStateService ClientLanguagesService { get; set; }

        [Inject]
        public ClientLanguagesViewState ViewState { get; set; }

        private void ValueChangedHandler(LanguageViewState value)
        {
            ClientLanguagesService.SetCurrentLanguageAsync(value.TwoLetterName);
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
