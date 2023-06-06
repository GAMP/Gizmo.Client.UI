using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class NotificationsHost : CustomDOMComponentBase
    {
        private bool _slideOut = false;
        private int _total = 1;
        private int _tmp = -1;
        private int _slideInId = -1;
        private int _slideOutId = -1;
        private List<int> _newItems = new List<int>();
        private List<int> _removedItems = new List<int>();
        private readonly SemaphoreSlim _animationLock = new(1);

        private List<INotificationController> _visible;

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

        private void OnMouseOverHandler(MouseEventArgs args)
        {
            NotificationsService.SuspendTimeOutAll();
        }

        private void OnMouseOutHandler(MouseEventArgs args)
        {
            NotificationsService.ResumeTimeOutAll();
        }

        private async Task CloseNotifications()
        {
            NotificationsService.DismissAll();
        }

        private async Task SetAnimationHeight(int item)
        {
            await InvokeVoidAsync("setNotificationsAnimationHeight", item);
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
            if (await _animationLock.WaitAsync(TimeSpan.Zero))
            {
                try
                {
                    _newItems.Clear();
                    _removedItems.Clear();

                    //TODO: AAA BLOCK UNTIL FINISHED.
                    if (_visible == null)
                    {
                        _visible = ViewState.Visible.ToList();
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

                                foreach (var item in _newItems)
                                {
                                    _tmp = item;
                                    _visible.Add(ViewState.Visible.Where(a => a.Identifier == item).FirstOrDefault());
                                    await InvokeAsync(StateHasChanged);

                                    _tmp = -1;
                                    await SetAnimationHeight(item);
                                    _slideInId = item;
                                    await InvokeAsync(StateHasChanged);
                                    await Task.Delay(500);
                                }

                                _slideInId = -1;

                                foreach (var item in _removedItems)
                                {
                                    await SetAnimationHeight(item);
                                    _slideOutId = item;
                                    await InvokeAsync(StateHasChanged);
                                    await Task.Delay(500);
                                }

                                _slideOutId = -1;
                            }
                        }
                        else
                        {
                            //TODO: AAA THIS DOES NOT WORK, ViewState_OnChange FIRES MULTIPLE TIMES ON DISMISS ALL.
                            _slideOut = true;
                            await InvokeAsync(StateHasChanged);
                            await Task.Delay(1000);
                        }

                        _visible = ViewState.Visible.ToList();
                        await InvokeAsync(StateHasChanged);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _animationLock.Release();
                }
            }
        }
    }
}
