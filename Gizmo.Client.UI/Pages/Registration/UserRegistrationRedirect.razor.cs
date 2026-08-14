using System;
using System.Threading.Tasks;
using Gizmo;
using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationRedirectRoute)]
    public partial class UserRegistrationRedirect : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserRegistrationRedirectViewService RegistrationRedirectViewService { get; set; }

        [Inject]
        UserRegistrationRedirectViewState ViewState { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        public void OnCloseErrorHandler()
        {
            RegistrationRedirectViewService.Reset();
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

        private string GetProviderDisplayName()
        {
            return RegistrationSession.SelectedProvider?.Name ?? string.Empty;
        }

        private Icons GetProviderIcon()
        {
            var channelGuid = RegistrationSession.SelectedProvider?.ChannelGuid ?? Guid.Empty;
            return ChannelIcons.ResolveChannelIcon(channelGuid);
        }

        private string GetProviderIconCssClass()
        {
            var channelGuid = RegistrationSession.SelectedProvider?.ChannelGuid.ToString("D") ?? string.Empty;
            if (channelGuid.Equals(CommunicationChannels.Telegram, StringComparison.OrdinalIgnoreCase))
                return "giz-registration-redirect__provider-icon--telegram";
            if (channelGuid.Equals(CommunicationChannels.FacebookMessenger, StringComparison.OrdinalIgnoreCase))
                return "giz-registration-redirect__provider-icon--facebook";
            return string.Empty;
        }
    }
}
