using Gizmo.Client.Options;
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
        private const int BAR_ITEMS = 4;
        private const int STRIP_APPS = 4;
        private const int PACKS_CEILING = 24;

        private static readonly TimeSpan SLIDE_INTERVAL = TimeSpan.FromSeconds(7);

        #endregion

        #region FIELDS

        private IEnumerable<UserProductViewState> _catalogue = Enumerable.Empty<UserProductViewState>();
        private IEnumerable<AppViewState> _apps = Enumerable.Empty<AppViewState>();

        private Timer? _slideTimer;
        private int _slide;

        //Пакет, который сейчас кладётся в корзину: кнопка на это время гаснет,
        //иначе вторым нажатием в корзину уедет второй такой же.
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
        private IReadOnlyList<AppViewState> StripApps
        {
            get
            {
                var popularOrder = ViewState.PopularApplications
                    .Select((app, index) => (app.ApplicationId, index))
                    .ToDictionary(a => a.ApplicationId, a => a.index);

                var source = _apps.Any() ? _apps : ViewState.PopularApplications;

                return source
                    .OrderBy(a => popularOrder.TryGetValue(a.ApplicationId, out var rank) ? rank : int.MaxValue)
                    .ThenBy(a => a.Title)
                    .Take(STRIP_APPS)
                    .ToList();
            }
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
        /// </remarks>
        private enum HeroKind { Banner, Product, Empty }

        private HeroKind Hero
        {
            get
            {
                if (HasPromo)
                    return HeroKind.Banner;

                return HeroProducts.Count > 0 ? HeroKind.Product : HeroKind.Empty;
            }
        }

        private int SlideCount => Hero switch
        {
            HeroKind.Banner => Banners.Count,
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

        private UserProductViewState? CurrentHeroProduct =>
            Hero == HeroKind.Product && HeroProducts.Count > 0 ? HeroProducts[SlideIndex] : null;

        /// <summary>
        /// Новости клуба для героя, в порядке показа.
        /// </summary>
        /// <remarks>
        /// Берутся ВСЕ, а не только с картинкой. Раньше стоял фильтр по
        /// изображению - и новость, набранная в менеджере одним текстом (а это
        /// обычный случай: объявление о турнире, о ценах, о режиме работы),
        /// отсеивалась. Герой молча откатывался на популярные товары, и со
        /// стороны это читалось как «баннер не работает».
        ///
        /// Рисует новость штатный AdsCarouselItem, отсюда ему нужен только
        /// идентификатор; список держим ради порядка показа и счётчика слайдов.
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
            if (DialogService is not ClientDialogService dialogService)
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
                    //Пакет кладётся в корзину ЗДЕСЬ, до открытия окна покупки, и
                    //это не косметика.
                    //
                    //ClientServerCartViewService.ValidateRequestAsync, прежде чем
                    //добавить пакет времени, у которого сейчас не его час
                    //(UsageAvailability), показывает штатный вопрос «товар сейчас
                    //недоступен, всё равно добавить?» и ЖДЁТ ответа. Диалоги в
                    //оболочке - очередь, а не стопка: пока открыто моё окно, этот
                    //вопрос стоит в очереди за ним и на экран не выходит. Ответа
                    //нет - запись в корзине не появляется - моё окно крутит
                    //спиннер вечно. Ровно это и видел клуб на «Ночи» вне её окна
                    //покупки.
                    //
                    //Пока моего окна нет, вопрос показывается нормально, и я
                    //открываю окно только с уже лежащей в корзине записью.
                    var before = CartService.ViewState.Products.Select(a => a.Guid).ToHashSet();

                    CartService.AddProduct(productId);

                    var entryId = await WaitForCartEntryAsync(before);

                    //Ответили «нет» на предупреждение или добавление не прошло -
                    //показывать окно не с чем.
                    if (entryId is null)
                        return;

                    var dialog = await dialogService.ShowPackagePurchaseDialogAsync(productId, entryId.Value);

                    if (dialog.Result == AddComponentResultCode.Opened)
                        await dialog.WaitForResultAsync();
                }
                finally
                {
                    _buyingProductId = null;
                    StateHasChanged();
                }
            });
        }

        /// <summary>
        /// Ждёт появления в корзине записи, которой там не было.
        /// </summary>
        /// <remarks>
        /// Опрос, а не подписка: изменение корзины приходит через RaiseChanged на
        /// её view state, но ждать надо конкретную запись, и опрос раз в пятую
        /// долю секунды тут дешевле и понятнее.
        ///
        /// Полторы минуты - это не «сколько идёт запрос», а «сколько человек
        /// может читать предупреждение перед тем, как нажать Да»: сам запрос
        /// укладывается в доли секунды. Отличить «читает» от «отказался» нельзя:
        /// проверка идёт в конвейере ДО того, как корзина поднимает флаги
        /// занятости, так что по состоянию корзины оба случая выглядят одинаково
        /// пустыми. Поэтому просто потолок, после которого кнопка оживает.
        /// </remarks>
        private async Task<Guid?> WaitForCartEntryAsync(HashSet<Guid> before)
        {
            var deadline = DateTime.UtcNow.AddSeconds(90);

            while (DateTime.UtcNow < deadline)
            {
                var entry = CartService.ViewState.Products.FirstOrDefault(a => !before.Contains(a.Guid));

                if (entry is not null)
                    return entry.Guid;

                await Task.Delay(200);
            }

            return null;
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserBalanceViewState);
            this.SubscribeChange(AdvertisementsViewState);

            _slideTimer = new Timer(OnSlideTick, null, SLIDE_INTERVAL, SLIDE_INTERVAL);

            base.OnInitialized();
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
