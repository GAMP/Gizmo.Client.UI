// The account page's Progress tab, mirroring Pages/Profile/Progress.razor and the two
// components it places (Components/Loyalty/ChallengeCard, AchievementTile). Same class
// names, same nesting; the data comes from the scenario instead of api/user/v3.
//
// The styles are the shipped ones (_page-progress.scss, _loyalty.scss): when the Razor
// changes shape, this file changes with it.
"use strict";

const { esc, number } = require("../lib/html");
const { frame } = require("./frame");
const { head, tabs } = require("./account");

const C = 2 * Math.PI * 18; // circumference of the r = 18 ring (LoyaltyRing.Offset)

function ring(progress) {
  const off = (C * (1 - Math.max(0, Math.min(1, progress)))).toFixed(2).replace(/\.?0+$/, "");
  return `<svg viewBox="0 0 40 40" aria-hidden="true"><circle class="gg-ring__track" cx="20" cy="20" r="18"></circle><circle class="gg-ring__bar" cx="20" cy="20" r="18" style="stroke-dashoffset: ${off}"></circle></svg>`;
}

// A points ladder of four levels; the scenario says whether the customer is earning
// the second one or has the top one secured.
const LEVELS = [
  { name: "Гость", threshold: 0 },
  { name: "Игрок", threshold: 1000 },
  { name: "Praxilla", threshold: 2002 },
  { name: "Alternatif", threshold: 3000 },
];

// `levels` in the scenario replaces the table (the promo shots name their own).
function standing(d) {
  const LEVELS = d.levels || module.exports.LEVELS;
  const secured = d.ladder === "secured";
  const rank = secured ? 3 : 2;
  const score = d.score == null ? (secured ? 3412 : 513) : d.score;
  const next = LEVELS[rank + 1] || null;
  return {
    secured, rank, score, next,
    level: LEVELS[rank],
    progress: secured ? 1 : Math.min(1, score / next.threshold),
    toRetain: Math.max(0, LEVELS[rank].threshold - score),
  };
}

function ladder(d) {
  const LEVELS = d.levels || module.exports.LEVELS;
  const s = standing(d);
  const max = LEVELS[LEVELS.length - 1].threshold;
  const pct = (v) => (Math.min(1, v / max) * 100).toFixed(1).replace(/\.0$/, "");
  const stops = LEVELS.map((l, i) => `<span class="gg-rail__stop ${i < s.rank ? "is-passed" : ""} ${i === s.rank ? "is-current" : ""}" style="left: ${pct(l.threshold)}%"></span>`).join("");
  const labels = LEVELS.map((l, i) => {
    const labelled = i === s.rank || (s.next && i === s.rank + 1);
    const keep = i === s.rank && !s.secured ? "удержать · " : "";
    return `<span class="gg-rail__label ${i === s.rank ? "is-current" : ""}" style="left: ${pct(l.threshold)}%">${l.name}${labelled ? `<span>${keep}${number(l.threshold)}</span>` : ""}</span>`;
  }).join("");
  // LoyaltyLines.Long / Next and LoyaltyText.Perk, as the Razor joins them.
  const line = s.secured
    ? "Это высший уровень, и он закреплён до 1 октября."
    : `Наберите ещё ${number(s.toRetain)} очков до 1 октября — и уровень останется.`;
  const perks = s.secured ? ["скидка 15 %", "приоритет в очереди"] : ["скидка 10 %", "приоритет в очереди"];
  const nextLine = s.secured ? "" : ` Следующий уровень — ${s.next.name}: ещё ${number(s.next.threshold - s.score)} очков.`;
  return `<section class="gg-card">
    <div class="gg-ladder">
      <div class="gg-ladder__emblem">
        ${ring(s.progress)}
        <div class="gg-ladder__disc">${s.level.name[0]}</div>
        <span class="gg-ladder__pct">${number(s.score)} очков</span>
      </div>
      <div class="gg-ladder__body">
        <div class="gg-ladder__cap">Ваш уровень · за месяц</div>
        <h2 class="gg-ladder__name">
          ${s.level.name}
          <span class="gg-ladder__state ${s.secured ? "gg-ladder__state--secured" : ""}"><i class="ph-bold ${s.secured ? "ph-check" : "ph-trend-up"}"></i>${s.secured ? "Закреплён" : "Набираете"}</span>
        </h2>
        <p class="gg-ladder__line">${line}</p>
        <div class="gg-rail">
          <div class="gg-rail__track"><span class="gg-rail__fill" style="width: ${pct(s.score)}%"></span>${stops}</div>
          ${labels}
        </div>
        <p class="gg-ladder__bonus">Ваши бонусы: ${perks.map((p, i) => `<b>${p}</b>${i < perks.length - 1 ? " ·" : "."}`).join(" ")}${nextLine}</p>
      </div>
    </div>
  </section>`;
}

