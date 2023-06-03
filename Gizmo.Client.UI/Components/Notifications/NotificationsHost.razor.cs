using System.Linq;
using System.Threading.Tasks;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class NotificationsHost : ComponentBase
    {
        private bool _slideOut = false;
        private int _total = 1;
        private int _slideInIndex = -1;
        private int _slideOutIndex = -1;

        [Inject()]
        private NotificationsHostViewState ViewState
        {
            get; set;
        }

        [Inject()]
        private INotificationsService  NotificationsService
        {
            get; 
            set; 
        }

        private void DemoAdd()
        {
            _slideOutIndex = -1;
            _slideInIndex = _total;
            _total += 1;
        }

        private void CloseNotifications()
        {
            NotificationsService.DismissAll();
        }

        private async Task OnCloseHandler(int index)
        {
            _slideOutIndex = index;
            _slideInIndex = -1;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(500);
            _total -= 1;
            _slideOutIndex = -1;
            await InvokeAsync(StateHasChanged);
        }

        protected override Task OnParametersSetAsync()
        {
            return base.OnParametersSetAsync();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ViewState.OnChange += ViewState_OnChange;
            this.SubscribeChange(ViewState); 
        }

        private void ViewState_OnChange(object sender, System.EventArgs e)
        {
            var newCount = ViewState.Visible.Count();

            if(newCount > 0 && _slideOut)
            {
                _slideOut = false;
            }
            else if(newCount <=0 && 
                !_slideOut)
            {
                _slideOut = true;
            }
        }
    }
}
