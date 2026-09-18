// The sign-in screen without a club wallpaper: the shell's own atmosphere, the moving
// gradient on it and the host number. Mirrors the background layers of
// Shared/_Layout_Login.razor; with `card` the sign-in card of Pages/Login/Login.razor
// and Components/Login/LoginCard.razor is up as well (the vendor inputs and buttons
// render to the markup their class mappers produce). The harness parks infinite
// animations at frame zero, so this shows the gradient at its starting position only.
"use strict";

const { esc } = require("../lib/html");

const EYE = `<div class="giz-icon giz-icon--medium giz-input-button-reveal"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M2 12s3.5-6 10-6 10 6 10 6-3.5 6-10 6-10-6-10-6z" stroke-width="2" stroke-linejoin="round"/><circle cx="12" cy="12" r="3" stroke-width="2"/></svg></div>`;
const LANG = `<div class="giz-icon giz-icon--small"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><circle cx="12" cy="12" r="9" stroke-width="2"/><path d="M3 12h18M12 3c3 3.5 3 14.5 0 18M12 3c-3 3.5-3 14.5 0 18" stroke-width="2"/></svg></div>`;
const KEYBOARD = `<div class="giz-icon giz-icon--small"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><rect x="3" y="6" width="18" height="12" rx="2" stroke-width="2"/><path d="M7 10h.01M11 10h.01M15 10h.01M7 14h10" stroke-width="2" stroke-linecap="round"/></svg></div>`;
const DNS = `<div class="giz-icon giz-icon--medium giz-server__icon"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><rect x="3" y="4" width="18" height="6" rx="1.5" stroke-width="2"/><rect x="3" y="14" width="18" height="6" rx="1.5" stroke-width="2"/><path d="M7 7h.01M7 17h.01" stroke-width="2" stroke-linecap="round"/></svg></div>`;
const CHEVRON = `<div class="giz-icon giz-icon--small giz-input__icon-right"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M6 9l6 6 6-6" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg></div>`;

// A small Select showing one language, as ClientLanguageMenu / InputLanguageMenu do.
function languageSelect(cls, icon, text) {
  return `<div class="giz-select ${cls}"><div class="giz-select__content_wrapper"><div class="giz-input-control"><div class="giz-input-root giz-input-root--small giz-input-root--outline"><div class="giz-select__content" tabindex="0" style="outline: none"><div class="giz-input-language-menu-item"><div class="giz-input-language-menu-item__flag">${icon}</div><div>${text}</div></div></div>${CHEVRON}</div></div></div></div>`;
}

