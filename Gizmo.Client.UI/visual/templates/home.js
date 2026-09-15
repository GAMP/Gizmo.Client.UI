// The home board, mirroring Pages/Home.razor and the components it places. Same class
// names, same nesting; the data comes from the scenario instead of the server.
//
// When Home.razor changes shape, this file changes with it - that is the maintenance
// this harness asks for, and the reason its markup is kept as literal as the Razor.
"use strict";

const { esc, img, art, artFor, money, number } = require("../lib/html");
const { frame } = require("./frame");

const NAMES = {
  normal: {
    apps: ["Counter-Strike 2", "Dota 2", "Valorant", "Apex Legends", "Fortnite", "League of Legends", "Rust", "PUBG", "Minecraft", "GTA V", "Warframe", "Overwatch 2"],
    packs: ["Стандарт 1 час", "Стандарт 3 часа", "Стандарт 5 часов", "Ночь до утра", "VIP 3 часа", "VIP ночь", "Выходной день", "Утро 2 часа", "Buddy 2×3 часа", "Марафон 10 часов", "Ученик 2 часа", "Турнир"],
    goods: ["Кола 0,5", "Ред Булл", "Чипсы Лейс", "Сникерс", "Кофе латте", "Вода без газа", "Круассан", "Чай", "Пицца Маргарита", "Энергетик Burn"],
    news: ["Ночной турнир по CS2 — в пятницу", "Новые VIP-места уже открыты", "Скидка 20% на дневные пакеты"],
  },
  long: {
    apps: ["Tom Clancy's Rainbow Six Siege Deluxe Edition Year 9 Season 3 Operation Twin Shells", "Warhammer 40,000: Space Marine 2 — Ultra Edition with all season passes included", "The Elder Scrolls Online: Gold Road Collector's Edition Upgrade Bundle", "S.T.A.L.K.E.R. 2: Heart of Chornobyl Ultimate Collector's Pack"],
    packs: ["Пакет выходного дня для двоих с двумя бутылками воды и одним энергетиком в подарок", "Ночной безлимит с пятницы на субботу для VIP-зала (только для зарегистрированных гостей)", "Стандартный дневной тариф 3 часа", "Промо-пакет «Приведи друга» на 10 часов с двойными баллами"],
    goods: ["Пицца Четыре сыра большая 40 см с дополнительной моцареллой", "Кола Зеро 0,5 л стеклянная бутылка охлаждённая", "Сэндвич с курицей и соусом барбекю на цельнозерновом хлебе", "Энергетический напиток Monster Ultra Zero Sugar 0,5 л"],
    news: ["Большой осенний турнир по Counter-Strike 2 с призовым фондом и бесплатными напитками для всех участников — регистрация на стойке"],
  },
  // The promo shots: titles that belong to no real game, goods without a brand, so the
  // pictures can be abstract and nothing on screen is somebody's trademark.
  promo: {
    apps: ["Nightfall", "Meridian", "Orbital", "Ashfall", "Halcyon", "Kestrel", "Sable", "Redline", "Voidbound", "Solstice", "Ironwake", "Tidewater", "Glasshouse", "Aurora Station", "Lowland", "Northlight", "Paradox", "Ember", "Quiet Sky", "Vantage", "Drift", "Cinder", "Harbor", "Zenith"],
    cats: ["Шутеры", "Стратегии", "Гонки", "RPG", "Кооператив", "Инди", "Спорт", "Приключения"],
    packs: ["Час", "3 часа", "5 часов", "Ночь", "VIP · 3 часа", "Выходной", "Утро · 2 часа", "Марафон · 10 часов"],
    goods: ["Кола 0,5", "Энергетик 0,45", "Капучино", "Круассан", "Вода 0,5", "Пицца Маргарита", "Чипсы", "Шоколад", "Латте", "Сэндвич"],
    news: ["Ночной турнир — пятница, 22:00", "Новые VIP-места уже открыты", "Скидка 20 % на дневные пакеты"],
  },
};

function pick(list, i) {
  return list[i % list.length] + (i >= list.length ? " " + (Math.floor(i / list.length) + 1) : "");
}

function gizImage(src, fit) {
  return `<img src="${src}" class="giz-image--${fit}" alt="image" />`;
}

function placeholderImage(name) {
  return `<div class="giz-default-image"><img src="${img(name)}" alt="" /></div>`;
}

