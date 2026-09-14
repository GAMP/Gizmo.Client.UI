// The shell frame around a page: top bar, left rail, body. Mirrors Shared/_Layout.razor
// and the small components it hosts, with the same class names, so the page under test
// gets exactly the room it gets in the client.
//
// Only the parts that take space are reproduced. Dropdowns and menus are closed in
// every scenario and contribute nothing to the layout; the exceptions are the tariff
// tooltip, which a scenario can open with `tooltip` (see templates/tooltip.js), and the
// "My applications" panel, opened with `appsOpen: "rows" | "empty"`.
"use strict";

const { esc, img, art, money, number } = require("../lib/html");

function topBar(d) {
  const reservation = d.reservation
    ? `<div class="giz-resv${d.reservation === "soon" ? " giz-resv--soon" : ""}">
        <i class="ph-fill ph-calendar-check giz-resv__icon"></i>
        <b class="giz-resv__lead">Устройство забронировано</b>
        <span class="giz-resv__at">с 18:30</span>
        <span class="giz-resv__count">через 42 мин</span>
      </div>`
    : "";

  const deploy = d.deploy
    ? `<div class="giz-deploy open${d.deploy === "done" ? " is-done" : ""}${d.deploy === "waiting" ? " is-waiting" : ""}">
        <span class="giz-deploy__fill" style="width: ${d.deploy === "done" ? 100 : 63}%"></span>
        <div class="giz-deploy__badge">
          <svg class="giz-deploy__ring" viewBox="0 0 40 40" aria-hidden="true">
            <circle class="giz-deploy__ring-track" cx="20" cy="20" r="17.5"></circle>
            <circle class="giz-deploy__ring-bar" cx="20" cy="20" r="17.5" style="stroke-dashoffset: ${d.deploy === "running" ? (109.96 * 0.37).toFixed(2) : 0}"></circle>
          </svg>
          <i class="${d.deploy === "done" ? "ph-bold ph-check" : "ph-fill ph-cloud-arrow-down"}"></i>
        </div>
        <button type="button" class="giz-deploy__body">
          <span class="giz-deploy__title">${d.deploy === "done" ? "1 приложение готово" : "Развёртывание"}</span>
          <span class="giz-deploy__sub">${d.deploy === "done" ? "Counter-Strike 2" : "Копируем файлы игры"}</span>
        </button>
        ${d.deploy === "done" ? "" : `<div class="giz-deploy__pct">${d.deploy === "waiting" ? "…" : "63%"}</div>`}
        <button type="button" class="giz-deploy__close"><i class="ph-bold ph-x"></i></button>
      </div>`
    : "";

  const time = d.time
    ? `<i class="ph-bold ph-clock giz-ph-icon"></i><span class="giz-top-bar__time-value">${esc(d.time)}</span>`
    : `<i class="ph-bold ph-clock giz-ph-icon"></i><i class="ph-bold ph-infinity giz-ph-icon" style="color:#FFC700"></i>`;

  // Shared/MenuActiveApplicationsContainer.razor + MenuActiveApplicationCard.razor, with
  // the vendor ComboButton/Button markup the launch button renders to.
  const launchButton = (state) => {
    const progress = state === "deploying";
    const left = `<button class="left-button giz-button giz-button--large accent ${progress ? "giz-button--progress" : "giz-button--fill"} giz-button--full-width" type="button">
        <div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: ${progress ? 42 : 0}%"></div></div>
        <div class="giz-button__content_wrapper"><div class="giz-button__content"><div>${progress ? "Отмена" : "Запустить"}</div></div></div>
      </button>`;
    const right = `<button class="right-button giz-button giz-button--large accent giz-button--fill giz-button--icon" type="button">
        <div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div>
        <div class="giz-button__content_wrapper"><span class="giz-icon giz-icon--large"><svg viewBox="0 0 24 24" width="24" height="24"><path d="M7 10l5 5 5-5z" fill="currentColor"/></svg></span></div>
      </button>`;
    return `<div class="giz-combo-button giz-combo-button--full-width">${left}${right}</div>`;
  };
  const appRow = (name, artName, state) => `<div class="giz-active-app-card">
      <div class="giz-active-app-card__image"><img src="${art(artName)}" class="giz-image--cover" alt="" /></div>
      <div class="giz-active-app-card__info"><div class="giz-active-app-card__info__title">${esc(name)}</div></div>
      <div class="giz-active-app-card__actions">${launchButton(state)}</div>
    </div>`;
  const appsPanel = d.appsOpen
    ? `<div class="giz-dropdown-menu open">
        <div class="giz-dropdown-menu__content giz-active-apps">
          <div class="giz-active-apps__header">
            <div class="giz-heading">Мои приложения</div>
            <div class="giz-active-apps__header__hint">Управление запущенными приложениями</div>
          </div>
          <div class="giz-active-apps__body giz-scrollbar--v">
            ${d.appsOpen === "empty"
              ? `<div class="giz-active-apps__empty">
                  <i class="ph-fill ph-game-controller giz-ph-icon"></i>
                  <div class="giz-active-apps__empty__title">Пока пусто</div>
                  <div class="giz-active-apps__empty__text">Запустите игру из каталога — она появится здесь</div>
                </div>`
              : appRow("VALORANT", "exe-1.jpg", "running") + appRow("Genshin Impact", "exe-2.jpg", "deploying") + appRow("Counter-Strike 2", "exe-3.jpg", "idle")}
          </div>
        </div>
      </div>`
    : "";

  return `
    <div class="giz-top-bar">
      <div class="giz-top-bar__left">
        <div class="giz-top-bar__search">
          <div class="giz-header__global-search">
            <div class="giz-global-search" tabindex="0">
              <i class="ph-bold ph-magnifying-glass giz-ph-icon"></i>
              <input type="text" value="" placeholder="Поиск" />
            </div>
          </div>
        </div>
        ${reservation}
      </div>
      <div class="giz-top-bar__hud">
        <div class="giz-topbar-stat giz-topbar-stat--pc">PC ${d.pc || 12}</div>
        <span class="giz-top-bar__divider"></span>
        <div class="giz-topbar-tariff">
          <div class="giz-header-user-balance">
            <i class="ph-fill ph-lightning giz-ph-icon"></i>
            <div>${esc(d.tariff || "Стандарт")}</div>
            ${d.tooltip ? require("./tooltip").tooltipHtml(d.tooltip) : ""}
          </div>
        </div>
        <span class="giz-top-bar__divider"></span>
        <div class="giz-top-bar__time">${time}</div>
        <span class="giz-top-bar__divider"></span>
        <div class="giz-topbar-stat giz-topbar-stat--points">
          <i class="ph-fill ph-coins giz-ph-icon" style="color:#e8b84b"></i>
          <span>${number(d.points || 0)}</span>
        </div>
        <span class="giz-top-bar__divider"></span>
        <div class="giz-topbar-stat giz-topbar-stat--balance"><span>${money(d.balance || 0)}</span></div>
      </div>
      <div class="giz-top-bar__right-group">
        <div class="giz-top-bar__icons">
          <div class="giz-actions-bar">
            <div class="giz-header__user-menu-item giz-user-online-deposit-dropdown">
              <button class="giz-topup-pill"><i class="ph-fill ph-wallet giz-ph-icon"></i><span>Пополнить</span></button>
            </div>
            <div class="giz-header__user-menu-item giz-active-apps-dropdown${d.appsOpen ? " open" : ""}">
              <button class="user-menu-item-button--box"><i class="ph-bold ph-squares-four giz-ph-icon"></i></button>
              ${appsPanel}
            </div>
            <div class="giz-header__user-menu-item giz-notifications-dropdown">
              <button class="user-menu-item-button--box"><i class="ph-bold ph-bell giz-ph-icon"></i></button>
            </div>
            <div class="giz-header__user-menu-item giz-assistance-dropdown">
              <button class="user-menu-item-button--box"><i class="ph-bold ph-question giz-ph-icon"></i></button>
            </div>
          </div>
        </div>
        ${deploy}
        <div class="giz-top-bar__account">
          <div class="giz-header__user-menu-item giz-user-dropdown">
            <button class="giz-user-menu-button${d.levelRing != null ? " gg-has-level" : ""}">
              <span class="giz-user-avatar giz-user-avatar--glyph"><i class="ph ph-user"></i></span>
              ${levelRing(d)}
            </button>
          </div>
        </div>
      </div>
    </div>`;
}

