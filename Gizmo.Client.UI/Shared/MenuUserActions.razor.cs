using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuUserActions : CustomDOMComponentBase
    {
        [Inject]
        UserService UserService { get; set; }

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