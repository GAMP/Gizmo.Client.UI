// CONCEPT, not a mirror of any Razor file yet: the account page with a "Progress" tab
// for the achievements, challenges and ladder that Gizmo 3.0.95 serves through
// api/user/v3 (see ../../GRAFIT.md, "Achievements"). The styles live here in the
// template so nothing ships; when the tab is built they move to _page-progress.scss
// and this template becomes a mirror like the others.
//
// Second pass: fewer, larger words. One sentence per card says what to do; the lists
// of small captions went.
"use strict";

const { esc, number } = require("../lib/html");
const { frame } = require("./frame");

const CSS = `
<style>
/* ── level chip in the head ─────────────────────────────────────── */
.gg-level { display: inline-flex; align-items: center; gap: 0.8rem; padding: 0.45rem 1.2rem 0.45rem 0.5rem; border-radius: 99rem;
  background: rgba(var(--gg-accent-rgb), 0.12); border: 1px solid rgba(var(--gg-accent-rgb), 0.28); color: var(--gg-ink); font-size: 1.45rem; font-weight: 700; }
.gg-level__mark { width: 2.6rem; height: 2.6rem; border-radius: 50%; display: inline-flex; align-items: center; justify-content: center;
  background: var(--gg-accent); color: var(--gg-on-accent); font-family: Manrope, sans-serif; font-weight: 800; font-size: 1.2rem; }
.gg-level small { font-size: 1.3rem; font-weight: 500; color: rgba(var(--gg-ink-rgb), 0.6); }

/* ── level card ──────────────────────────────────────────────────── */
.gg-ladder { display: grid; grid-template-columns: auto minmax(0, 1fr); gap: clamp(2rem, 2.4vw, 3.6rem); align-items: center; }
.gg-ladder__emblem { position: relative; width: 15rem; height: 15rem; display: flex; align-items: center; justify-content: center; }
.gg-ladder__emblem svg { position: absolute; inset: 0; width: 100%; height: 100%; transform: rotate(-90deg); }
.gg-ladder__emblem circle { fill: none; stroke-width: 2.4; }
.gg-ladder__emblem .track { stroke: rgba(255,255,255,0.1); }
.gg-ladder__emblem .bar { stroke: var(--gg-accent-light); stroke-linecap: round; stroke-dasharray: 113.1; }
.gg-ladder__disc { width: 10.6rem; height: 10.6rem; border-radius: 50%; display: flex; align-items: center; justify-content: center;
  background: radial-gradient(circle at 35% 30%, rgba(var(--gg-accent-rgb), 0.55), rgba(var(--gg-accent-rgb), 0.12) 70%);
  border: 1px solid rgba(var(--gg-accent-rgb), 0.4); color: var(--gg-accent-pale); font-family: Manrope, sans-serif; font-weight: 800; font-size: 4rem; letter-spacing: -0.04em; }
.gg-ladder__pct { position: absolute; left: 50%; bottom: -0.6rem; transform: translateX(-50%); padding: 0.2rem 0.8rem; border-radius: 99rem; background: var(--gg-bg-1); border: 1px solid rgba(var(--gg-accent-rgb), 0.4);
  font-family: Manrope, sans-serif; font-size: 1.2rem; font-weight: 800; color: var(--gg-accent-pale); font-variant-numeric: tabular-nums; white-space: nowrap; }
.gg-ladder__cap { font-family: Manrope, sans-serif; font-size: 1.15rem; font-weight: 700; letter-spacing: 0.18em; text-transform: uppercase; color: rgba(var(--gg-ink-rgb), 0.45); }
.gg-ladder__name { margin: 0.3rem 0 0; font-family: Manrope, sans-serif; font-size: clamp(3rem, 2.2rem + 1vw, 4rem); font-weight: 800; letter-spacing: -0.02em; line-height: 1.05; color: var(--gg-ink); display: flex; align-items: center; gap: 1.4rem; flex-wrap: wrap; }
.gg-ladder__state { display: inline-flex; align-items: center; gap: 0.5rem; padding: 0.45rem 1.1rem; border-radius: 99rem; font-size: 1.3rem; font-weight: 700;
  background: rgba(94,216,148,0.14); color: #7ce8ad; border: 1px solid rgba(94,216,148,0.3); }
.gg-ladder__state--earning { background: rgba(var(--gg-accent-rgb), 0.14); color: var(--gg-accent-pale); border-color: rgba(var(--gg-accent-rgb), 0.35); }
.gg-ladder__line { margin: 1rem 0 0; font-size: 1.7rem; line-height: 1.4; color: rgba(var(--gg-ink-rgb), 0.78); max-width: 72rem; }
.gg-ladder__line b { color: var(--gg-ink); font-weight: 800; }
.gg-ladder__bonus { margin-top: 0.8rem; font-size: 1.4rem; color: rgba(var(--gg-ink-rgb), 0.6); }
.gg-ladder__bonus b { color: rgba(var(--gg-ink-rgb), 0.9); font-weight: 700; }

/* the rail: every level is a stop, the fill is this month's points */
.gg-rail { position: relative; margin: 2.4rem 0 0; padding-bottom: 3.6rem; }
.gg-rail__track { position: relative; height: 0.8rem; border-radius: 99rem; background: rgba(255,255,255,0.08); }
.gg-rail__fill { position: absolute; left: 0; top: 0; bottom: 0; border-radius: 99rem; background: linear-gradient(90deg, rgba(var(--gg-accent-rgb), 0.55), var(--gg-accent)); }
.gg-rail__stop { position: absolute; top: 50%; transform: translate(-50%, -50%); width: 1.8rem; height: 1.8rem; border-radius: 50%; background: var(--gg-bg-1); border: 2px solid rgba(255,255,255,0.22); }
.gg-rail__stop.is-passed { background: var(--gg-accent); border-color: var(--gg-accent-light); }
.gg-rail__stop.is-current { width: 2.6rem; height: 2.6rem; background: var(--gg-accent); border-color: #fff; box-shadow: 0 0 0 0.6rem rgba(var(--gg-accent-rgb), 0.22); }
.gg-rail__label { position: absolute; top: 1.8rem; transform: translateX(-50%); text-align: center; white-space: nowrap; font-size: 1.4rem; font-weight: 700; color: rgba(var(--gg-ink-rgb), 0.55); }
.gg-rail__label span { display: block; font-size: 1.2rem; font-weight: 500; color: rgba(var(--gg-ink-rgb), 0.42); font-variant-numeric: tabular-nums; }
.gg-rail__label.is-current { color: var(--gg-accent-pale); }
.gg-rail__label.is-current span { color: rgba(var(--gg-ink-rgb), 0.7); }
.gg-rail__label:first-child { transform: none; text-align: left; }
.gg-rail__label:last-child { transform: translateX(-100%); text-align: right; }

/* ── section head ────────────────────────────────────────────────── */
.gg-sec { display: flex; align-items: baseline; gap: 1.2rem; margin: 0.8rem 0 0; }
.gg-sec h2 { margin: 0; font-family: Manrope, sans-serif; font-size: 2rem; font-weight: 800; letter-spacing: -0.01em; color: var(--gg-ink); }
.gg-sec span { font-size: 1.4rem; color: rgba(var(--gg-ink-rgb), 0.5); }
.gg-sec a { margin-left: auto; font-size: 1.3rem; font-weight: 700; color: rgba(var(--gg-ink-rgb), 0.5); text-decoration: none; }

/* ── challenges: one card = one thing left to do ─────────────────── */
.gg-chal { display: grid; grid-template-columns: repeat(auto-fill, minmax(34rem, 1fr)); gap: 1.4rem; }
.gg-chal__card { display: flex; flex-direction: column; gap: 1.2rem; padding: 2rem 2.2rem 1.8rem; border-radius: 2rem;
  background: linear-gradient(180deg, rgba(var(--gg-glass-rgb), 0.55), rgba(var(--gg-glass-2-rgb), 0.7)); border: 1px solid rgba(255,255,255,0.09); box-shadow: inset 0 1px 0 rgba(255,255,255,0.1); }
.gg-chal__card.is-done { border-color: rgba(94,216,148,0.35); }
.gg-chal__top { display: flex; align-items: center; gap: 1.4rem; }
.gg-chal__art { width: 5.6rem; height: 5.6rem; flex: none; border-radius: 1.6rem; display: flex; align-items: center; justify-content: center; background: rgba(var(--gg-accent-rgb), 0.14); color: var(--gg-accent-soft); font-size: 2.6rem; }
.gg-chal__card.is-done .gg-chal__art { background: rgba(94,216,148,0.14); color: #7ce8ad; }
.gg-chal__name { font-family: Manrope, sans-serif; font-size: 1.8rem; font-weight: 800; color: var(--gg-ink); letter-spacing: -0.01em; line-height: 1.15; }
.gg-chal__when { margin-top: 0.3rem; font-size: 1.3rem; color: rgba(var(--gg-ink-rgb), 0.5); }
.gg-chal__count { margin-left: auto; flex: none; text-align: right; font-family: Manrope, sans-serif; font-size: 2rem; font-weight: 800; color: var(--gg-ink); font-variant-numeric: tabular-nums; }
.gg-chal__count small { display: block; font-size: 1.15rem; font-weight: 700; letter-spacing: 0.14em; text-transform: uppercase; color: rgba(var(--gg-ink-rgb), 0.42); }
.gg-chal__segs { display: flex; gap: 0.5rem; }
.gg-chal__segs i { flex: 1; height: 0.7rem; border-radius: 99rem; background: rgba(255,255,255,0.09); }
.gg-chal__segs i.is-on { background: var(--gg-accent); }
.gg-chal__card.is-done .gg-chal__segs i.is-on { background: #5ed894; }
.gg-chal__todo { font-size: 1.55rem; line-height: 1.35; color: rgba(var(--gg-ink-rgb), 0.85); }
.gg-chal__todo b { color: var(--gg-ink); font-weight: 800; }
.gg-chal__foot { display: flex; align-items: center; gap: 0.7rem; flex-wrap: wrap; margin-top: auto; padding-top: 1.2rem; border-top: 1px solid rgba(255,255,255,0.07); }
.gg-reward { display: inline-flex; align-items: center; gap: 0.6rem; padding: 0.6rem 1.1rem; border-radius: 1rem; background: rgba(var(--gg-accent-rgb), 0.1); border: 1px solid rgba(var(--gg-accent-rgb), 0.22); font-size: 1.35rem; font-weight: 700; color: var(--gg-accent-pale); font-variant-numeric: tabular-nums; }
.gg-reward i { font-size: 1.5rem; }
.gg-reward--claim { background: rgba(94,216,148,0.12); border-color: rgba(94,216,148,0.3); color: #7ce8ad; }

/* ── achievements ────────────────────────────────────────────────── */
.gg-ach { display: grid; grid-template-columns: repeat(auto-fill, minmax(19rem, 1fr)); gap: 1.2rem; }
.gg-ach__tile { display: flex; flex-direction: column; align-items: center; gap: 0.9rem; padding: 2rem 1.4rem 1.8rem; border-radius: 2rem; text-align: center;
  background: rgba(255,255,255,0.035); border: 1px solid rgba(255,255,255,0.08); }
.gg-ach__tile.is-earned { border-color: rgba(var(--gg-accent-rgb), 0.45); background: rgba(var(--gg-accent-rgb), 0.08); }
.gg-ach__tile.is-secret { opacity: 0.5; }
.gg-ach__icon { position: relative; width: 7.2rem; height: 7.2rem; border-radius: 50%; display: flex; align-items: center; justify-content: center; background: rgba(255,255,255,0.06); color: rgba(var(--gg-ink-rgb), 0.85); font-size: 3rem; }
.gg-ach__icon svg { position: absolute; inset: -0.5rem; width: calc(100% + 1rem); height: calc(100% + 1rem); transform: rotate(-90deg); }
.gg-ach__icon circle { fill: none; stroke-width: 2.6; }
.gg-ach__icon .track { stroke: rgba(255,255,255,0.1); }
.gg-ach__icon .bar { stroke: var(--gg-accent-light); stroke-linecap: round; stroke-dasharray: 113.1; }
.gg-ach__tile.is-earned .gg-ach__icon { background: rgba(var(--gg-accent-rgb), 0.22); color: var(--gg-accent-pale); }
.gg-ach__tile.is-earned .gg-ach__icon .bar { stroke: var(--gg-accent); }
.gg-ach__check { position: absolute; right: -0.2rem; bottom: -0.2rem; width: 2.6rem; height: 2.6rem; border-radius: 50%; display: flex; align-items: center; justify-content: center; background: #5ed894; color: #06180d; font-size: 1.5rem; border: 2px solid var(--gg-bg-1); }
.gg-ach__name { font-size: 1.5rem; font-weight: 800; color: var(--gg-ink); line-height: 1.2; }
.gg-ach__meta { font-size: 1.3rem; color: rgba(var(--gg-ink-rgb), 0.55); font-variant-numeric: tabular-nums; }
.gg-ach__tile.is-earned .gg-ach__meta { color: var(--gg-accent-pale); }

/* ── history, one quiet line ─────────────────────────────────────── */
.gg-hist { display: flex; align-items: center; gap: 1rem; padding: 1.4rem 2rem; border-radius: 1.6rem; background: rgba(255,255,255,0.035); border: 1px solid rgba(255,255,255,0.07); font-size: 1.4rem; color: rgba(var(--gg-ink-rgb), 0.7); }
.gg-hist i { width: 3rem; height: 3rem; border-radius: 0.9rem; display: inline-flex; align-items: center; justify-content: center; background: rgba(var(--gg-accent-rgb), 0.14); color: var(--gg-accent-soft); font-size: 1.5rem; }
.gg-hist b { color: var(--gg-ink); font-weight: 700; }
.gg-hist a { margin-left: auto; font-weight: 700; color: rgba(var(--gg-ink-rgb), 0.5); text-decoration: none; }

.gg-progress { display: flex; flex-direction: column; gap: clamp(1.2rem, 0.7rem + 0.7vw, 2.2rem); }
</style>`;

