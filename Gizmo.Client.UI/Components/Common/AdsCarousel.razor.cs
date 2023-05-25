using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarousel : CustomDOMComponentBase
    {
        const int ANIMATION_TIME = 500;

        #region FIELDS

        private Timer _timer;

        private readonly List<AdsCarouselItem> _items = new();

        private int _selectedIndex;

        #endregion

        #region PROPERTIES

        [Inject]
        AdvertisementsViewService AdvertisementsService { get; set; }

        [Inject]
        AdvertisementsViewState ViewState { get; set; }

        [Parameter]
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (_selectedIndex == value)
                    return;

                if (_items.Count > 2)
                {
                    BringToFront(value);
                }

                _selectedIndex = value;
                _ = SelectedIndexChanged.InvokeAsync(_selectedIndex);
            }
        }

        [Parameter]
        public EventCallback<int> SelectedIndexChanged { get; set; }

        [Parameter]
        public int Interval { get; set; }

        #endregion

        #region EVENTS

        private void SelectedIndexChangedHandler(int index)
        {
            SelectedIndex = index;
        }

        private void OnMouseOverHandler(MouseEventArgs args)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        private void OnMouseOutHandler(MouseEventArgs args)
        {
            if (_timer != null)
            {
                _timer.Start();
            }
        }

        #endregion

        #region METHODS

        private void timer_Elapsed(object _, ElapsedEventArgs __)
        {
            SlideNext();
        }

        private void BringToFront(int index)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].Hide();
            }

            if (_items != null && index >= 0 && _selectedIndex >= 0 && index < _items.Count && _selectedIndex < _items.Count)
            {
                int direction = 1;

                if (_selectedIndex == GetItemIndex(index, 1))
                    direction = -1;

                if (ViewState.Advertisements.Count() > 3 && direction == 1)
                    _items[GetItemIndex(GetItemIndex(index, -1), -1)].ShowInPosition(4, direction, false);

                _items[GetItemIndex(index, -1)].ShowInPosition(1, direction, ViewState.Advertisements.Count() == 3);
                _items[index].ShowInPosition(2, direction, false);
                _items[GetItemIndex(index, 1)].ShowInPosition(3, direction, ViewState.Advertisements.Count() == 3);

                if (ViewState.Advertisements.Count() > 3 && direction == -1)
                    _items[GetItemIndex(GetItemIndex(index, 1), 1)].ShowInPosition(4, direction, false);
            }
        }

        private int GetItemIndex(int index, int direction)
        {
            if (direction == 1)
            {
                if (index == _items.Count - 1)
                {
                    return 0;
                }
                else
                {
                    return index + 1;
                }
            }
            else
            {
                if (index == 0)
                {
                    return _items.Count - 1;
                }
                else
                {
                    return index - 1;
                }
            }
        }

        internal void Register(AdsCarouselItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(AdsCarouselItem item)
        {
            _items.Remove(item);
        }

        private void SlideNext()
        {
            int newIndex = SelectedIndex + 1;
            if (newIndex > _items.Count - 1)
                newIndex = 0;

            SelectedIndex = newIndex;

            InvokeAsync(StateHasChanged);
        }

        internal void SetCurrentIndex(int index)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }

            SelectedIndex = index;

            InvokeAsync(StateHasChanged);

            if (_timer != null)
            {
                _timer.Start();
            }
        }

        internal void SetCurrent(int advertisementId)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].AdvertisementId == advertisementId)
                {
                    if (_selectedIndex == i)
                        return;

                    if (_timer != null)
                    {
                        _timer.Stop();
                    }

                    SelectedIndex = i;

                    InvokeAsync(StateHasChanged);

                    if (_timer != null)
                    {
                        _timer.Start();
                    }

                    return;
                }
            }
        }

        #endregion

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var intervalChanged = parameters.TryGetValue<int>(nameof(Interval), out var newInterval);
            if (intervalChanged)
            {
                if (Interval > 0)
                {
                    if (_timer != null)
                    {
                        _timer.Elapsed -= timer_Elapsed;
                        _timer.Stop();
                        _timer.Dispose();
                        _timer = null;
                    }
                    _timer = new System.Timers.Timer(Interval);
                    _timer.Elapsed += timer_Elapsed;
                    _timer.Start();
                }
                else
                {
                    if (_timer != null)
                    {
                        _timer.Elapsed -= timer_Elapsed;
                        _timer.Stop();
                        _timer.Dispose();
                        _timer = null;
                    }
                }
            }
        }

        protected override Task OnFirstAfterRenderAsync()
        {
            if (_items.Count > 2)
            {
                _items[GetItemIndex(0, -1)].ShowInPosition(1, 1, ViewState.Advertisements.Count() == 3);
                _items[0].ShowInPosition(2, 1, false);
                _items[GetItemIndex(0, 1)].ShowInPosition(3, 1, ViewState.Advertisements.Count() == 3);
            }

            return base.OnFirstAfterRenderAsync();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            if (_timer != null)
            {
                _timer.Elapsed -= timer_Elapsed;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            base.Dispose();
        }

        #endregion
    }
}
