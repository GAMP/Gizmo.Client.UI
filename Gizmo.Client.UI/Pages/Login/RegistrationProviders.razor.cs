using System;
using System.Threading.Tasks;
using Gizmo;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(NavigationHelper.RegistrationProviders)]
    public partial class RegistrationProviders : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        RegistrationProvidersViewState ViewState { get; set; }

        [Inject]
        RegistrationProvidersViewService RegistrationProvidersViewService { get; set; }

        public Task SelectProvider(Guid channelGuid) =>
            RegistrationProvidersViewService.SelectProviderAsync(channelGuid);

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

        private static string GetProviderDisplayName(Guid channelGuid, string fallback)
        {
            var id = channelGuid.ToString("D");
            if (id.Equals(CommunicationChannels.Telegram, StringComparison.OrdinalIgnoreCase))
                return "Telegram";
            if (id.Equals(CommunicationChannels.FacebookMessenger, StringComparison.OrdinalIgnoreCase))
                return "Facebook";
            if (id.Equals(CommunicationChannels.Sms, StringComparison.OrdinalIgnoreCase))
                return "SMS";
            if (id.Equals(CommunicationChannels.WhatsApp, StringComparison.OrdinalIgnoreCase))
                return "WhatsApp";
            if (id.Equals(CommunicationChannels.Viber, StringComparison.OrdinalIgnoreCase))
                return "Viber";
            if (id.Equals(CommunicationChannels.Email, StringComparison.OrdinalIgnoreCase))
                return "Email";
            return fallback;
        }

        private static Icons GetProviderIcon(Guid channelGuid)
        {
            var id = channelGuid.ToString("D");
            if (id.Equals(CommunicationChannels.Telegram, StringComparison.OrdinalIgnoreCase))
                return Icons.Telegram_Client;
            if (id.Equals(CommunicationChannels.FacebookMessenger, StringComparison.OrdinalIgnoreCase))
                return Icons.Facebook_Client;
            return Icons.Globe_Client;
        }

        private static string GetProviderCssClass(Guid channelGuid)
        {
            var id = channelGuid.ToString("D");
            if (id.Equals(CommunicationChannels.Telegram, StringComparison.OrdinalIgnoreCase))
                return "giz-registration-provider-btn--telegram";
            if (id.Equals(CommunicationChannels.FacebookMessenger, StringComparison.OrdinalIgnoreCase))
                return "giz-registration-provider-btn--facebook";
            return string.Empty;
        }
    }
}
