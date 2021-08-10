using Gizmo.Client.UI.Code.ViewModels.Page;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    /// <summary>
    /// Main layout component.
    /// </summary>
    public partial class MainLayout : LayoutComponentBase
    {
        #region PROPERTIES
        [Inject()]
        public AppPageViewModel ViewModel
        {
            get; protected set;
        }
        #endregion


    }
}
