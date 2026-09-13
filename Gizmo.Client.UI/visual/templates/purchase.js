// The package purchase dialog, mirroring Components/Shop/PackagePurchaseDialog.razor.
// Drawn over the home board the way the client draws it: the DialogHost overlay with
// the page still visible behind the glass.
//
// The figures are worked out here the way the code-behind works them out, from the
// scenario's inputs, so a case reads as "price 650, balance 500, paying from the
// balance" rather than as a list of strings to paste.
"use strict";

const { esc, money, number, plural } = require("../lib/html");
const home = require("./home");

const POINTS = ["{0} балл", "{0} балла", "{0} баллов"];

const WAYS = {
  balance: { kind: "balance", name: "Со счёта" },
  points: { kind: "points", name: "Баллами" },
  cash: { kind: "counter", name: "Наличные" },
  card: { kind: "counter", name: "Банковская карта" },
  custom: { kind: "counter", name: "Kaspi QR на стойке" },
  custom2: { kind: "counter", name: "Оплата по QR-коду через администратора" },
};

const SPINNER = `<div class="giz-animate-spinner"><svg viewBox="0 0 20 20" fill="none" class="giz-spinner"><path d="M13.4126 2.02586C13.7008 1.35243 13.3897 0.563535 12.6841 0.36692C10.8062 -0.156332 8.80583 -0.121741 6.93209 0.482229C4.63531 1.22256 2.6837 2.76847 1.43725 4.83479C0.190808 6.90111 -0.266394 9.34847 0.14998 11.7254C0.566354 14.1024 1.82822 16.2486 3.70281 17.7682C5.57741 19.2878 7.93828 20.0783 10.35 19.9939C12.7616 19.9094 15.0614 18.9557 16.8251 17.3087C18.5888 15.6617 19.6975 13.4325 19.9466 11.0322C20.1498 9.07405 19.7697 7.10983 18.8693 5.38084C18.5309 4.73117 17.6947 4.59002 17.0955 5.01132C16.4963 5.43262 16.3647 6.25681 16.6716 6.92194C17.2211 8.11308 17.4452 9.43762 17.3081 10.7584C17.1251 12.522 16.3105 14.1599 15.0147 15.37C13.7188 16.5801 12.0291 17.2808 10.2571 17.3429C8.48518 17.4049 6.75056 16.8241 5.37322 15.7076C3.99589 14.5911 3.06875 13.0142 2.76283 11.2677C2.4569 9.5213 2.79283 7.72313 3.70864 6.20493C4.62445 4.68673 6.05836 3.5509 7.7459 3.00694C9.00974 2.59956 10.352 2.54475 11.631 2.83594C12.3453 2.99854 13.1244 2.69929 13.4126 2.02586Z" fill="currentColor"/></svg></div>`;

function pts(n) {
  return `<span class="giz-buy__pts">${number(n)}<i class="ph-fill ph-coins"></i></span>`;
}

