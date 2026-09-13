using Gizmo.Client.Options;
using Gizmo.Client.UI.Localization;
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
    public partial class Home : CustomDOMComponentBase
    {
        #region CONSTANTS

        //The board shows what a person acts on, not a catalogue: the full lists
        //live behind the Shop and Games routes on the left rail. Nothing here is
        //driven by a config number - these are the sizes the layout was drawn for.
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

        //Executables of the applications the hero shows, fetched the first time a slide
        //needs them - see HeroExecutables.
        private readonly Dictionary<int, IReadOnlyList<AppExeViewState>> _heroExecutables = new();
        private readonly HashSet<int> _heroExecutablesLoading = new();

        private Timer? _slideTimer;
        private int _slide;

        //Package currently being added to the cart: the button goes dark meanwhile, or a
        //second press sends a second copy.
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

        /// <summary>
        /// Time packages that can be bought at this moment, most popular first.
        /// </summary>
        /// <remarks>
        /// Sourced from the whole catalogue, not from HomePageViewState's popular
        /// list. That list is truncated server side by MaxPopularProducts BEFORE
        /// anything can be filtered out of it, so on a club whose popular items
        /// are mostly food only a package or two survived while plenty more were
        /// on sale. Popularity is still honoured - the popular ones keep their
        /// order and lead, the rest simply follow.
        ///
        /// DisallowPurchase is computed server side from the availability window
        /// (dates, weekdays, time of day), guest and user group restrictions and
        /// stock, so a package that cannot be bought right now is never shown.
        /// </remarks>
        private IEnumerable<UserProductViewState> TimeProducts =>
            Purchasable(a => a.ProductType == ProductType.ProductTime).Take(PACKS_CEILING);

        private IEnumerable<UserProductViewState> ShopProducts =>
            Purchasable(a => a.ProductType != ProductType.ProductTime);

        /// <summary>
        /// Goods the hero rotates through when the club has no banner.
        /// </summary>
        private IReadOnlyList<UserProductViewState> HeroProducts =>
            ShopProducts.Take(HERO_SLIDES).ToList();

        /// <summary>
        /// The short shop list in the till.
        /// </summary>
        /// <remarks>
        /// Takes what the hero is not already showing, so the same three drinks
        /// are not on screen twice - unless the club sells so little that there
        /// is nothing else to show, in which case repeating beats an empty list.
        /// </remarks>
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

        /// <summary>
        /// Applications, most popular first.
        /// </summary>
        /// <remarks>
        /// Same trap as the products had, a different setting: PopularApplications
        /// is truncated server side by MaxPopularApplications, so raising any
        /// number on this page changed nothing at all. The full catalogue is read
        /// instead and the popular ones keep their order at the front.
        /// </remarks>
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

        /// <summary>
        /// The band of games under the hero: the most popular ones, always.
        /// </summary>
        /// <remarks>
        /// The band is the fixed row a person scans for the game they came to play, so
        /// it gets the head of the popularity order regardless of what the hero shows.
        /// </remarks>
        private IReadOnlyList<AppViewState> StripApps => Apps.Take(STRIP_APPS).ToList();

        /// <summary>
        /// Applications the hero rotates through when the club has no news.
        /// </summary>
        /// <remarks>
        /// The ones after the band: the hero is the slot for what a person would not
        /// have looked for, so it promotes the next tier rather than repeating the
        /// four games already on the row below. A club with fewer games than the band
        /// holds has nothing left to promote and the hero shows the band's own instead
        /// of going empty.
        /// </remarks>
        private IReadOnlyList<AppViewState> HeroApps
        {
            get
            {
                var next = Apps.Skip(STRIP_APPS).Take(HERO_SLIDES).ToList();

                return next.Count > 0 ? next : Apps.Take(HERO_SLIDES).ToList();
            }
        }

        /// <summary>
        /// Launch buttons for the application on the hero: its executables, at most a few.
        /// </summary>
        /// <remarks>
        /// Fetched the first time a slide asks for them and kept for the life of the page;
        /// the slide renders without buttons for the moment the lookup takes and again
        /// with them. Most applications have exactly one executable. The rest are one
        /// click away behind Details.
        /// </remarks>
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
                        //No buttons for this one, then; Details still opens the page.
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
                //popular first in their own order, everything else after by the
                //display order the operator set in the admin
                .OrderBy(a => popularOrder.TryGetValue(a.Id, out var rank) ? rank : int.MaxValue)
                .ThenBy(a => a.DisplayOrder)
                .ThenBy(a => a.Name);
        }

        #endregion

        #region HERO

        /// <summary>
        /// What the hero is showing.
        /// </summary>
        /// <remarks>
        /// One slot, one rule, and it is never the shop list: the hero answers
        /// "what is worth looking at" while the till answers "what can I buy".
        /// Because they answer different questions they never take each other's
        /// room - adding a banner hides nothing, and removing the last one leaves
        /// no hole.
        /// <para>
        /// News first, then the popular games, then goods. Games before goods because
        /// that is what a club is: somebody who sat down came to play, and a slot this
        /// big should show them what to play before it shows them a drink.
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// Index of the slide on screen, kept inside the current slide count.
        /// </summary>
        /// <remarks>
        /// Wrapped on read rather than reset when the content changes: banners and
        /// goods can both come and go while the page is open, and a stale index
        /// would otherwise blank the hero until the next tick.
        /// </remarks>
        private int SlideIndex => SlideCount > 0 ? _slide % SlideCount : 0;

        private AdvertisementViewState? CurrentBanner =>
            Hero == HeroKind.Banner && Banners.Count > 0 ? Banners[SlideIndex] : null;

        private AppViewState? CurrentHeroApp =>
            Hero == HeroKind.App && HeroApps.Count > 0 ? HeroApps[SlideIndex] : null;

        private UserProductViewState? CurrentHeroProduct =>
            Hero == HeroKind.Product && HeroProducts.Count > 0 ? HeroProducts[SlideIndex] : null;

        /// <summary>
        /// Club news for the hero, in display order.
        /// </summary>
        /// <remarks>
        /// All of them, not only those with an image. The old image filter dropped news
        /// written as plain text - a tournament notice, prices, opening hours - and the
        /// hero silently fell back to popular products, which read as "the banner is
        /// broken". The stock AdsCarouselItem draws the item, so only the id is needed
        /// here; the list exists for display order and the slide count.
        /// </remarks>
        private List<AdvertisementViewState> Banners =>
            AdvertisementsViewState.Advertisements
                .Take(HERO_SLIDES)
                .ToList();

        private bool HasPromo => Banners.Count > 0;

        /// <summary>
        /// Moves the hero on one slide.
        /// </summary>
        /// <remarks>
        /// A timer rather than a CSS animation because each slide carries its own
        /// button: with every slide rendered at once, the buttons of the invisible
        /// ones stay in the document and stay clickable. One slide is rendered at
        /// a time and the fade is a mount animation on it.
        ///
        /// The callback is deliberately not async: an unobserved exception on a
        /// timer thread takes the whole client down (see DispatchWorkflow).
        /// </remarks>
        private void OnSlideTick(object? _)
        {
            if (SlideCount <= 1)
                return;

            _slide++;
            DispatchStateHasChanged();
        }

        #endregion

        #region FUNCTIONS

        private void OpenProduct(int productId)
        {
            if (ProductDetailsPageViewState.DisableProductDetails)
                return;

            NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + $"?ProductId={productId}");
        }

        /// <summary>
        /// The word on a lone launch button, as a CSS string literal.
        /// </summary>
        private static string LaunchLabel =>
            ShellStringOverrides.Get(ShellStringOverrides.GEN_LAUNCH).Replace("\\", "\\\\").Replace("'", "\\'");

        /// <summary>
        /// Whether the button for <paramref name="exe"/> should say "Launch" rather than
        /// its own caption: the application has one executable and it is named after the
        /// application, which the title above already says.
        /// </summary>
        private static bool IsPlainLaunch(AppViewState app, AppExeViewState exe, int count) =>
            count == 1 &&
            (string.IsNullOrWhiteSpace(exe.Caption) ||
             string.Equals(exe.Caption.Trim(), app.Title?.Trim(), StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Opens the application's own page, where every executable is listed.
        /// </summary>
        private void OpenApp(int applicationId)
        {
            if (AppDetailsPageViewState.DisableAppDetails)
                return;

            NavigationService.NavigateTo(ClientRoutes.ApplicationDetailsRoute + $"?ApplicationId={applicationId}");
        }

        /// <summary>
        /// The price on a package row: money, or points for a package sold for points
        /// alone - a zero there would read as free.
        /// </summary>
        private static bool IsPointsOnly(UserProductViewState product) =>
            product.UnitPrice == 0 && (product.UnitPointsPrice ?? 0) > 0;

        /// <summary>
        /// Puts a shop item in the cart and takes the customer to it.
        /// </summary>
        /// <remarks>
        /// Goods are ordered by the basket - the cart panel lives on the shop
        /// page, so that is where the order is finished. Adding without going
        /// there left people staring at a home screen wondering whether the tap
        /// had registered.
        /// </remarks>
        private void AddToCart(int productId)
        {
            CartService.AddProduct(productId);
            NavigationService.NavigateTo(ClientRoutes.ShopRoute);
        }

        /// <summary>
        /// Buys a single time package without the cart screen.
        /// </summary>
        /// <remarks>
        /// The dialog is only on the concrete ClientDialogService, not on the
        /// interface: IClientDialogService lives in an assembly the server does
        /// not reload from the skin, so it cannot grow new members. If some other
        /// implementation is ever injected the package simply opens its details
        /// page, which is where Buy used to lead.
        /// </remarks>
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
                    //The ordering lives in PackagePurchaseFlow: the same button exists on
                    //a package in the shop, and it must not be repeated in two places.
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

        /// <summary>
        /// Runs the hero's slide timer only while there is a slideshow worth running.
        /// </summary>
        /// <remarks>
        /// A tick is a re-render of the whole board, and the host never suspends the
        /// WebView when its window goes behind a game - so a timer left running there
        /// costs the customer frames in the game for a hero nobody can see. One slide has
        /// nothing to rotate to either.
        /// </remarks>
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

        //Static event arriving from JS interop; marshal to the UI thread, like everything
        //else subscribed to something that outlives the component.
        private void OnActivityChanged() => DispatchWorkflow(() =>
        {
            ApplySlideTimer();
            return Task.CompletedTask;
        });

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            //News and products load after the first render, so the slide count moves under
            //our feet; re-check after every render.
            ApplySlideTimer();
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                //GetFilteredStatesAsync already drops deleted, counter-only and
                //guest-restricted products, so what comes back is what this user
                //may actually order.
                _catalogue = await ProductLookupService.GetFilteredStatesAsync(null);
            }
            catch (Exception)
            {
                //A catalogue that will not load is not a reason to show nothing:
                //the popular list is still there as a fallback (see Purchasable).
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
            //Before dropping the timer: the event is static and outlives the page.
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
