using Gizmo.Client.Options;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
        private const int HERO_EXECUTABLES = 3;
        private const int BAR_ITEMS = 4;
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

        #endregion

        #region PROPERTIES

        [Inject] IOptionsMonitor<ClientInterfaceOptions> ClientInterfaceOptions { get; set; }
        [Inject] ILocalizationService LocalizationService { get; set; }
        [Inject] HomePageViewState ViewState { get; set; }
        [Inject] UserBalanceViewState UserBalanceViewState { get; set; }
        [Inject] AdvertisementsViewState AdvertisementsViewState { get; set; }
        [Inject] ProductDetailsPageViewState ProductDetailsPageViewState { get; set; }
        [Inject] UserProductViewStateLookupService ProductLookupService { get; set; }
        [Inject] AppViewStateLookupService AppLookupService { get; set; }
        [Inject] AppExeViewStateLookupService AppExeLookupService { get; set; }
        [Inject] AppDetailsPageViewState AppDetailsPageViewState { get; set; }
        [Inject] ClientServerCartViewService CartService { get; set; }
        [Inject] IClientDialogService DialogService { get; set; }
        [Inject] NavigationService NavigationService { get; set; }

        #endregion

        #region CONTENT

        private IEnumerable<UserProductViewState> TimeProducts =>
            Purchasable(a => a.ProductType == ProductType.ProductTime).Take(PACKS_CEILING);

        private IEnumerable<UserProductViewState> ShopProducts =>
            Purchasable(a => a.ProductType != ProductType.ProductTime);

        private IReadOnlyList<UserProductViewState> HeroProducts =>
            ShopProducts.Take(HERO_SLIDES).ToList();

        private IReadOnlyList<UserProductViewState> BarProducts
        {
            get
            {
                if (HasPromo)
                    return ShopProducts.Take(BAR_ITEMS).ToList();

                var rest = ShopProducts.Skip(HeroProducts.Count).Take(BAR_ITEMS).ToList();

                return rest.Count > 0 ? rest : ShopProducts.Take(BAR_ITEMS).ToList();
            }
        }

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

        private IReadOnlyList<AppViewState> StripApps => Apps.Take(STRIP_APPS).ToList();

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

        private AdvertisementViewState? CurrentBanner =>
            Hero == HeroKind.Banner && Banners.Count > 0 ? Banners[SlideIndex] : null;

        private AppViewState? CurrentHeroApp =>
            Hero == HeroKind.App && HeroApps.Count > 0 ? HeroApps[SlideIndex] : null;

        private UserProductViewState? CurrentHeroProduct =>
            Hero == HeroKind.Product && HeroProducts.Count > 0 ? HeroProducts[SlideIndex] : null;

        private List<AdvertisementViewState> Banners =>
            AdvertisementsViewState.Advertisements
                .Take(HERO_SLIDES)
                .ToList();

        private bool HasPromo => Banners.Count > 0;

        private void OnSlideTick(object? _)
        {
            if (SlideCount <= 1)
                return;

            _slide++;
            DispatchRender();
        }

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

        private void AddToCart(int productId)
        {
            CartService.AddProduct(productId);
            NavigationService.NavigateTo(ClientRoutes.ShopRoute);
        }

        private void BuyPackage(int productId)
        {
            if (DialogService is not ClientDialogService)
            {
                OpenProduct(productId);
                return;
            }

            if (_buyingProductId.HasValue)
                return;

            _buyingProductId = productId;
            StateHasChanged();

            DispatchWorkflow(async () =>
            {
                try
                {
                    await PackagePurchaseFlow.RunAsync(productId, CartService, DialogService);
                }
                finally
                {
                    _buyingProductId = null;
                    StateHasChanged();
                }
            });
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserBalanceViewState);
            this.SubscribeChange(AdvertisementsViewState);

            ShellActivity.Changed += OnActivityChanged;

            ApplySlideTimer();

            base.OnInitialized();
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
            catch (Exception)
            {
                _catalogue = Enumerable.Empty<UserProductViewState>();
            }

            try
            {
                _apps = await AppLookupService.GetFilteredStatesAsync();
            }
            catch (Exception)
            {
                _apps = Enumerable.Empty<AppViewState>();
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            ShellActivity.Changed -= OnActivityChanged;

            _slideTimer?.Dispose();
            _slideTimer = null;

            this.UnsubscribeChange(AdvertisementsViewState);
            this.UnsubscribeChange(UserBalanceViewState);
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }

        #endregion
    }
}
