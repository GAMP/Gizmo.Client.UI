// The tariff tooltip in the top bar, mirroring
// Shared/HeaderUserBalanceCurrentTimeProductTooltip.razor, drawn open over the home
// board. The frame places it inside the tariff when a scenario passes `tooltip`.
"use strict";

const { esc } = require("../lib/html");

const ICONS = { package: "ph-package", fixed: "ph-hourglass-medium", rate: "ph-timer" };
const TYPES = { package: "Пакет времени", fixed: "Пакет с фиксированным временем", rate: "Тариф" };

// t: { products: [{ name, type: package|fixed|rate, left, spendable, credit }],
//      loading, topUp, shop, credit, ending, unlimited }
// `ending` and `unlimited` stand in for the account's remaining time (15 minutes or
// less, or no clock at all), which is what the component reads from UserBalanceViewState.
function tooltipHtml(t) {
  const products = t.products || [];
  const topUp = t.topUp !== false;
  const shop = t.shop !== false;

  let list;
  if (t.loading) list = `<div class="gg-tt__wait"><div class="giz-animate-spinner"></div></div>`;
  else if (!products.length) list = `<div class="gg-tt__empty">Активных пакетов нет</div>`;
  else {
    list = `<div class="gg-tt__list">` + products.map((p, i) => `
      <div class="gg-tt__row${i === 0 ? " is-current" : ""}${p.credit ? " is-credit" : ""}">
        <span class="gg-tt__order">${i + 1}</span>
        <span class="gg-tt__type"><i class="ph-fill ${ICONS[p.type || "package"]}"></i></span>
        <div class="gg-tt__name"><b>${esc(p.name)}</b><small>${TYPES[p.type || "package"]}${p.credit ? "<span> · В кредит</span>" : ""}</small></div>
        <div class="gg-tt__stat"><small>Осталось</small><b>${esc(p.left || "∞")}</b></div>
        <div class="gg-tt__stat"><small>Можно потратить</small><b>${esc(p.spendable || "∞")}</b></div>
      </div>`).join("") + `</div>`;
  }

  let foot = "";
  if (products.length && (t.ending || t.unlimited)) {
    const unlimited = !!t.unlimited;
    const cta = unlimited ? "Списывается со счёта, пока вы играете"
      : shop && topUp ? "Чтобы продолжить — пополните счёт или возьмите пакет времени"
      : shop ? "Чтобы продолжить — возьмите пакет времени"
      : topUp ? "Чтобы продолжить — пополните счёт" : "";
    const actions = (topUp || shop) ? `<div class="gg-tt__actions">
        ${topUp ? `<button type="button" class="gg-tt__btn${shop ? "" : " gg-tt__btn--primary"}"><i class="ph-fill ph-wallet"></i><span>Пополнить</span></button>` : ""}
        ${shop ? `<button type="button" class="gg-tt__btn gg-tt__btn--primary"><i class="ph-fill ph-package"></i><span>В магазин</span></button>` : ""}
      </div>` : "";
    foot = `<div class="gg-tt__foot">
      <div class="gg-tt__note${unlimited ? "" : " gg-tt__note--warn"}">
        <i class="ph-fill ${unlimited ? "ph-infinity" : "ph-warning-circle"}"></i>
        <div><b>${unlimited ? "Время не ограничено" : "Время скоро закончится"}</b>${cta ? `<span>${cta}</span>` : ""}</div>
      </div>
      ${actions}
    </div>`;
  } else if (t.credit && !t.loading && products.length) {
    foot = `<div class="gg-tt__foot"><div class="gg-tt__note"><i class="ph-fill ph-hand-coins"></i><div><b>Вам доступен кредит времени</b></div></div></div>`;
  }

  return `<div class="giz-user-balance-tooltip-wrapper giz-user-balance-tooltip-wrapper--visible">
    <div class="gg-tt">
      <div class="gg-tt__head">
        <div class="gg-tt__title">Ваше время</div>
        <button type="button" class="gg-tt__link"><span>Все пакеты</span><i class="ph-bold ph-arrow-right"></i></button>
      </div>
      ${list}
      ${foot}
    </div>
  </div>`;
}

function render(d) {
  const home = require("./home");
  return home.render({ hero: "news", apps: 8, packs: 6, bar: 4, balance: 1250, points: 3400, time: "2ч 15м", pinned: 3, ...d });
}

module.exports = { tooltipHtml, render };
