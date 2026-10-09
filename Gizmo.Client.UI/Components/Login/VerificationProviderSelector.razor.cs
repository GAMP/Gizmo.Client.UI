using Gizmo;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using System;
using System.Collections.Generic;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components.Login
{
    public partial class VerificationProviderSelector : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public IReadOnlyList<ProviderOption> PriorityProviders { get; set; } = Array.Empty<ProviderOption>();

        [Parameter]
        public IReadOnlyList<ProviderOption> AltProviders { get; set; } = Array.Empty<ProviderOption>();

        [Parameter]
        public bool HasError { get; set; }

        [Parameter]
        public string ErrorMessage { get; set; } = string.Empty;

        [Parameter]
        public bool ShowAll { get; set; }

        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public string Subtitle { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<int> OnSelect { get; set; }

        [Parameter]
        public bool ShowHeading { get; set; } = true;

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        private string HintOf(ProviderOption provider)
        {
            var channel = provider.ChannelGuid.ToString("D");

            if (channel.Equals(CommunicationChannels.Telegram, StringComparison.OrdinalIgnoreCase))
                return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_WAY_TELEGRAM);

            if (channel.Equals(CommunicationChannels.Sms, StringComparison.OrdinalIgnoreCase))
                return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_WAY_SMS);

            if (channel.Equals(CommunicationChannels.Email, StringComparison.OrdinalIgnoreCase))
                return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_WAY_EMAIL);

            return null;
        }
    }
}