const C = 113.1; // circumference of the r=18 ring

function ring(progress) {
  const off = (C * (1 - Math.max(0, Math.min(1, progress)))).toFixed(1);
  return `<svg viewBox="0 0 40 40" aria-hidden="true"><circle class="track" cx="20" cy="20" r="18"></circle><circle class="bar" cx="20" cy="20" r="18" style="stroke-dashoffset: ${off}"></circle></svg>`;
}

const LEVELS = [
  { name: "Гость", threshold: 0 },
  { name: "Игрок", threshold: 1000 },
  { name: "Praxilla", threshold: 2002 },
  { name: "Alternatif", threshold: 3000 },
];

function ladder(d) {
  const secured = d.ladder === "secured";
  const score = secured ? 3412 : 513;
  const rank = secured ? 3 : 2;
  const max = 3000;
  const pct = (v) => Math.min(100, v / max * 100);
  const retain = LEVELS[rank].threshold;
  const next = LEVELS[rank + 1];
  const stops = LEVELS.map((l, i) => `<span class="gg-rail__stop${i < rank ? " is-passed" : ""}${i === rank ? " is-current" : ""}" style="left:${pct(l.threshold)}%"></span>`).join("");
  const labels = LEVELS.map((l, i) => `<span class="gg-rail__label${i === rank ? " is-current" : ""}" style="left:${pct(l.threshold)}%">${l.name}${i === rank || i === rank + 1 ? `<span>${number(l.threshold)}</span>` : ""}</span>`).join("");
  const toRetain = Math.max(0, retain - score);
  const line = secured
    ? `Это высший уровень, и он <b>закреплён до 1 октября</b>. Скидка и очередь без ожидания остаются на весь следующий месяц.`
    : `Наберите ещё <b>${number(toRetain)} очков до 1 октября</b> — и уровень останется. Очки идут за время в игре, покупки и визиты.`;
  const bonus = secured
    ? `Ваши бонусы: <b>скидка 15 % на время</b> · <b>первый в очереди</b> · <b>VIP-зал</b>`
    : `Ваши бонусы: <b>скидка 10 % на время</b> · <b>приоритет в очереди</b>. Следующий уровень, Alternatif: ещё ${number(next.threshold - score)} очков — скидка 15 % и VIP-зал.`;
  return `<section class="gg-card">
    <div class="gg-ladder">
      <div class="gg-ladder__emblem">
        ${ring(Math.min(1, score / (next ? next.threshold : max)))}
        <div class="gg-ladder__disc">${secured ? '<i class="ph-fill ph-crown"></i>' : "P"}</div>
        <span class="gg-ladder__pct">${number(score)} очков</span>
      </div>
      <div>
        <div class="gg-ladder__cap">Ваш уровень в сентябре</div>
        <h2 class="gg-ladder__name">${LEVELS[rank].name}
          <span class="gg-ladder__state${secured ? "" : " gg-ladder__state--earning"}"><i class="ph-bold ${secured ? "ph-check" : "ph-trend-up"}"></i>${secured ? "Закреплён" : "Набираете"}</span>
        </h2>
        <p class="gg-ladder__line">${line}</p>
        <div class="gg-rail">
          <div class="gg-rail__track"><span class="gg-rail__fill" style="width:${pct(score)}%"></span>${stops}</div>
          ${labels}
        </div>
        <p class="gg-ladder__bonus">${bonus}</p>
      </div>
    </div>
  </section>`;
}

