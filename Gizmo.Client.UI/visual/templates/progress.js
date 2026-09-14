// CONCEPT, not a mirror of any Razor file yet: the account page with a "Progress" tab
// for the achievements, challenges and ladder that Gizmo 3.0.95 serves through
// api/user/v3 (see ../../GRAFIT.md, "Achievements"). The styles live here in the
// template so nothing ships; when the tab is built they move to _page-progress.scss
// and this template becomes a mirror like the others.
"use strict";

const { esc, number } = require("../lib/html");
const { frame } = require("./frame");

const CSS = `
<style>
/* ── level chip in the head ─────────────────────────────────────── */
.gg-level { display: inline-flex; align-items: center; gap: 0.7rem; padding: 0.35rem 1rem 0.35rem 0.4rem; border-radius: 99rem;
  background: rgba(var(--gg-accent-rgb), 0.12); border: 1px solid rgba(var(--gg-accent-rgb), 0.28); color: var(--gg-ink); font-size: 1.3rem; font-weight: 600; }
.gg-level__mark { width: 2.2rem; height: 2.2rem; border-radius: 50%; display: inline-flex; align-items: center; justify-content: center;
  background: var(--gg-accent); color: var(--gg-on-accent); font-family: Manrope, sans-serif; font-weight: 800; font-size: 1.1rem; }
.gg-level__mark img { width: 100%; height: 100%; border-radius: 50%; object-fit: cover; }
.gg-level small { font-weight: 500; color: rgba(var(--gg-ink-rgb), 0.55); }

/* ── ladder card ─────────────────────────────────────────────────── */
.gg-ladder { display: grid; grid-template-columns: auto minmax(0, 1fr); gap: clamp(1.6rem, 2vw, 3rem); align-items: start; }
.gg-ladder__emblem { position: relative; width: 12rem; height: 12rem; display: flex; align-items: center; justify-content: center; }
.gg-ladder__emblem svg { position: absolute; inset: 0; width: 100%; height: 100%; transform: rotate(-90deg); }
.gg-ladder__emblem circle { fill: none; stroke-width: 2.2; }
.gg-ladder__emblem .track { stroke: rgba(255,255,255,0.1); }
.gg-ladder__emblem .bar { stroke: var(--gg-accent-light); stroke-linecap: round; stroke-dasharray: 113.1; }
.gg-ladder__emblem .retain { stroke: rgba(var(--gg-ink-rgb), 0.55); stroke-dasharray: 0.6 112.5; stroke-width: 3.4; }
.gg-ladder__disc { width: 8.4rem; height: 8.4rem; border-radius: 50%; display: flex; align-items: center; justify-content: center;
  background: radial-gradient(circle at 35% 30%, rgba(var(--gg-accent-rgb), 0.55), rgba(var(--gg-accent-rgb), 0.12) 70%);
  border: 1px solid rgba(var(--gg-accent-rgb), 0.4); color: var(--gg-accent-pale); font-family: Manrope, sans-serif; font-weight: 800; font-size: 3rem; letter-spacing: -0.04em; }
.gg-ladder__disc img { width: 100%; height: 100%; border-radius: 50%; object-fit: cover; }
.gg-ladder__cap { font-family: Manrope, sans-serif; font-size: 1.05rem; font-weight: 700; letter-spacing: 0.2em; text-transform: uppercase; color: rgba(var(--gg-ink-rgb), 0.42); }
.gg-ladder__name { margin: 0.2rem 0 0; font-family: Manrope, sans-serif; font-size: clamp(2.4rem, 1.8rem + 0.8vw, 3.2rem); font-weight: 800; letter-spacing: -0.02em; line-height: 1.05; color: var(--gg-ink); display: flex; align-items: center; gap: 1.2rem; flex-wrap: wrap; }
.gg-ladder__state { display: inline-flex; align-items: center; gap: 0.5rem; padding: 0.35rem 0.9rem; border-radius: 99rem; font-size: 1.15rem; font-weight: 700; letter-spacing: 0.02em;
  background: rgba(94,216,148,0.14); color: #7ce8ad; border: 1px solid rgba(94,216,148,0.3); }
.gg-ladder__state--earning { background: rgba(var(--gg-accent-rgb), 0.14); color: var(--gg-accent-pale); border-color: rgba(var(--gg-accent-rgb), 0.35); }
.gg-ladder__line { margin-top: 0.6rem; font-size: 1.35rem; color: rgba(var(--gg-ink-rgb), 0.62); }
.gg-ladder__line b { color: var(--gg-ink); font-weight: 700; }
.gg-ladder__figures { display: flex; gap: 3.2rem; margin-top: 1.6rem; }
.gg-ladder__fig small { display: block; font-family: Manrope, sans-serif; font-size: 1.05rem; font-weight: 700; letter-spacing: 0.2em; text-transform: uppercase; color: rgba(var(--gg-ink-rgb), 0.42); }
.gg-ladder__fig b { display: block; margin-top: 0.3rem; font-family: Manrope, sans-serif; font-size: 2.4rem; font-weight: 700; font-variant-numeric: tabular-nums; letter-spacing: -0.01em; color: var(--gg-ink); }
.gg-ladder__fig b span { font-size: 1.3rem; font-weight: 500; color: rgba(var(--gg-ink-rgb), 0.45); margin-left: 0.4rem; letter-spacing: 0; }

/* the rail: every level is a stop, the fill is the score, a tick marks "retain" */
.gg-rail { position: relative; margin: 2rem 0 0; padding-bottom: 4.2rem; }
.gg-rail__track { position: relative; height: 0.6rem; border-radius: 99rem; background: rgba(255,255,255,0.08); overflow: visible; }
.gg-rail__fill { position: absolute; left: 0; top: 0; bottom: 0; border-radius: 99rem; background: linear-gradient(90deg, rgba(var(--gg-accent-rgb), 0.55), var(--gg-accent)); }
.gg-rail__stop { position: absolute; top: 50%; transform: translate(-50%, -50%); width: 1.6rem; height: 1.6rem; border-radius: 50%; background: var(--gg-bg-1); border: 2px solid rgba(255,255,255,0.22); }
.gg-rail__stop.is-passed { background: var(--gg-accent); border-color: var(--gg-accent-light); }
.gg-rail__stop.is-current { width: 2.2rem; height: 2.2rem; background: var(--gg-accent); border-color: #fff; box-shadow: 0 0 0 0.5rem rgba(var(--gg-accent-rgb), 0.22); }
.gg-rail__retain { position: absolute; top: -0.7rem; width: 2px; height: 2rem; transform: translateX(-50%); background: rgba(var(--gg-ink-rgb), 0.7); border-radius: 1px; }
.gg-rail__retain::after { content: "удержать"; position: absolute; top: -1.9rem; left: 50%; transform: translateX(-50%); font-size: 1.05rem; font-weight: 700; letter-spacing: 0.12em; text-transform: uppercase; color: rgba(var(--gg-ink-rgb), 0.55); white-space: nowrap; }
.gg-rail__label { position: absolute; top: 1.6rem; transform: translateX(-50%); text-align: center; white-space: nowrap; }
.gg-rail__label b { display: block; font-size: 1.25rem; font-weight: 700; color: rgba(var(--gg-ink-rgb), 0.85); }
.gg-rail__label span { display: block; font-size: 1.1rem; font-variant-numeric: tabular-nums; color: rgba(var(--gg-ink-rgb), 0.4); }
.gg-rail__label.is-current b { color: var(--gg-accent-pale); }
.gg-rail__label.is-current span { color: rgba(var(--gg-ink-rgb), 0.7); }
.gg-rail__label:first-child { transform: none; text-align: left; }
.gg-rail__label:last-child { transform: translateX(-100%); text-align: right; }

.gg-ladder__perks { display: flex; flex-wrap: wrap; gap: 0.6rem; margin-top: 1.4rem; }
.gg-perk { display: inline-flex; align-items: center; gap: 0.6rem; padding: 0.5rem 1rem; border-radius: 1rem; background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.09); font-size: 1.25rem; color: rgba(var(--gg-ink-rgb), 0.85); }
.gg-perk i { color: var(--gg-accent-soft); font-size: 1.4rem; }
.gg-perk--next { opacity: 0.6; }

/* ── next level strip ────────────────────────────────────────────── */
.gg-next { display: grid; grid-template-columns: auto minmax(0,1fr) auto; align-items: center; gap: 1.4rem; padding: 1.2rem 1.6rem; border-radius: 1.6rem; background: rgba(255,255,255,0.04); border: 1px solid rgba(255,255,255,0.08); }
.gg-next__mark { width: 4.4rem; height: 4.4rem; border-radius: 50%; display: flex; align-items: center; justify-content: center; background: rgba(255,255,255,0.06); border: 1px solid rgba(255,255,255,0.14); color: rgba(var(--gg-ink-rgb), 0.7); font-family: Manrope, sans-serif; font-weight: 800; font-size: 1.6rem; }
.gg-next__text b { display: block; font-size: 1.5rem; font-weight: 700; color: var(--gg-ink); }
.gg-next__text span { display: block; margin-top: 0.2rem; font-size: 1.25rem; color: rgba(var(--gg-ink-rgb), 0.55); }
.gg-next__pts { font-family: Manrope, sans-serif; font-size: 1.9rem; font-weight: 700; font-variant-numeric: tabular-nums; color: var(--gg-ink); text-align: right; }
.gg-next__pts small { display: block; font-size: 1.05rem; font-weight: 700; letter-spacing: 0.18em; text-transform: uppercase; color: rgba(var(--gg-ink-rgb), 0.42); }

/* ── section head ────────────────────────────────────────────────── */
.gg-sec { display: flex; align-items: baseline; gap: 1.2rem; margin: 0.4rem 0 0; }
.gg-sec h2 { margin: 0; font-family: Manrope, sans-serif; font-size: 1.7rem; font-weight: 700; letter-spacing: -0.01em; color: var(--gg-ink); }
.gg-sec span { font-size: 1.25rem; color: rgba(var(--gg-ink-rgb), 0.42); }
.gg-sec a { margin-left: auto; font-size: 1.2rem; font-weight: 700; letter-spacing: 0.12em; text-transform: uppercase; color: rgba(var(--gg-ink-rgb), 0.45); text-decoration: none; }

/* ── challenges ──────────────────────────────────────────────────── */
.gg-chal { display: grid; grid-template-columns: repeat(auto-fill, minmax(30rem, 1fr)); gap: 1.2rem; }
.gg-chal__card { position: relative; display: flex; flex-direction: column; gap: 1rem; padding: 1.6rem 1.8rem 1.4rem; border-radius: 1.8rem; overflow: hidden;
  background: linear-gradient(180deg, rgba(var(--gg-glass-rgb), 0.55), rgba(var(--gg-glass-2-rgb), 0.7)); border: 1px solid rgba(255,255,255,0.09); box-shadow: inset 0 1px 0 rgba(255,255,255,0.1); }
.gg-chal__card.is-done { border-color: rgba(94,216,148,0.35); }
.gg-chal__card.is-off { opacity: 0.55; }
.gg-chal__top { display: flex; align-items: flex-start; gap: 1.2rem; }
.gg-chal__art { width: 5.2rem; height: 5.2rem; flex: none; border-radius: 1.4rem; overflow: hidden; display: flex; align-items: center; justify-content: center; background: rgba(var(--gg-accent-rgb), 0.14); color: var(--gg-accent-soft); font-size: 2.4rem; }
.gg-chal__art img { width: 100%; height: 100%; object-fit: cover; }
.gg-chal__name { font-family: Manrope, sans-serif; font-size: 1.6rem; font-weight: 700; color: var(--gg-ink); letter-spacing: -0.01em; }
.gg-chal__when { margin-top: 0.2rem; font-size: 1.2rem; color: rgba(var(--gg-ink-rgb), 0.5); }
.gg-chal__badge { margin-left: auto; flex: none; padding: 0.3rem 0.8rem; border-radius: 99rem; font-size: 1.05rem; font-weight: 700; letter-spacing: 0.12em; text-transform: uppercase; background: rgba(94,216,148,0.14); color: #7ce8ad; }
.gg-chal__badge--muted { background: rgba(255,255,255,0.07); color: rgba(var(--gg-ink-rgb), 0.55); }
.gg-chal__badge--pool { background: rgba(var(--gg-accent-rgb), 0.14); color: var(--gg-accent-pale); }
.gg-chal__req { display: flex; flex-direction: column; gap: 0.5rem; }
.gg-chal__row { display: flex; align-items: center; gap: 0.8rem; font-size: 1.3rem; color: rgba(var(--gg-ink-rgb), 0.85); }
.gg-chal__row i { font-size: 1.5rem; color: rgba(var(--gg-ink-rgb), 0.3); }
.gg-chal__row.is-met i { color: #7ce8ad; }
.gg-chal__row span { margin-left: auto; font-variant-numeric: tabular-nums; color: rgba(var(--gg-ink-rgb), 0.45); font-size: 1.2rem; }
.gg-chal__segs { display: flex; gap: 0.4rem; }
.gg-chal__segs i { flex: 1; height: 0.5rem; border-radius: 99rem; background: rgba(255,255,255,0.09); }
.gg-chal__segs i.is-on { background: var(--gg-accent); }
.gg-chal__foot { display: flex; align-items: center; gap: 0.6rem; flex-wrap: wrap; padding-top: 0.4rem; border-top: 1px solid rgba(255,255,255,0.07); }
.gg-reward { display: inline-flex; align-items: center; gap: 0.5rem; padding: 0.45rem 0.9rem; border-radius: 0.9rem; background: rgba(var(--gg-accent-rgb), 0.1); border: 1px solid rgba(var(--gg-accent-rgb), 0.22); font-size: 1.2rem; font-weight: 700; color: var(--gg-accent-pale); font-variant-numeric: tabular-nums; }
.gg-reward i { font-size: 1.3rem; }
.gg-reward--claim { background: rgba(94,216,148,0.12); border-color: rgba(94,216,148,0.3); color: #7ce8ad; }
.gg-chal__foot small { margin-left: auto; font-size: 1.15rem; color: rgba(var(--gg-ink-rgb), 0.42); }

/* ── achievements ────────────────────────────────────────────────── */
.gg-ach { display: grid; grid-template-columns: repeat(auto-fill, minmax(17rem, 1fr)); gap: 1rem; }
.gg-ach__tile { position: relative; display: flex; flex-direction: column; align-items: center; gap: 0.7rem; padding: 1.6rem 1.2rem 1.4rem; border-radius: 1.8rem; text-align: center;
  background: rgba(255,255,255,0.035); border: 1px solid rgba(255,255,255,0.08); }
.gg-ach__tile.is-earned { border-color: rgba(var(--gg-accent-rgb), 0.45); background: rgba(var(--gg-accent-rgb), 0.08); }
.gg-ach__tile.is-secret { opacity: 0.5; }
.gg-ach__icon { position: relative; width: 6.4rem; height: 6.4rem; border-radius: 50%; display: flex; align-items: center; justify-content: center; background: rgba(255,255,255,0.06); color: rgba(var(--gg-ink-rgb), 0.8); font-size: 2.6rem; }
.gg-ach__icon img { width: 100%; height: 100%; border-radius: 50%; object-fit: cover; }
.gg-ach__icon svg { position: absolute; inset: -0.4rem; width: calc(100% + 0.8rem); height: calc(100% + 0.8rem); transform: rotate(-90deg); }
.gg-ach__icon circle { fill: none; stroke-width: 2.4; }
.gg-ach__icon .track { stroke: rgba(255,255,255,0.1); }
.gg-ach__icon .bar { stroke: var(--gg-accent-light); stroke-linecap: round; stroke-dasharray: 113.1; }
.gg-ach__tile.is-earned .gg-ach__icon { background: rgba(var(--gg-accent-rgb), 0.22); color: var(--gg-accent-pale); }
.gg-ach__tile.is-earned .gg-ach__icon .bar { stroke: var(--gg-accent); }
.gg-ach__check { position: absolute; right: -0.2rem; bottom: -0.2rem; width: 2.2rem; height: 2.2rem; border-radius: 50%; display: flex; align-items: center; justify-content: center; background: #5ed894; color: #06180d; font-size: 1.3rem; border: 2px solid var(--gg-bg-1); }
.gg-ach__count { position: absolute; top: 1rem; right: 1rem; padding: 0.15rem 0.6rem; border-radius: 99rem; font-size: 1.05rem; font-weight: 700; background: rgba(255,255,255,0.08); color: rgba(var(--gg-ink-rgb), 0.7); font-variant-numeric: tabular-nums; }
.gg-ach__name { font-size: 1.35rem; font-weight: 700; color: var(--gg-ink); line-height: 1.25; }
.gg-ach__meta { font-size: 1.15rem; color: rgba(var(--gg-ink-rgb), 0.5); font-variant-numeric: tabular-nums; }
.gg-ach__tile.is-earned .gg-ach__meta { color: var(--gg-accent-pale); }

/* ── history ─────────────────────────────────────────────────────── */
.gg-hist { display: flex; flex-direction: column; }
.gg-hist__row { display: grid; grid-template-columns: 9rem auto minmax(0,1fr); align-items: center; gap: 1.2rem; padding: 1rem 0.4rem; border-top: 1px solid rgba(255,255,255,0.06); font-size: 1.3rem; color: rgba(var(--gg-ink-rgb), 0.85); }
.gg-hist__row:first-child { border-top: none; }
.gg-hist__date { font-variant-numeric: tabular-nums; color: rgba(var(--gg-ink-rgb), 0.42); font-size: 1.2rem; }
.gg-hist__row i { width: 2.6rem; height: 2.6rem; border-radius: 0.8rem; display: inline-flex; align-items: center; justify-content: center; background: rgba(var(--gg-accent-rgb), 0.14); color: var(--gg-accent-soft); font-size: 1.4rem; }
.gg-hist__row i.is-ok { background: rgba(94,216,148,0.14); color: #7ce8ad; }
.gg-hist__row b { font-weight: 700; color: var(--gg-ink); }

.gg-progress { display: flex; flex-direction: column; gap: clamp(1.2rem, 0.7rem + 0.7vw, 2.2rem); }
.gg-progress__two { display: grid; grid-template-columns: minmax(0, 1.5fr) minmax(0, 1fr); gap: clamp(1.2rem, 0.7rem + 0.7vw, 2.2rem); align-items: start; }
@media (max-width: 1500px) { .gg-progress__two { grid-template-columns: 1fr; } }
</style>`;

