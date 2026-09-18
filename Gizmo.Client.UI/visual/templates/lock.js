// The profile lock over the home board, mirroring Shared/UserLock.razor: setting a PIN
// (`step: "set"`) or the locked screen asking for it (`step: "locked"`, the default),
// with `error` for a wrong code.
"use strict";

const { esc } = require("../lib/html");
const home = require("./home");

const EYE = `<div class="giz-icon giz-icon--medium giz-input-button-reveal"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M2 12s3.5-6 10-6 10 6 10 6-3.5 6-10 6-10-6-10-6z" stroke-width="2" stroke-linejoin="round"/><circle cx="12" cy="12" r="3" stroke-width="2"/></svg></div>`;
const BACKSPACE = `<div class="giz-icon giz-icon--extra-large"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M9 5h11a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H9l-6-7 6-7z" stroke-width="2" stroke-linejoin="round"/><path d="M12 10l5 5M17 10l-5 5" stroke-width="2" stroke-linecap="round"/></svg></div>`;

function button(inner, cls) {
  return `<button class="giz-button giz-button--extra-large ${cls}"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper">${inner}</div></button>`;
}

// d: { step: set|locked, pin, error } plus the home board's keys
function render(d) {
  const setting = d.step === "set";
  const pin = d.pin == null ? "••" : d.pin;
  const numpad = Array.from({ length: 10 }, (_, i) => button(`<div class="giz-button__content">${i}</div>`, "primary giz-button--fill")).join("") + button(BACKSPACE, "primary giz-button--fill giz-button--icon");
  const overlay = `<form><div class="giz-user-lock-overlay open">
    <div class="giz-user-lock-wrapper">
      <div class="giz-user-lock${d.error ? " error" : ""}">
        <div class="giz-user-lock__title">${setting ? "Блокировка профиля" : "Введите PIN-код для разблокировки"}</div>
        <div class="giz-user-lock__subtitle">${setting ? "Заблокируйте этот профиль, создав 4-значный PIN-код. Запомните его или запишите в надёжном месте." : "Блокировка профиля включена"}</div>
        <div class="giz-user-lock__code">
          <div class="giz-password-input giz-input-control"><div class="giz-input-root giz-input-root--medium giz-input-root--outline giz-input-root--transparent"><div class="giz-input-wrapper"><input type="password" value="${esc(pin)}" maxlength="4" /></div>${EYE}</div></div>
        </div>
        ${setting ? "" : `<div class="giz-user-lock__error">${esc(d.error || "")}</div>`}
        <div class="giz-user-lock__numpad">${numpad}</div>
        <div class="giz-user-lock__actions">
          ${setting
            ? button(`<div class="giz-button__content">Сохранить</div>`, "accent giz-button--fill") + button(`<div class="giz-button__content">Отмена</div>`, "accent giz-button--outline")
            : button(`<div class="giz-button__content">Разблокировать</div>`, "accent giz-button--fill")}
        </div>
        <div class="giz-user-lock__note"></div>
      </div>
    </div>
  </div></form>`;
  const boardData = { hero: "news", apps: 4, packs: 6, bar: 4, balance: 1250, points: 3400, ...(d.board || {}) };
  return home.render(boardData).replace(/\n$/, "") + "\n" + overlay;
}

module.exports = { render };
