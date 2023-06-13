using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class ApplicationCardHover : CustomDOMComponentBase
    {
        #region FIELDS

        public AppViewState _appViewState;
        public IEnumerable<AppExeViewState> _executables = Enumerable.Empty<AppExeViewState>();

        private IGizmoClient _client;

        #endregion

        [Inject]
        public AppViewState AppViewState
        {
            get { return _appViewState; }
            private set { _appViewState = value; }
        }

        [Inject]
        public AppViewStateLookupService AppViewStateLookupService { get; set; }

        [Inject]
        public AppExeViewStateLookupService AppExeViewStateLookupService { get; set; }

        [Parameter]
        public int ApplicationId { get; set; }

        [Inject()]
        private IGizmoClient Client
        {
            get { return _client; }
            set { _client = value; }
        }

        #region OVERRIDE

        protected override async Task OnInitializedAsync()
        {
            _appViewState = await AppViewStateLookupService.GetStateAsync(ApplicationId);

            if (_appViewState != null)
            {
                this.SubscribeChange(_appViewState);
            }

            _executables = await AppExeViewStateLookupService.GetFilteredStatesAsync(ApplicationId);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_appViewState != null)
            {
                this.UnsubscribeChange(_appViewState);
            }

            base.Dispose();
        }

        #endregion
    }
}
