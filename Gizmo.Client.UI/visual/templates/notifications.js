// The notification cards, mirroring Shared/GizNotification.razor and
// Shared/ConfirmReservationNotification.razor inside the stack of
// Components/Notifications/NotificationsHost.razor. In the client they float in their
// own transparent window over whatever is on screen; here they sit on the atmosphere.
"use strict";

const { esc } = require("../lib/html");

const ICONS = { info: "ph-info", success: "ph-check-circle", warning: "ph-warning", danger: "ph-x-circle" };

function card(n) {
  const type = n.type || "info";
  return `<div class="giz-notification-wrapper">
    <div class="giz-notification${type === "info" ? "" : " giz-notification--" + type}">
      <div class="giz-notification__icon"><i class="ph-fill ${ICONS[type]}"></i></div>
      <div class="giz-notification__body">
        <div class="giz-notification__body__title">${esc(n.title)}</div>
        ${n.message ? `<div class="giz-notification__body__message">${esc(n.message)}</div>` : ""}
      </div>
      <button type="button" class="giz-notification__close"><i class="ph-bold ph-x"></i></button>
    </div>
  </div>`;
}

function reservation(n) {
  return `<div class="giz-notification-wrapper">
    <div class="giz-reservation-notification">
      <div class="giz-reservation-notification__icon"><i class="ph-fill ph-calendar-check"></i></div>
      <div class="giz-reservation-notification__body">
        <div class="giz-reservation-notification__body__title">${esc(n.title || "Бронь подтверждена")}</div>
        <div class="giz-reservation-notification__body__message">${esc(n.message || "Устройство забронировано на 18:30. Оплатите бронь, чтобы её не сняли.")}</div>
        <div class="giz-reservation-notification__body__actions">
          <button type="button" class="giz-notification__btn giz-notification__btn--primary">Оплатить</button>
          <button type="button" class="giz-notification__btn">Позже</button>
        </div>
      </div>
      <button type="button" class="giz-notification__close"><i class="ph-bold ph-x"></i></button>
    </div>
  </div>`;
}

// d: { items: [{ type, title, message }], reservation: bool, board } - with `board`
// the stack is drawn over the home board the way the customer sees it: the
// notification window sits top-centre over whatever is on screen.
function render(d) {
  const items = d.items || [
    { type: "info", title: "Новое сообщение от администратора", message: "Турнир по CS2 начнётся в 20:00, регистрация на стойке." },
    { type: "success", title: "Оплата прошла", message: "Пакет «Стандарт 3 часа» добавлен на ваш счёт." },
    { type: "warning", title: "Осталось 10 минут", message: "Пополните счёт или возьмите пакет, чтобы не прерываться." },
    { type: "danger", title: "Не удалось запустить игру", message: "Файлы игры ещё копируются. Попробуйте через минуту." },
  ];
  const stack = `<div client-theme="true" class="giz-notifications">
  <div class="giz-notifications__body">
    ${items.map(card).join("\n")}
    ${d.reservation === false ? "" : reservation(d)}
  </div>
</div>`;
  if (!d.board) return stack;
  return require("./home").render(d.board).replace(/\n$/, "") + "\n" + stack;
}

module.exports = { render };
