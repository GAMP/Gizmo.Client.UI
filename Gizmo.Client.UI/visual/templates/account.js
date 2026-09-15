// The account page, mirroring Components/Profile/AccountFrame.razor and its tabs
// (Pages/Profile/Profile, Products, Purchases; Progress lives in templates/progress.js).
// Same class names, same nesting.
"use strict";

const { esc, money, number, plural } = require("../lib/html");
const { frame, avatar } = require("./frame");

const ITEMS = ["{0} товар", "{0} товара", "{0} товаров"];

function stat(cap, value, cls, note) {
  return `<div class="gg-account__stat${cls ? " " + cls : ""}"><small>${cap}</small><b>${value}</b>${note ? `<span>${note}</span>` : ""}</div>`;
}

function head(d) {
  const guest = !!d.guest;
  const name = guest ? "Гость" : (d.username || "xennon");
  const full = d.fullName == null ? "Иван Петров" : d.fullName;
  const stats = [
    stat("На счете", money(d.balance == null ? 1250 : d.balance), "gg-account__stat--money"),
    stat("Баллы", `${number(d.points == null ? 3400 : d.points)} <i class="ph-fill ph-coins"></i>`),
    stat("Время", d.time == null ? "2:15" : d.time),
  ];
  if (d.credit) stats.push(stat("Кредит", d.credit === "unlimited" ? "∞" : money(500), "gg-account__stat--credit",
    d.credit === "unlimited" ? "Неограниченный кредит" : "Вы можете продолжать играть в кредит даже после того, как закончится ваше время."));
  return `<header class="gg-account__head">
    ${avatar(d, "gg-account__avatar")}
    <div class="gg-account__who">
      <h1 class="gg-account__name">${esc(name)}</h1>
      <div class="gg-account__sub">${full ? `<span>${esc(full)}</span>` : ""}${guest ? "" : `<span>В клубе с 12 марта 2024</span>`}</div>
    </div>
    <div class="gg-account__stats">${stats.join("")}</div>
  </header>`;
}

// The Progress tab exists only where the club runs the ladder or achievements
// (Loyalty.State.IsAvailable): `loyalty` in the scenario.
function tabs(d) {
  const t = (id, icon, label) => `<a class="gg-account__tab${d.tab === id ? " is-on" : ""}" href="#"><i class="ph-bold ${icon}"></i><span>${label}</span></a>`;
  return `<nav class="gg-account__tabs">
    ${d.guest ? "" : t("profile", "ph-user", "Профиль")}
    ${t("time", "ph-hourglass-medium", "Время")}
    ${d.noHistory ? "" : t("purchases", "ph-receipt", "Покупки")}
    ${d.loyalty ? t("progress", "ph-trophy", "Прогресс") : ""}
  </nav>`;
}

function field(cap, value) {
  return `<div class="gg-field"><small>${cap}</small>${value ? `<b>${esc(value)}</b>` : `<span class="gg-field__empty">Не указан</span>`}</div>`;
}

function profileTab(d) {
  return `<div class="gg-account__grid">
    <section class="gg-card">
      <div class="gg-card__head"><i class="ph-fill ph-address-book"></i><h2>Контактная информация</h2></div>
      <div class="gg-card__fields">
        ${field("E-mail адрес", d.email === null ? "" : (d.email || "ivan.petrov@example.com"))}
        ${field("Телефон", d.phone === null ? "" : (d.phone || "+7 912 345-67-89"))}
      </div>
      <p class="gg-card__note">Изменить контакты может администратор клуба</p>
    </section>
    <section class="gg-card">
      <div class="gg-card__head"><i class="ph-fill ph-shield-check"></i><h2>Безопасность</h2></div>
      <div class="gg-card__fields">
        <div class="gg-field"><small>Пароль</small><b class="gg-field__secret">••••••••</b></div>
      </div>
      <p class="gg-card__note">Пароль для входа на любом ПК клуба</p>
      <div class="gg-card__actions"><button type="button" class="gg-btn"><i class="ph-bold ph-key"></i><span>Изменить пароль</span></button></div>
    </section>
  </div>
  <div class="gg-account__version">Grafit 1.1.2</div>`;
}

