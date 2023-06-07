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
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class NotificationsHost : CustomDOMComponentBase, IAsyncDisposable
    {
        private bool _slideOut = false;
        private int _total = 1;
        private int _newlyAddedItemId = -1;
        private int _slideInItemId = -1;
        private int _slideOutItemId = -1;
        private List<int> _dismissAllItems = new List<int>();
        private List<int> _newItems = new List<int>();
        private List<int> _removedItems = new List<int>();
        private readonly SemaphoreSlim _animationLock = new(1);

        private ILogger<NotificationsHost> _logger;

        [Inject]
        private ILogger<NotificationsHost> Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        private List<INotificationController> _visible;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject()]
        private NotificationsHostViewState ViewState
        {
            get; set;
        }

        [Inject()]
        private INotificationsService NotificationsService
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
            _dismissAllItems = _visible.Select(a => a.Identifier).ToList();
            NotificationsService.DismissAll();

            await SlideWindowOut();

            _visible.Clear();
            _dismissAllItems.Clear();
        }

        private async Task SlideWindowIn()
        {
            _slideOut = false;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(1000);
        }

        private async Task SlideWindowOut()
        {
            _slideOut = true;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(1000);
            _slideOut = false;
        }

        private async Task SlideItemIn(int item)
        {
            await SetAnimationHeight(item);
            _slideInItemId = item;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(500);
            _slideInItemId = -1;
        }

        private async Task SlideItemOut(int item)
        {
            await SetAnimationHeight(item);
            _slideOutItemId = item;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(500);
            _slideOutItemId = -1;
        }

        private async Task SetAnimationHeight(int item)
        {
            await InvokeVoidAsync("setNotificationsAnimationHeight", item);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            //await InvokeVoidAsync("writeLine", $"OnInitializedAsync {this.ToString()}");
            await UpdateUI();

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
            //await InvokeVoidAsync("writeLine", $"ViewState_OnChange {this.ToString()}");
            await UpdateUI();
        }

        private async Task UpdateUI()
        {
            if (await _animationLock.WaitAsync(TimeSpan.Zero))
            {
                try
                {
                    var snapShot = ViewState.Visible.ToList();

                    if (_dismissAllItems.Count == 0)
                    {
                        _newItems.Clear();
                        _removedItems.Clear();

                        if (_visible == null || _visible.Count == 0)
                        {
                            if (snapShot.Count() > 0)
                            {
                                //First run.
                                _visible = snapShot;
                                await SlideWindowIn();
                            }
                            else
                            {
                                await InvokeVoidAsync("writeLine", $"0 items {this.ToString()}");
                            }
                        }
                        else
                        {
                            if (snapShot.Count() > 0)
                            {
                                foreach (var item in snapShot)
                                {
                                    if (!_visible.Contains(item))
                                        _newItems.Add(item.Identifier);
                                }

                                foreach (var item in _visible)
                                {
                                    if (!snapShot.Contains(item))
                                        _removedItems.Add(item.Identifier);
                                }

                                foreach (var item in _newItems)
                                {
                                    //We need to add the item to the DOM first.
                                    //TODO: AAA ADD ITEM IN THE RIGHT POSITION.
                                    _newlyAddedItemId = item;
                                    _visible.Add(snapShot.Where(a => a.Identifier == item).FirstOrDefault());
                                    await InvokeAsync(StateHasChanged);
                                    _newlyAddedItemId = -1;
                                    //await InvokeVoidAsync("writeLine", $"tmpItemAdded {this.ToString()}");

                                    await SlideItemIn(item);
                                }

                                foreach (var item in _removedItems)
                                {
                                    await SlideItemOut(item);
                                }
                            }
                            else
                            {
                                await SlideWindowOut();
                            }

                            _visible = snapShot;
                            await InvokeAsync(StateHasChanged);
                        }
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

        private AnimationEventInterop AnimationEventInterop { get; set; }

        private Task AnimationHandler(AnimationEventArgs args)
        {
            //args.Id is the host component Id or the item Identifier.
            if (args.Id == Id)
            {
                _logger.LogTrace("Notification host animation state changed, new stat {state}", args.AnimationState);
            }

            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            //await InvokeVoidAsync("writeLine", $"OnAfterRenderAsync {this.ToString()}");

            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("registerAnimatedComponent", Ref);
                AnimationEventInterop = new AnimationEventInterop(JsRuntime);
                await AnimationEventInterop.SetupAnimationEventCallback(args => AnimationHandler(args));
            }
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterAnimatedComponent", Ref).ConfigureAwait(false);
            //await InvokeVoidAsync("writeLine", $"DisposeAsync {this.ToString()}");

            if (AnimationEventInterop != null)
                await AnimationEventInterop.DisposeAsync();

            Dispose();
        }

        #endregion
    }
}