// CONCEPT (templates/progress.js): the ladder level on the avatar - a progress ring to
// the next level around the button and the level's mark at its corner. Styles inline
// here so nothing ships until the feature is built.
function levelRing(d) {
  if (d.levelRing == null) return "";
  const C = 119.4; // r = 19
  const off = (C * (1 - Math.max(0, Math.min(1, d.levelRing)))).toFixed(1);
  return `<style>
    .giz-user-menu-button.gg-has-level { position: relative; overflow: visible !important; }
    .gg-avatar-ring { position: absolute; inset: -0.35rem; width: calc(100% + 0.7rem); height: calc(100% + 0.7rem); transform: rotate(-90deg); pointer-events: none; }
    .gg-avatar-ring circle { fill: none; stroke-width: 2.2; }
    .gg-avatar-ring .track { stroke: rgba(255,255,255,0.1); }
    .gg-avatar-ring .bar { stroke: var(--gg-accent-light); stroke-linecap: round; stroke-dasharray: ${C}; }
    .gg-avatar-mark { position: absolute; right: -0.3rem; bottom: -0.3rem; width: 2rem; height: 2rem; border-radius: 50%; display: flex; align-items: center; justify-content: center;
      background: var(--gg-accent); color: var(--gg-on-accent); font-family: Manrope, sans-serif; font-weight: 800; font-size: 1rem; border: 2px solid var(--gg-bg-1); pointer-events: none; }
  </style>
  <svg class="gg-avatar-ring" viewBox="0 0 40 40" aria-hidden="true"><circle class="track" cx="20" cy="20" r="19"></circle><circle class="bar" cx="20" cy="20" r="19" style="stroke-dashoffset: ${off}"></circle></svg>
  <span class="gg-avatar-mark">${d.levelRing >= 1 ? '<i class="ph-fill ph-crown"></i>' : "P"}</span>`;
}

