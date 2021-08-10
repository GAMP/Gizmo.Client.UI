using Gizmo.Client.UI.Code.ViewModels.Page;
using Gizmo.Client.UI.Components.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    public partial class Home //: PageComponentWithModelBase<HomePageViewModel>
    {
        [Inject()]
        IJSRuntime JSRuntime { get; set; }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JSRuntime.InvokeVoidAsync("initCustomScroll");

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
