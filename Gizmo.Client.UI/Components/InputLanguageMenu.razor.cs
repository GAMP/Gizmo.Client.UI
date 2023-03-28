using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class InputLanguageMenu : CustomDOMComponentBase
    {
        [Inject()]
        public InputLanguagesViewStateService InputLanguagesService { get; set; }

        [Inject()]
        public InputLanguagesViewState ViewState { get; set; }

        private Task ValueChangedHandler(LanguageViewState value)
        {
            return InputLanguagesService.SetCurrentRegionAsync(value.Culture.TwoLetterISOLanguageName);
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
