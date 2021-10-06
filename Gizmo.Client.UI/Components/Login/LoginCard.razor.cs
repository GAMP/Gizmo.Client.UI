using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class LoginCard : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment CardBody { get; set; }

        [Parameter]
        public RenderFragment CardFooter { get; set; }

        [Parameter]
        public bool ShowLanguageMenu { get; set; }

        [Parameter]
        public bool ShowCloseButton { get; set; }
        
        #endregion

        public Task Navigate()
        {
            return Task.CompletedTask;
        }
    }
}