// Pages/Login/Login.razor inside Components/Login/LoginCard.razor.
function card(d) {
  const phone = d.method === "phone";
  const field = phone
    ? `<div class="giz-text-input giz-text-input--full-width giz-input-control"><label class="giz-input-label">Номер телефона</label><div class="giz-input-root giz-input-root--medium giz-input-root--outline giz-input-root--full-width"><div class="giz-input-wrapper"><input type="text" value="${esc(d.login || "+7 912 345-67-89")}" /></div></div></div>`
    : `<div class="giz-text-input giz-text-input--full-width giz-input-control"><label class="giz-input-label">Имя пользователя</label><div class="giz-input-root giz-input-root--medium giz-input-root--outline giz-input-root--full-width"><div class="giz-input-wrapper"><input type="text" value="${esc(d.login == null ? "nightowl" : d.login)}" /></div></div></div>`;
  const error = d.error ? `<div class="giz-login-error-block giz-error-block giz-error-block--with-icon" role="alert"><svg class="giz-error-block__icon" width="15" height="15" viewBox="0 0 15 15" fill="none" aria-hidden="true"><circle cx="7.5" cy="7.5" r="6.5" stroke="currentColor" stroke-width="1.3" /><line x1="7.5" y1="4.5" x2="7.5" y2="8.5" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" /><circle cx="7.5" cy="10.5" r="0.7" fill="currentColor" /></svg><span>${esc(d.error)}</span></div>` : "";
  // Components/Login/ReservationNotice.razor above the card.
  const reserved = d.reserved ? `<div class="giz-login-reserved"><div class="giz-login-reserved__badge"><i class="ph-fill ph-calendar-check giz-ph-icon"></i></div><div class="giz-login-reserved__text"><div class="giz-login-reserved__title"><span>Компьютер забронирован</span><span class="giz-login-reserved__time">с ${esc(d.reserved)}</span></div><div class="giz-login-reserved__note">Войти сможет только тот, кто бронировал: введите PIN-код из подтверждения.</div></div></div>` : "";
  return `<div class="giz-login-stack">
    ${reserved}
    <div class="giz-login-card">
      <div class="giz-login-card__header"></div>
      <div class="giz-login-card__body">
        ${error}
        <div class="giz-login-title">Войти</div>
        <div>
          <div class="giz-login-method giz-button-group">
            <button class="giz-button giz-button--medium primary giz-button--group-button${phone ? "" : " selected"}" type="button"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content">Имя пользователя</div></div></button>
            <button class="giz-button giz-button--medium primary giz-button--group-button${phone ? " selected" : ""}" type="button"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content">Номер телефона</div></div></button>
          </div>
        </div>
        <form>
          <div class="giz-form-input giz-form-input--swappable">${field}</div>
          <div class="giz-form-input">
            <div class="giz-password-input giz-password-input--full-width giz-input-control"><label class="giz-input-label">Пароль</label><div class="giz-input-root giz-input-root--medium giz-input-root--outline giz-input-root--full-width"><div class="giz-input-wrapper"><input type="password" value="${esc(d.password == null ? "••••••••" : d.password)}" /></div>${EYE}</div></div>
            <div class="giz-login-forgot-password"><a tabindex="-1" href="#">Забыли пароль?</a></div>
          </div>
          <div>
            <button class="giz-button giz-button--extra-large accent giz-button--fill giz-button--full-width giz-login-submit" type="button"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content"><div class="giz-login-submit__content"><span>Продолжить</span><i class="ph-bold ph-arrow-right"></i></div></div></div></button>
          </div>
          ${d.register === false ? "" : `<div class="giz-login-new-user"><span>Нет аккаунта клуба?</span><a>Зарегистрироваться</a></div>`}
        </form>
      </div>
      <div class="giz-login-card__footer">
        <div class="giz-input-language-menu"><div class="giz-input-language-menu-item"><div class="giz-input-language-menu-item__flag">${KEYBOARD}</div><div>РУС</div></div></div>
      </div>
    </div>
  </div>`;
}

// d: { pc, card, method: username|phone, login, password, error, reserved, register,
//      version }
function render(d) {
  const up = !!d.card;
  return `<div class="giz-main-container">
  <div class="giz-login-content giz-login-content--own-bg${up ? " collapsed" : ""}">
    <div class="giz-login-hardcoded-bg">
      <div class="gg-flow" aria-hidden="true">
        <span class="gg-flow__sheet gg-flow__sheet--1"></span>
        <span class="gg-flow__sheet gg-flow__sheet--2"></span>
      </div>
    </div>
    <div class="giz-login-hardcoded-bg__icons">
      <i class="ph-fill ph-game-controller"></i>
      <i class="ph-fill ph-desktop-tower"></i>
      <i class="ph-fill ph-headset"></i>
    </div>
    <div class="giz-host-number${up ? "" : " top-right"}">PC ${esc(d.pc || 12)}</div>
    <div class="giz-login-warnings"></div>
    ${up ? `<div class="giz-client-status">
      ${languageSelect("giz-client-language-menu", LANG, "РУС")}
      <div class="giz-server">
        ${DNS}
        <div class="giz-version">
          <div class="giz-version__title">Gizmo Версия</div>
          <div class="giz-version__version">${esc(d.version || "3.0.95")}</div>
        </div>
      </div>
    </div>` : ""}
  </div>
  ${up ? `<div class="giz-login__login collapsed">${card(d)}<div class="giz-dialog" tabindex="-1"></div></div>` : ""}
</div>`;
}

module.exports = { render };
