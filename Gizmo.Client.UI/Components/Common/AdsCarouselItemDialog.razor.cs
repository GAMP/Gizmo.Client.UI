using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarouselItemDialog : CustomDOMComponentBase
    {
        [Parameter]
        public AdvertisementViewState AdvertisementViewState { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private async Task CloseDialogAsync()
        {
            await CancelCallback.InvokeAsync();
        }
    }
}
