using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderGlobalSearchResultCard : CustomDOMComponentBase
    {
        protected bool _shouldRender;

        [Inject]
        UserCartService UserCartService { get; set; }

        [Parameter]
        public SearchResultViewState Result { get; set; }

        private Task OnClickActionButtonHandler()
        {
            if (Result.Type == View.SearchResultTypes.Applications)
            {
                //TODO: A LAUNCH APPLICATION
                return Task.CompletedTask;
            }
            else
            {
                return UserCartService.AddProductAsyc(Result.Id);
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
