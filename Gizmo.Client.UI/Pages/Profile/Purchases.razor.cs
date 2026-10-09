using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserPurchasesRoute)]
    public partial class Purchases : CustomDOMComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        protected bool HasOrders => ViewState.Orders.Any();

        protected static bool IsVoided(UserOrderViewState order) => order.Invoice?.IsVoided == true;

        protected static string StatusKey(UserOrderViewState order) => IsVoided(order)
            ? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_PURCHASES_ORDER_STATUS_VOIDED)
            : order.OrderStatus switch
            {
                OrderStatus.Completed => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_PURCHASES_ORDER_STATUS_COMPLETED),
                OrderStatus.Accepted => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_PURCHASES_ORDER_STATUS_ACCEPTED),
                OrderStatus.Canceled => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_PURCHASES_ORDER_STATUS_CANCELED),
                _ => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_PURCHASES_ORDER_STATUS_ON_HOLD),
            };

        protected static string StatusModifier(UserOrderViewState order) => IsVoided(order)
            ? "off"
            : order.OrderStatus switch
            {
                OrderStatus.Completed => "good",
                OrderStatus.Accepted => "on",
                OrderStatus.Canceled => "off",
                _ => "wait",
            };

        protected static string StatusClass(UserOrderViewState order) => $"giz-feed-order__state giz-feed-order__state--{StatusModifier(order)}";

        protected static string OrderClass(UserOrderViewState order) => $"giz-feed-order giz-feed-order--{StatusModifier(order)}";

        private enum OrderFilter { All, Time, Bar }

        private OrderFilter _filter;

        private string FilterClass(OrderFilter filter) => _filter == filter
            ? "giz-feed__chip giz-feed__chip--active"
            : "giz-feed__chip";

        private static bool IsTimeLine(UserOrderLineViewState line) =>
            line.LineType is LineType.TimeProduct or LineType.FixedTime or LineType.SessionTime;

        private IEnumerable<UserOrderViewState> Filtered => _filter switch
        {
            OrderFilter.Time => ViewState.Orders.Where(a => a.OrderLines.Any(IsTimeLine)),
            OrderFilter.Bar => ViewState.Orders.Where(a => a.OrderLines.Any(l => l.LineType == LineType.Product)),
            _ => ViewState.Orders,
        };

        private sealed record DayGroup(DateTime Date, string Label, IReadOnlyList<UserOrderViewState> Orders);

        private IReadOnlyList<DayGroup> Days => Filtered
            .GroupBy(a => a.OrderDate.Date)
            .OrderByDescending(a => a.Key)
            .Select(a => new DayGroup(a.Key, DayLabel(a.Key), a.OrderByDescending(o => o.OrderDate).ToList()))
            .ToList();

        private string DayLabel(DateTime day)
        {
            var today = DateTime.Today;

            if (day == today)
                return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PURCHASES_TODAY);

            if (day == today.AddDays(-1))
                return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PURCHASES_YESTERDAY);

            return day.ToString(day.Year == today.Year ? "d MMMM" : "d MMMM yyyy", CultureInfo.CurrentCulture);
        }

        private string Facts(UserOrderViewState order)
        {
            var parts = new List<string> { order.OrderDate.ToString("HH:mm", CultureInfo.CurrentCulture) };

            if (!string.IsNullOrWhiteSpace(order.Invoice?.PaymentMethodNames))
                parts.Add(order.Invoice.PaymentMethodNames);

            if (order.Id > 0)
                parts.Add(GrafitLocalization.GetString(GrafitResourceKeys.SHELL_ACCOUNT_ORDER_NUMBER, order.Id));

            return string.Join(" · ", parts);
        }

        private static IReadOnlyList<UserOrderLineViewState> Thumbs(UserOrderViewState order) => order.OrderLines
            .GroupBy(a => IsTimeLine(a) ? -1 : a.ProductId ?? -2 - a.Id)
            .Select(a => a.First())
            .Take(2)
            .ToList();

        private sealed record MonthTotal(string Label, decimal Spent, int Points, int Count, int Earned);

        private MonthTotal Month
        {
            get
            {
                var orders = ViewState.Orders.ToList();

                if (ViewState.PrevCursor != null || orders.Count == 0)
                    return null;

                var today = DateTime.Today;
                var start = new DateTime(today.Year, today.Month, 1);

                if (ViewState.NextCursor != null && orders.Min(a => a.OrderDate) >= start)
                    return null;

                var month = orders
                    .Where(a => a.OrderDate >= start && !IsVoided(a) && a.OrderStatus != OrderStatus.Canceled)
                    .ToList();

                if (month.Count == 0)
                    return null;

                var name = today.ToString("MMMM", CultureInfo.CurrentCulture);
                name = char.ToUpper(name[0], CultureInfo.CurrentCulture) + name[1..];

                return new MonthTotal(
                    GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PURCHASES_SPENT, name),
                    month.Sum(a => a.TotalPrice),
                    month.Sum(a => a.TotalPointsPrice),
                    month.Count,
                    month.Sum(a => a.TotalPointsAward));
            }
        }

        protected static string PaymentKey(UserOrderViewState order) => order.Invoice?.PaymentStatus switch
        {
            InvoiceStatus.Unpaid => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_PURCHASES_ORDER_INVOICE_STATUS_UNPAID),
            InvoiceStatus.PartiallyPaid => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_PURCHASES_ORDER_INVOICE_STATUS_PARTIALLY_PAID),
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
