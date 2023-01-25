using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserPurchasesRoute)]
    public partial class Purchases : ComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        PurchasesService PurchasesService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await PurchasesService.LoadPurchasesAsync();

            await base.OnInitializedAsync();
        }
    }
}