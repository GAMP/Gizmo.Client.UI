﻿using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryConfirmationRoute)]
    public partial class PasswordRecoveryConfirmation : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserPasswordRecoveryViewStateService UserPasswordRecoveryService { get; set; }

        [Inject]
        UserPasswordRecoveryConfirmationViewStateService UserPasswordRecoveryConfirmationService { get; set; }

        [Inject]
        UserLoginViewStateService UserLoginService { get; set; }

        [Inject]
        HostNumberViewStateService HostService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        UserPasswordRecoveryConfirmationViewState ViewState { get; set; }

        [Inject]
        UserPasswordRecoveryViewState UserPasswordRecoveryViewState { get; set; }

        [Inject]
        UserVerificationFallbackViewState UserVerificationFallbackViewState { get; set; }

        [Inject()]
        HostConfigurationViewState HostConfigurationViewState
        {
            get; init;
        }


        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserVerificationFallbackViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserVerificationFallbackViewState);
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
