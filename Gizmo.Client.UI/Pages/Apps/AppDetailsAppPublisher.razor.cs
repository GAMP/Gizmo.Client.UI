﻿using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    public partial class AppDetailsAppPublisher : CustomDOMComponentBase
    {
        private AppEnterpriseViewState _appEnterpriseViewState;

        [Inject]
        AppEnterpriseViewStateLookupService AppEnterpriseViewStateLookupService { get; set; }

        [Parameter]
        public int PublisherId { get; set; }

        [Inject()]
        public AppEnterpriseViewState ViewState
        {
            get { return _appEnterpriseViewState;}
            private set { _appEnterpriseViewState = value; }
        }

        protected override async Task OnInitializedAsync()
        {
            _appEnterpriseViewState = await AppEnterpriseViewStateLookupService.GetStateAsync(PublisherId);
            this.SubscribeChange(_appEnterpriseViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_appEnterpriseViewState);

            base.Dispose();
        }
    }
}
