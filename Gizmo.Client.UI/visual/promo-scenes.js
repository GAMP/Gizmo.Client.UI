// The promo shots' cast: the palettes, the person at the keyboard and the scenes, one
// per place in the shell. Titles belong to no real game and goods to no brand (see
// NAMES.promo in templates/home.js), the pictures are drawn by lib/artwork.js.
//
// A scene is { id, page, data, note, hover? }: `page` picks the template, `data` goes
// over the PROFILE below, `hover` is a selector drawn in its :hover state.
"use strict";

const PALETTES = { blue: "#4f8cff", purple: "#b06bff", red: "#ff3b46", orange: "#ff8a3d", amber: "#f0b83d", green: "#4ade80", teal: "#22d3ee", pink: "#ff5fa8" };

// The ladder the club runs: four levels a month, points from play and the bar.
const LEVELS = [
  { name: "Новичок", threshold: 0 },
  { name: "Игрок", threshold: 1000 },
  { name: "Ветеран", threshold: 2500 },
  { name: "Легенда", threshold: 5000 },
];

const PROFILE = {
  username: "nightowl",
  fullName: "Артём Воронов",
  balance: 2480,
  points: 5120,
  time: "3ч 40м",
  pc: 7,
  tariff: "Стандарт",
  pinned: 4,
  names: "promo",
  busyPack: -1,
  deployName: "Nightfall",
  // The level: Veteran, 1 830 of the 2 500 to keep it, 5 000 for the next.
  levels: LEVELS,
  score: 1830,
  levelName: "Ветеран",
  levelMark: "В",
  levelProgress: 0.37,
  toRetain: 670,
  nextStep: "Заказ в баре",
  nextChallenge: "Ночной марафон",
  nextReward: 500,
};

// The operator's own HTML for the news tile, written the way a club writes one.
const NEWS_BODY = `<div style="padding: 3.2rem 3.6rem; max-width: 70%; text-shadow: 0 0.2rem 1.2rem rgba(0,0,0,0.45)">
  <div style="font-family: 'Manrope', sans-serif; font-size: 1.2rem; font-weight: 800; letter-spacing: 0.22em; text-transform: uppercase; opacity: 0.75">Турнир · пятница</div>
  <div style="margin-top: 1rem; font-family: 'Manrope', sans-serif; font-size: 4.4rem; font-weight: 800; line-height: 1.05; letter-spacing: -0.02em">Ночной марафон — с 22:00 до утра</div>
  <div style="margin-top: 1.4rem; font-size: 1.7rem; line-height: 1.4; opacity: 0.85">Команды по пять, призовой фонд 30 000 ₽ и бесплатные напитки участникам. Регистрация у администратора до 21:00.</div>
</div>`;

const HOME = { hero: "news", newsBody: NEWS_BODY, newsAction: "Смотреть подробности", apps: 8, packs: 6, bar: 4 };
const LOYAL = { ...HOME, loyalty: "earning" };

const TOOLTIP_TWO = { products: [{ name: "3 часа", type: "package", left: "2 ч 15 м", spendable: "2 ч 15 м" }, { name: "Ночь", type: "fixed", left: "8 ч", spendable: "8 ч" }], credit: true };
const TOOLTIP_ENDING = { products: [{ name: "Час", type: "package", left: "12 м", spendable: "12 м" }], ending: true };

const NOTIFICATIONS = [
  { type: "success", title: "Оплата прошла", message: "Пакет «3 часа» добавлен на ваш счёт." },
  { type: "info", title: "Сообщение от администратора", message: "Турнир начнётся в 22:00, сбор команд у стойки." },
  { type: "warning", title: "Осталось 10 минут", message: "Пополните счёт или возьмите пакет, чтобы не прерываться." },
];

const ORDERS = [
  { open: true, earned: 65, note: "Без льда", lines: [{ name: "3 часа", qty: 1, price: 650 }, { name: "Кола 0,5", qty: 2, price: 240 }] },
  { status: "accepted", lines: [{ name: "Ночь", qty: 1, price: 1000, link: false }], method: "Со счёта" },
  { status: "hold", unpaid: true, lines: [{ name: "Пицца Маргарита", qty: 1, price: 590 }, { name: "Кола 0,5", qty: 3, price: 360 }], method: "Наличные" },
  { status: "completed", lines: [{ name: "Шоколад", qty: 1, pts: 300 }], total: 0, points: 300, method: "Баллы" },
  { status: "canceled", lines: [{ name: "Энергетик 0,45", qty: 2, price: 360 }] },
  { status: "voided", lines: [{ name: "5 часов", qty: 1, price: 900, link: false }], method: "Банковская карта" },
];

