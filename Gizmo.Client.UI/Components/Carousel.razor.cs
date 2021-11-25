﻿using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class Carousel : CustomDOMComponentBase
    {
        #region FIELDS

        private List<CarouselItem> _items = new List<CarouselItem>();

        private int _selectedIndex = 0;

        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public bool ShowIndicator { get; set; }

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

                if (_selectedIndex < value)
                {
                    SlideLeft(value);
                }
                else
                {
                    SlideRight(value);
                }

                _selectedIndex = value;
                SelectedIndexChanged.InvokeAsync(_selectedIndex);
            }
        }

        [Parameter]
        public EventCallback<int> SelectedIndexChanged { get; set; }

        #endregion

        #region EVENTS

        public void SelectedIndexChangedHandler(int index)
        {
            SelectedIndex = index;
        }

        protected Task OnClickPreviousButtonHandler(MouseEventArgs args)
        {
            if (_items.Count > 0)
            {
                if (SelectedIndex - 1 < 0)
                {
                    SelectedIndex = _items.Count - 1;
                }
                else
                {
                    SelectedIndex -= 1;
                }
            }

            return Task.CompletedTask;
        }

        protected Task OnClickNextButtonHandler(MouseEventArgs args)
        {
            if (_items.Count > 0)
            {
                if (SelectedIndex + 1 > _items.Count - 1)
                {
                    SelectedIndex = 0;
                }
                else
                {
                    SelectedIndex += 1;
                }
            }

            return Task.CompletedTask;
        }

        #endregion

        #region METHODS

        private void SlideLeft(int index)
        {
            if (_items != null && index >= 0 && _selectedIndex >= 0 && index < _items.Count && _selectedIndex < _items.Count)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    _items[i].Clear();
                }

                _items[index].SlideLeft(false);
                _items[_selectedIndex].SlideLeft(true);
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

                _items[index].SlideRight(false);
                _items[_selectedIndex].SlideRight(true);
            }
        }

        internal void Register(CarouselItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(CarouselItem item)
        {
            _items.Remove(item);
        }

        #endregion

        #region OVERRIDES

        protected override Task OnFirstAfterRenderAsync()
        {
            if (_items.Count > 0)
            {
                _items[0].SlideLeft(false, true);

                if (ShowIndicator)
                {
                    StateHasChanged();
                }
            }

            return base.OnFirstAfterRenderAsync();
        }

        #endregion

    }
}