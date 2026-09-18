// The shop, mirroring Pages/Shop/ProductsIndex.razor: the group tabs (ClientTab), one
// section per product group with the cards of Components/Shop/ProductTimeCard,
// ProductSimpleCard and ProductBundleCard, and the cart column (GizOrder, shared with
// the product page). Same class names, same nesting.
"use strict";

const { esc, artFor, money, number } = require("../lib/html");
const { frame } = require("./frame");
const { cart, price, button } = require("./product");

const HEX = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M12 2l8.66 5v10L12 22l-8.66-5V7L12 2z" stroke-width="2" stroke-linejoin="round"/></svg>`;
const CLOCK = `<div class="giz-icon giz-icon--small"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><circle cx="12" cy="12" r="9" stroke-width="2"/><path d="M12 7v5l3 2" stroke-width="2" stroke-linecap="round"/></svg></div>`;
const INFINITE = `<div class="giz-icon giz-icon--small"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M8 8c-2.2 0-4 1.8-4 4s1.8 4 4 4c4 0 4-8 8-8 2.2 0 4 1.8 4 4s-1.8 4-4 4c-4 0-4-8-8-8z" stroke-width="2"/></svg></div>`;
const AWARD = `<svg viewBox="0 0 24 24"><path d="M12 2l2.4 4.9 5.4.8-3.9 3.8.9 5.4L12 14.4 7.2 16.9l.9-5.4L4.2 7.7l5.4-.8L12 2z"/></svg>`;

// Components/Shop/ProductQuantityPicker.razor: the button, or the picker once the
// product is in the cart.
function footer(p) {
  if (p.inCart) {
    return `<div class="giz-shop-quantity-picker giz-shop-quantity-picker--medium">
      <button class="giz-button giz-button--small primary giz-button--text giz-decrease-btn"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content">-</div></div></button>
      <span>${p.inCart}</span>
      <button class="giz-button giz-button--small primary giz-button--text giz-increase-btn"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content">+</div></div></button>
    </div>`;
  }
  if (p.disallowed) {
    return `<div class="giz-client-tooltip-root giz-product-host-groups-tooltip"><button class="giz-button giz-button--medium accent giz-button--fill giz-button--full-width disabled" disabled><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content">В корзину</div></div></button></div>`;
  }
  const label = p.kind === "time" ? "Купить" : "В корзину";
  return `<button class="giz-button giz-button--medium accent giz-button--fill giz-button--full-width giz-product-card-primary-button"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper">${p.kind === "time" ? "" : `<div class="giz-icon giz-icon--medium giz-button__icon-left"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M12 5v14M5 12h14" stroke-width="2" stroke-linecap="round"/></svg></div>`}<div class="giz-button__content">${label}</div></div></button>`;
}

// The hover panel: ProductTimeCardHover / ProductSimpleCardHover / ProductBundleCardHover.
function hoverPanel(p) {
  const row = (label, value) => `<div class="giz-timeline-item"><div class="giz-time-product-details"><div>${label}</div><div class="giz-time-product-time-availabile">${value}</div></div></div>`;
  if (p.kind === "time") {
    return `<div class="giz-timeline">
      <div class="giz-timeline-header">${HEX}<div>Пакет времени</div></div>
      <div class="giz-timeline-item"><div class="giz-time-product-time">${CLOCK}${esc(p.duration || "3 ч")}</div></div>
      ${row("Доступен для покупки", p.buy ? esc(p.buy) : INFINITE)}
      ${row("Доступен для использования", p.use ? esc(p.use) : INFINITE)}
      ${p.expires ? `<div class="giz-timeline-item"><div class="giz-time-product-details"><div>Сгорает</div><div class="giz-time-product-expirations"><div class="giz-time-product-expiration">${esc(p.expires)}</div></div></div></div>` : ""}
    </div>`;
  }
  if (p.kind === "bundle") {
    return `<div class="giz-timeline">
      <div class="giz-timeline-header">${HEX}<div>В комплекте</div></div>
      ${(p.contents || []).map((c) => `<div class="giz-timeline-item"><div class="giz-bundle-product-details"><div class="giz-icon giz-icon--small">${HEX}</div><div class="giz-bundle-product-details__product-name">${esc(c[0])}</div><span>×${c[1]}</span></div></div>`).join("")}
    </div>`;
  }
  return `<div class="giz-timeline">
    <div class="giz-timeline-header">${HEX}<div>Условия покупки</div></div>
    ${row("Доступен для покупки", p.buy ? esc(p.buy) : INFINITE)}
  </div>`;
}