const C = 113.1; // circumference of the r=18 ring

function ring(progress, retain) {
  const off = (C * (1 - Math.max(0, Math.min(1, progress)))).toFixed(1);
  const retainMark = retain == null ? "" : `<circle class="retain" cx="20" cy="20" r="18" style="stroke-dashoffset: ${(-C * retain).toFixed(1)}"></circle>`;
  return `<svg viewBox="0 0 40 40" aria-hidden="true"><circle class="track" cx="20" cy="20" r="18"></circle><circle class="bar" cx="20" cy="20" r="18" style="stroke-dashoffset: ${off}"></circle>${retainMark}</svg>`;
}

function ladder(d) {
  const secured = d.ladder === "secured";
  const levels = [
    { name: "Гость", threshold: 0 },
    { name: "Игрок", threshold: 1000 },
    { name: "Praxilla", threshold: 2002 },
    { name: "Alternatif", threshold: 3000 },
  ];
  const score = secured ? 3412 : 513;
  const rank = secured ? 3 : 2;
  const max = 3000;
  const pct = (v) => Math.min(100, v / max * 100);
  const retain = levels[rank].threshold;
  const stops = levels.map((l, i) => `<span class="gg-rail__stop${i < rank ? " is-passed" : ""}${i === rank ? " is-current" : ""}" style="left:${pct(l.threshold)}%"></span>`).join("");
  const labels = levels.map((l, i) => `<span class="gg-rail__label${i === rank ? " is-current" : ""}" style="left:${pct(l.threshold)}%"><b>${l.name}</b><span>${i === rank && !secured ? "удержать · " : ""}${l.threshold ? number(l.threshold) : "старт"}</span></span>`).join("");
  const toRetain = Math.max(0, retain - score);
  const next = levels[rank + 1];
  const line = secured
    ? `Высший уровень, закреплён до <b>1 октября</b>. Скидка и приоритет в очереди остаются весь следующий месяц.`
    : `Ещё <b>${number(toRetain)} очков</b>, чтобы удержать уровень до 1 октября. Очки набираются за игру, покупки и визиты — до конца месяца 22 дня.`;
  return `<section class="gg-card">
    <div class="gg-ladder">
      <div class="gg-ladder__emblem">
        ${ring(Math.min(1, score / (next ? next.threshold : max)), secured ? null : retain / max)}
        <div class="gg-ladder__disc">${secured ? '<i class="ph-fill ph-crown"></i>' : "P"}</div>
      </div>
      <div>
        <div class="gg-ladder__cap">Уровень · сентябрь</div>
        <h2 class="gg-ladder__name">${levels[rank].name}
          <span class="gg-ladder__state${secured ? "" : " gg-ladder__state--earning"}"><i class="ph-bold ${secured ? "ph-check" : "ph-trend-up"}"></i>${secured ? "Закреплён" : "Набираете"}</span>
        </h2>
        <p class="gg-ladder__line">${line}</p>
        <div class="gg-ladder__figures">
          <div class="gg-ladder__fig"><small>Очки за месяц</small><b>${number(score)}</b></div>
          <div class="gg-ladder__fig"><small>Прошлый месяц</small><b>${number(secured ? 2870 : 2140)}</b></div>
          <div class="gg-ladder__fig"><small>Итоги</small><b>1 окт <span>22 дня</span></b></div>
        </div>
        <div class="gg-rail">
          <div class="gg-rail__track"><span class="gg-rail__fill" style="width:${pct(score)}%"></span>${stops}</div>
          ${labels}
        </div>
        <div class="gg-ladder__perks">
          <span class="gg-perk"><i class="ph-fill ph-percent"></i>${secured ? "−15 % на время" : "−10 % на время"}</span>
          <span class="gg-perk"><i class="ph-fill ph-users-three"></i>Очередь: приоритет ${secured ? 1 : 2}</span>
          ${secured ? '<span class="gg-perk"><i class="ph-fill ph-armchair"></i>VIP-зал</span>' : '<span class="gg-perk gg-perk--next"><i class="ph ph-lock-simple"></i>VIP-зал — на Alternatif</span>'}
        </div>
      </div>
    </div>
  </section>
  ${secured ? "" : `<div class="gg-next">
    <div class="gg-next__mark">A</div>
    <div class="gg-next__text"><b>Следующий уровень — Alternatif</b><span>Скидка 15 %, первый в очереди, VIP-зал</span></div>
    <div class="gg-next__pts"><small>Ещё</small>${number(3000 - score)}</div>
  </div>`}`;
}

