using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Components
{
    public partial class PurchaseThumb : CustomDOMComponentBase
    {
        private int? _imageId;
        private int? _loadedFor;

        [Inject]
        UserProductViewStateLookupService ProductLookup { get; set; }

        [Inject]
        ILogger<PurchaseThumb> Logger { get; set; }

        [Parameter]
        public UserOrderLineViewState Line { get; set; }

        private bool IsTime => Line.LineType is LineType.TimeProduct or LineType.FixedTime or LineType.SessionTime;

        protected override async Task OnParametersSetAsync()
        {
            if (!IsTime && Line.ProductId is int productId && _loadedFor != productId)
            {
                _loadedFor = productId;

                try
                {
                    _imageId = (await ProductLookup.GetStateAsync(productId))?.DefaultImageId;
                }
                catch (Exception exception)
                {
                    Logger.LogWarning(exception, "Could not load product {ProductId} for a purchase picture.", productId);
                    _imageId = null;
                }
            }

            await base.OnParametersSetAsync();
        }
    }
}
