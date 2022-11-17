﻿using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.Services;

namespace Gizmo.Client.UI.Pages
{
    [Route("/registrationconfirmationmethod")]
    public partial class RegistrationConfirmationMethod : ComponentBase
    {
        [Inject]
        UserRegistrationConfirmationMethodService UserRegistrationConfirmationMethodService { get; set; }

        [Inject()]
        IClientDialogService DialogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var s = await DialogService.ShowUserAgreementDialogAsync();
            if (s.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await s.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}