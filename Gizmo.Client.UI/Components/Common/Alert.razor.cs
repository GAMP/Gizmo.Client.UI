using Gizmo.UI;
using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class Alert : CustomDOMComponentBase
    {
        [Parameter]
        public AlertTypes AlertType { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Text { get; set; }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-alert")
                 .Add($"giz-alert--{AlertType.ToDescriptionString()}")
                 .AsString();

        #endregion
    }
}
