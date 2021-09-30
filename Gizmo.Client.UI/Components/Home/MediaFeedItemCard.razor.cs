using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class MediaFeedItemCard : CustomDOMComponentBase
    {
        [Parameter]
        public MediaFeedViewModel MediaFeed { get; set; }

        private bool _mediaFeedItemDialogIsOpen { get; set; }

        private void ShowModal()
        {
            _mediaFeedItemDialogIsOpen = true;
        }
    }
}
