using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class GizRatingCard : CustomDOMComponentBase
    {
        [Parameter]
        public ApplicationViewModel Application { get; set; }

        [Parameter]
        public EventCallback<int> OnOpenDetails { get; set; }

        public async Task OpenDetails()
        {
            await OnOpenDetails.InvokeAsync(Application.Id);
        }
    }
}
