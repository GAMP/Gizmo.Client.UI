using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class Collapse : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Collapse()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Expanded { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-collapse")
                 .If("giz-collapse--expanded", () => Expanded)
                 .AsString();

        #endregion

    }
}
