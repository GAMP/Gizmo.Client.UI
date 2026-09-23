// The shop checkout dialog, mirroring Components/Shop/CheckoutDialog.razor: the same card
// as the package purchase with the cart's lines where the package name goes.
"use strict";

const { esc, money, number, plural } = require("../lib/html");
const { wrap, WAYS, POINTS, SPINNER, pts } = require("./purchase");

const ITEMS = ["{0} товар", "{0} товара", "{0} товаров"];

const GOODS = ["Кола 0,5", "Ред Булл", "Чипсы Лейс", "Сникерс", "Кофе латте", "Вода без газа", "Круассан", "Чай", "Пицца Маргарита", "Энергетик Burn", "Пицца Четыре сыра большая 40 см с дополнительной моцареллой", "Сэндвич с курицей"];

// d: { items: [{name, qty, price, points}] | count, ways, selected, discount, fees,
//      balance, points, step, topUpEnabled, longNames }
function render(d) {
  const ways = (d.ways || ["balance"]).map((key) => ({ key, ...WAYS[key] }));
  const selectedKey = d.selected || (ways[0] && ways[0].key);
  const selected = ways.find((w) => w.key === selectedKey) || null;
  const fromBalance = selected && selected.kind === "balance";
  const atCounter = selected && selected.kind === "counter";

  let items = d.items;
  if (!items) {
    const count = d.count == null ? 3 : d.count;
    const goods = d.goods || GOODS;
    items = [];
    for (let i = 0; i < count; i++) {
      items.push({ name: d.longNames ? GOODS[10 + (i % 2)] : goods[i % goods.length], qty: 1 + (i % 3), price: 90 + i * 35, points: 0 });
    }
  }
  const total0 = items.reduce((sum, it) => sum + (it.points ? 0 : it.price * it.qty), 0);
  const pointsDue = items.reduce((sum, it) => sum + (it.points ? it.points * it.qty : 0), 0);
  const discount = d.discount || 0;
  const fees = d.fees || 0;
  const total = Math.max(0, total0 - discount + fees);
  const balance = d.balance == null ? 1250 : d.balance;
  const points = d.points == null ? 3400 : d.points;
  const isFree = total === 0 && pointsDue === 0;
  const shortfall = fromBalance && total > balance ? total - balance : 0;
  const pointsShort = pointsDue > points ? pointsDue - points : 0;
  const canTopUp = d.topUpEnabled !== false;
  const loading = d.step === "loading";
  const paying = d.step === "paying";
  const busy = d.step === "busy";
  const qty = items.reduce((sum, it) => sum + it.qty, 0);

  const kick = d.step === "topup" ? "Пополнение счёта" : "Оформление заказа";
  const head = `<header class="giz-buy__head">
    <div class="giz-buy__kick">${kick}</div>
    <button type="button" class="giz-buy__close" aria-label="Закрыть"${paying ? " disabled" : ""}><i class="ph-bold ph-x"></i></button>
    <h2 class="giz-buy__title">${plural(qty, ITEMS)}</h2>
    ${d.step === "topup" ? `<div class="giz-buy__sub">Не хватает ${money(shortfall)} — после пополнения вернёмся к покупке</div>` : ""}
  </header>`;

  if (d.step === "done" || d.step === "error") {
    const ok = d.step === "done";
    const note = ok ? (d.counterPaid ? "Оплатите его у администратора" : "") : (d.error || "Возникла ошибка.");
    const body = `<div class="giz-buy__result ${ok ? "is-ok" : "is-error"}">
      <div class="giz-buy__result-icon"><i class="ph-bold ${ok ? "ph-check" : "ph-x"}"></i></div>
      <div class="giz-buy__result-title">${ok ? (d.counterPaid ? "Заказ оформлен" : "Заказ оплачен") : "Не получилось"}</div>
      ${note ? `<div class="giz-buy__result-note${ok ? "" : " giz-buy__result-note--error"}">${esc(note)}</div>` : ""}
      <div class="giz-buy__actions giz-buy__actions--center">
        <button type="button" class="giz-buy__btn giz-buy__btn--primary">${ok ? "Готово" : "Закрыть"}</button>
      </div>
    </div>`;
    return wrap(d, `<div class="giz-buy giz-buy--checkout giz-buy--done">${body}</div>`);
  }

  if (d.step === "topup") {
    const body = `<div class="giz-buy__topup"><div class="giz-user-online-deposit"><div class="giz-user-online-deposit__body">
      <div class="giz-user-online-deposit__label">Выберите сумму</div>
      <div><div class="giz-button-group">${[200, 500, 1000, 2000].map((p) => `<button class="giz-button giz-button--fill primary giz-button--large"><div class="giz-button__content_wrapper"><div class="giz-button__content">${money(p)}</div></div></button>`).join("")}</div></div>
      <div class="giz-user-online-deposit__summary">Зачислим на счёт <span class="giz-user-online-deposit__summary__amount">${money(shortfall)}</span></div>
      </div><div class="giz-user-online-deposit__footer"><button class="giz-button giz-button--fill accent giz-button--extra-large giz-button--full-width"><div class="giz-button__content_wrapper"><div class="giz-button__content"><div>Пополнение</div></div></div></button></div></div></div>
      <div class="giz-buy__actions"><button type="button" class="giz-buy__btn">Назад</button></div>`;
    return wrap(d, `<div class="giz-buy giz-buy--checkout">${head}${body}</div>`);
  }

  const rows = items.map((it) => `<div class="giz-buy__row">
      <span class="giz-buy__row-name">${esc(it.name)}</span>
      ${it.qty > 1 ? `<span class="giz-buy__row-qty">× ${it.qty}</span>` : ""}
      ${it.points ? `<span class="giz-buy__row-sum giz-buy__pts">${number(it.points * it.qty)}<i class="ph-fill ph-coins"></i></span>` : `<span class="giz-buy__row-sum">${money(it.price * it.qty)}</span>`}
    </div>`);
  const itemsHtml = `<div class="giz-buy__items giz-scrollbar--v">${rows.join("\n")}</div>`;

  const counters = ways.filter((w) => w.kind === "counter");
  const grouped = counters.length > 1;
  let waysHtml = "";
  if (ways.length > 1 && !isFree) {
    const cells = [];
    for (const w of ways) {
      if (w.kind === "balance") cells.push({ name: w.name, note: money(balance), short: total > balance, on: w === selected });
      else if (!grouped) cells.push({ name: w.name, note: "у администратора", short: false, on: w === selected });
      else if (!cells.some((c) => c.counter)) cells.push({ counter: true, name: "На кассе", note: atCounter ? selected.name : "у администратора", short: false, on: !!atCounter });
    }
    const seg = cells.map((c) => `<button type="button" class="giz-buy__seg-item${c.on ? " is-on" : ""}${c.short ? " is-short" : ""}"${paying || loading ? " disabled" : ""}>
        <span class="giz-buy__seg-name">${esc(c.name)}</span><span class="giz-buy__seg-note">${esc(c.note)}</span></button>`);
    const chips = grouped && atCounter
      ? `<div class="giz-buy__chips">${counters.map((w) => `<button type="button" class="giz-buy__chip${w === selected ? " is-on" : ""}">${esc(w.name)}</button>`).join("")}</div>` : "";
    waysHtml = `<section class="giz-buy__ways"><div class="giz-buy__cap">Способ оплаты</div><div class="giz-buy__seg">${seg.join("\n")}</div>${chips}</section>`;
  } else if (ways.length === 1 && !isFree) {
    const w = ways[0];
    const note = w.kind === "balance" ? `На счёте ${money(balance)}` : "Оплата у администратора";
    waysHtml = `<div class="giz-buy__way-line"><span class="giz-buy__cap">Способ оплаты</span><span class="giz-buy__way-line-name">${esc(w.name)}</span><span class="giz-buy__way-line-note">${note}</span></div>`;
  }

  let bill;
  if (loading) bill = `<div class="giz-buy__wait">${SPINNER}</div>`;
  else {
    const lines = [];
    if (discount > 0) lines.push(`<div class="giz-buy__line giz-buy__line--good"><span>Скидка</span><span>−${money(discount)}</span></div>`);
    if (fees > 0) lines.push(`<div class="giz-buy__line"><span>Налоги и сборы</span><span>${money(fees)}</span></div>`);
    const ruled = discount > 0 || fees > 0;
    let sum;
    if (isFree) sum = `<span class="giz-buy__total-free">Оплата не требуется</span>`;
    else sum = (pointsDue > 0 ? pts(pointsDue) : "") + (total > 0 ? `<span>${pointsDue > 0 ? "+ " : ""}${money(total)}</span>` : "");
    lines.push(`<div class="giz-buy__total${ruled ? " giz-buy__total--ruled" : ""}"><span class="giz-buy__total-label">К оплате</span><span class="giz-buy__total-sum">${sum}</span></div>`);
    if (!isFree && ways.length === 0) lines.push(`<div class="giz-buy__short">Оплата с этого компьютера недоступна — обратитесь к администратору</div>`);
    else if (shortfall > 0) lines.push(`<div class="giz-buy__short"><span>Не хватает</span><span class="giz-buy__short-sum">${money(shortfall)}</span></div>`);
    else if (pointsShort > 0) lines.push(`<div class="giz-buy__short"><span>Не хватает</span><span class="giz-buy__short-sum">${plural(pointsShort, POINTS)}</span></div>`);
    else if (fromBalance && total > 0) lines.push(`<div class="giz-buy__after"><span>Останется на счёте</span><span>${money(balance - total)}</span></div>`);
    else if (total === 0 && pointsDue > 0) lines.push(`<div class="giz-buy__after"><span>Останется</span><span>${plural(points - pointsDue, POINTS)}</span></div>`);
    else if (atCounter) lines.push(`<div class="giz-buy__after giz-buy__after--hint">Заказ оплачивается у администратора</div>`);
    bill = lines.join("\n");
  }
  const billHtml = `<section class="giz-buy__bill${busy ? " is-busy" : ""}">${bill}</section>`;

  let primary, enabled;
  if (isFree) { primary = "Получить"; enabled = !loading; }
  else if (shortfall > 0) { primary = canTopUp ? `Пополнить на ${money(shortfall)}` : "Пополните на стойке"; enabled = canTopUp; }
  else if (total === 0 && pointsDue > 0) { primary = `Заказать за ${plural(pointsDue, POINTS)}`; enabled = pointsShort === 0; }
  else if (atCounter) { primary = "Заказать"; enabled = true; }
  else { primary = `Заказать за ${money(total)}`; enabled = !!selected && pointsShort === 0; }
  if (loading || busy || (ways.length === 0 && !isFree)) enabled = false;

  const actions = `<div class="giz-buy__actions">
    <button type="button" class="giz-buy__btn"${paying ? " disabled" : ""}>Отмена</button>
    <button type="button" class="giz-buy__btn giz-buy__btn--primary"${enabled && !paying ? "" : " disabled"}>${paying ? SPINNER : primary}</button>
  </div>`;

  return wrap(d, `<div class="giz-buy giz-buy--checkout">${head}${itemsHtml}${waysHtml}${billHtml}${actions}</div>`);
}

module.exports = { render };