const TIME_PRODUCTS = [
  { name: "3 часа", type: "package", left: "2 ч 15 м", usable: "2 ч 15 м" },
  { name: "Ночь", type: "fixed", left: "8 ч", usable: "8 ч", expiry: "Сгорает в 06:00 · Сгорает при выходе", open: false },
  { name: "VIP · 3 часа", type: "package", left: "3 ч", usable: "∞", hosts: ["VIP"], expiry: "Не сгорает" },
  { name: "Стандарт", type: "rate", order: null, credit: true, bought: null, expiry: null, hosts: ["Зал", "VIP", "Стрим", "Приставки"], open: false },
];

const CHALLENGES = [
  { name: "Ночной марафон", when: "до 30 сентября · 21 день", met: 2, total: 3, left: "Заказ в баре", leftProgress: "0,00 ₽ из 300,00 ₽", rewards: [{ icon: "ph-coins", text: "+500 очков" }, { icon: "ph-clock", text: "+1 ч" }] },
  { name: "Первый месяц", when: "выполнен 26 августа", met: 2, total: 2, done: true, waiting: true, rewards: [{ icon: "ph-gift", text: "Подарок", claim: true }] },
  { name: "Воин недели", when: "до 20 сентября · 5 дней", met: 1, total: 2, left: "Час в игре", leftProgress: "0 из 1 ч", pool: 3, rewards: [{ icon: "ph-coins", text: "+300 очков" }] },
];

const ACHIEVEMENTS = [
  { name: "Ночной игрок", earned: true, times: 9 },
  { name: "Стратег", earned: true },
  { name: "Ветеран", earned: true },
  { name: "Еженедельный гость", p: 0.6, meta: "3 из 5" },
  { name: "Верный клиент", p: 0.85, meta: "85,00 ₽ из 100,00 ₽" },
  { name: "Ежедневный гринд", p: 0.66, meta: "4 из 6 ч" },
  { name: "Коллекционер", p: 0.52, meta: "260 из 500 очков" },
  { name: "Щедрый заказ", p: 0.4, meta: "20,00 ₽ из 50,00 ₽" },
  { name: "Секретное", secret: true },
];

const CART = { cartItems: 2, cartNames: ["Кола 0,5", "Капучино"], cartPrices: [120, 190] };
const GOODS = ["Кола 0,5", "Капучино", "Круассан", "Энергетик 0,45", "Чипсы", "Латте"];

