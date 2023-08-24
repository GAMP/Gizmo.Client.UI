using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    public partial class ProductsProductHosts : CustomDOMComponentBase
    {
        private IEnumerable<UserHostGroupViewState> _hostGroups;
        private ElementReference _hostGroupContainer;

        [Inject]
        UserHostGroupViewStateLookupService UserHostGroupViewStateLookupService { get; set; }

        [Inject]
        HostGroupViewState HostGroupViewState { get; set; }

        [Parameter]
        public TimeProductViewState TimeProduct { get; set; }

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
            await InvokeVoidAsync("userTimeProductsFitHostGroups", _hostGroupContainer);

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion
    }
}
