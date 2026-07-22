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
    [Route(ClientRoutes.RegistrationProvidersRoute)]
    public partial class UserRegistrationProviders : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserRegistrationProvidersViewState ViewState { get; set; }

        [Inject]
        UserRegistrationProvidersViewService RegistrationProvidersViewService { get; set; }

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
            
            //TODO вынести в отдельный хелпер
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

        private static string GetProviderCssClass(bool isPrimary)
        {
            return isPrimary ? "giz-registration-provider-btn--primary" : string.Empty;
        }
    }
}
