using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class Spinner : CustomDOMComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-animate-spinner")
                 .AsString();

        #endregion
    }
}
