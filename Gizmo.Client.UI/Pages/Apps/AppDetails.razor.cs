using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Pages
{
    [Route("/appdetails/{ApplicationId:int}")]
    public partial class AppDetails : ComponentBase
    {
        public AppDetails()
        {
        }

        [Parameter]
        public int ApplicationId { get; set; }

        [Inject]
        ApplicationDetailsPageService ApplicationDetailsPageService { get; set; }
    }
}