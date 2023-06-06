using Gizmo.UI;
using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class Toast : CustomDOMComponentBase
    {
        [Parameter]
        public AlertTypes AlertType { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public bool CanClose { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnCloseButtonClick { get; set; }

        public Task OnCloseButtonClickHandler(MouseEventArgs args)
        {
            return OnCloseButtonClick.InvokeAsync(args);
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-toast")
                 .Add($"giz-toast--{AlertType.ToDescriptionString()}")
                 .AsString();

        #endregion
    }
}
