// The product page, mirroring Pages/Shop/ProductDetails.razor with the cart column
// (Components/Shop/GizOrder.razor) beside it. Same class names, same nesting.
"use strict";

const { esc, art, artFor, img, money, number } = require("../lib/html");
const { frame } = require("./frame");

function button(text, cls) {
  return `<button class="giz-button giz-button--fill accent ${cls || "giz-button--extra-large"}"><div class="giz-button__content_wrapper"><div class="giz-button__content"><div>${text}</div></div></div></button>`;
}

function price(p) {
  if (!p.price && !p.points) return `<span>Бесплатно</span>`;
  let out = "";
  if (p.price) out += `<span>${money(p.price)}</span>`;
  if (p.price && p.points) out += `<span> ${p.and ? "и" : "или"} </span>`;
  if (p.points) out += `<span class="giz-product-card__price__points">${number(p.points)}</span><i class="ph-fill ph-coins giz-icon"></i>`;
  return out;
}

function hostChips(list, active) {
  return `<div class="giz-time-product-host-groups">${list.map((h, i) => `<div class="giz-time-product-host-group dynamic${i === active ? " active" : ""}"><div>${esc(h)}</div></div>`).join("")}</div>`;
}

function fact(icon, cap, lines) {
  return `<div class="gg-pd__fact"><i class="ph ${icon}"></i><div><small>${cap}</small>${lines}</div></div>`;
}

function card(name, i, p) {
  return `<div class="giz-product-card product">
    <div class="giz-product-card__content">
      <div class="giz-product-card__content__top">
        <div class="giz-product-card__content__image"><img src="${artFor("product", i, 10)}" class="giz-image--cover" alt="image" /></div>
        <div class="giz-product-card__content__details">
          <div class="giz-product-card__price">${price(p)}</div>
          <div class="giz-product-card__title">${esc(name)}</div>
          <div class="giz-product-card__host-group"></div>
        </div>
      </div>
      <div class="giz-product-card__content__footer">${button("В корзину", "giz-button--medium giz-button--full-width giz-product-card-primary-button")}</div>
    </div>
  </div>`;
}

const TRASH = `<div class="giz-icon giz-icon--small"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M4 7h16M10 11v6M14 11v6M6 7l1 13h10l1-13M9 7V4h6v3" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg></div>`;
const AWARD = `<svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 2l2.4 4.9 5.4.8-3.9 3.8.9 5.4L12 14.4 7.2 16.9l.9-5.4L4.2 7.7l5.4-.8L12 2z"/></svg>`;