// One card. p: { kind: time|good|bundle, name, price, points, and, number, unit, hosts,
//   duration, buy, use, expires, contents, inCart, disallowed, award, hover }
function card(p, i) {
  const kind = p.kind || "good";
  const image = kind === "time" && !p.picture
    ? `<div class="giz-product-time-image-wrapper"><div class="giz-product-time-image"><div class="giz-default-image"><img src="" alt="loading" style="display:none" /></div><div class="giz-product-time-image__time"><div class="giz-product-time-image__time__number">${esc(p.number || "3")}</div><div class="giz-product-time-image__time__text">${esc(p.unit || "часа")}</div></div></div></div>`
    : `<img src="${artFor("product", i, kind === "bundle" ? 40 : 0)}" class="giz-image--cover" alt="image" />`;
  const hosts = kind === "time"
    ? `<div class="giz-time-product-host-groups">${(p.hosts || ["Зал", "VIP"]).map((h, k) => `<div class="giz-time-product-host-group dynamic${k === 0 ? " active" : ""}"><div>${esc(h)}</div></div>`).join("")}</div>`
    : `<div class="giz-product-card__host-group"></div>`;
  return `<div class="giz-product-card ${kind === "good" ? "product" : kind}">
    <div class="giz-product-card__content">
      <div class="giz-product-card__content__top">
        <div class="giz-product-card__content__image${kind === "time" ? " giz-product-card__content__image--time" : ""}">${image}</div>
        <div class="giz-product-card__content__details">
          <div class="giz-product-card__price">${price({ price: p.price, points: p.points, and: p.and })}</div>
          <div class="giz-product-card__title">${esc(p.name)}</div>
          ${hosts}
        </div>
        <div class="giz-product-card__hover-overlay">
          <div class="giz-product-card__content--hovered">${hoverPanel({ ...p, kind })}</div>
          <div class="giz-product-card-award-indicator">${p.award ? AWARD : ""}</div>
        </div>
      </div>
      <div class="giz-product-card__content__footer">${footer({ ...p, kind })}</div>
    </div>
  </div>`;
}

const GROUPS = [
  { name: "Пакеты времени", items: [
    { kind: "time", name: "Час", number: "1", unit: "час", price: 250, duration: "1 ч", buy: "10:00–22:00", use: "10:00–23:00", expires: "При выходе" },
    { kind: "time", name: "3 часа", number: "3", unit: "часа", price: 650, duration: "3 ч", expires: "Через 3 дня" },
    { kind: "time", name: "5 часов", number: "5", unit: "часов", price: 900, duration: "5 ч", award: true },
    { kind: "time", name: "Ночь", number: "8", unit: "часов", price: 1000, duration: "8 ч", buy: "22:00–02:00", use: "22:00–08:00", expires: "В 08:00", hosts: ["Зал", "VIP", "Стрим"] },
    { kind: "time", name: "VIP · 3 часа", number: "3", unit: "часа", price: 850, duration: "3 ч", hosts: ["VIP"] },
    { kind: "time", name: "Выходной", number: "10", unit: "часов", price: 1500, duration: "10 ч", buy: "Сб–Вс", disallowed: true },
  ] },
  { name: "Бар", items: [
    { name: "Кола 0,5", price: 120, award: true },
    { name: "Энергетик 0,45", price: 180, inCart: 2 },
    { name: "Капучино", price: 190 },
    { name: "Круассан", price: 150 },
    { name: "Вода 0,5", price: 80 },
    { name: "Чипсы", price: 130 },
    { name: "Шоколад", price: 110, inCart: 1 },
    { name: "Латте", price: 210 },
    { name: "Сэндвич", price: 260 },
    { name: "Пицца Маргарита", price: 590, award: true },
    { name: "Чай", price: 90 },
    { name: "Мороженое", price: 140 },
  ] },
  { name: "Комбо", items: [
    { kind: "bundle", name: "Комбо: пицца и кола", price: 650, contents: [["Пицца Маргарита", 1], ["Кола 0,5", 1]], award: true },
    { kind: "bundle", name: "Ночной набор", price: 420, contents: [["Энергетик 0,45", 2], ["Чипсы", 1]] },
    { kind: "bundle", name: "Завтрак", price: 320, contents: [["Капучино", 1], ["Круассан", 1]] },
  ] },
];

