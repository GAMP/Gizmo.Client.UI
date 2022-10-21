using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarousel : CustomDOMComponentBase
    {
        #region FIELDS

        private System.Threading.Timer _timer;

        private List<AdsCarouselItem> _items = new List<AdsCarouselItem>();

        private int _selectedIndex;

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

                if (_items.Count > 3)
                {
                    if (_selectedIndex < value)
                    {
                        if (_selectedIndex == 0 && value == _items.Count - 1)
                        {
                            SlideRight(value);
                        }
                        else
                        {
                            if (_selectedIndex + 1 == value)
                            {
                                SlideLeft(value);
                            }
                            else
                            {
                                _ = FadeOut(value);
                            }
                        }
                    }
                    else
                    {
                        if (_selectedIndex == _items.Count - 1 && value == 0)
                        {
                            SlideLeft(value);
                        }
                        else
                        {
                            if (_selectedIndex - 1 == value)
                            {
                                SlideRight(value);
                            }
                            else
                            {
                                _ = FadeOut(value);
                            }
                        }
                    }
                }

                _selectedIndex = value;
                SelectedIndexChanged.InvokeAsync(_selectedIndex);
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

        private async Task FadeOut(int index)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].FadeOut();
            }

            await Task.Delay(500);

            if (_items != null && index >= 0 && _selectedIndex >= 0 && index < _items.Count && _selectedIndex < _items.Count)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    _items[i].Clear();
                }

                _items[index].FadeIn(1);

                if (index + 1 < _items.Count)
                    _items[index + 1].FadeIn(2);
                else
                    _items[index + 1 - _items.Count].FadeIn(2);

                if (index + 2 < _items.Count)
                    _items[index + 2].FadeIn(3);
                else
                    _items[_selectedIndex + 2 - _items.Count].FadeIn(3);
            }
        }

        private void SlideLeft(int index)
        {
            if (_items != null && index >= 0 && _selectedIndex >= 0 && index < _items.Count && _selectedIndex < _items.Count)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    _items[i].Clear();
                }

                _items[_selectedIndex].SetIndex(1, -1);

                if (_selectedIndex + 1 < _items.Count)
                    _items[_selectedIndex + 1].SetIndex(2, -1);
                else
                    _items[_selectedIndex + 1 - _items.Count].SetIndex(2, -1);

                if (_selectedIndex + 2 < _items.Count)
                    _items[_selectedIndex + 2].SetIndex(3, -1);
                else
                    _items[_selectedIndex + 2 - _items.Count].SetIndex(3, -1);

                if (_selectedIndex + 3 < _items.Count)
                    _items[_selectedIndex + 3].SetIndex(4, -1);
                else
                    _items[_selectedIndex + 3 - _items.Count].SetIndex(4, -1);
            }
        }

        private void SlideRight(int index)
        {
            if (_items != null && index >= 0 && _selectedIndex >= 0 && index < _items.Count && _selectedIndex < _items.Count)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    _items[i].Clear();
                }

                _items[index].SetIndex(1, 1);

                if (index + 1 < _items.Count)
                    _items[index + 1].SetIndex(2, 1);
                else
                    _items[index + 1 - _items.Count].SetIndex(2, 1);

                if (index + 2 < _items.Count)
                    _items[index + 2].SetIndex(3, 1);
                else
                    _items[index + 2 - _items.Count].SetIndex(3, 1);

                if (index + 3 < _items.Count)
                    _items[index + 3].SetIndex(4, 1);
                else
                    _items[index + 3 - _items.Count].SetIndex(4, 1);
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

        private void SlideNext(object stateInfo)
        {
            int newIndex = SelectedIndex + 1;
            if (newIndex > _items.Count - 1)
                newIndex = 0;

            SelectedIndex = newIndex;

            StateHasChanged();
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
                        _timer.Dispose();
                        _timer = null;
                    }
                    _timer = new System.Threading.Timer(SlideNext, new System.Threading.AutoResetEvent(false), Interval, Interval);
                }
                else
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }
                }
            }
        }

        protected override Task OnFirstAfterRenderAsync()
        {
            if (_items.Count > 3)
            {
                _items[0].SetIndex(1, 0);
                _items[1].SetIndex(2, 0);
                _items[2].SetIndex(3, 0);
            }

            return base.OnFirstAfterRenderAsync();
        }

        #endregion
    }
}