// Components/Shop/GizOrder.razor with GizOrderItem.razor lines: thumb, name, the price
// column in its loader wrapper, remove and quantity under the name. `cartNames`,
// `cartPrices` and `cartQty` describe the lines (the harness's cart is a column of
// colas); `cartPoints` lists which lines are an either/or choice with points.
function cart(d) {
  const items = d.cartItems || 0;
  const rows = [];
  let total = 0, award = 0;
  for (let i = 0; i < items; i++) {
    const name = d.cartNames ? d.cartNames[i % d.cartNames.length] : "Кола 0,5";
    const qty = d.cartQty ? d.cartQty[i % d.cartQty.length] : 1;
    const price = (d.cartPrices ? d.cartPrices[i % d.cartPrices.length] : 120) * qty;
    const points = d.cartPoints ? d.cartPoints[i] : 0;
    total += price;
    award += Math.round(price / 20);
    const details = points
      ? `<div class="giz-order-item__details__option"><div class="giz-radio-button"><input type="radio" checked /><label></label></div><span class="giz-order-item__details__price">${money(price)}</span></div><div class="giz-order-item__details__option"><div class="giz-radio-button"><input type="radio" /><label></label></div><span class="giz-order-item__details__points">${AWARD}${number(points)}</span></div>`
      : `<div class="giz-order-item__details__price">${money(price)}</div>`;
    rows.push(`<div class="giz-order-item">
      <div class="giz-order-item__thumb"><img src="${artFor("product", i, 20)}" class="giz-image--cover" alt="image" /></div>
      <div class="giz-order-item__description">${esc(name)}</div>
      <div class="giz-wm-icart-loader" style="min-height: 6.8rem"><div class="giz-wm-icart-loader-content"><div class="giz-order-item__details">${details}</div></div><div class="giz-image-loading--activity"></div></div>
      <div class="giz-order-item__remove"><button class="giz-icon-button giz-icon-button--small primary giz-icon-button--text"><div class="giz-icon-button__content">${TRASH}</div></button></div>
      <div class="giz-order-item__quantity"><div class="giz-shop-quantity-picker giz-shop-quantity-picker--small"><button class="giz-button giz-button--extra-small primary giz-button--text giz-decrease-btn${qty === 1 ? " disabled" : ""}"${qty === 1 ? " disabled" : ""}><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content">-</div></div></button><span>${qty}</span><button class="giz-button giz-button--extra-small primary giz-button--text giz-increase-btn"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content">+</div></div></button></div></div>
    </div>`);
  }
  const textInput = (cls, inner) => `<div class="giz-text-input giz-text-input--full-width ${cls} giz-input-control"><div class="giz-input-root giz-input-root--medium giz-input-root--outline giz-input-root--transparent giz-input-root--full-width"><div class="giz-input-wrapper">${inner}</div></div></div>`;
  const clear = items ? `<button class="giz-button giz-button--small primary giz-button--text"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper">${TRASH}<div class="giz-button__content">Очистить</div></div></button>` : "";
  return `<div class="giz-order">
    <div class="giz-order__items">
      <div class="giz-order__items__header"><i class="ph-fill ph-shopping-cart-simple"></i><span>Заказ</span><span class="giz-order__items__header__count">${items}</span></div>
      <div class="giz-order__items__body giz-scrollbar--v">
        ${items ? `<div class="giz-order__items-wrapper">${rows.join("")}</div>` : `<div class="giz-no-order-wrapper"><div class="giz-empty-state"><div class="giz-empty-state__icon"><i class="ph-bold ph-shopping-bag-open"></i></div><div class="giz-empty-state__title">Корзина пуста</div><div class="giz-empty-state__text">Добавьте товары из магазина</div></div></div>`}
      </div>
    </div>
    <div class="giz-order__notes">
      <div class="giz-order__notes__header"><span>Комментарий к заказу</span>${clear}</div>
      <div class="giz-order__notes__body">${textInput("giz-order-notes", `<textarea placeholder="Например, без льда"></textarea>`)}</div>
    </div>
    <div class="giz-order__totals">
      <div class="giz-order-promocode">${textInput("", `<input type="text" placeholder="Промокод" />`)}<button class="giz-button giz-button--extra-large accent giz-button--outline disabled" disabled><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content">Применить</div></div></button></div>
      <div class="giz-order-summary">
        <div class="giz-order-summary-text-bold">Итого</div>
        <div class="giz-order-summary-total"><div class="giz-wm-icart-loader" style="min-height: 3rem"><div class="giz-wm-icart-loader-content"><div class="giz-order-summary-total-content"><span>${money(total)}</span></div></div><div class="giz-image-loading--activity"></div></div></div>
        <div class="giz-order-summary-text">Баллы за заказ</div>
        <div class="giz-order-summary-points-award"><div class="giz-wm-icart-loader" style="min-height: 2.2rem"><div class="giz-wm-icart-loader-content"><div class="giz-order-summary-points-award-content"><span>${award}</span>${AWARD}</div></div><div class="giz-image-loading--activity"></div></div></div>
      </div>
      <div>${button("Заказать", "giz-button--large giz-button--full-width" + (items ? "" : " disabled"))}</div>
    </div>
  </div>`;
}

