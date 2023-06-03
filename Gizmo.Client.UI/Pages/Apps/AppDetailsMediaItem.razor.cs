using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    public partial class AppDetailsMediaItem : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Inject]
        IClientDialogService DialogService { get; set; }

        [Parameter]
        public AppLinkViewState Link { get; set; }

        #endregion

        private async Task ShowMediaDialogAsync()
        {
            var dialog = await DialogService.ShowMediaDialogAsync(new MediaDialogParameters()
            {
                Title = Link.Caption,
                MediaUrlType = Link.MediaUrlType,
                MediaUrl = Link.MediaUrl
            });
            if (dialog.Result == AddComponentResultCode.Opened)
                _ = await dialog.WaitForResultAsync();
        }

        protected override void OnInitialized()
        {
            if (Link != null)
            {
                this.SubscribeChange(Link);
            }

            base.OnInitialized();
        }

        public override void Dispose()
        {
            if (Link != null)
            {
                this.UnsubscribeChange(Link);
            }

            base.Dispose();
        }
    }
}
