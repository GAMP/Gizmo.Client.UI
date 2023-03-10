using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderGlobalSearchResultCard : CustomDOMComponentBase
    {
        private bool _clickHandled = false;

        protected bool _shouldRender;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Inject()]
        IClientDialogService DialogService { get; set; }

        [Parameter]
        public GlobalSearchResultViewState Result { get; set; }

        private void OnClickHandler()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            if (Result.Type == View.SearchResultTypes.Applications)
            {
                NavigationService.NavigateTo(ClientRoutes.ApplicationDetailsRoute + $"?ApplicationId={ Result.Id.ToString() }");
            }
            else
            {
                NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + $"?ProductId={ Result.Id.ToString() }");
            }
        }

        private async Task OnClickActionButtonHandler()
        {
            _clickHandled = true;

            if (Result.Type == View.SearchResultTypes.Applications)
            {
                var s = await DialogService.ShowExecutableSelectorDialogAsync(Result.Id);
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
            else
            {
                await UserCartService.AddUserCartProductAsync(Result.Id);
            }
        }

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<GlobalSearchResultViewState>(nameof(Result), out var newResult))
            {
                var resultChanged = !EqualityComparer<GlobalSearchResultViewState>.Default.Equals(Result, newResult);
                if (resultChanged)
                {
                    _shouldRender = true;
                }
            }

            await base.SetParametersAsync(parameters);
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion
    }
}
