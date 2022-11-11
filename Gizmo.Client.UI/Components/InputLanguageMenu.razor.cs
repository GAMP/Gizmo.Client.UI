using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class InputLanguageMenu : CustomDOMComponentBase
    {
        [Inject()]
        public InputLanguagesService InputLanguagesService
        {
            get; init;
        }

        private void ValueChangedHandler(RegionViewState value)
        {
            InputLanguagesService.SetCurrentRegionAsync(value.TwoLetterISORegionName);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(InputLanguagesService.ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(InputLanguagesService.ViewState);
            base.Dispose();
        }
    }
}
