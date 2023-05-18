using Gizmo.Client.UI.View.Services;
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
        public AppCategoryViewState AppCategoryViewState
        {
            get { return _appCategoryViewState; }
            private set { _appCategoryViewState = value; }
        }

        [Inject]
        AppCategoryViewStateLookupService AppCategoryViewStateLookupService { get; set; }

        [Parameter]
        public int ApplicationCategoryId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _appCategoryViewState = await AppCategoryViewStateLookupService.GetStateAsync(ApplicationCategoryId);

            if (_appCategoryViewState != null)
            {
                this.SubscribeChange(_appCategoryViewState);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_appCategoryViewState != null)
            {
                this.UnsubscribeChange(_appCategoryViewState);
            }

            base.Dispose();
        }
    }
}
