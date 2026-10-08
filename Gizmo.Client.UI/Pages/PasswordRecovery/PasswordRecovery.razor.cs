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
        PasswordRecoveryViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        private IReadOnlyList<ProviderOption> PriorityOptions =>
            ViewState.PriorityProviders.Select(ToOption).ToList();

        private IReadOnlyList<ProviderOption> AltOptions =>
            ViewState.AltProviders.Select(ToOption).ToList();

        public Task SelectProvider(int methodId) =>
            PasswordRecoveryViewService.SelectProviderAsync(methodId);

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

        private static ProviderOption ToOption(PasswordRecoveryProvider provider) =>
            new(provider.MethodId, provider.Name, provider.ChannelGuid, provider.IsPrimary);
    }
}
