using System;
using System.Threading.Tasks;
using Gizmo;
using Gizmo.Client;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationRedirectRoute)]
    public partial class RegistrationRedirect : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        RegistrationRedirectViewService RegistrationRedirectViewService { get; set; }

        [Inject]
        RegistrationRedirectViewState ViewState { get; set; }

        [Inject]
        UserRegistrationViewState UserRegistrationViewState { get; set; }

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
            var provider = UserRegistrationViewState.SelectedProvider;
            if (provider is null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(provider.Name))
                return provider.Name;

            var channelGuid = provider.ChannelGuid.ToString("D");
            if (channelGuid.Equals(CommunicationChannels.Telegram, StringComparison.OrdinalIgnoreCase))
                return "Telegram";
            if (channelGuid.Equals(CommunicationChannels.FacebookMessenger, StringComparison.OrdinalIgnoreCase))
                return "Facebook";
            if (channelGuid.Equals(CommunicationChannels.Sms, StringComparison.OrdinalIgnoreCase))
                return "SMS";
            if (channelGuid.Equals(CommunicationChannels.WhatsApp, StringComparison.OrdinalIgnoreCase))
                return "WhatsApp";
            if (channelGuid.Equals(CommunicationChannels.Viber, StringComparison.OrdinalIgnoreCase))
                return "Viber";
            if (channelGuid.Equals(CommunicationChannels.Email, StringComparison.OrdinalIgnoreCase))
                return "Email";

            return provider.Name ?? string.Empty;
        }

        private Icons GetProviderIcon()
        {
            var channelGuid = UserRegistrationViewState.SelectedProvider?.ChannelGuid.ToString("D") ?? string.Empty;
            if (channelGuid.Equals(CommunicationChannels.Telegram, StringComparison.OrdinalIgnoreCase))
                return Icons.Telegram_Client;
            if (channelGuid.Equals(CommunicationChannels.FacebookMessenger, StringComparison.OrdinalIgnoreCase))
                return Icons.Facebook_Client;
            return Icons.Globe_Client;
        }

        private string GetProviderIconCssClass()
        {
            var channelGuid = UserRegistrationViewState.SelectedProvider?.ChannelGuid.ToString("D") ?? string.Empty;
            if (channelGuid.Equals(CommunicationChannels.Telegram, StringComparison.OrdinalIgnoreCase))
                return "giz-registration-redirect__provider-icon--telegram";
            if (channelGuid.Equals(CommunicationChannels.FacebookMessenger, StringComparison.OrdinalIgnoreCase))
                return "giz-registration-redirect__provider-icon--facebook";
            return string.Empty;
        }
    }
}