const TP_ICON = { package: "ph-package", fixed: "ph-hourglass-medium", rate: "ph-timer" };
const TP_KIND = { package: "Пакет времени", fixed: "Пакет с фиксированным временем", rate: "Тариф" };

function hosts(list, active) {
  const chips = list.map((h, i) => `<div class="giz-user-time-products-host-group dynamic${i === active ? " active" : ""}"><div>${esc(h)}</div></div>`).join("");
  return `<div class="giz-user-time-products-host-groups">${chips}${list.length > 1 ? `<div class="giz-user-time-products-host-group--additional" style="display:none">+99</div>` : ""}</div>`;
}

function tpRow(p, i) {
  const order = p.order === null ? null : (p.order == null ? i + 1 : p.order);
  const current = order === 1;
  const cls = `${current ? " is-current" : ""}${order === null ? " is-idle" : ""}${p.credit ? " is-credit" : ""}`;
  const tag = current ? `<span class="gg-tag gg-tag--on">Идёт сейчас</span>`
    : order ? `<span class="gg-tag">${order}-й в очереди</span>` : `<span class="gg-tag gg-tag--off">Не на этом ПК</span>`;
  const facts = [];
  if (p.bought !== null) facts.push(`<span class="gg-tp__fact"><i class="ph ph-calendar-blank"></i><span>Куплен: ${esc(p.bought || "12.03.2026 14:00")}</span></span>`);
  if (p.expiry !== null) facts.push(`<span class="gg-tp__fact"><i class="ph ph-hourglass-low"></i><span>${esc(p.expiry || "Сгорает через 3 д. с момента покупки пакета · Активирован 12.03.2026 14:00")}</span></span>`);
  facts.push(`<span class="gg-tp__fact gg-tp__fact--hosts"><i class="ph ph-desktop"></i>${hosts(p.hosts || ["Зал", "VIP"], 0)}</span>`);
  return `<div class="gg-tp__row${cls}">
    <div class="gg-tp__order">${order == null ? "–" : order}</div>
    <div class="gg-tp__type"><i class="ph-fill ${TP_ICON[p.type || "package"]}"></i></div>
    <div class="gg-tp__main">
      <div class="gg-tp__line"><b class="gg-tp__name">${esc(p.name)}</b><span class="gg-tp__kind">${TP_KIND[p.type || "package"]}</span>${tag}${p.credit ? `<span class="gg-tag gg-tag--credit">В кредит</span>` : ""}</div>
      <div class="gg-tp__meta">${facts.join("")}</div>
    </div>
    <div class="gg-tp__stat"><small>Осталось</small><b>${esc(p.left || "∞")}</b></div>
    <div class="gg-tp__stat"><small>На этом ПК</small><b>${esc(p.usable || "∞")}</b></div>
    ${p.open === false ? `<span class="gg-tp__open gg-tp__open--none"></span>` : `<button type="button" class="gg-tp__open"><i class="ph-bold ph-arrow-up-right"></i></button>`}
  </div>`;
}

function timeTab(d) {
  const products = d.products || [];
  let body;
  if (d.loading) body = `<div class="gg-card__wait"><div class="giz-animate-spinner"></div></div>`;
  else if (!products.length) body = `<div class="gg-card__empty"><i class="ph ph-hourglass"></i><b>Пакетов времени нет</b><span>Купленные пакеты появятся здесь</span></div>`;
  else body = `<div class="gg-tp">${products.map(tpRow).join("")}</div>`;
  return `<section class="gg-card gg-card--list">
    <div class="gg-card__head"><i class="ph-fill ph-hourglass-medium"></i><h2>Ваши пакеты времени</h2><span class="gg-card__hint">Пакеты расходуются по очереди, сверху вниз</span></div>
    ${body}
  </section>`;
}

const STATUS = {
  completed: ["Завершено", "is-good"], accepted: ["Принято", "is-on"], hold: ["В ожидании", "is-wait"],
  canceled: ["Отменено", "is-off"], voided: ["Аннулировано", "is-off"],
};

