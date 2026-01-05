using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrder : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartViewService Service { get; set; }

        [Inject]
        ClientServerCartViewService ClientServerCartViewService { get; set; }

        private Task PlaceOrder()
        {
            return Service.SubmitAsync();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(Service.ViewState);
            this.SubscribeChange(ClientServerCartViewService.ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ClientServerCartViewService.ViewState);
            this.UnsubscribeChange(Service.ViewState);

            base.Dispose();
        }
    }
}
