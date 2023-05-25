﻿using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Pages
{
    public partial class ProductDetailsBundledProduct : CustomDOMComponentBase
    {
        private UserProductViewState _userProductViewState;

        [Inject]
        public UserProductViewState UserProductViewState
        {
            get { return _userProductViewState; }
            private set { _userProductViewState = value; }
        }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public int ProductId { get; set; }
        protected override async Task OnInitializedAsync()
        {
            _userProductViewState = await UserProductViewStateLookupService.GetStateAsync(ProductId);

            if (_userProductViewState != null)
            {
                this.SubscribeChange(_userProductViewState);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_userProductViewState != null)
            {
                this.UnsubscribeChange(_userProductViewState);
            }

            base.Dispose();
        }
    }
}
