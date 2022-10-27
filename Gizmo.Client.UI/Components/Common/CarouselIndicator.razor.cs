using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class CarouselIndicator : CustomDOMComponentBase
    {
        private int _selectedIndex;

        [Parameter]
        public int Size { get; set; }

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

                _selectedIndex = value;
                _ = SelectedIndexChanged.InvokeAsync(_selectedIndex);
            }
        }

        [Parameter]
        public EventCallback<int> SelectedIndexChanged { get; set; }

        public void OnClickButton(int index)
        {
            SelectedIndex = index;
        }
    }
}