// ── hero ────────────────────────────────────────────────────────────────
// The body of a news item is the operator's own HTML from the Manager. `newsBody` is
// such a body, written the way a club would write one; without it, the harness's plain
// heading and line.
function heroNews(d, names) {
  const title = pick(names.news, 0);
  const body = d.newsBody || `<h2>${esc(title)}</h2><p>Подробности у администратора.</p>`;
  const action = d.newsAction
    ? `<button class="giz-button giz-button--large primary giz-button--fill giz-ads-carousel-item-command"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-icon giz-icon--large giz-button__icon-left"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M14 4h6v6M20 4l-9 9M18 14v5a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1V7a1 1 0 0 1 1-1h5" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg></div><div class="giz-button__content">${esc(d.newsAction)}</div></div></button>`
    : "";
  return `<section class="gg-hero gg-hero--news" aria-label="Акции">
    <div class="giz-ads-carousel-item">
      <div class="giz-ads-carousel-item__image">
        <img src="${artFor("news", d.newsIndex || 0)}" />
      </div>
      <div class="giz-ads-carousel-item__content">
        <div class="giz-ads-carousel-item__content__body">${body}</div>
        <div class="giz-ads-carousel-item__content__actions"><div>${action}</div><div></div></div>
      </div>
    </div>
  </section>`;
}

function heroApp(d, names) {
  const title = pick(names.apps, 0);
  const exes = Math.max(1, Math.min(3, d.exes == null ? 1 : d.exes));
  const launch = [];
  for (let i = 0; i < exes; i++) {
    // One executable named after the game: the button says "Launch" (via the CSS
    // variable on the row) instead of repeating the title - see Home.razor.
    const plain = exes === 1;
    const caption = `${title} — ${["Играть", "Лаунчер", "Настройки"][i]}`;
    launch.push(`<div class="gg-hero__exe${plain ? " gg-hero__exe--play" : ""}">
      <div class="giz-universal-executable">
        <div class="giz-universal-executable__icon">
          ${gizImage(artFor("exe", i), "cover")}
          <div class="giz-universal-executable-progress-bar">${i === 0 && d.running ? '<div class="running"></div>' : ""}</div>
        </div>
        ${plain ? "" : `<div class="giz-universal-executable__description">
          <div class="giz-universal-executable__description__exe">${esc(caption)}</div>
        </div>`}
      </div>
    </div>`);
  }
  return `<section class="gg-hero gg-hero--app" aria-label="Популярные игры">
    <div class="gg-hero__bg">${gizImage(artFor("app", d.heroIndex || 0), "cover")}</div>
    <div class="gg-hero__scrim"></div>
    <div class="gg-hero__art gg-hero__art--tall">${d.noArt ? placeholderImage("no-app-image.svg") : gizImage(artFor("app", d.heroIndex || 0), "cover")}</div>
    <div class="gg-hero__body">
      <div class="gg-hero__kick">Популярное</div>
      <div class="gg-hero__title">${esc(title)}</div>
      <div class="gg-hero__launch" style="--gg-play-label: 'Запустить'">
        ${launch.join("\n")}
        <button type="button" class="gg-hero__more"><span>Детали</span><i class="ph-bold ph-arrow-right"></i></button>
      </div>
    </div>
  </section>`;
}

function heroProduct(d, names) {
  const title = pick(names.goods, 0);
  return `<section class="gg-hero gg-hero--goods" aria-label="Популярное">
    <div class="gg-hero__bg">${gizImage(artFor("product", 0), "cover")}</div>
    <div class="gg-hero__scrim"></div>
    <div class="gg-hero__art">${gizImage(artFor("product", 0), "cover")}</div>
    <div class="gg-hero__body">
      <div class="gg-hero__kick">Популярное</div>
      <div class="gg-hero__title">${esc(title)}</div>
      <div class="gg-hero__price">${money(150)}</div>
      <button type="button" class="gg-hero__cta"><i class="ph-bold ph-shopping-bag"></i><span>В корзину</span></button>
    </div>
  </section>`;
}

function heroEmpty() {
  return `<section class="gg-hero gg-hero--empty"><div class="gg-hero__empty-note">Здесь пока ничего нет</div></section>`;
}

function dots(count) {
  if (count <= 1) return "";
  const items = [];
  for (let i = 0; i < count; i++) {
    items.push(`<span class="gg-dots__dot${i === 0 ? " is-on" : ""}">${i === 0 ? '<span class="gg-dots__fill"></span>' : ""}</span>`);
  }
  return `<div class="gg-dots">${items.join("")}</div>`;
}

