// The shell frame around a page: top bar, left rail, body. Mirrors Shared/_Layout.razor
// and the small components it hosts, with the same class names, so the page under test
// gets exactly the room it gets in the client.
//
// Only the parts that take space are reproduced. Dropdowns and menus are closed in
// every scenario and contribute nothing to the layout; the exceptions are the tariff
// tooltip, which a scenario can open with `tooltip` (see templates/tooltip.js), the
// "My applications" panel (`appsOpen: "rows" | "empty" | [[name, state]]`), the profile
// card (`userOpen`), the notifications list (`notificationsOpen: [{type, title,
// message}]` or "empty") and the call-an-administrator form (`assistanceOpen: "form" |
// "sent"`).
"use strict";

const { esc, img, art, artFor, money, number } = require("../lib/html");

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
          <span class="giz-deploy__sub">${d.deploy === "done" ? esc(d.deployName || "Counter-Strike 2") : "Копируем файлы игры"}</span>
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
              : (d.appsOpen === "rows" ? [["VALORANT", "running"], ["Genshin Impact", "deploying"], ["Counter-Strike 2", "idle"]] : d.appsOpen)
                  .map((row, i) => appRow(row[0], `exe-${i + 1}.jpg`, row[1])).join("")}
          </div>
        </div>
      </div>`
    : "";

  // Shared/MenuNotificationsContainer.razor with Shared/GizNotification.razor cards.
  const ICONS = { info: "ph-info", success: "ph-check-circle", warning: "ph-warning", danger: "ph-x-circle" };
  const notificationsPanel = d.notificationsOpen
    ? `<div class="giz-dropdown-menu open">
        <div class="giz-dropdown-menu__content giz-menu-notifications">
          <div class="giz-menu-notifications__header">
            <div class="giz-heading">Уведомления</div>
            <div class="giz-menu-notifications__header__hint">Всё, что мы вам присылали</div>
          </div>
          <div class="giz-menu-notifications__body giz-scrollbar--v">
            ${d.notificationsOpen === "empty"
              ? `<div class="giz-menu-notification-default-item-wrapper"><div class="giz-empty-state"><i class="ph-fill ph-bell-slash giz-ph-icon"></i><div class="giz-empty-state__title">Тишина</div><div class="giz-empty-state__text">Новых уведомлений нет</div></div></div>`
              : d.notificationsOpen.map((n) => `<div class="giz-notification-wrapper"><div class="giz-notification${(n.type || "info") === "info" ? "" : " giz-notification--" + n.type}"><div class="giz-notification__icon"><i class="ph-fill ${ICONS[n.type || "info"]}"></i></div><div class="giz-notification__body"><div class="giz-notification__body__title">${esc(n.title)}</div>${n.message ? `<div class="giz-notification__body__message">${esc(n.message)}</div>` : ""}</div><button type="button" class="giz-notification__close"><i class="ph-bold ph-x"></i></button></div></div>`).join("")}
          </div>
          ${d.notificationsOpen === "empty" ? "" : `<div class="giz-menu-notifications__footer"><div class="giz-menu-notifications__footer__action">Очистить список</div></div>`}
        </div>
      </div>`
    : "";

  // Shared/MenuAssistanceContainer.razor: the request form, or the sent state.
  const assistancePanel = d.assistanceOpen
    ? `<div class="giz-dropdown-menu open">
        <div class="giz-dropdown-menu__content giz-menu-assistance">
          ${d.assistanceOpen === "sent"
            ? `<div class="giz-alert giz-alert--info"><div class="giz-alert__icon"><div class="giz-icon giz-icon--medium"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><circle cx="12" cy="12" r="9" stroke-width="2"/><path d="M12 8h.01M12 11v5" stroke-width="2" stroke-linecap="round"/></svg></div></div><div class="giz-alert__body"><div class="giz-alert__body__title">Запрос отправлен.</div><div class="giz-alert__body__text">Подождите, пока оператор рассмотрит ваш запрос. Ответ появится в разделе «Уведомления».</div><div class="giz-alert__body__actions"><button class="giz-button giz-button--small info giz-button--outline"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content"><div>Отменить запрос</div></div></div></button></div></div></div>`
            : `<div><div class="giz-select giz-select--full-width"><div class="giz-select__content_wrapper"><div class="giz-input-control"><label class="giz-input-label">Выберите причину обращения</label><div class="giz-input-root giz-input-root--extra-small giz-input-root--outline giz-input-root--full-width"><div class="giz-select__content" tabindex="0" style="outline: none">Не запускается игра</div><div class="giz-icon giz-icon--extra-small giz-input__icon-right"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M6 9l6 6 6-6" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg></div></div></div></div></div></div>
            <div><div class="giz-text-input giz-text-input--full-width giz-input-control"><label class="giz-input-label">Чем мы можем вам помочь?</label><div class="giz-input-root giz-input-root--medium giz-input-root--full-width"><div class="giz-input-wrapper"><textarea class="giz-scrollbar--v" placeholder="Опишите проблему…">Лаунчер закрывается сразу после запуска, PC 07</textarea></div></div></div><div class="giz-menu-assistance-note-counter">45/200</div></div>
            <div><button class="giz-button giz-button--large accent giz-button--fill giz-button--full-width"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content"><div>Отправить</div></div></div></button></div>`}
        </div>
      </div>`
    : "";

  // Shared/MenuUserLinks.razor: the profile card with the icons in orbit around the
  // avatar (the angles, radii and sizes are the component's own table).
  const ORBIT = [["ph-game-controller", 12, 64, 15, .80], ["ph-headset", 58, 92, 10, .45], ["ph-desktop-tower", 104, 58, 13, .70], ["ph-keyboard", 146, 99, 9, .35], ["ph-crosshair-simple", 188, 70, 14, .75], ["ph-mouse", 221, 106, 8, .30], ["ph-joystick", 252, 61, 12, .65], ["ph-lightning", 283, 90, 11, .50], ["ph-cpu", 312, 73, 15, .78], ["ph-wifi-high", 341, 101, 9, .38]];
  const userPanel = d.userOpen
    ? `<div class="giz-dropdown-menu giz-profile-popup open">
        <div class="giz-dropdown-menu__content giz-profile-popup__content">
          <div class="giz-profile-popup__avatar-zone">
            <div class="giz-profile-popup__orbit">
              ${ORBIT.map(([icon, angle, radius, size, opacity]) => `<i class="ph-bold ${icon} giz-orbit-icon" style="font-size:${size}px; opacity:${opacity}; transform: rotate(${angle}deg) translate(0, -${radius}px) rotate(${(-angle + Math.sin(angle * Math.PI / 180) * 15).toFixed(2)}deg);"></i>`).join("")}
              <div class="giz-profile-popup__avatar">${d.picture ? `<div class="giz-avatar giz-avatar--medium giz-avatar--circle"><img src="${d.picture}" alt=""></div>` : `<i class="ph ph-user"></i>`}</div>
            </div>
          </div>
          <div class="giz-profile-popup__name">${esc(d.username || "xennon")}</div>
          <div class="giz-profile-popup__actions">
            <button class="giz-profile-popup__account-btn"><i class="ph-bold ph-user"></i><span>Мой профиль</span><i class="ph-bold ph-arrow-right giz-profile-popup__account-arrow"></i></button>
            <div class="giz-profile-popup__actions-row">
              <button class="giz-profile-popup__change-password-btn"><i class="ph-bold ph-key"></i><span>Изменить пароль</span></button>
              <button class="giz-profile-popup__lock-btn" title="Заблокировать ПК"><i class="ph-bold ph-lock-simple"></i></button>
            </div>
            <button class="giz-profile-popup__logout-btn"><i class="ph-bold ph-sign-out"></i><span>Выход</span></button>
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
            <div class="giz-header__user-menu-item giz-user-online-deposit-dropdown" data-tooltip="Пополнить">
              <button class="giz-topup-pill"><i class="ph-fill ph-wallet giz-ph-icon"></i><span>Пополнить</span></button>
            </div>
            <div class="giz-header__user-menu-item giz-active-apps-dropdown${d.appsOpen ? " open" : ""}" data-tooltip="Мои приложения">
              <button class="user-menu-item-button--box"><i class="ph-bold ph-squares-four giz-ph-icon"></i></button>
              ${appsPanel}
            </div>
            <div class="giz-header__user-menu-item giz-notifications-dropdown${d.notificationsOpen ? " open" : ""}" data-tooltip="Уведомления">
              ${d.unread ? `<div class="giz-badge giz-badge--corner"><span><button class="user-menu-item-button--box"><i class="ph-bold ph-bell giz-ph-icon"></i></button></span><span class="giz-badge__wrapper"><span class="giz-badge__badge">${d.unread}</span></span></div>` : `<button class="user-menu-item-button--box"><i class="ph-bold ph-bell giz-ph-icon"></i></button>`}
              ${notificationsPanel}
            </div>
            <div class="giz-header__user-menu-item giz-assistance-dropdown${d.assistanceOpen ? " open" : ""}" data-tooltip="Позвать администратора">
              <button class="user-menu-item-button--box"><i class="ph-bold ph-question giz-ph-icon"></i></button>
              ${assistancePanel}
            </div>
          </div>
        </div>
        ${deploy}
        ${levelHint(d)}
        <div class="giz-top-bar__account">
          <div class="giz-header__user-menu-item giz-user-dropdown${d.userOpen ? " open" : ""}">
            <span class="gg-avatar-host">
              <button class="giz-user-menu-button">
                ${avatar(d)}
              </button>
              ${levelRing(d)}
            </span>
            ${userPanel}
          </div>
        </div>
      </div>
    </div>`;
}

// Shared/UserAvatar.razor: the person's picture when the account has one (`picture`),
// otherwise the glyph.
function avatar(d, cls) {
  return d.picture
    ? `<div class="giz-user-avatar ${cls || ""} giz-avatar giz-avatar--medium giz-avatar--circle"><img src="${d.picture}" alt=""></div>`
    : `<span class="giz-user-avatar giz-user-avatar--glyph ${cls || ""}"><i class="ph ph-user"></i></span>`;
}

// Shared/LoyaltyAvatarRing.razor: the ladder level worn on the avatar - the level's
// mark at the corner and, from one percent of the way up, a ring that fills towards
// the next level - drawn on the host span outside the button. `level: { progress,
// mark }`; nothing without a ladder.
function levelRing(d) {
  if (!d.level) return "";
  const C = 2 * Math.PI * 19; // r = 19
  const off = (C * (1 - Math.max(0, Math.min(1, d.level.progress)))).toFixed(2).replace(/\.?0+$/, "");
  const ring = d.level.progress >= 0.01
    ? `<svg class="gg-avatar-ring" viewBox="0 0 40 40" aria-hidden="true"><circle class="gg-avatar-ring__track" cx="20" cy="20" r="19"></circle><circle class="gg-avatar-ring__bar" cx="20" cy="20" r="19" style="stroke-dashoffset: ${off}"></circle></svg>`
    : "";
  return `${ring}<span class="gg-avatar-mark">${esc(d.level.mark)}</span>`;
}

// Shared/LoyaltyHint.razor: for ten seconds after sign-in, a pill slides out next to
// the avatar to say what the ring on it means. Same pill as the deployment banner,
// without the fill. `levelHint: earning | secured`.
function levelHint(d) {
  if (!d.levelHint) return "";
  const secured = d.levelHint === "secured";
  const name = d.levelName || (secured ? "Alternatif" : "Praxilla");
  return `<div class="giz-deploy gg-hint open">
    <div class="giz-deploy__badge gg-hint__badge">${esc(d.levelMark || name[0])}</div>
    <button type="button" class="giz-deploy__body" title="Мой прогресс">
      <span class="giz-deploy__title">${esc(name)} — ваш уровень</span>
      <span class="giz-deploy__sub">Нажмите, чтобы узнать больше</span>
    </button>
    <i class="ph-bold ph-arrow-right gg-hint__arrow"></i>
  </div>`;
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
          <img src="${artFor("exe", i)}" class="giz-image--cover" alt="image" />
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

// d: { balance, points, time, pc, tariff, reservation, deploy, pinned, active, level,
//      levelHint, picture, username, appsOpen, notificationsOpen, assistanceOpen,
//      userOpen }
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

module.exports = { frame, avatar };
