﻿using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    public partial class AppDetailsAppCategory : CustomDOMComponentBase
    {
        private AppCategoryViewState _appCategoryViewState;

        [Inject]
        AppCategoryViewStateLookupService AppCategoryViewStateLookupService { get; set; }

        [Parameter]
        public int ApplicationCategoryId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _appCategoryViewState = await AppCategoryViewStateLookupService.GetStateAsync(ApplicationCategoryId);
            this.SubscribeChange(_appCategoryViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_appCategoryViewState);

            base.Dispose();
        }
    }
}