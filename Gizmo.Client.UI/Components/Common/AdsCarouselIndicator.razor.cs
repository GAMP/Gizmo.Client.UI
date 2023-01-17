using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarouselIndicator : CustomDOMComponentBase
    {
        private const decimal INDICATOR_BORDER = 0;
        private const decimal INDICATOR_DIMENSIONS = 16;
        private const decimal SPACE_BETWEEN = 16;

        private int _selectedIndex;
        private int _direction;

        [CascadingParameter]
        protected AdsCarousel Parent { get; set; }

        [Parameter]
        public int Size { get; set; }

        [Parameter]
        public int Maximum { get; set; }

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

                if (Math.Abs(_selectedIndex - value) == 1)
                {
                    if (_selectedIndex > value)
                        _direction = 1;
                    else
                        _direction = -1;
                }
                else
                {
                    _direction = 0;
                }

                _selectedIndex = value;
                _ = SelectedIndexChanged.InvokeAsync(_selectedIndex);
            }
        }

        [Parameter]
        public EventCallback<int> SelectedIndexChanged { get; set; }

        public void OnClickButton(int index)
        {
            Parent?.SetCurrentIndex(index);
        }

        public void OnClickPreviousButtonHandler()
        {
            if (SelectedIndex > 0)
                Parent?.SetCurrentIndex(SelectedIndex - 1);
            else
                Parent?.SetCurrentIndex(Size - 1);
        }

        public void OnClickNextButtonHandler()
        {
            if (SelectedIndex < Size - 1)
                Parent?.SetCurrentIndex(SelectedIndex + 1);
            else
                Parent?.SetCurrentIndex(0);
        }

        private string GetIndicatorClass(int index)
        {
            string result = string.Empty;

            if (index == SelectedIndex)
            {
                result = "active";
            }

            if (_direction == 0)
                return result;

            if (index == SelectedIndex)
            {
                if (_direction < 0)
                    result += " current";
                else
                    result += " next";
            }

            if (index == SelectedIndex + _direction)
            {
                if (_direction < 0)
                    result = "next";
                else
                    result = "current";
            }

            return result;
        }

        private decimal GetIndicatorListSize()
        {
            return ((Size * INDICATOR_DIMENSIONS) + ((Size - 1) * SPACE_BETWEEN) + (INDICATOR_BORDER * 2)) / 10;
        }

        private decimal GetIndicatorPosition(int index)
        {
            decimal result = INDICATOR_BORDER;

            if (_direction != 0)
            {
                if (index == SelectedIndex)
                {
                    if (_direction < 0)
                        index -= 1;
                    else
                        index += 1;
                }
                else
                {
                    if (index == SelectedIndex + _direction)
                    {
                        if (_direction < 0)
                            index += 1;
                        else
                            index -= 1;
                    }
                }
            }

            result += (index + 1) * INDICATOR_DIMENSIONS / 2;
            result += index * SPACE_BETWEEN;

            return result / 10;
        }
    }
}