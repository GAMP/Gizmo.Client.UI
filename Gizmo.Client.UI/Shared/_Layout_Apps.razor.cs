﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class _Layout_Apps : LayoutComponentBase
    {
        [Inject]
        IOptions<ClientInterfaceOptions> ClientUIOptions { get; set; }

        [Inject]
        IJSRuntime JsRuntime { get; set; }

        private async Task OnMainClick(MouseEventArgs e)
        {
            await JsRuntime.InvokeVoidAsync("closeOpenPopups", e);
        }
    }
}
