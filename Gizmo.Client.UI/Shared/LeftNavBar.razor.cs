using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class LeftNavBar : ComponentBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets component discovery service.
        /// </summary>
        [Inject()]
        public IComponentDiscoveryService ComponentDiscoveryService
        {
            get;
            init;
        }

        #endregion
    }
}
