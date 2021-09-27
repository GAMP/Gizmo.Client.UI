using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ApplicationFilterCard : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public ApplicationFilterViewModel ApplicationFilter { get; set; }

        #endregion
    }
}
