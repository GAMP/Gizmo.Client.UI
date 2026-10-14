using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gizmo.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class SearchAppLine : CustomDOMComponentBase
    {
        [Inject]
        AppViewStateLookupService AppViewStateLookupService { get; set; }

        [Inject]
        AppCategoryViewStateLookupService AppCategoryViewStateLookupService { get; set; }

        [Inject]
        AppDetailsPageViewState AppDetailsPageViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Parameter]
        public int ApplicationId { get; set; }

        [Parameter]
        public IReadOnlyList<int> ExecutableIds { get; set; } = Array.Empty<int>();

        [Parameter]
        public string Pattern { get; set; }

        private AppViewState _app;

        private AppCategoryViewState _category;

        protected string MetaText => _category?.Name;

        private void OpenApp()
        {
            if (AppDetailsPageViewState.DisableAppDetails)
                return;

            NavigationService.NavigateTo(ClientRoutes.ApplicationDetailsRoute + $"?ApplicationId={ApplicationId}");
        }

        protected override async Task OnInitializedAsync()
        {
            _app = await AppViewStateLookupService.GetStateAsync(ApplicationId);

            if (IsDisposed)
            {
                _app = null;
                return;
            }

            if (_app is not null)
            {
                this.SubscribeChange(_app);
                _category = await AppCategoryViewStateLookupService.GetStateAsync(_app.ApplicationCategoryId);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_app is not null)
                this.UnsubscribeChange(_app);

            base.Dispose();
        }
    }
}
