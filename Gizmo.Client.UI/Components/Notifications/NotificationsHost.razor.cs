using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class NotificationsHost : CustomDOMComponentBase
    {
        private bool _slideOut = false;
        private int _total = 1;
        private List<int> _newItems = new List<int>();
        private List<int> _removedItems = new List<int>();

        private IEnumerable<INotificationController> _visible;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

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

        private void CloseNotifications()
        {
            NotificationsService.DismissAll();
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

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }

        private async void ViewState_OnChange(object sender, System.EventArgs e)
        {
            _newItems.Clear();
            _removedItems.Clear();
            
            //TODO: AAA BLOCK UNTIL FINISHED.
            if (_visible == null)
            {
                _visible = ViewState.Visible;
            }
            else
            {
                if (ViewState.Visible.Count() > 0)
                {
                    if (_slideOut)
                    {
                        _slideOut = false;
                        await InvokeAsync(StateHasChanged);
                        await Task.Delay(1000);
                    }
                    else
                    {
                        foreach (var item in ViewState.Visible)
                        {
                            if (!_visible.Contains(item))
                                _newItems.Add(item.Identifier);
                        }

                        foreach (var item in _visible)
                        {
                            if (!ViewState.Visible.Contains(item))
                                _removedItems.Add(item.Identifier);
                        }

                        await InvokeAsync(StateHasChanged);
                        await Task.Delay(500);
                    }
                }
                else
                {
                    _slideOut = true;
                    await InvokeAsync(StateHasChanged);
                    await Task.Delay(1000);
                }

                _visible = ViewState.Visible;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