// ── games band ──────────────────────────────────────────────────────────
function appCard(title, i, category) {
  return `<div class="giz-app-card">
    <div class="giz-app-card__content">
      <div class="giz-app-card__content__image">
        ${gizImage(artFor("app", i), "fill")}
      </div>
      <div class="giz-app-card__content__details">
        <div class="giz-app-card__title">${esc(title)}</div>
      </div>
      <div class="giz-app-card__content__footer">
        <div class="giz-app-card-text"><div class="giz-app-card__content__footer-category">${esc(category || "Шутеры")}</div></div>
        <button type="button" class="giz-app-card__open"><i class="ph-bold ph-arrow-square-out"></i></button>
      </div>
    </div>
  </div>`;
}

// ── till ────────────────────────────────────────────────────────────────
// The second row is the one being bought ("Минуту…") unless `busyPack` says which,
// or -1 for none.
function pack(name, price, points, i, busy) {
  const priceHtml = price > 0 || !points
    ? money(price)
    : `<span class="gg-pack__pts">${number(points)}<i class="ph-fill ph-coins"></i></span>`;
  return `<article class="gg-pack">
    <span class="gg-pack__name">${esc(name)}</span>
    <span class="gg-pack__price">${priceHtml}</span>
    <button type="button" class="gg-buy">${i === busy ? "Минуту…" : "Купить"}</button>
  </article>`;
}

// Components/Loyalty/LoyaltySummary.razor: with the ladder or achievements on
// (`loyalty: earning | secured | achievements | challenges`), the board takes two rows
// (.gg-board--split): the till level with the hero, this summary level with the strip,
// and the bar leaves the home board. Without it the till is one tile with the bar at
// its foot.
function loyaltyTile(d) {
  const mode = d.loyalty;
  const ladder = mode === "earning" || mode === "secured";
  const secured = mode === "secured";
  // The ring: progress to the next level, or the share of achievements / challenges.
  const p = ladder ? (secured ? 1 : (d.levelProgress || 0.17)) : mode === "achievements" ? 3 / 9 : 1 / 3;
  const off = (2 * Math.PI * 18 * (1 - p)).toFixed(2).replace(/\.?0+$/, "");
  const ring = `<svg viewBox="0 0 40 40" aria-hidden="true"><circle class="gg-ring__track" cx="20" cy="20" r="18"></circle><circle class="gg-ring__bar" cx="20" cy="20" r="18" style="stroke-dashoffset: ${off}"></circle></svg>`;
  const link = ladder ? `<a href="#">Достижения 3 из 9</a>` : mode === "achievements" ? `<a href="#">Челленджи 1 из 3</a>` : "";
  // `levelName` / `levelMark` name the level (the harness's ladder is Praxilla / Alternatif).
  const levelName = d.levelName || (secured ? "Alternatif" : "Praxilla");
  const levelMark = d.levelMark || levelName[0];
  const disc = ladder
    ? `<div class="gg-loyal__disc">${esc(levelMark)}</div>`
    : `<div class="gg-loyal__disc gg-loyal__disc--plain"><i class="ph-fill ${mode === "achievements" ? "ph-trophy" : "ph-flag-checkered"}"></i></div>`;
  const who = ladder
    ? `<b>${esc(levelName)}</b><span>${secured ? "Закреплён до 1 октября" : `Ещё ${number(d.toRetain || 1489)} очков, чтобы удержать`}</span>`
    : mode === "achievements" ? `<b>Достижения</b><span>3 из 9</span>` : `<b>Челленджи</b><span>1 из 3</span>`;
  return `<aside class="gg-till gg-till--loyal" aria-label="Ваш прогресс">
    <div class="gg-cap gg-cap--rule"><span>Ваш прогресс</span><span class="gg-cap__rule"></span>${link}</div>
    <div class="gg-loyal">
      <div class="gg-loyal__top">
        <div class="gg-loyal__mark">${ring}${disc}</div>
        <div class="gg-loyal__who">${who}</div>
      </div>
      ${ladder ? `<div class="gg-loyal__track"><i style="width: ${secured ? 100 : Math.round(p * 100)}%"></i></div>` : ""}
      <div class="gg-loyal__next"><i class="ph-fill ph-flag-checkered"></i><span>${d.nextStep || "Заказ в баре"} — <b>«${esc(d.nextChallenge || "Ночной марафон")}»</b></span><em>+${number(d.nextReward || 500)} очков</em></div>
      <button type="button" class="gg-loyal__more"><i class="ph-bold ph-trophy"></i>Мой прогресс<i class="ph-bold ph-arrow-right"></i></button>
    </div>
  </aside>`;
}