// Components/Loyalty/ChallengeCard.razor
function challengeCard(c) {
  const done = !!c.done;
  const segs = Array.from({ length: c.total }, (_, i) => `<i class="${i < c.met ? "is-on" : ""}"></i>`).join("");
  const todo = c.waiting
    ? "Награда ждёт вас у стойки — покажите администратору эту вкладку."
    : done ? esc(c.description || "") : `Осталось: <b>${esc(c.left)}</b>${c.leftProgress ? " · " + c.leftProgress : ""}`;
  const rewards = (c.rewards || []).map((r) => `<span class="gg-reward ${r.claim ? "gg-reward--claim" : ""}"><i class="ph-fill ${r.icon}"></i>${r.text}</span>`).join("");
  return `<article class="gg-chal__card ${done ? "is-done" : ""} ${c.off ? "is-off" : ""}">
    <div class="gg-chal__top">
      <div class="gg-chal__art"><i class="ph-fill ${done ? "ph-check" : "ph-flag-checkered"}"></i></div>
      <div class="gg-chal__head">
        <div class="gg-chal__name">${esc(c.name)}</div>
        <div class="gg-chal__when">${c.when}</div>
      </div>
      <div class="gg-chal__count">${c.met} из ${c.total}<small>${done ? "готово" : "шагов"}</small></div>
    </div>
    <div class="gg-chal__segs">${segs}</div>
    <p class="gg-chal__todo">${todo}</p>
    <div class="gg-chal__foot">${rewards}${c.pool ? `<small>Осталось ${c.pool} на всех</small>` : ""}</div>
  </article>`;
}

const CHALLENGES = [
  { name: "Ночной марафон", when: "до 30 сентября · 21 день", met: 2, total: 3, left: "Заказ в баре", leftProgress: "0,00 ₽ из 300,00 ₽",
    rewards: [{ icon: "ph-coins", text: "+500 очков" }, { icon: "ph-clock", text: "+1 ч" }] },
  { name: "Первый месяц", when: "выполнен 26 августа", met: 2, total: 2, done: true, waiting: true,
    rewards: [{ icon: "ph-gift", text: "Подарок", claim: true }] },
  { name: "Воин недели", when: "до 14 сентября · 5 дней", met: 1, total: 2, left: "Час в игре", leftProgress: "0 из 1 ч", pool: 3,
    rewards: [{ icon: "ph-coins", text: "+300 очков" }] },
];

function challenges(d) {
  const list = (d && d.challengeList) || CHALLENGES;
  const done = list.filter((c) => c.done).length;
  return `<div class="gg-sec"><h2>Челленджи</h2><span>${done} из ${list.length}</span></div>
  <div class="gg-chal">${list.map(challengeCard).join("\n")}</div>`;
}

// Components/Loyalty/AchievementTile.razor
function achievementTile(a) {
  const meta = a.secret ? "Откроется, когда получите" : a.earned ? (a.times > 1 ? `Получено ×${a.times}` : "Получено") : a.meta;
  return `<div class="gg-ach__tile ${a.earned ? "is-earned" : ""} ${a.secret ? "is-secret" : ""}">
    <div class="gg-ach__icon">
      ${a.secret ? "" : ring(a.earned ? 1 : a.p)}
      <i class="ph-fill ${a.secret ? "ph-question" : "ph-medal"}"></i>
      ${a.earned ? `<span class="gg-ach__check"><i class="ph-bold ph-check"></i></span>` : ""}
    </div>
    <div class="gg-ach__name">${a.secret ? "Секретное" : esc(a.name)}</div>
    <div class="gg-ach__meta">${meta}</div>
  </div>`;
}

const ACHIEVEMENTS = [
  { name: "Ночной игрок", earned: true, times: 9 },
  { name: "Стратег", earned: true },
  { name: "Ветеран", earned: true },
  { name: "Еженедельный гость", p: 0.6, meta: "3 из 5" },
  { name: "Верный клиент", p: 0.85, meta: "85,00 ₽ из 100,00 ₽" },
  { name: "Ежедневный гринд", p: 0.66, meta: "4 из 6 ч" },
  { name: "Коллекционер", p: 0.52, meta: "260 из 500 очков" },
  { name: "Щедрый заказ", p: 0.4, meta: "20,00 ₽ из 50,00 ₽" },
  { name: "Секретное", secret: true },
];

function achievements(d) {
  const list = (d && d.achievementList) || ACHIEVEMENTS;
  const earned = list.filter((a) => a.earned).length;
  return `<div class="gg-sec"><h2>Достижения</h2><span>${earned} из ${list.length}</span></div>
  <div class="gg-ach">${list.map(achievementTile).join("\n")}</div>`;
}

function history(d) {
  const s = standing(d);
  return `<div class="gg-hist"><i class="ph-bold ph-arrow-up"></i><span>1 сентября — уровень ${s.level.name}</span></div>`;
}

// The tab's body, for account.js as well: d.ladder = earning | secured | false (a club
// without a ladder: no level card, no history), d.empty (nothing at all: the empty
// card), d.challenges / d.achievements = false to leave a section out.
function progressTab(d) {
  if (d.empty) {
    return `<section class="gg-card"><div class="gg-card__empty"><i class="ph-fill ph-trophy"></i><b>Пока нечего показать</b></div></section>`;
  }
  const hasLadder = d.ladder !== false;
  return `<div class="gg-progress">
    ${hasLadder ? ladder(d) : ""}
    ${d.challenges === false ? "" : challenges(d)}
    ${d.achievements === false ? "" : achievements(d)}
    ${hasLadder ? history(d) : ""}
  </div>`;
}

// d: { ladder: earning | secured | false, challenges, achievements, empty } - the mark
// on the avatar follows the ladder.
function render(d) {
  const level = d.ladder === false ? null : (() => { const s = standing(d); return { progress: s.progress, mark: s.level.name[0] }; })();
  const body = `<div class="gg-account giz-scrollbar--v">
  <div class="gg-account__inner">
    ${head(d)}
    ${tabs({ ...d, tab: "progress", loyalty: true })}
    <div class="gg-account__body">
${progressTab(d)}
    </div>
  </div>
</div>`;
  return frame({ balance: 1250, points: 3400, time: "2ч 15м", pinned: 3, level, ...d, active: "profile" }, body);
}

module.exports = { render, progressTab, standing, LEVELS };
