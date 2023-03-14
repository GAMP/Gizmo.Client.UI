using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_HOME)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_HOME_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_HOME_TITLE"), ModuleDisplayOrder(0)]
    [Route(ClientRoutes.HomeRoute)]
    public partial class Home : CustomDOMComponentBase, IAsyncDisposable
    {
        public Home()
        {
        }

        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        HomePageService HomePageService { get; set; }

        [Inject]
        HomePageViewState ViewState { get; set; }

        [Inject()]
        IClientDialogService DialogService { get; set; }

        #endregion

        #region METHODS

        public async Task OpenExecutableSelector(int id)
        {
            var s = await DialogService.ShowExecutableSelectorDialogAsync(id);
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

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InvokeVoidAsync("registerHomeAdsAutoCollapse");
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterHomeAdsAutoCollapse", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}
