using System;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderModulesMenu : ComponentBase
    {
        [Inject()]
        private PageModulesViewState ViewState
        {
            get; set;
        }

        // GGBook gated two of their own nav items (Cases, Tasks) behind a lookup
        // against the GGBook server config. Neither module exists on V3, so the
        // gate is dropped and every discovered page module is shown.
        private static bool ShouldShowModule(string guid) => true;
    }
}
