using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class Carousel : CustomDOMComponentBase
    {
        private List<CarouselItem> _items = new List<CarouselItem>();

        private int _selectedIndex;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

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

        #region OVERRIDES

        protected override Task OnFirstAfterRenderAsync()
        {
            if (_items.Count > 0)
            {
                _items[0].SlideLeft(false, true);
            }

            return base.OnFirstAfterRenderAsync();
        }

        #endregion

    }
}