// d: { name, duration, ways: [...WAYS keys], selected, price, pointsPrice, discount,
//      other, fees, balance, points, step: confirm|topup|done|error|loading|paying|busy,
//      counterPaid, topUpEnabled, free, purchase: "or"|"and" }
function render(d) {
  const name = d.name || "Стандарт 3 часа";
  const duration = d.duration == null ? "3 ч" : d.duration;
  const ways = (d.ways || ["balance"]).map((key) => ({ key, ...WAYS[key] }));
  const selectedKey = d.selected || (ways[0] && ways[0].key);
  const selected = ways.find((w) => w.key === selectedKey) || null;
  const payingWithPoints = selected && selected.kind === "points";
  const fromBalance = selected && selected.kind === "balance";
  const atCounter = selected && selected.kind === "counter";

  const price = d.price == null ? 650 : d.price;
  const hasPoints = ways.some((w) => w.kind === "points");
  const pointsPrice = d.pointsPrice == null ? (hasPoints ? 1200 : 0) : d.pointsPrice;
  const discount = d.discount || 0;
  const other = d.other || 0;
  const fees = d.fees || 0;
  const balance = d.balance == null ? 1250 : d.balance;
  const points = d.points == null ? 3400 : d.points;

  const packageTotal = payingWithPoints ? 0 : price;
  const total = d.free ? 0 : Math.max(0, packageTotal + other - discount + fees);
  const pointsDue = payingWithPoints ? pointsPrice : 0;
  const isFree = !!d.free || (total === 0 && pointsDue === 0);
  const shortfall = fromBalance && total > balance ? total - balance : 0;
  const pointsShort = pointsDue > points ? pointsDue - points : 0;
  const canTopUp = d.topUpEnabled !== false;
  const busy = d.step === "busy";
  const loading = d.step === "loading";
  const paying = d.step === "paying";

  // ── head ──
  const kick = d.step === "topup" ? "Пополнение счёта" : "Покупка пакета";
  let catalogue;
  if (d.free || (price <= 0 && pointsPrice <= 0)) catalogue = "Бесплатно";
  else if (price > 0 && pointsPrice > 0) catalogue = `${money(price)} ${d.purchase === "and" ? "и" : "или"} ${plural(pointsPrice, POINTS)}`;
  else catalogue = price > 0 ? money(price) : plural(pointsPrice, POINTS);
  const sub = d.step === "topup"
    ? `<div class="giz-buy__sub">Не хватает ${money(shortfall)} — после пополнения вернёмся к покупке</div>`
    : `<div class="giz-buy__sub">${esc(catalogue)}</div>`;
  const head = `<header class="giz-buy__head">
    <div class="giz-buy__kick">${kick}</div>
    <button type="button" class="giz-buy__close" aria-label="Закрыть"${paying ? " disabled" : ""}><i class="ph-bold ph-x"></i></button>
    <h2 class="giz-buy__title">${esc(name)}</h2>
    ${sub}
  </header>`;

  // ── result ──
  if (d.step === "done" || d.step === "error") {
    const ok = d.step === "done";
    const note = ok
      ? (d.counterPaid ? "Оплатите его у администратора" : duration)
      : (d.error || "Возникла ошибка.");
    const body = `<div class="giz-buy__result ${ok ? "is-ok" : "is-error"}">
      <div class="giz-buy__result-icon"><i class="ph-bold ${ok ? "ph-check" : "ph-x"}"></i></div>
      <div class="giz-buy__result-title">${ok ? (d.counterPaid ? "Заказ оформлен" : "Пакет куплен") : "Не получилось"}</div>
      ${ok ? `<div class="giz-buy__result-name">${esc(name)}</div>` : ""}
      ${note ? `<div class="giz-buy__result-note${ok ? "" : " giz-buy__result-note--error"}">${esc(note)}</div>` : ""}
      <div class="giz-buy__actions giz-buy__actions--center">
        <button type="button" class="giz-buy__btn giz-buy__btn--primary">${ok ? "Готово" : "Закрыть"}</button>
      </div>
    </div>`;
    return wrap(d, `<div class="giz-buy giz-buy--done">${body}</div>`);
  }

  // ── top-up step ──
  if (d.step === "topup") {
    const presets = [200, 500, 1000, 2000];
    const amount = shortfall || 500;
    const body = `<div class="giz-buy__topup">
      <div class="giz-user-online-deposit">
        <div class="giz-user-online-deposit__body">
          <div class="giz-user-online-deposit__label">Выберите сумму</div>
          <div><div class="giz-button-group">
            ${presets.map((p) => `<button class="giz-button giz-button--fill primary giz-button--large${p === amount ? " selected" : ""}"><div class="giz-button__content_wrapper"><div class="giz-button__content">${money(p)}</div></div></button>`).join("")}
          </div></div>
          <div><div class="giz-input-control"><label class="giz-input-label">Или впишите свою</label><div class="giz-input-root giz-input-root--transparent giz-input-root--small giz-input-root--full-width"><div class="giz-input-wrapper"><input type="text" value="${amount}" /></div></div></div></div>
          <div class="giz-user-online-deposit__summary">Зачислим на счёт <span class="giz-user-online-deposit__summary__amount">${money(amount)}</span></div>
        </div>
        <div class="giz-user-online-deposit__footer">
          <button class="giz-button giz-button--fill accent giz-button--extra-large giz-button--full-width"><div class="giz-button__content_wrapper"><div class="giz-button__content"><div>Пополнение</div></div></div></button>
        </div>
      </div>
    </div>
    <div class="giz-buy__actions"><button type="button" class="giz-buy__btn">Назад</button></div>`;
    return wrap(d, `<div class="giz-buy">${head}${body}</div>`);
  }

  // ── ways ──
  const counters = ways.filter((w) => w.kind === "counter");
  const grouped = counters.length > 1;
  let waysHtml = "";
  if (ways.length > 1 && !isFree) {
    const cells = [];
    for (const w of ways) {
      if (w.kind === "balance") {
        const short = price > balance;
        cells.push({ name: w.name, note: money(balance), coin: false, short, on: w === selected });
      } else if (w.kind === "points") {
        const short = pointsPrice > points;
        cells.push({ name: w.name, note: number(points), coin: true, short, on: w === selected });
      } else if (!grouped) {
        cells.push({ name: w.name, note: "у администратора", coin: false, short: false, on: w === selected });
      } else if (!cells.some((c) => c.counter)) {
        cells.push({ counter: true, name: "На кассе", note: atCounter ? selected.name : "у администратора", coin: false, short: false, on: !!atCounter });
      }
    }
    const seg = cells.map((c) => `<button type="button" class="giz-buy__seg-item${c.on ? " is-on" : ""}${c.short ? " is-short" : ""}"${paying || loading ? " disabled" : ""}>
        <span class="giz-buy__seg-name">${esc(c.name)}</span>
        <span class="giz-buy__seg-note">${esc(c.note)}${c.coin ? '<i class="ph-fill ph-coins"></i>' : ""}</span>
      </button>`);
    const chips = grouped && atCounter
      ? `<div class="giz-buy__chips">${counters.map((w) => `<button type="button" class="giz-buy__chip${w === selected ? " is-on" : ""}"${paying ? " disabled" : ""}>${esc(w.name)}</button>`).join("")}</div>`
      : "";
    waysHtml = `<section class="giz-buy__ways">
      <div class="giz-buy__cap">Способ оплаты</div>
      <div class="giz-buy__seg">${seg.join("\n")}</div>
      ${chips}
    </section>`;
  } else if (ways.length === 1 && !isFree) {
    const w = ways[0];
    const note = w.kind === "balance" ? `На счёте ${money(balance)}` : w.kind === "points" ? `У вас ${plural(points, POINTS)}` : "Оплата у администратора";
    waysHtml = `<div class="giz-buy__way-line">
      <span class="giz-buy__cap">Способ оплаты</span>
      <span class="giz-buy__way-line-name">${esc(w.name)}</span>
      <span class="giz-buy__way-line-note">${note}</span>
    </div>`;
  }

  // ── bill ──
  let bill;
  if (loading) {
    bill = `<div class="giz-buy__wait">${SPINNER}</div>`;
  } else {
    const showLines = other > 0 || discount > 0 || fees > 0;
    const lines = [];
    if (showLines) {
      lines.push(`<div class="giz-buy__line"><span>Цена</span>${payingWithPoints ? pts(pointsDue) : `<span>${money(price)}</span>`}</div>`);
      if (other > 0) lines.push(`<div class="giz-buy__line"><span>Товары в корзине</span><span>${money(other)}</span></div>`);
      if (discount > 0) lines.push(`<div class="giz-buy__line giz-buy__line--good"><span>Скидка</span><span>−${money(discount)}</span></div>`);
      if (fees > 0) lines.push(`<div class="giz-buy__line"><span>Налоги и сборы</span><span>${money(fees)}</span></div>`);
    }

    let sum;
    if (isFree) sum = `<span class="giz-buy__total-free">Оплата не требуется</span>`;
    else sum = (pointsDue > 0 ? pts(pointsDue) : "") + (total > 0 ? `<span>${pointsDue > 0 ? "+ " : ""}${money(total)}</span>` : "");
    lines.push(`<div class="giz-buy__total${showLines ? " giz-buy__total--ruled" : ""}"><span class="giz-buy__total-label">К оплате</span><span class="giz-buy__total-sum">${sum}</span></div>`);

    if (!isFree && ways.length === 0) lines.push(`<div class="giz-buy__short">Оплата с этого компьютера недоступна — обратитесь к администратору</div>`);
    else if (shortfall > 0) lines.push(`<div class="giz-buy__short"><span>Не хватает</span><span class="giz-buy__short-sum">${money(shortfall)}</span></div>`);
    else if (pointsShort > 0) lines.push(`<div class="giz-buy__short"><span>Не хватает</span><span class="giz-buy__short-sum">${plural(pointsShort, POINTS)}</span></div>`);
    else if (payingWithPoints && total === 0) lines.push(`<div class="giz-buy__after"><span>Останется</span><span>${plural(points - pointsDue, POINTS)}</span></div>`);
    else if (fromBalance && total > 0) lines.push(`<div class="giz-buy__after"><span>Останется на счёте</span><span>${money(balance - total)}</span></div>`);
    else if (atCounter) lines.push(`<div class="giz-buy__after giz-buy__after--hint">Заказ оплачивается у администратора</div>`);

    bill = lines.join("\n");
  }
  const billHtml = `<section class="giz-buy__bill${busy ? " is-busy" : ""}">${bill}</section>`;

  // ── button ──
  let primary, enabled;
  if (isFree) { primary = "Получить"; enabled = !loading; }
  else if (shortfall > 0) { primary = canTopUp ? `Пополнить на ${money(shortfall)}` : "Пополните на стойке"; enabled = canTopUp; }
  else if (payingWithPoints) { primary = `Купить за ${plural(pointsDue, POINTS)}`; enabled = pointsShort === 0; }
  else if (atCounter) { primary = "Заказать"; enabled = true; }
  else { primary = `Купить за ${money(total)}`; enabled = !!selected; }
  if (loading || busy || ways.length === 0 && !isFree) enabled = false;

  const actions = `<div class="giz-buy__actions">
    <button type="button" class="giz-buy__btn"${paying ? " disabled" : ""}>Отмена</button>
    <button type="button" class="giz-buy__btn giz-buy__btn--primary"${enabled && !paying ? "" : " disabled"}>${paying ? SPINNER : primary}</button>
  </div>`;

  return wrap(d, `<div class="giz-buy">${head}${waysHtml}${billHtml}${actions}</div>`);
}

// The dialog over the board, as DialogHost and HostedDialog nest it.
function wrap(d, dialog) {
  const overlay = `<div class="giz-dialog giz-dialog--open" tabindex="-1">
    <div class="giz-dialog__content-wrapper">
      <div class="giz-dialog__content">${dialog}</div>
    </div>
  </div>`;
  const boardData = { hero: "news", apps: 4, packs: 6, bar: 4, balance: d.balance == null ? 1250 : d.balance, points: d.points == null ? 3400 : d.points, ...(d.board || {}) };
  // home.render wraps the board in the frame; the overlay follows the frame as a sibling.
  return home.render(boardData).replace(/\n$/, "") + "\n" + overlay;
}

module.exports = { render, wrap, WAYS, POINTS, SPINNER, pts };
