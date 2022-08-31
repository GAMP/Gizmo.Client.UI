using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Components
{
    public partial class QuickLauncher : CustomDOMComponentBase
    {
        public QuickLauncher()
        {            
        }

        [Inject]
        QuickLaunchService QuickLaunchService { get; set; }

    }
}