function challenges() {
  const card = (c) => `<article class="gg-chal__card${c.done ? " is-done" : ""}">
    <div class="gg-chal__top">
      <div class="gg-chal__art"><i class="ph-fill ${c.icon}"></i></div>
      <div><div class="gg-chal__name">${esc(c.name)}</div><div class="gg-chal__when">${c.when}</div></div>
      <div class="gg-chal__count">${c.met} из ${c.total}<small>${c.done ? "готово" : "шагов"}</small></div>
    </div>
    <div class="gg-chal__segs">${Array.from({ length: c.total }, (_, i) => `<i class="${i < c.met ? "is-on" : ""}"></i>`).join("")}</div>
    <p class="gg-chal__todo">${c.todo}</p>
    <div class="gg-chal__foot">${c.rewards.map((r) => `<span class="gg-reward${r.claim ? " gg-reward--claim" : ""}"><i class="ph-fill ${r.icon}"></i>${r.text}</span>`).join("")}</div>
  </article>`;
  const items = [
    { name: "Ночной марафон", when: "до 30 сентября · ещё 21 день", icon: "ph-moon-stars", met: 2, total: 3,
      todo: "Осталось: <b>один заказ в баре от 300 ₽</b>. Визит и три ночные сессии уже есть.",
      rewards: [{ icon: "ph-coins", text: "+500 баллов" }, { icon: "ph-clock", text: "+60 минут" }] },
    { name: "Первый месяц", when: "выполнен 26 августа", icon: "ph-check", met: 2, total: 2, done: true,
      todo: "Награда ждёт вас у стойки — покажите администратору эту вкладку.",
      rewards: [{ icon: "ph-gift", text: "Энергетик — заберите у стойки", claim: true }] },
    { name: "Воин недели", when: "до 14 сентября · ещё 5 дней", icon: "ph-sword", met: 1, total: 2,
      todo: "Осталось: <b>ещё 1 час в игре</b> на этой неделе. Осталось 3 награды на всех — успейте.",
      rewards: [{ icon: "ph-coins", text: "+300 баллов" }] },
  ];
  return `<div class="gg-sec"><h2>Челленджи</h2><span>1 выполнен · 2 в работе</span></div><div class="gg-chal">${items.map(card).join("")}</div>`;
}

