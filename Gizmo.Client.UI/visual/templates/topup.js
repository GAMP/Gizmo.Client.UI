// The top-up dialog over the home board, mirroring
// Components/Common/UserOnlineDepositsDialog.razor with UserOnlineDeposits.razor inside:
// the amount step, the QR step and the success state.
"use strict";

const { esc, money } = require("../lib/html");
const home = require("./home");
const { hash } = require("../lib/artwork");

const CLOSE = `<div class="giz-icon-button__content"><div class="giz-icon giz-icon--small"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M6 6l12 12M18 6L6 18" stroke-width="2" stroke-linecap="round"/></svg></div></div>`;

function button(text, cls, disabled) {
  return `<button class="giz-button ${cls}${disabled ? " disabled" : ""}"${disabled ? " disabled" : ""}><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content"><div>${text}</div></div></div></button>`;
}

// Something that reads as a QR code from across the room (the real one is drawn by the
// server for the payment link). Deterministic.
function qr(seed) {
  const n = 29, cells = [];
  let h = hash("qr" + seed);
  const finder = (x, y) => (x < 7 && y < 7) || (x >= n - 7 && y < 7) || (x < 7 && y >= n - 7);
  for (let y = 0; y < n; y++) for (let x = 0; x < n; x++) {
    let on;
    if (finder(x, y)) {
      const fx = x < 7 ? x : x - (n - 7), fy = y < 7 ? y : y - (n - 7);
      on = fx === 0 || fx === 6 || fy === 0 || fy === 6 || (fx >= 2 && fx <= 4 && fy >= 2 && fy <= 4);
    } else if ((x === 7 && y < 8) || (y === 7 && x < 8) || (x === n - 8 && y < 8) || (y === 7 && x >= n - 8) || (x === 7 && y >= n - 8) || (y === n - 8 && x < 8)) {
      on = false;
    } else {
      h = (Math.imul(h ^ (h >>> 13), 0x5bd1e995) >>> 0);
      on = (h & 0x80) !== 0;
    }
    if (on) cells.push(`<rect x="${x}" y="${y}" width="1" height="1"/>`);
  }
  return `<svg viewBox="0 0 ${n} ${n}" xmlns="http://www.w3.org/2000/svg" fill="#111">${cells.join("")}</svg>`;
}

// d: { step: amount|qr|done, amount, presets, custom } plus the home board's keys
function render(d) {
  const presets = d.presets || [200, 500, 1000, 2000];
  const amount = d.amount == null ? 500 : d.amount;
  let body;
  if (d.step === "done") {
    body = `<div class="giz-user-online-deposit__success">
      <div class="giz-user-online-deposit__success__countdown"><div class="giz-user-online-deposit__success__countdown__fill"></div></div>
      <div class="giz-user-online-deposit__success__icon"><i class="ph-fill ph-check-circle"></i></div>
      <div class="giz-user-online-deposit__success__title">Счёт пополнен</div>
      <div class="giz-user-online-deposit__success__text">${money(amount)} уже на вашем счёте</div>
      <div class="giz-user-online-deposit__success__hint">Окно закроется само</div>
    </div>`;
  } else if (d.step === "qr") {
    body = `<div class="giz-user-online-deposit__body">
      <div class="giz-user-online-deposit__submitted__summary">К оплате <span class="giz-user-online-deposit__submitted__summary__amount">${money(amount)}</span></div>
      <div class="giz-user-online-deposit__submitted__qr">
        <div class="giz-user-online-deposit__submitted__qr__label">Отсканируйте QR-код телефоном</div>
        <div class="giz-user-online-deposit__submitted__qr__image">${qr(amount)}</div>
      </div>
    </div>
    <div class="giz-user-online-deposit__footer">
      <div class="giz-user-online-deposit__submitted__action">
        <div class="giz-user-online-deposit__submitted__action__label">или</div>
        ${button("Оплатить с этого ПК", "giz-button--extra-large accent giz-button--fill giz-button--full-width")}
      </div>
      <div class="giz-user-online-deposit-clear-button">${button("Отменить", "giz-button--extra-large primary giz-button--outline giz-button--full-width")}</div>
    </div>`;
  } else {
    body = `<div class="giz-user-online-deposit__body">
      <div class="giz-user-online-deposit__label">Быстрый выбор</div>
      <div><div class="giz-button-group">${presets.map((p) => `<button class="giz-button giz-button--large primary giz-button--group-button${p === amount ? " selected" : ""}"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content">${money(p)}</div></div></button>`).join("")}</div></div>
      ${d.custom === false ? "" : `<form><div><div class="giz-text-input giz-text-input--full-width giz-input-control"><label class="giz-input-label">Или впишите свою</label><div class="giz-input-root giz-input-root--small giz-input-root--outline giz-input-root--transparent giz-input-root--full-width"><div class="giz-input-wrapper"><input type="text" value="${presets.includes(amount) ? "" : amount}" /></div></div></div></div></form>`}
      <div class="giz-user-online-deposit__summary">Зачислим на счёт <span class="giz-user-online-deposit__summary__amount">${money(amount)}</span></div>
    </div>
    <div class="giz-user-online-deposit__footer">${button("Пополнить", "giz-button--extra-large accent giz-button--fill giz-button--full-width")}</div>`;
  }

  const dialog = `<div class="giz-dialog giz-dialog--open" tabindex="-1">
    <div class="giz-dialog__content-wrapper">
      <div class="giz-dialog__content">
        <div class="giz-client-dialog giz-user-online-deposit-dialog">
          <div class="giz-client-dialog__header">
            <div class="giz-client-dialog__header__title">Пополнить</div>
            <button class="close-btn giz-icon-button giz-icon-button--small primary giz-icon-button--text">${CLOSE}</button>
          </div>
          <div class="giz-client-dialog__body"><div class="giz-user-online-deposit">${body}</div></div>
        </div>
      </div>
    </div>
  </div>`;
  const boardData = { hero: "news", apps: 4, packs: 6, bar: 4, balance: 1250, points: 3400, ...(d.board || {}) };
  return home.render(boardData).replace(/\n$/, "") + "\n" + dialog;
}

module.exports = { render, qr };