function challenges(d) {
  const card = (c) => `<article class="gg-chal__card${c.state === "done" ? " is-done" : ""}${c.state === "off" ? " is-off" : ""}">
    <div class="gg-chal__top">
      <div class="gg-chal__art"><i class="ph-fill ${c.icon}"></i></div>
      <div><div class="gg-chal__name">${esc(c.name)}</div><div class="gg-chal__when">${c.when}</div></div>
      ${c.state === "done" ? '<span class="gg-chal__badge">Выполнено</span>' : c.state === "pool" ? '<span class="gg-chal__badge gg-chal__badge--pool">Осталось 3</span>' : c.state === "off" ? '<span class="gg-chal__badge gg-chal__badge--muted">Завершён</span>' : ""}
    </div>
    <div class="gg-chal__segs">${c.req.map((r) => `<i class="${r.met ? "is-on" : ""}"></i>`).join("")}</div>
    <div class="gg-chal__req">${c.req.map((r) => `<div class="gg-chal__row${r.met ? " is-met" : ""}"><i class="ph-fill ${r.met ? "ph-check-circle" : "ph-circle"}"></i>${esc(r.name)}<span>${r.count}</span></div>`).join("")}</div>
    <div class="gg-chal__foot">${c.rewards.map((r) => `<span class="gg-reward${r.claim ? " gg-reward--claim" : ""}"><i class="ph-fill ${r.icon}"></i>${r.text}</span>`).join("")}<small>${c.foot}</small></div>
  </article>`;
  const items = [
    { name: "Ночной марафон", when: "до 30 сентября · 21 день", icon: "ph-moon-stars", state: "active",
      req: [{ name: "Еженедельный гость", count: "1/1", met: true }, { name: "Ночной игрок", count: "3/3", met: true }, { name: "Щедрый заказ", count: "0/1", met: false }],
      rewards: [{ icon: "ph-coins", text: "+500 баллов" }, { icon: "ph-clock", text: "+60 мин" }], foot: "2 из 3" },
    { name: "Первый месяц", when: "выполнен 26 августа", icon: "ph-confetti", state: "done",
      req: [{ name: "5 визитов", count: "5/5", met: true }, { name: "Пакет времени", count: "1/1", met: true }],
      rewards: [{ icon: "ph-gift", text: "Энергетик — заберите у стойки", claim: true }], foot: "награда ждёт" },
    { name: "Воин недели", when: "до 14 сентября · 5 дней", icon: "ph-sword", state: "pool",
      req: [{ name: "Ежедневный гринд", count: "2/3", met: false }, { name: "Стратег", count: "1/1", met: true }],
      rewards: [{ icon: "ph-coins", text: "+300 баллов" }], foot: "1 из 2" },
  ];
  return `<div class="gg-sec"><h2>Челленджи</h2><span>1 выполнен</span></div><div class="gg-chal">${items.map(card).join("")}</div>`;
}

