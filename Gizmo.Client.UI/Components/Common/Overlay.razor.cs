using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class Overlay : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Overlay()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnMouseDown { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnMouseUp { get; set; }

        #endregion

        #region EVENTS

        protected void OnClickHandler(MouseEventArgs args)
        {
        }

        protected Task OnMouseDownHandler(MouseEventArgs args)
        {
            return OnMouseDown.InvokeAsync(args);
        }

        protected Task OnMouseUpHandler(MouseEventArgs args)
        {
            return OnMouseUp.InvokeAsync(args);
        }

        protected Task OnContextMenuHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-overlay")
                 .If("giz-overlay--visible", () => Visible)
                 .AsString();

        #endregion

    }
}
