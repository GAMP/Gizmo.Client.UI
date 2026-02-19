using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.Client.Options;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class NotificationsHost : CustomDOMComponentBase, IAsyncDisposable
    {
        private enum NotificationsAnimations
        {
            None,
            WindowSlideIn,
            WindowSlideOut,
            ItemSlideIn,
            ItemSlideOut
        }

        #region Fields

        private float _lastItemHeight;
        private System.Drawing.Size _componentSize = new System.Drawing.Size();
        private float _fontSize = 10;
        private bool _isTemp;
        private bool _hidden = true;
        private bool _shouldRender;
        private NotificationsAnimations _currentAnimation = NotificationsAnimations.None;
        private bool _slideIn = false;
        private bool _slideOut = false;
        private int _newlyAddedItemId = -1;
        private int _slideInItemId = -1;
        private int _slideOutItemId = -1;
        private List<int> _dismissAllItems = new List<int>();
        private List<int> _newItems = new List<int>();
        private List<int> _removedItems = new List<int>();
        private readonly SemaphoreSlim _animationLock = new(1);
        private AnimationEventInterop? _animationEventInterop;

        private bool _slideInComplete = false;
        private bool _slideOutComplete = false;

        private List<INotificationController> _visible = new List<INotificationController>();

        #endregion

        #region Injects

        [Inject]
        private IOptionsMonitor<ClientInterfaceOptions> ClientInterfaceOptions { get; set; }

        [Inject]
        private ILogger<NotificationsHost> Logger { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private NotificationsHostViewState ViewState { get; set; } = null!;

        [Inject]
        private INotificationsService NotificationsService { get; set; } = null!;

        #endregion

        #region Methods

        private Task Rerender()
        {
            _shouldRender = true;
            return InvokeAsync(StateHasChanged);
        }

        private async Task SlideWindowIn()
        {
            await SetNotificationsContainerHeight();

            _hidden = false;
            _slideIn = true;

            Logger.LogDebug($"NotificationsMessage: SlideWindowIn {this.ToString()}");

            _slideInComplete = false;

            await Rerender();

            do
            {
                await Task.Delay(100); //200
            } while (!_slideInComplete);

            _slideIn = false;
        }

        private async Task SlideWindowOut()
        {
            _slideOut = true;

            //Logger.LogDebug($"NotificationsMessage: SlideWindowOut {this.ToString()}");

            _slideOutComplete = false;

            await Rerender();

            do
            {
                await Task.Delay(100); //200
            } while (!_slideOutComplete);

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

        private async Task SetNotificationsContainerHeight()
        {
            await JsInvokeAsync<float>("setNotificationsContainerHeight", @Ref);
        }

        private async Task SetNotificationHeight(int item)
        {
            _lastItemHeight = await JsInvokeAsync<float>("setNotificationHeight", item);
        }

        private async Task UpdateUI()
        {
            //await InvokeVoidAsync("writeLine", $"UpdateUI {this.ToString()}");
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
                            //await InvokeVoidAsync("writeLine", $"Error: New items could be ignored {this.ToString()}");

                            snapShot = addedItems;
                        }
                    }

                    _newItems.Clear();
                    _removedItems.Clear();

                    if (_visible.Count == 0)
                    {
                        if (snapShot.Count() > 0)
                        {
                            //First render after window shown.
                            _visible = snapShot;

                            //Render invisible to get height.
                            _isTemp = true;
                            await Rerender();
                            var size = await GetElementSize();
                            size.Height += _fontSize * 2;
                            _componentSize.Width = (int)size.Width;
                            _componentSize.Height = (int)size.Height;
                            Logger.LogDebug($"NotificationsMessage: Height {_componentSize.Height.ToString()}");
                            //await InvokeVoidAsync("writeLine", $"Height: {_componentSize.Height.ToString()}");
                            NotificationsService.RequestNotificationHostSize(_componentSize);
                            _isTemp = false;

                            foreach (var item in snapShot)
                            {
                                NotificationsService.TryResetTimeout(item.Identifier);
                            }

                            //Render visible to show window slide in animation.
                            _currentAnimation = NotificationsAnimations.WindowSlideIn;
                            await SlideWindowIn();
                            _currentAnimation = NotificationsAnimations.None;
                        }
                        else
                        {
                            Logger.LogError($"NotificationsMessage: Error: 0 items {this.ToString()}");
                            //await InvokeVoidAsync("writeLine", $"Error: 0 items {this.ToString()}");
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

                            foreach (var item in _removedItems)
                            {
                                _currentAnimation = NotificationsAnimations.ItemSlideOut;

                                await SetNotificationHeight(item);

                                await SlideItemOut(item);
                                _currentAnimation = NotificationsAnimations.None;
                            }

                            var size = await GetElementSize();
                            size.Height += _fontSize * 2;
                            _componentSize.Width = (int)size.Width;
                            _componentSize.Height = (int)size.Height;
                            Logger.LogDebug($"NotificationsMessage: Height {_componentSize.Height.ToString()}");
                            //await InvokeVoidAsync("writeLine", $"Height: {_componentSize.Height.ToString()}");
                            NotificationsService.RequestNotificationHostSize(_componentSize);

                            foreach (var item in _newItems)
                            {
                                //We need to add the item to the DOM first.
                                //TODO: C ADD ITEM IN THE RIGHT POSITION?
                                _newlyAddedItemId = item;
                                var newlyAddedItem = snapShot.Where(a => a.Identifier == _newlyAddedItemId).FirstOrDefault();
                                if (newlyAddedItem != null)
                                {
                                    var index = snapShot.IndexOf(newlyAddedItem);
                                    if (index >= 0)
                                    {
                                        _visible.Insert(index, newlyAddedItem);
                                    }
                                    else
                                    {
                                        //TODO: A ERROR
                                    }
                                }
                                else
                                {
                                    //TODO: A ERROR
                                }
                                await Rerender();
                                _newlyAddedItemId = -1;
                                //await InvokeVoidAsync("writeLine", $"tmpItemAdded {this.ToString()}");

                                _currentAnimation = NotificationsAnimations.ItemSlideIn;

                                await SetNotificationHeight(item);
                                _componentSize.Height += (int)_lastItemHeight;
                                Logger.LogDebug($"NotificationsMessage: Height {_componentSize.Height.ToString()}");
                                //await InvokeVoidAsync("writeLine", $"Height: {_componentSize.Height.ToString()}");
                                NotificationsService.RequestNotificationHostSize(_componentSize);

                                await SlideItemIn(item);
                                _currentAnimation = NotificationsAnimations.None;

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
                Logger.LogError($"NotificationsMessage: Error: _animationLock not available {this.ToString()}");
                //await InvokeVoidAsync("writeLine", $"Error: _animationLock not available {this.ToString()}");
            }
        }

        public async Task UpdateItem(int identifier)
        {

        }

        #endregion

        #region Handlers

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

                    //await InvokeVoidAsync("writeLine", $"CloseNotifications {this.ToString()}");

                    await SlideWindowOut();

                    _visible.Clear();
                    //_dismissAllItems.Clear();
                    await Rerender();
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

        private async void ViewState_OnChange(object sender, System.EventArgs e)
        {
            Logger.LogDebug($"NotificationsMessage: ViewState_OnChange {this.ToString()}");
            //await InvokeVoidAsync("writeLine", $"ViewState_OnChange {this.ToString()}");
            await UpdateUI();
        }

        private Task AnimationHandler(AnimationEventArgs args)
        {
            //args.Id is the host component Id or the item Identifier.
            if (args.Id == Id)
            {
                Logger.LogDebug($"Notification host animation {args.AnimationName} state changed, new stat {args.AnimationState}");

                if (args.AnimationName == "notifications-slide-in-anim")
                {
                    if (args.AnimationState == AnimationStates.End)
                    {
                        _slideInComplete = true;
                    }
                }
                else if (args.AnimationName == "notifications-slide-out-anim")
                {
                    if (args.AnimationState == AnimationStates.End)
                    {
                        _slideOutComplete = true;
                    }
                }
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Overrides

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        private int _retriesCounter = 0;
        private int _logErrorCounter = 0;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //Logger.LogDebug($"NotificationsMessage: After Render firstRender:{firstRender} {this.ToString()}");
            if (firstRender)
            {
                //await Task.Delay(10);

                bool done = false;

                do
                {
                    _retriesCounter += 1;

                    try
                    {
                        //Logger.LogDebug($"NotificationsMessage: Before getFontSize");
                        _fontSize = await JsInvokeAsync<float>("getFontSize");
                        //Logger.LogDebug($"NotificationsMessage: After getFontSize");

                        await JsRuntime.InvokeVoidAsync("registerAnimatedComponent", Ref);
                        _animationEventInterop = new AnimationEventInterop(JsRuntime);
                        await _animationEventInterop.SetupAnimationEventCallback(args => AnimationHandler(args));

                        //_hidden = false;

                        //Logger.LogDebug($"NotificationsMessage: Before UpdateUI");
                        await UpdateUI();
                        //Logger.LogDebug($"NotificationsMessage: After UpdateUI");

                        if (_retriesCounter > 1)
                        {
                            Logger.LogDebug($"NotificationsMessage: retries: {_retriesCounter}");
                        }

                        if (_logErrorCounter > 0)
                        {
                            Logger.LogDebug($"NotificationsMessage: logging errors: {_logErrorCounter}");
                        }

                        done = true;
                    }
                    catch (Exception ex)
                    {
                        //try
                        //{
                        Logger.LogError($"NotificationsMessage: {ex.Message}");
                        //}
                        //catch (Exception ex2)
                        //{
                        //    _logErrorCounter += 1;
                        //}
                        await Task.Delay(100);
                    }
                } while (!done);

                ViewState.OnChange += ViewState_OnChange;
            }
            else
            {
                _shouldRender = false;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public override string ToString()
        {
            return base.ToString() + $" Current animation: {_currentAnimation.ToString()} Items: {_visible.Count} _isTemp: {_isTemp} _hidden: {_hidden}";
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            //await InvokeVoidAsync("writeLine", $"OnInitializedAsync {this.ToString()}");
        }

        #endregion

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            Logger.LogDebug($"NotificationsMessage: DisposeAsync {this.ToString()}");

            try
            {
                await InvokeVoidAsync("unregisterAnimatedComponent", Ref).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "exception unregisterAnimatedComponent");
            }

            //Logger.LogDebug($"NotificationsMessage: DisposeAsync {this.ToString()}");

            if (_animationEventInterop != null)
            {
                await _animationEventInterop.DisposeAsync();
                _animationEventInterop = null;
            }

            Dispose();
        }

        #endregion

        #region ClassMappers

        protected string ClassName => new ClassMapper()
                .Add("giz-notifications")
                .If("slide-in", () => _slideIn)
                .If("slide-out", () => _slideOut)
                .If("prerender", () => _isTemp || _hidden)
                .AsString();

        #endregion
    }
}
