// The application page, mirroring Pages/Apps/AppDetails.razor: the cover and the
// brand facts on the left, the category, title, description, the launch shortcuts
// (Components/Apps/UniversalExecutable) and the media / links tabs on the right.
"use strict";

const { esc, artFor } = require("../lib/html");
const { frame } = require("./frame");
const { NAMES } = require("./home");

const ARROW_LEFT = `<div class="giz-icon giz-icon--medium"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M19 12H5M11 18l-6-6 6-6" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg></div>`;

function executable(caption, i, state) {
  return `<div class="giz-universal-executable">
    <div class="giz-universal-executable__icon">
      <img src="${artFor("exe", i, 50)}" class="giz-image--cover" alt="image" />
      <div class="giz-universal-executable-progress-bar">${state === "running" ? '<div class="running"></div>' : state === "deploying" ? '<div class="giz-progress-bar"><div class="giz-progress-bar__background" style="left: 0.1rem"></div><div class="giz-progress-bar__value-bar" style="width: 46%" role="progressbar"></div></div>' : ""}</div>
    </div>
    <div class="giz-universal-executable__description">
      <div class="giz-universal-executable__description__exe">${esc(caption)}</div>
    </div>
  </div>`;
}

// d: { index, title, category, publisher, released, added, description, exes: [[caption,
//      state]], media: count, links: [[caption, url]], tab: media|links } plus the frame's
function render(d) {
  const names = NAMES[d.names || "promo"];
  const index = d.index || 0;
  const title = d.title || names.apps[index % names.apps.length];
  const category = d.category || (names.cats ? names.cats[index % names.cats.length] : "Шутеры");
  const exes = d.exes || [["Играть", "idle"], ["Лаунчер", "idle"]];
  const media = d.media == null ? 4 : d.media;
  const links = d.links || [["Официальный сайт", "#"], ["Руководство для новичков", "#"], ["Патч-ноты", "#"]];
  const tab = d.tab || "media";
  const description = d.description || "Кооперативный шутер на четверых: ночные вылазки, снаряжение, которое собирается из того, что нашли, и город, который меняется от матча к матчу. Поддерживает кросс-плей и сохранения в облаке клуба.";

  const mediaItems = Array.from({ length: media }, (_, i) => `<div class="giz-client-tab-item"><div class="giz-app-media-item"><img src="${artFor("media", i)}" alt="loading" /></div></div>`).join("");
  const arrow = (cls, path) => `<button class="giz-button giz-button--extra-small primary giz-button--fill giz-button--icon ${cls}"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-icon giz-icon--extra-small"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="${path}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg></div></div></button>`;

  const body = `<div class="giz-app-details">
  <div class="giz-app-details__app">
    <div class="giz-app-details__app__navigation">
      <div>
        <div class="giz-app-details__app__navigation__icon">${ARROW_LEFT}</div>
        <div class="giz-app-details__app__navigation__label">Назад</div>
      </div>
    </div>
    <div class="giz-app-details__app__info">
      <div class="giz-app-details__app__info__side">
        <div class="giz-app-details__app__info__image">
          <img src="${artFor("app", index)}" class="giz-image--cover" alt="image" />
        </div>
        <div class="giz-app-details-card-brand-info">
          <div class="giz-app-details-card-brand-title">Издатель</div>
          <div class="giz-app-details-card-brand-text">${esc(d.publisher || "Northlight Studio")}</div>
          <div class="giz-app-details-card-brand-title">Дата выхода</div>
          <div class="giz-app-details-card-brand-text">${esc(d.released || "14.03.2025")}</div>
          <div class="giz-app-details-card-brand-title">Дата добавления</div>
          <div class="giz-app-details-card-brand-text">${esc(d.added || "02.09.2026")}</div>
        </div>
      </div>
      <div class="giz-app-details__app__info__main giz-scrollbar--v">
        <div class="giz-app-details__app__info__details">
          <div class="giz-app-details__app__info__details__category">${esc(category)}</div>
          <div class="giz-app-details__app__info__details__title">${esc(title)}</div>
          <div class="giz-app-details__app__info__details__description">${esc(description)}</div>
          <div class="giz-app-details__app__info__details__executables">
            ${exes.slice(0, 4).map((e, i) => executable(e[0], i, e[1])).join("\n")}
          </div>
          ${exes.length > 4 ? `<div><button class="giz-button giz-button--small primary giz-button--outline"><div class="giz-button__progress-wrapper"><div class="giz-button__progress" style="width: 0%"></div></div><div class="giz-button__content_wrapper"><div class="giz-button__content">Показать больше</div></div></button></div>` : ""}
        </div>
        <div class="giz-app-details__app__info__additional-content">
          ${media || links.length ? `<div class="giz-app-details__app__info__additional-content__tab">
            <div class="giz-client-tab">
              ${arrow("giz-client-tab__previous", "M15 18l-6-6 6-6")}
              <div class="giz-client-tab__wrapper"><div class="giz-client-tab__content">
                ${media ? `<div class="giz-client-tab-item${tab === "media" ? " active" : ""}"><div>Медиа</div></div>` : ""}
                ${links.length ? `<div class="giz-client-tab-item${tab === "links" ? " active" : ""}"><div>Ссылки</div></div>` : ""}
              </div></div>
              ${arrow("giz-client-tab__next", "M9 6l6 6-6 6")}
            </div>
          </div>
          <div class="giz-app-details__app__info__additional-content__media">
            ${tab === "media"
              ? `<div class="giz-client-tab giz-client-tab--internal">${arrow("giz-client-tab__previous", "M15 18l-6-6 6-6")}<div class="giz-client-tab__wrapper"><div class="giz-client-tab__content">${mediaItems}</div></div>${arrow("giz-client-tab__next", "M9 6l6 6-6 6")}</div>`
              : `<div class="giz-app-links">${links.map((l) => `<div class="giz-app-link-item"><a href="${esc(l[1])}">${esc(l[0])}</a></div>`).join("")}</div>`}
          </div>` : ""}
        </div>
      </div>
    </div>
  </div>
  <div class="giz-app-details__leaderboard"></div>
</div>`;
  return frame({ balance: 1250, points: 3400, time: "2ч 15м", pinned: 3, ...d, active: "apps" }, body);
}

module.exports = { render };
