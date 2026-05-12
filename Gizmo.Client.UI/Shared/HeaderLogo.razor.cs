using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderLogo : CustomDOMComponentBase
    {
        [Inject]
        private LogoViewState LogoViewState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(LogoViewState);
            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(LogoViewState);
            base.Dispose();
        }
    }
}
