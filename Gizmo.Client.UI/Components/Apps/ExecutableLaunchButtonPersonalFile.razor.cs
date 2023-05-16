using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableLaunchButtonPersonalFile : CustomDOMComponentBase
    {
        private PersonalFileViewState _personalFileViewState;

        [Inject]
        PersonalFileViewStateLookupService PersonalFileViewStateLookupService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject()]
        AppExecutionService AppExecutionService { get; set; }
        
        [Inject()]
        public PersonalFileViewState PersonalFileViewState
        {
            get { return _personalFileViewState; }
            private set { _personalFileViewState = value; }
        }

        [Parameter]
        public int ExecutableId { get; set; }

        [Parameter]
        public int PersonalFileId { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        private async Task OnClickPersonalFileButtonHandler(MouseEventArgs args)
        {
            await OnClick.InvokeAsync(args);
            await AppExecutionService.PersonalFileExploreAsync(ExecutableId, PersonalFileId);
        }

        protected override async Task OnInitializedAsync()
        {
            _personalFileViewState = await PersonalFileViewStateLookupService.GetStateAsync(PersonalFileId);
            this.SubscribeChange(_personalFileViewState);
            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_personalFileViewState);
            base.Dispose();
        }
    }
}
