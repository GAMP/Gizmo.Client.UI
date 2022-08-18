using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class CarouselNavigation : CustomDOMComponentBase
    {
        [Parameter]
        public string Title { get; set; }

        protected Task OnClickPreviousButtonHandler(MouseEventArgs args)
        {
            return OnClickPreviousButton.InvokeAsync(args);
        }

        protected Task OnClickNextButtonHandler(MouseEventArgs args)
        {
            return OnClickNextButton.InvokeAsync(args);
        }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickPreviousButton { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickNextButton { get; set; }
    }
}