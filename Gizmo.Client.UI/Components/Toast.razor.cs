using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;

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

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-toast")
                 .Add($"giz-toast--{AlertType.ToDescriptionString()}")
                 .AsString();

        #endregion
    }
}