// d: { groups: [{name, items}], tab: index|null (null = All), columns, cartItems,
//      cartNames, cartPrices, search, hover: [group, index] } plus the frame's keys
function render(d) {
  const groups = d.groups || GROUPS;
  const columns = Math.max(2, Math.min(10, d.columns || 6));
  const selected = d.tab == null ? null : d.tab;
  const tabs = [`<div class="giz-client-tab-item${selected == null ? " active" : ""}"><div><i class="ph-fill ph-squares-four"></i><span>Всё</span></div></div>`]
    .concat(groups.map((g, i) => `<div class="giz-client-tab-item${selected === i ? " active" : ""}"><div><i class="ph-fill ph-tag"></i><span>${esc(g.name)}</span></div></div>`));
  const arrow = (cls, d) => `<button class="giz-button giz-button--extra-small primary giz-button--fill giz-button--icon ${cls}"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-icon giz-icon--extra-small"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="${d}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg></div></div></button>`;

  const sections = groups.map((g, gi) => {
    if (selected != null && selected !== gi) return "";
    const rows = [];
    for (let r = 0; r < Math.ceil(g.items.length / columns); r++) {
      const cards = g.items.slice(r * columns, (r + 1) * columns).map((p, k) => card(p, gi * 20 + r * columns + k));
      rows.push(`<div class="virtual-chunk-grid" style="grid-template-columns: repeat(${columns}, 1fr)">${cards.join("\n")}</div>`);
    }
    return `<div class="giz-section">
      <div class="giz-section__header">${esc(g.name)}</div>
      <div class="giz-section__body">${rows.join("\n")}</div>
    </div>`;
  }).join("\n");

  const search = d.search
    ? `<div class="giz-section"><div class="giz-section__header__filters"><div><div class="giz-section__header__filters__title">Результаты для</div><div><div class="giz-chip"><div class="giz-chip__content_wrapper">${esc(d.search)}</div></div></div></div></div></div>`
    : "";

  const body = `<div class="giz-shop-wrapper">
  <div class="giz-shop">
    <div class="giz-shop__products">
      <div class="giz-shop__products__header">
        <div class="giz-shop__products__header__tab">
          <div class="giz-client-tab">
            ${arrow("giz-client-tab__previous", "M15 18l-6-6 6-6")}
            <div class="giz-client-tab__wrapper"><div class="giz-client-tab__content">${tabs.join("")}</div></div>
            ${arrow("giz-client-tab__next", "M9 6l6 6-6 6")}
          </div>
        </div>
      </div>
      <div class="giz-shop__products__body giz-scrollbar--v">
        ${search}
        ${sections}
      </div>
    </div>
    ${d.cart === false ? "" : `<div class="giz-shop__order">${cart(d)}</div>`}
  </div>
</div>`;
  return frame({ balance: 1250, points: 3400, time: "2ч 15м", pinned: 3, ...d, active: "shop" }, body);
}

module.exports = { render, card, GROUPS };
