using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ApplicationCards : CustomDOMComponentBase
    {
        [Inject()]
        IClientDialogService DialogService { get; set; }

        [Parameter]
        public IEnumerable<AppViewState> Applications { get; set; } = Enumerable.Empty<AppViewState>();

        public async Task OpenExecutableSelector(int id)
        {
            var s = await DialogService.ShowExecutableSelectorDialogAsync(id);
            if (s.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await s.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}
