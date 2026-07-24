using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo;
using Gizmo.Client;
using Gizmo.Client.UI.Components.Login;
using Gizmo.Client.UI.Services;
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

        private IReadOnlyList<ProviderOption> PriorityOptions =>
            ViewState.PriorityProviders.Select(ToOption).ToList();

        private IReadOnlyList<ProviderOption> AltOptions =>
            ViewState.AltProviders.Select(ToOption).ToList();

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

        private static ProviderOption ToOption(RegistrationProvider provider) =>
            new(provider.PublicId, provider.Name, provider.ChannelGuid, provider.IsPrimary);
    }
}
