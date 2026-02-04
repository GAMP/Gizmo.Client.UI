using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class Divider : CustomDOMComponentBase
    {
        [Parameter]
        public bool IsVertical { get; set; }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-divider")
                 .If("giz-divider--vertical", () => IsVertical)
                 .AsString();

        #endregion
    }
}
