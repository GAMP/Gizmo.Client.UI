using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductTimeHostGroups : CustomDOMComponentBase
    {
        private IEnumerable<UserHostGroupViewState> _hostGroups;
        private ElementReference _hostGroupContainer;

        [Inject]
        UserHostGroupViewStateLookupService UserHostGroupViewStateLookupService { get; set; }

        [Inject]
        HostGroupViewState HostGroupViewState { get; set; }

        [Parameter]
        public UserProductViewState Product { get; set; }

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            if (HostGroupViewState.HostGroupId.HasValue)
            {
                var hostGroups = await UserHostGroupViewStateLookupService.GetStatesAsync();
                var tmp = hostGroups.Where(a => a.Id != HostGroupViewState.HostGroupId.Value).ToList();
                var current = hostGroups.Where(a => a.Id == HostGroupViewState.HostGroupId.Value).FirstOrDefault();
                if (current != null)
                {
                    tmp.Insert(0, current);
                }
                _hostGroups = tmp;
            }
            else
            {
                _hostGroups = await UserHostGroupViewStateLookupService.GetStatesAsync();
            }

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InvokeVoidAsync("productDetailsFitHostGroups", _hostGroupContainer);

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion
    }
}