function navItem(icon, active) {
  return `<div class="giz-header__modules-menu-item">
    <a href="#" class="${active ? "active" : ""}">
      <i class="ph-bold ${icon} giz-ph-icon giz-nav-icon--bold"></i>
      <i class="ph-fill ${icon} giz-ph-icon giz-nav-icon--fill"></i>
    </a>
  </div>`;
}

function rail(d) {
  const pinned = Math.max(0, Math.min(8, d.pinned == null ? 3 : d.pinned));
  const dock = [];
  for (let i = 0; i < pinned; i++) {
    dock.push(`<div class="giz-dock-item">
      <div class="giz-universal-executable">
        <div class="giz-universal-executable__icon">
          <img src="${art("exe-" + ((i % 3) + 1) + ".jpg")}" class="giz-image--cover" alt="image" />
          <div class="giz-universal-executable-progress-bar"></div>
        </div>
      </div>
    </div>`);
  }
  return `
    <aside class="giz-left-panel">
      <div class="giz-sidebar__inner">
        <nav class="giz-sidebar__nav">
          <div class="giz-header__modules-menu">
            ${navItem("ph-house", d.active === "home")}
            ${navItem("ph-game-controller", d.active === "apps")}
            ${navItem("ph-shopping-cart-simple", d.active === "shop")}
            ${navItem("ph-user", d.active === "profile")}
          </div>
        </nav>
      </div>
      <div class="giz-left-panel__divider"></div>
      <div class="giz-left-panel__quicklaunch">
        <div class="quick-launcher">
          <div class="giz-dock"><div class="giz-dock__body">${dock.join("")}</div></div>
        </div>
      </div>
    </aside>`;
}

// d: { balance, points, time, pc, tariff, reservation, deploy, pinned, active }
function frame(d, bodyHtml, extraHtml) {
  return `
  <div class="giz-root">
    ${topBar(d)}
    <div class="giz-container">
      ${rail(d)}
      <div class="giz-app__body">
${bodyHtml}
      </div>
    </div>
  </div>
${extraHtml || ""}`;
}

module.exports = { frame };
