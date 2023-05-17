using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

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
            if (dialog.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await dialog.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(Link);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Link);

            base.Dispose();
        }
    }
}
