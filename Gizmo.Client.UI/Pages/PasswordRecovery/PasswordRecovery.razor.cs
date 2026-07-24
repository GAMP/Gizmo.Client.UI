using Gizmo.Client.UI.Components.Login;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public partial class PasswordRecovery : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        PasswordRecoveryViewService PasswordRecoveryViewService { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        [Inject]
        PasswordRecoveryViewState ViewState { get; set; }

        [Inject]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState { get; init; }

        [Inject]
        NavigationService NavigationService { get; set; }

        private IReadOnlyList<ProviderOption> PriorityOptions =>
            ViewState.PriorityProviders.Select(ToOption).ToList();

        private IReadOnlyList<ProviderOption> AltOptions =>
            ViewState.AltProviders.Select(ToOption).ToList();

        public Task SelectProvider(Guid publicId) =>
            PasswordRecoveryViewService.SelectProviderAsync(publicId);

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserRegisterConfigurationViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(UserRegisterConfigurationViewState);
            base.Dispose();
        }

        private static ProviderOption ToOption(PasswordRecoveryProvider provider) =>
            new(provider.PublicId, provider.Name, provider.ChannelGuid, provider.IsPrimary);
    }
}
