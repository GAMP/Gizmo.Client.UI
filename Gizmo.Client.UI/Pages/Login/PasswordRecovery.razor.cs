﻿using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public partial class PasswordRecovery : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserPasswordRecoveryViewStateService UserPasswordRecoveryService { get; set; }

        [Inject]
        UserVerificationViewState UserVerificationViewState { get; set; }

        [Inject]
        UserLoginViewStateService UserLoginService { get; set; }

        [Inject]
        UserPasswordRecoveryViewState ViewState { get; set; }

        [Inject()]
        HostConfigurationViewState HostConfigurationViewState
        {
            get;init;
        }

        [Inject]
        NavigationService NavigationService { get; set; }

        private void SelectRecoveryMethod(ICollection<Button> selectedItems)
        {
            if (selectedItems.Where(a => a.Name == "Email").Any())
                UserPasswordRecoveryService.SetSelectedRecoveryMethod(UserRecoveryMethod.Email);
            else
                UserPasswordRecoveryService.SetSelectedRecoveryMethod(UserRecoveryMethod.Mobile);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserVerificationViewState);
            this.SubscribeChange(HostConfigurationViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserVerificationViewState);
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(HostConfigurationViewState);

            base.Dispose();
        }
    }
}
