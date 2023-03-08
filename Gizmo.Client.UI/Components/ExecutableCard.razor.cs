using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableCard : CustomDOMComponentBase
    {
        private AppExeViewState _previousExecutable;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        public ActiveApplicationsService ActiveApplicationsService { get; set; }

        [Parameter]
        public AppExeViewState Executable { get; set; }

        #region OVERRIDES

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            var executableChanged = _previousExecutable != Executable;

            if (executableChanged)
            {
                if (_previousExecutable != null)
                {
                    //The same component used again with a different executable.
                    //We have to unbind from the old executable.
                    this.UnsubscribeChange(_previousExecutable);
                }

                _previousExecutable = Executable;

                //We have to bind to the new executable.
                this.SubscribeChange(Executable);
            }
        }

        public override void Dispose()
        {
            if (Executable != null)
            {
                this.UnsubscribeChange(Executable);
            }
            base.Dispose();
        }

        #endregion
    }
}
