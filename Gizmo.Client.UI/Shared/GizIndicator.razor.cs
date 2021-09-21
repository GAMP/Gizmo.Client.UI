﻿using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class GizIndicator
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
                SelectedIndexChanged.InvokeAsync(_selectedIndex);
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