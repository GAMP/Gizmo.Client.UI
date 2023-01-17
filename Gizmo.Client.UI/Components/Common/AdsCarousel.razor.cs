using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarousel : CustomDOMComponentBase
    {
        const int ANIMATION_TIME = 1000;

        #region FIELDS

        private System.Timers.Timer _timer;

        private List<AdsCarouselItem> _items = new List<AdsCarouselItem>();
        private List<AdsCarouselItem> _duplicates = new List<AdsCarouselItem>();

        private int _selectedIndex;

        private bool _duplicatesActive = false;

        #endregion

        #region PROPERTIES

        [Parameter]
        public ICollection<AdvertisementViewState> Advertisements { get; set; }

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
                    FadeOut(value);
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

        public void SelectedIndexChangedHandler(int index)
        {
            SelectedIndex = index;
        }

        #endregion

        #region METHODS

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SlideNext(sender);
        }

        private void FadeOut(int index)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (!_duplicatesActive)
                    _items[i].FadeOut();
                else
                    _duplicates[i].FadeOut();
            }

            if (_items != null && index >= 0 && _selectedIndex >= 0 && index < _items.Count && _selectedIndex < _items.Count)
            {
                if (!_duplicatesActive)
                {
                    _duplicates[GetItemIndex(index, -1)].FadeIn(1);
                    _duplicates[index].FadeIn(2);
                    _duplicates[GetItemIndex(index, 1)].FadeIn(3);
                }
                else
                {
                    _items[GetItemIndex(index, -1)].FadeIn(1);
                    _items[index].FadeIn(2);
                    _items[GetItemIndex(index, 1)].FadeIn(3);
                }
            }

            _duplicatesActive = !_duplicatesActive;
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

        internal void Register(AdsCarouselItem item, bool duplicate)
        {
            if (!duplicate)
                _items.Add(item);
            else
                _duplicates.Add(item);
        }

        internal void Unregister(AdsCarouselItem item, bool duplicate)
        {
            if (!duplicate)
                _items.Remove(item);
            else
                _duplicates.Remove(item);
        }

        private void SlideNext(object stateInfo)
        {
            int newIndex = SelectedIndex + 1;
            if (newIndex > _items.Count - 1)
                newIndex = 0;

            SelectedIndex = newIndex;

            InvokeAsync(StateHasChanged);
        }

        internal void SetCurrentIndex(int index)
        {
            _timer.Stop();

            SelectedIndex = index;

            InvokeAsync(StateHasChanged);

            _timer.Start();
        }

        internal void SetCurrent(AdvertisementViewState advertisement)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Advertisement == advertisement)
                {
                    if (_selectedIndex == i)
                        return;

                    _timer.Stop();

                    SelectedIndex = i;

                    InvokeAsync(StateHasChanged);

                    _timer.Start();

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
                _items[2].FadeIn(1);
                _items[0].FadeIn(2);
                _items[1].FadeIn(3);
            }

            return base.OnFirstAfterRenderAsync();
        }

        public override void Dispose()
        {
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