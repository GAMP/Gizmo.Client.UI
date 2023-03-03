using System.Threading.Tasks;
using Gizmo.Client.UI.Pages;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductTimeCardHover : CustomDOMComponentBase
    {
        private UserProductTimeViewState _previousTimeProduct;

        [Parameter]
        public UserProductTimeViewState TimeProduct { get; set; }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var timeProductChanged = _previousTimeProduct != TimeProduct;

            if (timeProductChanged)
            {
                if (_previousTimeProduct != null)
                {
                    //The same component used again with a different product.
                    //We have to unbind from the old product.
                    this.UnsubscribeChange(_previousTimeProduct);
                }

                _previousTimeProduct = TimeProduct;

                //We have to bind to the new product.
                this.SubscribeChange(TimeProduct);
            }
        }

        public override void Dispose()
        {
            if (TimeProduct != null)
            {
                this.UnsubscribeChange(TimeProduct);
            }
            base.Dispose();
        }
    }
}
