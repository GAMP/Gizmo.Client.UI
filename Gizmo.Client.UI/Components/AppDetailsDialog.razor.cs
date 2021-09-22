using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;

namespace Gizmo.Client.UI.Components
{
    public partial class AppDetailsDialog : CustomDOMComponentBase
    {
        public AppDetailsDialog()
        {
            Application = new ApplicationViewModel()
            {
                Name = "Fall Guys: Ultimate Knockout",
                Image = "",
                Ratings = 168,
                Rate = 4,
                Publisher = "Very Positive Inc",
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2020, 5, 13)
            };
        }

        private bool _isOpen { get; set; }

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

        public ApplicationViewModel Application { get; set; }

        private void CloseDialog()
        {
            IsOpen = false;
        }

    }
}