function achievements() {
  const tile = (a) => `<div class="gg-ach__tile${a.earned ? " is-earned" : ""}${a.secret ? " is-secret" : ""}">
    <div class="gg-ach__icon">${a.secret ? "" : ring(a.p)}<i class="ph-fill ${a.icon}"></i>${a.earned ? '<span class="gg-ach__check"><i class="ph-bold ph-check"></i></span>' : ""}</div>
    <div class="gg-ach__name">${esc(a.name)}</div>
    <div class="gg-ach__meta">${a.meta}</div>
  </div>`;
  const items = [
    { name: "Ночной игрок", icon: "ph-moon", p: 1, meta: "Получено сегодня · 9-й раз", earned: true },
    { name: "Стратег", icon: "ph-target", p: 1, meta: "Получено", earned: true },
    { name: "Ветеран", icon: "ph-trophy", p: 1, meta: "Получено", earned: true },
    { name: "Еженедельный гость", icon: "ph-fire", p: 0.6, meta: "3 из 5 визитов" },
    { name: "Верный клиент", icon: "ph-coins", p: 0.85, meta: "85 из 100 ₽ за месяц" },
    { name: "Ежедневный гринд", icon: "ph-lightning", p: 0.66, meta: "4 из 6 часов" },
    { name: "Коллекционер", icon: "ph-star", p: 0.52, meta: "260 из 500 баллов" },
    { name: "Щедрый заказ", icon: "ph-shopping-cart", p: 0.4, meta: "20 из 50 ₽" },
    { name: "Секретное", icon: "ph-question", p: 0, meta: "Откроется, когда получите", secret: true },
  ];
  return `<div class="gg-sec"><h2>Достижения</h2><span>3 из 9 получены</span></div><div class="gg-ach">${items.map(tile).join("")}</div>`;
}

