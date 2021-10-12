using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Components
{
    public partial class ApplicationDetailsDialog : CustomDOMComponentBase
    {
        public ApplicationDetailsDialog()
        {
        }

        #region FIELDS

        private bool _isOpen { get; set; }
        private int _selectedTabIndex = 0;

        #endregion

        #region PROPERTIES

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;

                _ = IsOpenChanged.InvokeAsync(_isOpen);
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public ApplicationViewModel Application { get; set; }

        #endregion

        #region METHODS

        private void CloseDialog()
        {
            IsOpen = false;
        }

        private void SelectTab(int index)
        {
            if (index < 3)
                _selectedTabIndex = index;
        }

        private int GetTagsByChars(int numberOfCharacters)
        {
            int spaceBetweenTags = 3;
            int c = 0;
            int index = 0;
            foreach (var item in Application.Tags)
            {
                c += item.Name.Length + spaceBetweenTags;
                if (c > numberOfCharacters)
                {
                    return index;
                }
                index += 1;
            }
            return index;
        }

        #endregion
    }
}