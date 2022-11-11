using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class ClientLanguageMenu : CustomDOMComponentBase
    {
        [Inject()]
        public ClientLanguagesService ClientLanguagesService
        {
            get; init;
        }

        private void ValueChangedHandler(RegionViewState value)
        {
            ClientLanguagesService.SetCurrentRegionAsync(value.TwoLetterISORegionName);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ClientLanguagesService.ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ClientLanguagesService.ViewState);
            base.Dispose();
        }
    }
}