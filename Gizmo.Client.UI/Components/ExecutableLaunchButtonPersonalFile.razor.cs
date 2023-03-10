using Gizmo.Client.UI.Pages;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
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

        [Parameter]
        public int PersonalFileId { get; set; }

        private Task OnClickPersonalFileButtonHandler()
        {
            return Task.CompletedTask;
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