function barRow(name, i) {
  return `<div class="gg-bar__row" title="${esc(name)}">
    <span class="gg-bar__thumb">${gizImage(artFor("product", i), "cover")}</span>
    <span class="gg-bar__name">${esc(name)}</span>
    <span class="gg-bar__price">${money(90 + i * 35)}</span>
    <button type="button" class="gg-bar__add" title="В корзину"><i class="ph-bold ph-plus"></i></button>
  </div>`;
}

// d: { hero: news|app|product|empty, slides, apps, packs, bar, names, balance, points,
//      shop, exes, running, noArt, pointsOnlyPack, loyalty: earning|secured, newsBody,
//      newsAction, newsIndex, heroIndex }
function render(d) {
  const names = NAMES[d.names || "normal"];
  const shop = d.shop !== false;

  let hero;
  switch (d.hero) {
    case "news": hero = heroNews(d, names); break;
    case "app": hero = heroApp(d, names); break;
    case "product": hero = heroProduct(d, names); break;
    default: hero = heroEmpty();
  }

  const appCount = d.apps == null ? 4 : d.apps;
  const stripApps = [];
  for (let i = 0; i < Math.min(4, appCount); i++) stripApps.push(appCard(pick(names.apps, i), i, names.cats && names.cats[i % names.cats.length]));

  const packCount = shop ? (d.packs == null ? 6 : d.packs) : 0;
  const packs = [];
  for (let i = 0; i < packCount; i++) {
    const points = d.pointsOnlyPack && i === 0 ? 1200 : 0;
    packs.push(pack(pick(names.packs, i), points ? 0 : 250 + i * 150, points, i, d.busyPack == null ? 1 : d.busyPack));
  }

  const barCount = shop ? (d.bar == null ? 4 : d.bar) : 0;
  const bar = [];
  for (let i = 0; i < barCount; i++) bar.push(barRow(pick(names.goods, i), i));

  const body = `<div class="gg-board${d.loyalty ? " gg-board--split" : ""}">
  <div class="gg-stage">
    ${hero}
    ${dots(d.slides == null ? (d.hero === "empty" ? 0 : 3) : d.slides)}
    <section class="gg-strip" aria-label="Игры и приложения">
      <div class="gg-cap gg-cap--rule"><span>Популярные приложения</span><span class="gg-cap__rule"></span><a href="#">Все приложения</a></div>
      ${stripApps.length ? `<div class="gg-strip__row">${stripApps.join("\n")}</div>` : `<div class="gg-empty">Пока пусто</div>`}
    </section>
  </div>
  <aside class="gg-till" aria-label="Покупки">
    <div class="gg-wallet">
      <div class="gg-cap">На счете</div>
      <div class="gg-wallet__row"><span class="gg-wallet__sum">${money(d.balance == null ? 1250 : d.balance)}</span></div>
    </div>
    <div class="gg-till__sec gg-till__sec--packs">
      <div class="gg-cap gg-cap--rule"><span>Пакеты времени</span><span class="gg-cap__rule"></span><a href="#">Все</a></div>
      ${packs.length ? `<div class="gg-packs">${packs.join("\n")}</div>` : `<div class="gg-empty gg-empty--small">Сейчас нет пакетов, доступных к покупке</div>`}
    </div>
    ${d.loyalty ? "" : `<div class="gg-till__sec gg-till__sec--bar">
      <div class="gg-cap gg-cap--rule"><span>Бар</span><span class="gg-cap__rule"></span><a href="#">Весь магазин</a></div>
      ${bar.length ? `<div class="gg-bar">${bar.join("\n")}</div>` : `<div class="gg-empty gg-empty--small">Пока пусто</div>`}
    </div>`}
  </aside>${d.loyalty ? loyaltyTile(d) : ""}
</div>`;

  // The mark on the avatar follows the ladder (none without one).
  const ladder = d.loyalty === "earning" || d.loyalty === "secured";
  const level = ladder ? { progress: d.loyalty === "secured" ? 1 : (d.levelProgress || 0.17), mark: d.levelMark || (d.levelName ? d.levelName[0] : (d.loyalty === "secured" ? "A" : "P")) } : null;
  return frame({ level, ...d, active: "home" }, body);
}

module.exports = { render, NAMES, appCard };
