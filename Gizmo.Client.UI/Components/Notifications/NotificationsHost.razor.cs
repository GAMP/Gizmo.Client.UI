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
        private enum Animations
        {
            None,
            WindowSlideIn,
            WindowSlideOut,
            ItemSlideIn,
            ItemSlideOut
        }

        private float _lastItemHeight;
        private System.Drawing.Size _componentSize = new System.Drawing.Size();
        private float _fontSize = 10;
        private bool _isTemp;
        private bool _hidden;
        private bool _shouldRender;
        private Animations _currentAnimation = Animations.None;
        private bool _slideIn = false;
        private bool _slideOut = false;
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

        private List<INotificationController> _visible = new List<INotificationController>();

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

        private Task Rerender()
        {
            _shouldRender = true;
            return InvokeAsync(StateHasChanged);
        }

        private Task OnMouseOverHandler(MouseEventArgs args)
        {
            NotificationsService.SuspendTimeOutAll();
            return Task.CompletedTask;
        }

        private Task OnMouseOutHandler(MouseEventArgs args)
        {
            NotificationsService.ResumeTimeOutAll();
            return Task.CompletedTask;
        }

        private async Task CloseNotifications()
        {
            if (await _animationLock.WaitAsync(TimeSpan.FromMinutes(1)))
            {
                try
                {
                    _dismissAllItems = _visible.Select(a => a.Identifier).ToList();
                    NotificationsService.DismissAll();

                    await InvokeVoidAsync("writeLine", $"CloseNotifications {this.ToString()}");

                    await SlideWindowOut();

                    _visible.Clear();
                    //_dismissAllItems.Clear();
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

        private async Task SlideWindowIn()
        {
            _hidden = false;
            _slideIn = true;
            await Rerender();
            await Task.Delay(1000);
            _slideIn = false;
        }

        private async Task SlideWindowOut()
        {
            _slideOut = true;
            await Rerender();
            await Task.Delay(1000);
            _slideOut = false;
            _hidden = true;
        }

        private async Task SlideItemIn(int item)
        {
            _slideInItemId = item;
            await Rerender();
            await Task.Delay(500);
            _slideInItemId = -1;
        }

        private async Task SlideItemOut(int item)
        {
            _slideOutItemId = item;
            await Rerender();
            await Task.Delay(500);
            _slideOutItemId = -1;
        }

        private async Task<BoundingClientRect> GetElementSize()
        {
            return await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);
        }

        private async Task SetAnimationHeight(int item)
        {
            _lastItemHeight = await JsInvokeAsync<float>("setNotificationsAnimationHeight", item);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _fontSize = await JsInvokeAsync<float>("getFontSize");

            await InvokeVoidAsync("writeLine", $"OnInitializedAsync {this.ToString()}");
            await UpdateUI();

            ViewState.OnChange += ViewState_OnChange;
        }

        private async void ViewState_OnChange(object sender, System.EventArgs e)
        {
            await InvokeVoidAsync("writeLine", $"ViewState_OnChange {this.ToString()}");
            await UpdateUI();
        }

        private async Task UpdateUI()
        {
            if (await _animationLock.WaitAsync(TimeSpan.FromMinutes(1)))
            {
                try
                {
                    var snapShot = ViewState.Visible.ToList();

                    //With dismiss all button we will get OnChange event for every item that was in the list.
                    //We have to ignore these items, but render the new items.
                    if (_dismissAllItems.Count > 0)
                    {
                        //Get all ids in the snapshot.
                        var snapShotIds = snapShot.Select(a => a.Identifier).ToList();
                        //Get ids that was in the dismiss list but not in the snapshot.
                        var removedItems = _dismissAllItems.Where(a => !snapShotIds.Contains(a)).ToList();
                        //Remove these items from the dismiss list.
                        _dismissAllItems.RemoveAll(a => removedItems.Contains(a));
                        //Check if there are new items.
                        var addedItems = snapShot.Where(a => !_dismissAllItems.Contains(a.Identifier)).ToList();
                        if (addedItems.Count == 0)
                        {
                            //If no new items found then do nothing.
                            return;
                        }
                        else
                        {
                            //Else keep in the snapshot only the new items and continue with render.
                            await InvokeVoidAsync("writeLine", $"Error: New items could be ignored {this.ToString()}");

                            snapShot = addedItems;
                        }
                    }

                    _newItems.Clear();
                    _removedItems.Clear();

                    if (_visible.Count == 0)
                    {
                        if (snapShot.Count() > 0)
                        {
                            //First run.
                            _visible = snapShot;

                            _isTemp = true;
                            await Rerender();
                            var size = await GetElementSize();
                            size.Height += _fontSize * 2;
                            _componentSize.Width = (int)size.Width;
                            _componentSize.Height = (int)size.Height;
                            await InvokeVoidAsync("writeLine", $"Height: {_componentSize.Height.ToString()}");
                            NotificationsService.RequestNotificationHostSize(_componentSize);
                            _isTemp = false;

                            _currentAnimation = Animations.WindowSlideIn;
                            await SlideWindowIn();
                            _currentAnimation = Animations.None;

                            foreach (var item in snapShot)
                            {
                                NotificationsService.TryResetTimeout(item.Identifier);
                            }
                        }
                        else
                        {
                            await InvokeVoidAsync("writeLine", $"Error: 0 items {this.ToString()}");
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

                            //var tmp = _visible.ToList();

                            //foreach (var item in _newItems)
                            //{
                            //    _visible.Add(snapShot.Where(a => a.Identifier == item).FirstOrDefault());
                            //}

                            //_isTemp = true;
                            //await Rerender();
                            //var size = await GetElementSize();
                            //size.Height += _fontSize * 2;
                            //await InvokeVoidAsync("writeLine", $"Height: {size.Height.ToString()}");
                            //_componentSize.Width = (int)size.Width;
                            //_componentSize.Height = (int)size.Height;
                            //NotificationsService.RequestNotificationHostSize(_componentSize);
                            //_isTemp = false;

                            //_visible = tmp;

                            foreach (var item in _removedItems)
                            {
                                _currentAnimation = Animations.ItemSlideOut;

                                await SetAnimationHeight(item);

                                await SlideItemOut(item);
                                _currentAnimation = Animations.None;
                            }

                            var size = await GetElementSize();
                            size.Height += _fontSize * 2;
                            _componentSize.Width = (int)size.Width;
                            _componentSize.Height = (int)size.Height;
                            await InvokeVoidAsync("writeLine", $"Height: {_componentSize.Height.ToString()}");
                            NotificationsService.RequestNotificationHostSize(_componentSize);

                            foreach (var item in _newItems)
                            {
                                //We need to add the item to the DOM first.
                                //TODO: AAA ADD ITEM IN THE RIGHT POSITION.
                                _newlyAddedItemId = item;
                                var newlyAddedItem = snapShot.Where(a => a.Identifier == item).FirstOrDefault();
                                var index = snapShot.IndexOf(newlyAddedItem);
                                _visible.Insert(index, newlyAddedItem);
                                await Rerender();
                                _newlyAddedItemId = -1;
                                await InvokeVoidAsync("writeLine", $"tmpItemAdded {this.ToString()}");

                                _currentAnimation = Animations.ItemSlideIn;

                                await SetAnimationHeight(item);
                                _componentSize.Height += (int)_lastItemHeight;
                                await InvokeVoidAsync("writeLine", $"Height: {_componentSize.Height.ToString()}");
                                NotificationsService.RequestNotificationHostSize(_componentSize);

                                await SlideItemIn(item);
                                _currentAnimation = Animations.None;

                                NotificationsService.TryResetTimeout(item);
                            }
                        }
                        else
                        {
                            await SlideWindowOut();
                        }

                        _visible = snapShot;
                        await Rerender();
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
            else
            {
                await InvokeVoidAsync("writeLine", $"Error: _animationLock not available {this.ToString()}");
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

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            await InvokeVoidAsync("writeLine", $"OnAfterRenderAsync {this.ToString()}");

            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("registerAnimatedComponent", Ref);
                AnimationEventInterop = new AnimationEventInterop(JsRuntime);
                await AnimationEventInterop.SetupAnimationEventCallback(args => AnimationHandler(args));
            }
            else
            {
                _shouldRender = false;
            }
        }

        public override string ToString()
        {
            return base.ToString() + $" Current animation: {_currentAnimation.ToString()} Items: {_visible.Count}";
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterAnimatedComponent", Ref).ConfigureAwait(false);
            await InvokeVoidAsync("writeLine", $"DisposeAsync {this.ToString()}");

            if (AnimationEventInterop != null)
            {
                await AnimationEventInterop.DisposeAsync();
            }

            Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-notifications")
                .If("slide-in", () => _slideIn)
                .If("slide-out", () => _slideOut)
                .If("prerender", () => _isTemp || _hidden)
                .AsString();

        #endregion
    }
}
