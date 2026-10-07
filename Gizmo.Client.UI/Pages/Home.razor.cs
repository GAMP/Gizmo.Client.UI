using Gizmo.Client.Options;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_HOME)]
    [PageUIModule(TitleLocalizationKey = "GIZ_MODULE_PAGE_HOME_TITLE", DescriptionLocalizationKey = "GIZ_MODULE_PAGE_HOME_TITLE"), ModuleDisplayOrder(0)]
    [Route(ClientRoutes.HomeRoute)]
    public partial class Home : ShellComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        #region CONSTANTS

        private const int HERO_SLIDES = 5;
        private const int HERO_NEWS_CEILING = 30;
        private const int HERO_DOTS = 10;
        private const int HERO_EXECUTABLES = 3;
        private const int GOODS_CEILING = 24;
        private const int STRIP_APPS = 4;
        private const int PACKS_CEILING = 24;

        private static readonly TimeSpan SLIDE_INTERVAL = TimeSpan.FromSeconds(7);

        #endregion

        #region FIELDS

        private IEnumerable<UserProductViewState> _catalogue = Enumerable.Empty<UserProductViewState>();
        private IEnumerable<AppViewState> _apps = Enumerable.Empty<AppViewState>();

        private readonly Dictionary<int, IReadOnlyList<AppExeViewState>> _heroExecutables = new();
        private readonly HashSet<int> _heroExecutablesLoading = new();

        private Timer? _slideTimer;
        private int _slide;

        private int? _buyingProductId;
        private TillTab _tab = TillTab.Time;
        private int? _group;
        private IReadOnlyList<UserProductGroupViewState> _groups = Array.Empty<UserProductGroupViewState>();
        private readonly CancellationTokenSource _lifetime = new();

        #endregion

        #region PROPERTIES

        [Inject] IOptionsMonitor<ClientInterfaceOptions> ClientInterfaceOptions { get; set; }
        [Inject] ILocalizationService LocalizationService { get; set; }
        [Inject] HomePageViewState ViewState { get; set; }
        [Inject] ILogger<Home> Logger { get; set; }
        [Inject] UserBalanceViewState UserBalanceViewState { get; set; }
        [Inject] AdvertisementsViewState AdvertisementsViewState { get; set; }
        [Inject] ProductDetailsPageViewState ProductDetailsPageViewState { get; set; }
        [Inject] UserProductViewStateLookupService ProductLookupService { get; set; }
        [Inject] AppViewStateLookupService AppLookupService { get; set; }
        [Inject] AppExeViewStateLookupService AppExeLookupService { get; set; }
        [Inject] AppDetailsPageViewState AppDetailsPageViewState { get; set; }
        [Inject] ClientServerCartViewService CartService { get; set; }
        [Inject] UserCartViewService CheckoutService { get; set; }
        [Inject] NavigationService NavigationService { get; set; }
        [Inject] UserProductGroupViewStateLookupService GroupLookupService { get; set; }

        #endregion

        #region CONTENT

        private IEnumerable<UserProductViewState> TimeProducts =>
            Purchasable(a => a.ProductType == ProductType.ProductTime).Take(PACKS_CEILING);

        private sealed record PackRow(UserProductViewState Product, string Length, string Unit, bool IsBest);

        private IReadOnlyList<PackRow> _packs = Array.Empty<PackRow>();

        private IReadOnlyList<PackRow> Packs => _packs;

        private void BuildPacks()
        {
            var products = TimeProducts.ToList();
            var best = BestValue(products);

            _packs = products.Select(a => ToRow(a, a.Id == best)).ToList();
        }

        private static int? BestValue(IReadOnlyList<UserProductViewState> products)
        {
            var hourly = products
                .Where(a => !IsPointsOnly(a) && a.UnitPrice > 0 && (a.TimeProduct?.Minutes ?? 0) >= 60)
                .Select(a => (a.Id, PerHour: a.UnitPrice / (a.TimeProduct.Minutes / 60m)))
                .OrderBy(a => a.PerHour)
                .ToList();

            return hourly.Count > 1 && hourly[0].PerHour < hourly[1].PerHour ? hourly[0].Id : null;
        }

        private void OnProductsChanged(object sender, EventArgs e) => BuildPacks();

        private PackRow ToRow(UserProductViewState product, bool isBest)
        {
            var minutes = product.TimeProduct?.Minutes ?? 0;

            string length = null;
            var unit = string.Empty;

            if (minutes > 0 && minutes < 60)
            {
                length = minutes.ToString(CultureInfo.CurrentCulture);
                unit = GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PACK_UNIT_MIN);
            }
            else if (minutes >= 60)
            {
                var hours = minutes / 60m;
                length = hours.ToString(hours == decimal.Truncate(hours) ? "0" : "0.#", CultureInfo.CurrentCulture);
                unit = GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PACK_UNIT_H);
            }

            return new PackRow(product, length, unit, isBest);
        }

        private string PackClass(PackRow pack) => _buyingProductId == pack.Product.Id
            ? "giz-home-pack giz-home-pack--busy"
            : "giz-home-pack";

        private enum TillTab { Time, Bar }

        private bool HasGoods => ShopProducts.Any();

        private bool ShowTillTabs => Packs.Count > 0 && HasGoods;

        private TillTab ActiveTab => !HasGoods ? TillTab.Time : Packs.Count == 0 ? TillTab.Bar : _tab;

        private string TabClass(TillTab tab) => ActiveTab == tab
            ? "giz-home-tabs__tab giz-home-tabs__tab--active"
            : "giz-home-tabs__tab";

        private void SelectTab(TillTab tab) => _tab = tab;

        private int GoodsCount => ShopProducts.Count();

        private IReadOnlyList<UserProductGroupViewState> BarGroups
        {
            get
            {
                var used = ShopProducts.Select(a => a.ProductGroupId).ToHashSet();

                return _groups
                    .Where(a => used.Contains(a.ProductGroupId))
                    .OrderBy(a => a.DisplayOrder)
                    .ThenBy(a => a.Name)
                    .ToList();
            }
        }

        private int? ActiveGroup => _group is int group && BarGroups.Any(a => a.ProductGroupId == group) ? group : null;

        private string GroupClass(int? group) => ActiveGroup == group
            ? "giz-home-cats__cat giz-home-cats__cat--active"
            : "giz-home-cats__cat";

        private void SelectGroup(int? group) => _group = group;

        private IReadOnlyList<UserProductViewState> BarGoods => ShopProducts
            .Where(a => ActiveGroup is not int group || a.ProductGroupId == group)
            .Take(GOODS_CEILING)
            .ToList();

        private UserCartProductViewState CartLine(int productId) =>
            CartService.ViewState.Products.FirstOrDefault(a => a.ProductId == productId);

        private string GoodClass(UserProductViewState product) => CartLine(product.Id) is { Quantity: > 0 }
            ? "giz-home-good giz-home-good--in"
            : "giz-home-good";

        private bool CartBusy => PackagePurchaseFlow.IsCartBusy(CartService);

        private int CartCount => CartService.ViewState.Products.Sum(a => a.Quantity);

        private bool ShowCartMoney => CartService.ViewState.Total > 0 || CartService.ViewState.PointsTotal == 0;

        private string CartPointsText => CartService.ViewState.PointsTotal.ToString("N0", CultureInfo.CurrentCulture);

        private string CartSummary
        {
            get
            {
                var lines = CartService.ViewState.Products.ToList();

                return lines.Count == 1
                    ? $"{lines[0].ProductName} × {lines[0].Quantity}"
                    : GrafitLocalization.GetPluralString(GrafitResourceKeys.SHELL_BUY_ITEMS_COUNT, CartCount);
            }
        }

        private void AddGood(int productId) => CartService.AddProduct(productId);

        private void Increase(UserCartProductViewState line) => CartService.SetQuantity(line.Guid, line.Quantity + 1);

        private void Decrease(UserCartProductViewState line)
        {
            if (line.Quantity > 1)
                CartService.SetQuantity(line.Guid, line.Quantity - 1);
            else
                CartService.RemoveEntry(line.Guid);
        }

        private void Checkout()
        {
            if (CartBusy)
                return;

            DispatchWorkflow(() => CheckoutService.SubmitAsync());
        }

        private bool HasSales => TimeProducts.Any() || HasGoods;

        private string BoardClass => HasSales ? "giz-home-board" : "giz-home-board giz-home-board--bare";

        private bool HasPoints => UserBalanceViewState.PointsBalance > 0;

        private string PointsText => UserBalanceViewState.PointsBalance.ToString("N0", CultureInfo.CurrentCulture);

        private IEnumerable<UserProductViewState> ShopProducts =>
            Purchasable(a => a.ProductType != ProductType.ProductTime);

        private IReadOnlyList<UserProductViewState> HeroProducts =>
            ShopProducts.Take(HERO_SLIDES).ToList();

        private IEnumerable<AppViewState> Apps
        {
            get
            {
                var popularOrder = ViewState.PopularApplications
                    .Select((app, index) => (app.ApplicationId, index))
                    .ToDictionary(a => a.ApplicationId, a => a.index);

                var source = _apps.Any() ? _apps : ViewState.PopularApplications;

                return source
                    .OrderBy(a => popularOrder.TryGetValue(a.ApplicationId, out var rank) ? rank : int.MaxValue)
                    .ThenBy(a => a.Title);
            }
        }

        private IReadOnlyList<AppViewState> StripApps => Apps.Take(HasSales ? STRIP_APPS : STRIP_APPS + 1).ToList();

        private IReadOnlyList<AppViewState> HeroApps
        {
            get
            {
                var next = Apps.Skip(STRIP_APPS).Take(HERO_SLIDES).ToList();

                return next.Count > 0 ? next : Apps.Take(HERO_SLIDES).ToList();
            }
        }

        private IReadOnlyList<AppExeViewState> HeroExecutables(int applicationId)
        {
            if (_heroExecutables.TryGetValue(applicationId, out var known))
                return known;

            if (_heroExecutablesLoading.Add(applicationId))
            {
                DispatchWorkflow(async () =>
                {
                    IReadOnlyList<AppExeViewState> executables;

                    try
                    {
                        var all = await AppExeLookupService.GetFilteredStatesAsync(applicationId);

                        executables = all
                            .OrderBy(a => a.DisplayOrder)
                            .Take(HERO_EXECUTABLES)
                            .ToList();
                    }
                    catch (Exception)
                    {
                        executables = Array.Empty<AppExeViewState>();
                    }

                    _heroExecutables[applicationId] = executables;
                    _heroExecutablesLoading.Remove(applicationId);
                    StateHasChanged();
                });
            }

            return Array.Empty<AppExeViewState>();
        }

        private IEnumerable<UserProductViewState> Purchasable(Func<UserProductViewState, bool> predicate)
        {
            if (!ViewState.IsShopEnabled)
                return Enumerable.Empty<UserProductViewState>();

            var popularOrder = ViewState.PopularProducts
                .Select((product, index) => (product.Id, index))
                .ToDictionary(a => a.Id, a => a.index);

            var source = _catalogue.Any() ? _catalogue : ViewState.PopularProducts;

            return source
                .Where(a => !a.IsDeleted && !a.DisallowPurchase)
                .Where(predicate)
                .OrderBy(a => popularOrder.TryGetValue(a.Id, out var rank) ? rank : int.MaxValue)
                .ThenBy(a => a.DisplayOrder)
                .ThenBy(a => a.Name);
        }

        #endregion

        #region HERO

        private enum HeroKind { Banner, App, Product, Empty }

        private HeroKind Hero
        {
            get
            {
                if (HasPromo)
                    return HeroKind.Banner;

                if (HeroApps.Count > 0)
                    return HeroKind.App;

                return HeroProducts.Count > 0 ? HeroKind.Product : HeroKind.Empty;
            }
        }

        private int SlideCount => Hero switch
        {
            HeroKind.Banner => Banners.Count,
            HeroKind.App => HeroApps.Count,
            HeroKind.Product => HeroProducts.Count,
            _ => 0,
        };

        private int SlideIndex => SlideCount > 0 ? _slide % SlideCount : 0;

        private bool ShowSlideDots => SlideCount <= HERO_DOTS;

        private string SlidePositionText => $"{SlideIndex + 1} / {SlideCount}";

        private AdvertisementViewState? CurrentBanner =>
            Hero == HeroKind.Banner && Banners.Count > 0 ? Banners[SlideIndex] : null;

        private AppViewState? CurrentHeroApp =>
            Hero == HeroKind.App && HeroApps.Count > 0 ? HeroApps[SlideIndex] : null;

        private UserProductViewState? CurrentHeroProduct =>
            Hero == HeroKind.Product && HeroProducts.Count > 0 ? HeroProducts[SlideIndex] : null;

        private List<AdvertisementViewState> Banners =>
            AdvertisementsViewState.Advertisements
                .Take(HERO_NEWS_CEILING)
                .ToList();

        private bool HasPromo => Banners.Count > 0;

        private void OnSlideTick(object? _) => DispatchWorkflow(() =>
        {
            if (SlideCount > 1)
            {
                _slide++;
                StateHasChanged();
            }

            return Task.CompletedTask;
        });

        #endregion

        #region FUNCTIONS

        private void OpenProduct(int productId)
        {
            if (ProductDetailsPageViewState.DisableProductDetails)
                return;

            NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + $"?ProductId={productId}");
        }

        private string LaunchLabel =>
            GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_LAUNCH)).Replace("\\", "\\\\").Replace("'", "\\'");

        private bool IsPlainLaunch(AppViewState app, AppExeViewState exe) =>
            HeroExecutables(app.ApplicationId).Count == 1 &&
            (string.IsNullOrWhiteSpace(exe.Caption) ||
             string.Equals(exe.Caption.Trim(), app.Title?.Trim(), StringComparison.OrdinalIgnoreCase));

        private string HeroExeClass(AppViewState app, AppExeViewState exe) =>
            IsPlainLaunch(app, exe) ? "giz-home-hero__exe giz-home-hero__exe--play" : "giz-home-hero__exe";

        private void OpenApp(int applicationId)
        {
            if (AppDetailsPageViewState.DisableAppDetails)
                return;

            NavigationService.NavigateTo(ClientRoutes.ApplicationDetailsRoute + $"?ApplicationId={applicationId}");
        }

        private static bool IsPointsOnly(UserProductViewState product) =>
            product.UnitPrice == 0 && (product.UnitPointsPrice ?? 0) > 0;

        private static string Money(decimal amount) => amount.ToString("C", CultureInfo.CurrentCulture);

        private static string ShortMoney(decimal amount) =>
            amount.ToString(amount == decimal.Truncate(amount) ? "C0" : "C", CultureInfo.CurrentCulture);

        private static string PointsPrice(UserProductViewState product) =>
            product.UnitPointsPrice.GetValueOrDefault().ToString("N0", CultureInfo.CurrentCulture);

        private void AddToCart(int productId)
        {
            CartService.AddProduct(productId);
            NavigationService.NavigateTo(ClientRoutes.ShopRoute);
        }

        private void BuyPackage(int productId)
        {
            if (_buyingProductId.HasValue || PackagePurchaseFlow.IsCartBusy(CartService))
                return;

            _buyingProductId = productId;
            StateHasChanged();

            DispatchWorkflow(async () =>
            {
                try
                {
                    await PackagePurchaseFlow.RunAsync(productId, CartService, CheckoutService, _lifetime.Token);
                }
                finally
                {
                    _buyingProductId = null;

                    if (!IsDisposed)
                        StateHasChanged();
                }
            });
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            BuildPacks();
            ViewState.OnChange += OnProductsChanged;
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserBalanceViewState);
            this.SubscribeChange(AdvertisementsViewState);
            this.SubscribeChange(CartService.ViewState);
            ShellActivity.Changed += OnActivityChanged;

            ApplySlideTimer();

            base.OnInitialized();
        }

        private void PreviousSlide() => StepSlide(SlideCount - 1);

        private void NextSlide() => StepSlide(1);

        private void StepSlide(int step)
        {
            if (SlideCount < 2)
                return;

            _slide = SlideIndex + step;
            _slideTimer?.Change(SLIDE_INTERVAL, SLIDE_INTERVAL);
        }

        private void ApplySlideTimer()
        {
            var wanted = ShellActivity.IsActive && SlideCount > 1;

            if (wanted == (_slideTimer is not null))
                return;

            if (wanted)
            {
                _slideTimer = new Timer(OnSlideTick, null, SLIDE_INTERVAL, SLIDE_INTERVAL);
            }
            else
            {
                _slideTimer?.Dispose();
                _slideTimer = null;
            }
        }

        private void OnActivityChanged() => DispatchWorkflow(() =>
        {
            ApplySlideTimer();
            return Task.CompletedTask;
        });

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            ApplySlideTimer();
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _catalogue = await ProductLookupService.GetFilteredStatesAsync(null);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Could not load the product catalogue for the home page.");
                _catalogue = Enumerable.Empty<UserProductViewState>();
            }

            BuildPacks();

            try
            {
                _groups = (await GroupLookupService.GetStatesAsync()).ToList();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Could not load the product groups for the home page.");
                _groups = Array.Empty<UserProductGroupViewState>();
            }

            try
            {
                _apps = await AppLookupService.GetFilteredStatesAsync();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Could not load the applications for the home page.");
                _apps = Enumerable.Empty<AppViewState>();
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            ShellActivity.Changed -= OnActivityChanged;

            _lifetime.Cancel();

            _slideTimer?.Dispose();
            _slideTimer = null;

            this.UnsubscribeChange(CartService.ViewState);
            this.UnsubscribeChange(AdvertisementsViewState);
            this.UnsubscribeChange(UserBalanceViewState);
            this.UnsubscribeChange(ViewState);
            ViewState.OnChange -= OnProductsChanged;

            base.Dispose();
        }

        #endregion
    }
}
