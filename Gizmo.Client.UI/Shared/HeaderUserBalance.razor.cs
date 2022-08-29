using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserBalance : ComponentBase
    {
        private bool _hideBalance;

        [Inject]
        UserService UserService { get; set; }

        private void ToggleBalanceVisibility()
        {
            _hideBalance = !_hideBalance;
        }
    }
}
