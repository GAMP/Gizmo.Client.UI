using System.Linq;

using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

using ShellText = Gizmo.Client.UI.Localization.ShellStringOverrides;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserPurchasesRoute)]
    public partial class Purchases : CustomDOMComponentBase
    {
        protected bool HasOrders => ViewState.Orders.Any();

        protected static bool IsVoided(UserOrderViewState order) => order.Invoice?.IsVoided == true;

        protected static string StatusKey(UserOrderViewState order) => IsVoided(order)
            ? ShellText.ORDER_STATUS_VOIDED
            : order.OrderStatus switch
            {
                OrderStatus.Completed => ShellText.ORDER_STATUS_COMPLETED,
                OrderStatus.Accepted => ShellText.ORDER_STATUS_ACCEPTED,
                OrderStatus.Canceled => ShellText.ORDER_STATUS_CANCELED,
                _ => ShellText.ORDER_STATUS_ON_HOLD,
            };

        protected static string StatusClass(UserOrderViewState order) => IsVoided(order)
            ? "giz-account-tag--off"
            : order.OrderStatus switch
            {
                OrderStatus.Completed => "giz-account-tag--good",
                OrderStatus.Accepted => "giz-account-tag--on",
                OrderStatus.Canceled => "giz-account-tag--off",
                _ => "giz-account-tag--wait",
            };

        protected static string PaymentKey(UserOrderViewState order) => order.Invoice?.PaymentStatus switch
        {
            InvoiceStatus.Unpaid => ShellText.INVOICE_UNPAID,
            InvoiceStatus.PartiallyPaid => ShellText.INVOICE_PARTIALLY_PAID,
            _ => null,
        };

        protected static bool ShowPaymentStatus(UserOrderViewState order) => PaymentKey(order) is not null && !IsVoided(order);

        protected bool IsLinkable(UserOrderLineViewState line) => ProductDetailsNavigationEnabled
            && line.LineType != LineType.SessionTime
            && line.LineType != LineType.FixedTime
            && line.ProductId.HasValue;

        protected static string ProductLink(UserOrderLineViewState line) => $"{ClientRoutes.ProductDetailsRoute}?ProductId={line.ProductId}";

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        PurchasesViewService PurchasesService { get; set; }

        [Inject]
        PurchasesViewState ViewState { get; set; }

        [Inject]
        ProductDetailsPageViewState ProductDetailsPageViewState { get; set; }

        /// <summary>
        /// Gets whether purchased products can be navigated to their details page.
        /// Disabled when the shop is off or product details are disabled.
        /// </summary>
        private bool ProductDetailsNavigationEnabled => ProductDetailsPageViewState.ProductDetailsNavigationEnabled;

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(ProductDetailsPageViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(ProductDetailsPageViewState);

            base.Dispose();
        }
    }
}
