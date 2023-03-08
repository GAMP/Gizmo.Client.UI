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
        private int _previousPersonalFileId;
        private PersonalFileViewState _personalFileViewState { get; set; }

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

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            var personalFileIdChanged = _previousPersonalFileId != PersonalFileId;

            if (personalFileIdChanged)
            {
                if (_personalFileViewState != null)
                {
                    //The same component used again with a different personal file id.
                    //We have to unbind from the old personal file.
                    this.UnsubscribeChange(_personalFileViewState);
                }

                _previousPersonalFileId = PersonalFileId;

                //We have to bind to the new personal file.
                _personalFileViewState = await PersonalFileViewStateLookupService.GetStateAsync(PersonalFileId);
                this.SubscribeChange(_personalFileViewState);
            }
        }

        public override void Dispose()
        {
            if (_personalFileViewState != null)
            {
                this.UnsubscribeChange(_personalFileViewState);
            }
            base.Dispose();
        }
    }
}
