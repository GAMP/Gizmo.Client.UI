using Gizmo.Shared.UI.Services;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class App : ComponentBase
    {
        #region CONSTRUTCOR
        public App() : base()
        {
        }
        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// Component discovery service.
        /// </summary>
        [Inject()]
        public IComponentDiscoveryService ComponentDiscoveryService
        {
            get;
            protected set;
        }

        [Inject()]
        private NavigationManager NavigationManager
        {
            get;set;
        }

        [Inject()]
        private NavigationService NavigationService
        {
            get;set;
        }

        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();
            NavigationService.AssociateNavigtionManager(NavigationManager);
        }
    }   
}
