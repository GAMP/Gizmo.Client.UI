using System.Globalization;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class ClientLanguageMenu : CustomDOMComponentBase
    {
        [Inject]
        public ClientLocalizationViewService CultureService { get; set; }

        [Inject]
        public ClientLocalizationViewState ViewState { get; set; }

        private void ValueChangedHandler(CultureInfo culture)
        {
            CultureService.SetCurrentCultureAsync(culture.TwoLetterISOLanguageName);
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
