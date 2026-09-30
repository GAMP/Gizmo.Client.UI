using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class Layout_LoginServerConnection : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ClientConnectionViewState ViewState { get; set; }

        private string ClassName => ViewState.IsConnected ? "giz-server" : "giz-server giz-server--offline";

        private string StatusText => GrafitLocalization.GetString(ViewState.IsConnected
            ? GrafitResourceKeys.SHELL_SERVER_ONLINE
            : GrafitResourceKeys.SHELL_SERVER_OFFLINE);

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
