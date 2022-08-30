using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarousel : CustomDOMComponentBase
    {
        private int _selectedIndex;

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
    }
}
