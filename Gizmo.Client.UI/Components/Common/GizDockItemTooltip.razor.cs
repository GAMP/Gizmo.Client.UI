﻿using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class GizDockItemTooltip : CustomDOMComponentBase
    {
        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public AppExeViewState Executable { get; set; }
    }
}