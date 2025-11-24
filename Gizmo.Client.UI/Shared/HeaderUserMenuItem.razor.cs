using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderUserMenuItem : CustomDOMComponentBase
    {
        [Inject]
        public IAssemblyResourcesLocalizationService AssemblyResourcesLocalizationService { get; set; }
        
        [Inject]
        public UserMenuViewState UserMenuViewState { get; set; }
        
        [Inject]
        private IServiceProvider ServiceProvider { get; set; }
        
        [Inject]
        private ILogger<HeaderUserMenuItem> Logger { get; set; }
        
        [Inject]
        private IClientDialogService DialogService { get; set; }
        
        [Parameter]
        public UIUserMenuModuleMetadata MetaData { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserMenuViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserMenuViewState);

            base.Dispose();
        }
    }
}
