﻿using Gizmo.Client.UI.Services;
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
        protected bool _shouldRender;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        ExecutableSelectorService ExecutableSelectorService { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Inject()]
        IClientDialogService DialogService { get; set; }

        [Parameter]
        public SearchResultViewState Result { get; set; }

        private void OnClickHandler()
        {
            if (Result.Type == View.SearchResultTypes.Applications)
            {
                NavigationService.NavigateTo(ClientRoutes.ApplicationDetailsRoute.Replace("{ApplicationId:int}", Result.Id.ToString()));
            }
            else
            {
                NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute.Replace("{ProductId:int}", Result.Id.ToString()));
            }
        }

        private async Task OnClickActionButtonHandler()
        {
            if (Result.Type == View.SearchResultTypes.Applications)
            {
                await ExecutableSelectorService.LoadApplicationAsync(Result.Id);

                var s = await DialogService.ShowExecutableSelectorDialogAsync();
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
                await UserCartService.AddProductAsyc(Result.Id);
            }
        }

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<SearchResultViewState>(nameof(Result), out var newResult))
            {
                var resultChanged = !EqualityComparer<SearchResultViewState>.Default.Equals(Result, newResult);
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
