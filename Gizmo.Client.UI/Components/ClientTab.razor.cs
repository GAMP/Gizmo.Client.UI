﻿using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ClientTab : CustomDOMComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
