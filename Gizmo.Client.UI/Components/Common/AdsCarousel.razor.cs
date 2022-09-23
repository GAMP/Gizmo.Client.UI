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

                /*if (_selectedIndex < value)
                {
                    SlideLeft(value);
                }
                else
                {
                    SlideRight(value);
                }*/

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

        internal void Register(AdsCarouselItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(AdsCarouselItem item)
        {
            _items.Remove(item);
        }

        #endregion

        #region OVERRIDES

        protected override Task OnFirstAfterRenderAsync()
        {
            if (_items.Count > 0)
            {
                _items[0].SetIndex(1);
                _items[1].SetIndex(2);
                _items[2].SetIndex(3);
            }

            return base.OnFirstAfterRenderAsync();
        }

        #endregion
    }
}