// d: { kind: time|good|bundle, name, group, price, points, and, art, artIndex,
//      description, hosts, expiry: [...], buy: [...], use: [...], more, unavailable,
//      related, relatedNames, cartItems, cartNames, cartPrices, cart (false hides the
//      column) }
function render(d) {
  const kind = d.kind || "time";
  const name = d.name || (kind === "time" ? "Стандарт 3 часа" : kind === "bundle" ? "Комбо: пицца и кола" : "Кола 0,5");
  const p = { price: d.price == null ? 650 : d.price, points: d.points == null ? (kind === "time" ? 1200 : 0) : d.points, and: d.and };
  const withCart = d.cart !== false;

  const artHtml = d.art === false
    ? (kind === "time"
      ? `<div class="gg-pd__time"><b>${esc(d.number || "3")}</b><span>${esc(d.unit || "часа")}</span></div>`
      : `<div class="gg-pd__no-art"><i class="ph ph-image"></i></div>`)
    : `<img src="${d.art ? art(d.art) : artFor("product", d.artIndex || 0)}" class="giz-image--cover" alt="image" />`;

  const facts = [];
  if (kind === "time") {
    facts.push(fact("ph-desktop", "Где действует", hostChips(d.hosts || ["Зал", "VIP"], 0)));
    const expiry = d.expiry || ["Сгорает через 3 д. после покупки"];
    if (expiry.length) facts.push(fact("ph-hourglass-low", "Срок действия", expiry.map((l) => `<b>${esc(l)}</b>`).join("")));
  }
  if (d.buy !== null) {
    const buy = d.buy || ["Пн–Пт 10:00–22:00"];
    facts.push(fact("ph-shopping-cart-simple", "Доступен для покупки", (d.more ? buy : buy.slice(0, 1)).map((l) => `<b>${esc(l)}</b>`).join("")));
  }
  if (kind === "time" && d.use) {
    facts.push(fact("ph-play-circle", "Доступен для использования", (d.more ? d.use : d.use.slice(0, 1)).map((l) => `<b>${esc(l)}</b>`).join("")));
  }
  const hasMore = (d.buy && d.buy.length > 1) || (d.use && d.use.length > 1);
  if (hasMore) facts.push(`<button type="button" class="gg-pd__more"><span>${d.more ? "Свернуть" : "Подробнее"}</span><i class="ph-bold ${d.more ? "ph-caret-up" : "ph-caret-down"}"></i></button>`);

  const description = d.description === null ? "" : (d.description || "Три часа игры на любом ПК основного зала. Время расходуется только пока вы в сети; пакет можно докупить в любой момент.");
  const related = d.related == null ? 4 : d.related;
  const relatedNames = d.relatedNames || ["Стандарт 1 час", "Стандарт 5 часов", "Ночь до утра", "VIP 3 часа", "Кола 0,5", "Ред Булл"];
  const bundled = kind === "bundle" ? (d.bundled || 3) : 0;

  const body = `<div class="gg-pd-wrap">
  <div class="gg-pd${withCart ? " gg-pd--with-cart" : ""}">
    <div class="gg-pd__page giz-scrollbar--v">
      <button type="button" class="gg-pd__back"><i class="ph-bold ph-arrow-left"></i><span>Назад</span></button>
      <section class="gg-pd__main">
        <div class="gg-pd__art${kind === "time" ? " gg-pd__art--time" : ""}">${artHtml}</div>
        <div class="gg-pd__info">
          <div class="gg-cap">${esc(d.group || (kind === "time" ? "Пакеты времени" : "Бар"))}</div>
          <h1 class="gg-pd__title">${esc(name)}</h1>
          <div class="gg-pd__price">${price(p)}</div>
          ${withCart ? `<div class="gg-pd__buy">${d.unavailable ? `<button class="giz-button giz-button--fill accent giz-button--extra-large" disabled><div class="giz-button__content_wrapper"><div class="giz-button__content"><div>${kind === "time" ? "Купить" : "Добавить в корзину"}</div></div></div></button><div class="gg-pd__why"><i class="ph-fill ph-info"></i><span>${esc(d.unavailable)}</span></div>` : button(kind === "time" ? "Купить" : "Добавить в корзину")}</div>` : ""}
          <div class="gg-pd__facts">${facts.join("")}</div>
        </div>
      </section>
      ${description ? `<section class="gg-pd__about"><div class="gg-cap gg-cap--rule"><span>Описание</span><span class="gg-cap__rule"></span></div><p class="gg-pd__desc">${esc(description)}</p></section>` : ""}
      ${bundled ? `<section class="gg-pd__bundle"><div class="gg-cap gg-cap--rule"><span>В комплекте</span><span class="gg-cap__rule"></span></div><div class="gg-pd__bundle-row">${Array.from({ length: bundled }, (_, i) => `<div class="giz-bundled-product"><img src="${artFor("product", i, 30)}" alt="image" /></div>`).join("")}</div></section>` : ""}
      ${withCart && related ? `<section class="gg-pd__related"><div class="gg-cap gg-cap--rule"><span>Похожие товары</span><span class="gg-cap__rule"></span></div><div class="gg-pd__related-row">${Array.from({ length: related }, (_, i) => card(relatedNames[i % relatedNames.length], i, { price: 250 + i * 150 })).join("")}</div></section>` : ""}
    </div>
    ${withCart ? `<aside class="gg-pd__cart">${cart(d)}</aside>` : ""}
  </div>
</div>`;
  return frame({ balance: 1250, points: 3400, time: "2ч 15м", pinned: 3, ...d, active: "shop" }, body);
}

module.exports = { render, cart, card, price, button };