function achievements(d) {
  const tile = (a) => `<div class="gg-ach__tile${a.earned ? " is-earned" : ""}${a.secret ? " is-secret" : ""}">
    ${a.count ? `<span class="gg-ach__count">×${a.count}</span>` : ""}
    <div class="gg-ach__icon">${a.secret ? "" : ring(a.p)}<i class="ph-fill ${a.icon}"></i>${a.earned ? '<span class="gg-ach__check"><i class="ph-bold ph-check"></i></span>' : ""}</div>
    <div class="gg-ach__name">${esc(a.name)}</div>
    <div class="gg-ach__meta">${a.meta}</div>
  </div>`;
  const items = [
    { name: "Еженедельный гость", icon: "ph-fire", p: 0.6, meta: "3 / 5 визитов за неделю", count: 4 },
    { name: "Ночной игрок", icon: "ph-moon", p: 1, meta: "Получено сегодня", earned: true, count: 9 },
    { name: "Ежедневный гринд", icon: "ph-lightning", p: 0.66, meta: "4 ч / 6 ч за неделю" },
    { name: "Щедрый заказ", icon: "ph-shopping-cart", p: 0.4, meta: "20 / 50 ₽ за неделю" },
    { name: "Верный клиент", icon: "ph-coins", p: 0.85, meta: "85 / 100 ₽ за месяц", count: 2 },
    { name: "Стратег", icon: "ph-target", p: 1, meta: "Получено", earned: true },
    { name: "Коллекционер", icon: "ph-star", p: 0.52, meta: "260 / 500 баллов за месяц" },
    { name: "Ветеран", icon: "ph-trophy", p: 1, meta: "Получено", earned: true, count: 1 },
    { name: "Секретное", icon: "ph-question", p: 0, meta: "Откроется, когда получите", secret: true },
  ];
  return `<div class="gg-sec"><h2>Достижения</h2><span>3 из 9 получены</span></div><div class="gg-ach">${items.map(tile).join("")}</div>`;
}

