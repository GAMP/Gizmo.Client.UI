using System;

namespace Gizmo.Client.UI
{
    public static class ChannelIcons
    {
        private static readonly Guid TelegramGuid = new(CommunicationChannels.Telegram);
        private static readonly Guid FacebookMessengerGuid = new(CommunicationChannels.FacebookMessenger);
        private static readonly Guid EmailGuid = new(CommunicationChannels.Email);
        private static readonly Guid SmsGuid = new(CommunicationChannels.Sms);

        public static Icons ResolveChannelIcon(Guid channelGuid)
        {
            if (channelGuid == TelegramGuid)
                return Icons.Telegram_Client;
            if (channelGuid == FacebookMessengerGuid)
                return Icons.Facebook_Client;
            if (channelGuid == EmailGuid)
                return Icons.Email;
            if (channelGuid == SmsGuid)
                return Icons.Sms;

            return Icons.Globe_Client;
        }
    }
}