const SCENES = [
  // ── sign-in ──
  { id: "login-idle", page: "login", data: { pc: 7 }, note: "Экран входа в покое: атмосфера, номер ПК." },
  { id: "login-card", page: "login", data: { pc: 7, card: true }, note: "Экран входа с карточкой: имя пользователя и пароль." },
  { id: "login-card-phone", page: "login", data: { pc: 7, card: true, method: "phone" }, note: "Вход по номеру телефона." },
  { id: "login-card-reserved", page: "login", data: { pc: 7, card: true, reserved: "18:30" }, note: "ПК забронирован: плашка над карточкой." },
  { id: "login-card-error", page: "login", data: { pc: 7, card: true, password: "•••", error: "Неверное имя пользователя или пароль" }, note: "Ошибка входа." },

  // ── home ──
  { id: "home-news", page: "home", data: { ...LOYAL, levelHint: "earning" }, note: "Главная: новость в герое, пакеты, сводка прогресса, пилюля уровня после входа." },
  { id: "home-news-plain", page: "home", data: { ...LOYAL }, note: "Главная без пилюли." },
  { id: "home-app-hero", page: "home", data: { ...LOYAL, hero: "app", heroIndex: 2 }, note: "Главная: популярная игра в герое, одна кнопка запуска." },
  { id: "home-app-hero-running", page: "home", data: { ...LOYAL, hero: "app", heroIndex: 5, exes: 3, running: true }, note: "Игра с тремя исполняемыми, одно запущено." },
  { id: "home-product-hero", page: "home", data: { ...LOYAL, hero: "product" }, note: "Главная: товар из бара в герое." },
  { id: "home-classic", page: "home", data: { ...HOME }, note: "Главная у клуба без программы лояльности: бар внизу правой колонки." },
  { id: "home-deploying", page: "home", data: { ...LOYAL, deploy: "running" }, note: "Идёт развёртывание игры: баннер в верхней панели." },
  { id: "home-deploy-done", page: "home", data: { ...LOYAL, deploy: "done", reservation: "soon" }, note: "Игра готова; ПК забронирован через 42 минуты." },
  { id: "home-apps-open", page: "home", data: { ...LOYAL, appsOpen: [["Nightfall", "running"], ["Meridian", "deploying"], ["Orbital", "idle"]] }, note: "Панель «Мои приложения»: запущено, разворачивается, готово." },
  { id: "home-apps-empty", page: "home", data: { ...LOYAL, appsOpen: "empty" }, note: "Панель «Мои приложения» пустая." },
  { id: "home-notifications", page: "home", data: { ...LOYAL, unread: 3, notificationsOpen: NOTIFICATIONS }, note: "Список уведомлений." },
  { id: "home-notifications-empty", page: "home", data: { ...LOYAL, notificationsOpen: "empty" }, note: "Уведомлений нет." },
  { id: "home-assistance", page: "home", data: { ...LOYAL, assistanceOpen: "form" }, note: "Позвать администратора: форма." },
  { id: "home-assistance-sent", page: "home", data: { ...LOYAL, assistanceOpen: "sent" }, note: "Запрос администратору отправлен." },
  { id: "home-user-menu", page: "home", data: { ...LOYAL, userOpen: true }, note: "Карточка профиля из верхней панели." },
  { id: "home-tooltip-time", page: "tooltip", data: { ...LOYAL, tooltip: TOOLTIP_TWO }, note: "Подсказка «Ваше время»: два пакета, кредит." },
  { id: "home-tooltip-ending", page: "tooltip", data: { ...LOYAL, time: "0ч 12м", tooltip: TOOLTIP_ENDING }, note: "Время заканчивается: предупреждение и два выхода." },
  { id: "home-unread", page: "home", data: { ...LOYAL, unread: 12 }, note: "Счётчик пропущенных уведомлений." },
  { id: "home-many", page: "home", data: { ...LOYAL, packs: 24, pinned: 8, slides: 5 }, note: "Много всего: 24 пакета, 8 закреплённых игр, 5 слайдов." },
  { id: "home-secured", page: "home", data: { ...HOME, loyalty: "secured", levelName: "Легенда", levelMark: "Л", levelHint: "secured" }, note: "Высший уровень закреплён." },
  { id: "home-achievements-only", page: "home", data: { ...HOME, loyalty: "achievements" }, note: "Клуб без лестницы: сводка по достижениям." },

  // ── dialogs over the board ──
  { id: "buy-balance", page: "purchase", data: { name: "3 часа", ways: ["balance", "cash"], price: 650, balance: 2480, board: LOYAL }, note: "Покупка пакета со счёта." },
  { id: "buy-points", page: "purchase", data: { name: "3 часа", ways: ["balance", "points"], selected: "points", price: 650, pointsPrice: 1200, balance: 2480, points: 5120, board: LOYAL }, note: "Покупка пакета баллами." },
  { id: "buy-counter", page: "purchase", data: { name: "Ночь", ways: ["balance", "points", "cash", "card"], selected: "cash", price: 1000, balance: 2480, points: 5120, board: LOYAL }, note: "Оплата на кассе." },
  { id: "buy-short", page: "purchase", data: { name: "VIP · 3 часа", ways: ["balance", "cash"], price: 850, balance: 500, board: LOYAL }, note: "Не хватает денег: кнопка пополняет на разницу." },
  { id: "buy-discount", page: "purchase", data: { name: "5 часов", ways: ["balance"], price: 900, discount: 180, fees: 20, balance: 2480, board: LOYAL }, note: "Скидка и сбор в чеке." },
  { id: "buy-topup", page: "purchase", data: { name: "VIP · 3 часа", ways: ["balance"], step: "topup", price: 850, balance: 500, board: LOYAL }, note: "Пополнение внутри покупки." },
  { id: "buy-paying", page: "purchase", data: { name: "3 часа", ways: ["balance"], step: "paying", price: 650, balance: 2480, board: LOYAL }, note: "Заказ отправляется." },
  { id: "buy-done", page: "purchase", data: { name: "3 часа", step: "done", price: 650, balance: 2480, board: LOYAL }, note: "Пакет куплен." },
  { id: "cart-checkout", page: "checkout", data: { count: 3, goods: GOODS, ways: ["balance", "cash"], balance: 2480, board: LOYAL }, note: "Оформление заказа из корзины." },
  { id: "cart-points-lines", page: "checkout", data: { items: [{ name: "Кола 0,5", qty: 2, price: 120 }, { name: "Шоколад", qty: 1, points: 300 }, { name: "Капучино", qty: 1, price: 190 }], ways: ["balance", "cash", "card"], balance: 2480, points: 5120, board: LOYAL }, note: "Строка за баллы и строки за деньги в одном заказе." },
  { id: "cart-done", page: "checkout", data: { step: "done", board: LOYAL }, note: "Заказ оплачен." },
  { id: "topup-amount", page: "topup", data: { step: "amount", amount: 500, board: LOYAL }, note: "Пополнение счёта: выбор суммы." },
  { id: "topup-qr", page: "topup", data: { step: "qr", amount: 500, board: LOYAL }, note: "Пополнение: QR-код для телефона." },
  { id: "topup-done", page: "topup", data: { step: "done", amount: 500, board: LOYAL }, note: "Счёт пополнен." },
  { id: "lock-locked", page: "lock", data: { step: "locked", board: LOYAL }, note: "Профиль заблокирован: PIN-код." },
  { id: "lock-set", page: "lock", data: { step: "set", board: LOYAL }, note: "Установка PIN-кода блокировки." },

  // ── applications ──
  { id: "apps-popular", page: "apps", data: { apps: 24, columns: 8 }, note: "Каталог игр: популярные." },
  { id: "apps-hover", page: "apps", data: { apps: 24, columns: 8, hover: 2, hoverExes: ["Играть", "Лаунчер", "Настройки"] }, hover: ".virtual-chunk-grid .giz-app-card:nth-child(3)", note: "Карточка игры под курсором: ярлыки запуска, категория, кнопка деталей." },
  { id: "apps-filtered", page: "apps", data: { apps: 8, columns: 8, sort: "all", category: "Шутеры", modes: ["Кооператив"] }, note: "Каталог с фильтрами: категория и режим." },
  { id: "apps-search", page: "apps", data: { apps: 3, columns: 8, search: "night" }, note: "Поиск по каталогу." },
  { id: "apps-search-empty", page: "apps", data: { empty: true, search: "quake" }, note: "Ничего не найдено." },
  { id: "app-details", page: "appdetails", data: { index: 0, exes: [["Играть", "idle"], ["Лаунчер", "idle"]] }, note: "Страница игры: обложка, описание, запуск, медиа." },
  { id: "app-details-running", page: "appdetails", data: { index: 6, exes: [["Играть", "running"], ["Редактор карт", "idle"], ["Сервер", "idle"]], tab: "links" }, note: "Страница игры: одно запущено, вкладка ссылок." },
  { id: "app-details-deploying", page: "appdetails", data: { index: 9, exes: [["Играть", "deploying"]], media: 5 }, note: "Страница игры во время развёртывания." },

  // ── shop ──
  { id: "shop-all", page: "shop", data: { ...CART }, note: "Магазин: все группы, корзина с двумя строками." },
  { id: "shop-time", page: "shop", data: { tab: 0, ...CART }, note: "Магазин: пакеты времени." },
  { id: "shop-bar", page: "shop", data: { tab: 1, cartItems: 3, cartNames: ["Кола 0,5", "Капучино", "Круассан"], cartPrices: [120, 190, 150] }, note: "Магазин: бар, корзина с тремя строками." },
  { id: "shop-hover", page: "shop", data: { tab: 0, ...CART }, hover: ".virtual-chunk-grid .giz-product-card:nth-child(4)", note: "Пакет под курсором: условия покупки и использования." },
  { id: "shop-empty-cart", page: "shop", data: { tab: 1, cartItems: 0 }, note: "Магазин с пустой корзиной." },
  { id: "product-time", page: "product", data: { kind: "time", name: "3 часа", art: false, buy: ["Пн–Пт 10:00–22:00", "Сб–Вс 12:00–02:00"], use: ["Пн–Пт 10:00–23:00", "Сб–Вс 12:00–06:00"], expiry: ["Сгорает через 3 д. после покупки", "Сгорает при выходе"], relatedNames: ["Час", "5 часов", "Ночь", "VIP · 3 часа"], ...CART }, note: "Страница пакета времени." },
  { id: "product-good", page: "product", data: { kind: "good", name: "Капучино", price: 190, buy: null, description: "Двойной эспрессо на молоке 3,2 %, 250 мл. Готовим на стойке за пару минут — принесём к вашему месту.", relatedNames: ["Латте", "Круассан", "Кола 0,5", "Шоколад"], ...CART }, note: "Страница товара из бара." },
  { id: "product-bundle", page: "product", data: { kind: "bundle", name: "Комбо: пицца и кола", price: 650, buy: null, bundled: 3, relatedNames: ["Ночной набор", "Завтрак", "Кола 0,5", "Пицца Маргарита"], ...CART }, note: "Страница комбо: состав." },
  { id: "product-unavailable", page: "product", data: { kind: "time", name: "Выходной", number: "10", unit: "часов", art: false, unavailable: "Пакет доступен для покупки только в субботу и воскресенье.", relatedNames: ["Час", "5 часов", "Ночь", "VIP · 3 часа"], ...CART }, note: "Пакет сейчас нельзя купить: кнопка объясняет почему." },

  // ── account ──
  { id: "account-profile", page: "account", data: { tab: "profile", credit: true, email: "artem.voronov@example.com", phone: "+7 912 345-67-89", loyalty: true }, note: "Профиль: контакты, безопасность." },
  { id: "account-time", page: "account", data: { tab: "time", credit: true, products: TIME_PRODUCTS, loyalty: true }, note: "Пакеты времени: очередь." },
  { id: "account-purchases", page: "account", data: { tab: "purchases", orders: ORDERS, loyalty: true }, note: "История покупок, первый заказ раскрыт." },
  { id: "account-guest", page: "account", data: { tab: "time", guest: true, noHistory: true, products: [{ name: "Гостевой час", type: "fixed", left: "45 м", usable: "45 м", open: false }] }, note: "Гостевой аккаунт." },
  { id: "progress", page: "progress", data: { ladder: "earning", challengeList: CHALLENGES, achievementList: ACHIEVEMENTS }, note: "Прогресс: уровень, челленджи, достижения." },
  { id: "progress-secured", page: "progress", data: { ladder: "secured", score: 5400, challengeList: CHALLENGES, achievementList: ACHIEVEMENTS }, note: "Прогресс: высший уровень закреплён." },
  { id: "progress-no-ladder", page: "progress", data: { ladder: false, challengeList: CHALLENGES, achievementList: ACHIEVEMENTS }, note: "Прогресс в клубе без лестницы." },

  // ── notifications window ──
  { id: "notifications-stack", page: "notifications", data: { board: LOYAL, items: [
    { type: "success", title: "Оплата прошла", message: "Пакет «3 часа» добавлен на ваш счёт." },
    { type: "info", title: "Сообщение от администратора", message: "Турнир начнётся в 22:00, сбор команд у стойки." },
  ] }, note: "Всплывающие уведомления над главной: оплата, сообщение, подтверждение брони." },
  { id: "notifications-warning", page: "notifications", data: { board: { ...LOYAL, time: "0ч 09м" }, reservation: false, items: [
    { type: "warning", title: "Осталось 10 минут", message: "Пополните счёт или возьмите пакет, чтобы не прерываться." },
  ] }, note: "Предупреждение о конце времени над главной." },
];

module.exports = { PALETTES, PROFILE, SCENES, LEVELS };
