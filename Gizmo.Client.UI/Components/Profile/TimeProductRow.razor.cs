using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using ShellText = Gizmo.Client.UI.Localization.ShellStringOverrides;

namespace Gizmo.Client.UI.Components
{
    public partial class TimeProductRow : CustomDOMComponentBase
    {
        //The catalogue product behind the time product, for its expiry rules. Null while
        //loading, for a product that is no longer in the catalogue, and for rate-based time.
        private UserProductViewState _product;

        [Inject]
        UserProductViewStateLookupService ProductLookup { get; set; }

        [Parameter]
        public TimeProductViewState Product { get; set; }

        [Parameter]
        public bool CanOpen { get; set; }

        [Parameter]
        public EventCallback OnOpen { get; set; }

        /// <summary>
        /// When the product stops working, in words: "Expires after 3 d. from purchase ·
        /// Activated 12.03.2026 14:00", "Expires at 06:00", "Expires on sign-out", or
        /// "Does not expire". Null until the catalogue product is known.
        /// </summary>
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
                        ExpireAfterType.Day => ShellText.EXPIRE_DAYS_ABBR,
                        ExpireAfterType.Hour => ShellText.EXPIRE_HOURS_ABBR,
                        _ => ShellText.EXPIRE_MINUTES_ABBR,
                    };
                    var fromUse = time.ExpireFromOptions == ExpireFromOptionType.Use;
                    var span = $"{time.ExpiresAfter} {ShellText.Get(unit)}";
                    var from = ShellText.Get(fromUse ? ShellText.TIME_EXPIRES_FROM_USE : ShellText.TIME_EXPIRES_FROM_PURCHASE, span);

                    parts.Add(ShellText.Get(ShellText.PD_EXPIRES_AFTER, from));

                    //The countdown has started once the product was used (or bought, for
                    //the other rule); say when, since that is what the remaining time hangs on.
                    var started = fromUse ? Product.FirstUsageTime : Product.PurchaseTime;
                    if (started.HasValue)
                        parts.Add(ShellText.Get(ShellText.TIME_ACTIVATED, started.Value.ToLocalTime().ToString("g", culture)));
                }
                else if (options.HasFlag(ProductTimeExpirationOptionType.ExpireAtDayTime))
                {
                    var at = DateTime.Today.AddMinutes(time.ExpireAtDayTimeMinute).ToString("t", culture);
                    parts.Add(ShellText.Get(ShellText.PD_EXPIRES_AT_DAYTIME, at));
                }

                if (options.HasFlag(ProductTimeExpirationOptionType.ExpiresAtLogout))
                    parts.Add(ShellText.Get(ShellText.PD_EXPIRES_AT_LOGOUT));

                return parts.Count == 0
                    ? ShellText.Get(ShellText.ACCOUNT_EXPIRES_NEVER)
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
                    //A product that has left the catalogue: the row shows without its expiry.
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