function order(o, i) {
  const [label, cls] = STATUS[o.status || "completed"];
  const lines = o.lines || [{ name: "Стандарт 3 часа", qty: 1, price: 650 }, { name: "Кола 0,5", qty: 2, price: 240 }];
  const names = lines.map((l) => l.name).join(", ");
  const total = o.total == null ? lines.reduce((s, l) => s + (l.price || 0), 0) : o.total;
  const pts = o.points || 0;
  return `<details class="gg-order ${cls}"${o.open ? " open" : ""}>
    <summary class="gg-order__sum">
      <div class="gg-order__when"><b>${esc(o.date || `${12 - i} сен`)}</b><small>${esc(o.time || "18:4" + (i % 10))}</small></div>
      <div class="gg-order__what"><b>${esc(names)}</b><small>Заказ №${o.id || 1040 - i} · ${plural(lines.length, ITEMS)}${o.method === null ? "" : `<span> · ${esc(o.method || "Наличные")}</span>`}</small></div>
      <div class="gg-order__sum-total">${total > 0 || pts === 0 ? `<b>${money(total)}</b>` : ""}${pts ? `<b class="gg-order__pts">${number(pts)} <i class="ph-fill ph-coins"></i></b>` : ""}</div>
      <div class="gg-order__status"><span class="gg-tag ${cls}">${label}</span>${o.unpaid ? `<span class="gg-tag gg-tag--off">Не оплачено</span>` : ""}</div>
      <i class="ph-bold ph-caret-down gg-order__caret"></i>
    </summary>
    <div class="gg-order__body">
      <div class="gg-order__lines">${lines.map((l) => `<div class="gg-order__line">${l.link === false ? `<span class="gg-order__line-name">${esc(l.name)}</span>` : `<a class="gg-order__line-name" href="#">${esc(l.name)}</a>`}<span class="gg-order__line-qty">${l.qty || 1} шт.</span><span class="gg-order__line-sum">${l.pts ? `<span class="gg-order__pts">${number(l.pts)} <i class="ph-fill ph-coins"></i></span>` : money(l.price || 0)}</span></div>`).join("")}</div>
      ${o.earned || o.note ? `<div class="gg-order__extra">${o.earned ? `<span class="gg-order__earned"><i class="ph-fill ph-sparkle"></i>Полученные баллы: ${number(o.earned)}</span>` : ""}${o.note ? `<span class="gg-order__note">Комментарии к заказу: ${esc(o.note)}</span>` : ""}</div>` : ""}
    </div>
  </details>`;
}

function purchasesTab(d) {
  const orders = d.orders || [];
  const pager = d.pager === false ? "" : `<div class="gg-pager">
      <button type="button" class="gg-pager__btn" disabled><i class="ph-bold ph-arrow-left"></i><span>Новее</span></button>
      <button type="button" class="gg-pager__btn"><span>Старее</span><i class="ph-bold ph-arrow-right"></i></button>
    </div>`;
  const body = orders.length ? `<div class="gg-orders">${orders.map(order).join("")}</div>`
    : `<div class="gg-card__empty"><i class="ph ph-receipt"></i><b>Покупок пока нет</b><span>Здесь будет всё, что вы заказывали</span></div>`;
  return `<section class="gg-card gg-card--list">
    <div class="gg-card__head"><i class="ph-fill ph-receipt"></i><h2>История покупок</h2>${pager}</div>
    ${body}
  </section>`;
}

// d: { tab: profile|time|purchases|progress, guest, username, fullName, email, phone,
//      balance, points, time, credit, products: [...], orders: [...], loading, noHistory,
//      mismatch, loyalty }
function render(d) {
  const tab = d.tab || "profile";
  // progress.js needs head() and tabs() from here: resolved at call time, not at load.
  const bodyByTab = { profile: profileTab, time: timeTab, purchases: purchasesTab, progress: (x) => require("./progress").progressTab(x) };
  const body = `<div class="gg-account giz-scrollbar--v">
  <div class="gg-account__inner">
    ${head(d)}
    ${tabs({ ...d, tab })}
    <div class="gg-account__body">
${bodyByTab[tab](d)}
    </div>
  </div>
</div>`;
  return frame({ balance: 1250, points: 3400, time: "2ч 15м", pinned: 3, ...d, active: "profile" }, body);
}

module.exports = { render, head, tabs };