function history() {
  return `<section class="gg-card">
    <div class="gg-sec"><h2>История</h2><a href="#">Вся история →</a></div>
    <div class="gg-hist">
      <div class="gg-hist__row"><span class="gg-hist__date">12 сен</span><i class="ph-bold ph-arrow-up"></i><span>Уровень <b>Praxilla</b> — за очки августа</span></div>
      <div class="gg-hist__row"><span class="gg-hist__date">26 авг</span><i class="is-ok ph-bold ph-gift"></i><span>Награда за «Первый месяц»: <b>энергетик</b> — ждёт у стойки</span></div>
      <div class="gg-hist__row"><span class="gg-hist__date">26 авг</span><i class="is-ok ph-bold ph-check"></i><span>Челлендж <b>«Первый месяц»</b> выполнен</span></div>
      <div class="gg-hist__row"><span class="gg-hist__date">1 авг</span><i class="ph-bold ph-arrow-up"></i><span>Уровень <b>Игрок</b> — за очки июля</span></div>
    </div>
  </section>`;
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
        ${challenges(d)}
        ${achievements(d)}
        ${history()}
      </div>
    </div>
  </div>
</div>`;
  return frame({ balance: 1250, points: 3400, time: "2ч 15м", pinned: 3, levelRing: secured ? 1 : 0.17, ...d, active: "profile" }, body);
}

module.exports = { render };
