using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class TimeProductRow : CustomDOMComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        protected int? Order => Product.ActivationOrder;

        protected bool IsCurrent => Order == 1;

        protected bool IsIdle => !Order.HasValue;

        protected string OrderText => Order?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;

        protected string RowClass
        {
            get
            {
                var css = "giz-account-time__row";

                if (IsCurrent)
                    css += " giz-account-time__row--current";

                if (IsIdle)
                    css += " giz-account-time__row--idle";

                if (Product.InCredit)
                    css += " giz-account-time__row--credit";

                return css;
            }
        }

        private UserProductViewState _product;

        [Inject]
        UserProductViewStateLookupService ProductLookup { get; set; }

        [Parameter]
        public TimeProductViewState Product { get; set; }

        [Parameter]
        public bool CanOpen { get; set; }

        [Parameter]
        public EventCallback OnOpen { get; set; }

        private string Expiry
        {
            get
            {
                if (_product is null || _product.ProductType != ProductType.ProductTime || _product.TimeProduct is null)
                    return null;

                var time = _product.TimeProduct;
                var options = time.ExpirationOptions;
                var parts = new List<string>();
                var culture = CultureInfo.CurrentCulture;

                if (options.HasFlag(ProductTimeExpirationOptionType.ExpireAfterTime))
                {
                    var unit = time.ExpireAfterType switch
                    {
                        ExpireAfterType.Day => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_DAYS_ABBREVIATED),
                        ExpireAfterType.Hour => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_HOURS_ABBREVIATED),
                        _ => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_MINUTES_ABBREVIATED),
                    };
                    var fromUse = time.ExpireFromOptions == ExpireFromOptionType.Use;
                    var span = $"{time.ExpiresAfter} {GrafitLocalization.GetString(unit)}";
                    var from = GrafitLocalization.GetString(fromUse ? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_TIME_PRODUCTS_PROPERTIES_EXPIRES_FROM_USE) : nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_TIME_PRODUCTS_PROPERTIES_EXPIRES_FROM_PURCHASE), span);

                    parts.Add(GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PD_EXPIRES_AFTER, from));

                    var started = fromUse ? Product.FirstUsageTime : Product.PurchaseTime;
                    if (started.HasValue)
                        parts.Add(GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_TIME_PRODUCTS_PROPERTIES_EXPIRES_ACTIVATED), started.Value.ToLocalTime().ToString("g", culture)));
                }
                else if (options.HasFlag(ProductTimeExpirationOptionType.ExpireAtDayTime))
                {
                    var at = DateTime.Today.AddMinutes(time.ExpireAtDayTimeMinute).ToString("t", culture);
                    parts.Add(GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PD_EXPIRES_AT_DAYTIME, at));
                }

                if (options.HasFlag(ProductTimeExpirationOptionType.ExpiresAtLogout))
                    parts.Add(GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PD_EXPIRES_AT_LOGOUT));

                return parts.Count == 0
                    ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_ACCOUNT_EXPIRES_NEVER)
                    : string.Join(" · ", parts);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            if (Product?.ProductId is int productId)
            {
                try
                {
                    _product = await ProductLookup.GetStateAsync(productId);
                }
                catch (Exception)
                {
                    _product = null;
                }

                if (_product is not null)
                    this.SubscribeChange(_product);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_product is not null)
                this.UnsubscribeChange(_product);

            base.Dispose();
        }
    }
}