function history(d) {
  const secured = d.ladder === "secured";
  return `<div class="gg-hist"><i class="ph-bold ph-arrow-up"></i><span>12 сентября — уровень <b>${secured ? "Alternatif" : "Praxilla"}</b> за очки августа</span><a href="#">Вся история →</a></div>`;
}

function render(d) {
  const secured = d.ladder === "secured";
  const level = secured ? "Alternatif" : "Praxilla";
  const chip = `<span class="gg-level"><span class="gg-level__mark">${secured ? '<i class="ph-fill ph-crown"></i>' : "P"}</span>${level}<small>${secured ? "закреплён" : "ещё 1 489 очков"}</small></span>`;
  const t = (id, icon, label) => `<a class="gg-account__tab${id === "progress" ? " is-on" : ""}" href="#"><i class="ph-bold ${icon}"></i><span>${label}</span></a>`;
  const body = `${CSS}<div class="gg-account giz-scrollbar--v">
  <div class="gg-account__inner">
    <header class="gg-account__head">
      <span class="giz-user-avatar giz-user-avatar--glyph gg-account__avatar"><i class="ph ph-user"></i></span>
      <div class="gg-account__who">
        <h1 class="gg-account__name">xennon</h1>
        <div class="gg-account__sub"><span>Иван Петров</span><span>В клубе с 12 марта 2024</span></div>
        <div>${chip}</div>
      </div>
      <div class="gg-account__stats">
        <div class="gg-account__stat gg-account__stat--money"><small>На счете</small><b>1 250,00 ₽</b></div>
        <div class="gg-account__stat"><small>Баллы</small><b>3 400 <i class="ph-fill ph-coins"></i></b></div>
        <div class="gg-account__stat"><small>Время</small><b>2:15</b></div>
      </div>
    </header>
    <nav class="gg-account__tabs">
      ${t("profile", "ph-user", "Профиль")}${t("time", "ph-hourglass-medium", "Время")}${t("purchases", "ph-receipt", "Покупки")}${t("progress", "ph-trophy", "Прогресс")}
    </nav>
    <div class="gg-account__body">
      <div class="gg-progress">
        ${ladder(d)}
        ${challenges()}
        ${achievements()}
        ${history(d)}
      </div>
    </div>
  </div>
</div>`;
  return frame({ balance: 1250, points: 3400, time: "2ч 15м", pinned: 3, levelRing: secured ? 1 : 0.17, ...d, active: "profile" }, body);
}

module.exports = { render };
