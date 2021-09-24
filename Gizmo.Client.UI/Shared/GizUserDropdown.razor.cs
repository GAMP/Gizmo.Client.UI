using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class GizUserDropdown : CustomDOMComponentBase
    {

        protected Task OnClickPreviousButtonHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected Task OnClickNextButtonHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}