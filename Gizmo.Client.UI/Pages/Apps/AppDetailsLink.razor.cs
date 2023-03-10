using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    public partial class AppDetailsLink : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public AppLinkViewState Link { get; set; }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(Link);
            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Link);
            base.Dispose();
        }
    }
}
