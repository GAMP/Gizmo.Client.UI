using System;
using System.Threading.Tasks;

using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ApplicationVirtualizedCards : CustomVirtualizedDOMComponentBase<AppViewState>
    {
        [Inject]
        IClientDialogService DialogService { get; set; }
    }
}
