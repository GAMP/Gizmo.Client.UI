using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route("/registrationconfirmationmethod")]
    public partial class RegistrationConfirmationMethod : ComponentBase
    {
        [Inject]
        UserRegistrationConfirmationMethodService UserRegistrationConfirmationMethodService { get; set; }

        [Inject()]
        DialogService DialogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CancellationTokenSource tcs = new CancellationTokenSource();
            var s = await DialogService.ShowDialogAsync<UserAgreementDialog>(new Dictionary<string, object>(), default, default, default);
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