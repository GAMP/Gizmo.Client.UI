// The applications catalogue, mirroring Pages/Apps/AppsIndex.razor with
// Components/Apps/AppFilters.razor in the section header and the grid of
// Components/Apps/ApplicationCard.razor from ApplicationVirtualizedCards. Same class
// names, same nesting; the vendor Select / MultiSelect / Chip render to the markup
// their class mappers produce.
"use strict";

const { esc, artFor } = require("../lib/html");
const { frame } = require("./frame");
const { NAMES } = require("./home");

const SORT = { popular: "Популярные приложения", all: "Все приложения", added: "Недавно добавленные", releases: "Новинки" };

const CHEVRON = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M6 9l6 6 6-6" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>`;
const SORT_ICON = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M4 6h16M7 12h10M10 18h4" stroke-width="2" stroke-linecap="round"/></svg>`;
const FILTER_ICON = `<svg class="giz-filters-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" width="16" height="16"><path d="M4 5h16l-6 8v5l-4 2v-7L4 5z" stroke-width="2" stroke-linejoin="round"/></svg>`;

// A vendor Select as GizInput renders it: label, the root with the value or the
// placeholder, the handle on the right. `filled` is the "a filter is set" class.
function select(label, value, { placeholder, icon, multi, filled } = {}) {
  const content = value
    ? `<div class="giz-input-wrapper"><input type="text" value="${esc(value)}" readonly /></div>`
    : `<div class="giz-select__content" tabindex="0" style="outline: none">${esc(placeholder || "")}</div>`;
  return `<div class="${multi ? "giz-multi-select" : "giz-select"} ${filled ? "app-filter--filled" : "app-filter--default"}">
    <div class="${multi ? "giz-multi-select" : "giz-select"}__content_wrapper">
      <div class="giz-input-control">
        <label class="giz-input-label">${esc(label)}</label>
        <div class="giz-input-root giz-input-root--small giz-input-root--outline">
          ${icon ? `<div class="giz-icon giz-icon--small giz-input__icon-left">${icon}</div>` : ""}
          ${content}
          <div class="giz-icon giz-icon--small giz-input__icon-right">${CHEVRON}</div>
        </div>
      </div>
    </div>
  </div>`;
}

function chip(inner) {
  return `<div class="giz-chip"><div class="giz-chip__content_wrapper">${inner}</div><button class="giz-icon-button giz-icon-button--small giz-input-button-clear"><div class="giz-icon giz-icon--small"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M6 6l12 12M18 6L6 18" stroke-width="2" stroke-linecap="round"/></svg></div></div></button></div>`;
}

// Components/Apps/AppFilters.razor: the caption (or the search chip) and the cluster.
function filters(d) {
  const caption = d.search
    ? `<div class="giz-section__header__filters"><div><div class="giz-section__header__filters__title">Результаты для</div><div>${chip(esc(d.search))}</div></div></div>`
    : `<div>${SORT[d.sort || "popular"]}</div>`;
  const total = (d.category ? 1 : 0) + (d.modes ? d.modes.length : 0);
  return `${caption}
    <div class="giz-apps-filters">
      ${total ? chip(`${FILTER_ICON}<span>${total}</span>`) : ""}
      ${select("Сортировать по:", SORT[d.sort || "popular"], { icon: SORT_ICON, filled: d.sort && d.sort !== "popular" })}
      ${select("Категории:", d.category, { placeholder: "Все категории", filled: !!d.category })}
      ${select("Фильтр:", d.modes ? d.modes.join(", ") : null, { placeholder: "Любые", multi: true, filled: !!d.modes })}
    </div>`;
}

// Components/Apps/ApplicationCard.razor. `hovered` renders the launch shortcuts the
// card shows under the cursor (the Razor mounts them on the first mouseover); the
// promo forces :hover on such a card.
function card(title, i, category, hovered) {
  const exes = hovered ? hovered.map((caption, k) => `<div class="giz-universal-executable">
            <div class="giz-universal-executable__icon"><img src="${artFor("exe", i * 3 + k)}" class="giz-image--cover" alt="image" /><div class="giz-universal-executable-progress-bar"></div></div>
            <div class="giz-universal-executable__description"><div class="giz-universal-executable__description__exe">${esc(caption)}</div></div>
          </div>`).join("") : "";
  return `<div class="giz-app-card">
    <div class="giz-app-card__content">
      <div class="giz-app-card__content__image">
        ${hovered ? `<div class="giz-app-card__content__image__hovered"><div class="giz-app-card__content__image__hovered__header"><div class="giz-app-card__content--hovered giz-scrollbar-slim--v"><div class="giz-exe-popup">${exes}</div></div></div></div>` : ""}
        <img src="${artFor("app", i)}" class="giz-image--fill" alt="image" />
      </div>
      <div class="giz-app-card__content__details">
        <div class="giz-app-card__title">${esc(title)}</div>
      </div>
      <div class="giz-app-card__content__footer">
        <div class="giz-app-card-text"><div class="giz-app-card__content__footer-category">${esc(category)}</div></div>
        <button type="button" class="giz-app-card__open" title="${esc(title)}"><i class="ph-bold ph-arrow-square-out"></i></button>
      </div>
    </div>
  </div>`;
}

// d: { apps, columns, names, sort: popular|all|added|releases, category, modes: [...],
//      search, empty, hover: index, hoverExes: [...] } plus the frame's keys
function render(d) {
  const names = NAMES[d.names || "normal"];
  const columns = Math.max(2, Math.min(10, d.columns || 8));
  const count = d.empty ? 0 : (d.apps == null ? 16 : d.apps);
  const cats = names.cats || ["Шутеры"];
  const rows = [];
  for (let r = 0; r < Math.ceil(count / columns); r++) {
    const cards = [];
    for (let i = r * columns; i < Math.min(count, (r + 1) * columns); i++) {
      const title = names.apps[i % names.apps.length] + (i >= names.apps.length ? " " + (Math.floor(i / names.apps.length) + 1) : "");
      cards.push(card(title, i, cats[i % cats.length], d.hover === i ? (d.hoverExes || ["Играть", "Лаунчер"]) : null));
    }
    rows.push(`<div class="virtual-chunk-grid" style="grid-template-columns: repeat(${columns}, 1fr)">${cards.join("\n")}</div>`);
  }
  const grid = count
    ? rows.join("\n")
    : `<div class="global-search-no-results"><div class="global-search-no-results__header">Ничего не найдено для <span class="global-search-pattern">${esc(d.search || "")}</span></div><div class="global-search-no-results__body">Попробуйте другое название или снимите фильтры</div></div>`;

  const body = `<div class="giz-home-apps-wrapper">
  <div class="giz-home-apps">
    <div class="giz-apps__body">
      <div class="giz-apps__body__content giz-ads-container giz-scrollbar--v">
        <div class="giz-apps__body__content__popular">
          <div class="giz-section">
            <div class="giz-section__header">
              ${filters(d)}
            </div>
            <div class="giz-section__body">
              ${grid}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>`;
  return frame({ balance: 1250, points: 3400, time: "2ч 15м", pinned: 3, ...d, active: "apps" }, body);
}

module.exports = { render, card, select, chip };
