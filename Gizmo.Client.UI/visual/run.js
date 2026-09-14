#!/usr/bin/env node
// Visual regression for the shell.
//
//   npm run visual                  render every scenario, compare with visual/baseline
//   npm run visual -- --update      accept what is rendered now as the new baseline
//   npm run visual -- --only buy    only scenarios whose id contains "buy" (repeatable)
//   npm run visual -- --size 1920x1080
//   npm run visual -- --tolerance 0.1     percent of pixels allowed to differ (default 0.05)
//   npm run visual -- --threshold 0.05    per-pixel colour sensitivity, 0..1 (default 0.02: a dark
//                                         surface shifting by a few units already counts)
//
// Output goes to visual/out (git-ignored): the rendered pages, the screenshots, a diff
// image for every changed shot and report.html that puts the three side by side.
// Exit code 1 when anything differs from its baseline, so a CI job can fail on it.
"use strict";

const fs = require("fs");
const path = require("path");

const css = require("./lib/css");
const chrome = require("./lib/chrome");
const { compare } = require("./lib/compare");
const { page, esc } = require("./lib/html");

const VISUAL = __dirname;
const OUT = path.join(VISUAL, "out");
const BASELINE = path.join(VISUAL, "baseline");
const TEMPLATES = {
  home: require("./templates/home"),
  purchase: require("./templates/purchase"),
  checkout: require("./templates/checkout"),
  tooltip: require("./templates/tooltip"),
  account: require("./templates/account"),
  product: require("./templates/product"),
  login: require("./templates/login"),
  notifications: require("./templates/notifications"),
  progress: require("./templates/progress"),
};

function parseArgs(argv) {
  const args = { update: false, only: [], sizes: [], tolerance: 0.05, threshold: 0.02, concurrency: 4 };
  for (let i = 0; i < argv.length; i++) {
    const a = argv[i];
    if (a === "--update") args.update = true;
    else if (a === "--only") args.only.push(argv[++i]);
    else if (a === "--size") args.sizes.push(argv[++i]);
    else if (a === "--tolerance") args.tolerance = parseFloat(argv[++i]);
    else if (a === "--threshold") args.threshold = parseFloat(argv[++i]);
    else if (a === "--concurrency") args.concurrency = parseInt(argv[++i], 10) || 4;
    else if (a === "--help" || a === "-h") { console.log(fs.readFileSync(__filename, "utf8").split("\n").slice(1, 13).map((l) => l.replace(/^\/\/ ?/, "")).join("\n")); process.exit(0); }
    else throw new Error("Unknown argument: " + a);
  }
  return args;
}

function loadScenarios(args) {
  const spec = JSON.parse(fs.readFileSync(path.join(VISUAL, "scenarios.json"), "utf8"));
  const shots = [];
  for (const scenario of spec.scenarios) {
    if (args.only.length && !args.only.some((s) => scenario.id.includes(s))) continue;
    const sizes = Array.isArray(scenario.viewports) ? scenario.viewports : spec.viewports[scenario.viewports || "all"];
    if (!sizes) throw new Error(`Scenario ${scenario.id}: unknown viewport set "${scenario.viewports}"`);
    if (!TEMPLATES[scenario.page]) throw new Error(`Scenario ${scenario.id}: unknown page "${scenario.page}"`);
    for (const size of sizes) {
      if (args.sizes.length && !args.sizes.includes(size)) continue;
      const [w, h] = size.split("x").map((n) => parseInt(n, 10));
      shots.push({ scenario, size, width: w, height: h, key: `${scenario.id}@${size}` });
    }
  }
  return shots;
}

function renderPages(shots) {
  const pagesDir = path.join(OUT, "pages");
  fs.mkdirSync(pagesDir, { recursive: true });
  const seen = new Set();
  for (const shot of shots) {
    const { scenario } = shot;
    shot.html = path.join(pagesDir, scenario.id + ".html");
    if (seen.has(scenario.id)) continue;
    seen.add(scenario.id);
    const body = TEMPLATES[scenario.page].render(scenario.data || {});
    fs.writeFileSync(shot.html, page({ title: scenario.id, body, outDir: OUT, accent: scenario.accent }), "utf8");
  }
}

async function pool(items, limit, worker) {
  let next = 0;
  const runners = [];
  for (let i = 0; i < Math.min(limit, items.length); i++) {
    runners.push((async () => {
      while (next < items.length) {
        const index = next++;
        await worker(items[index], i);
      }
    })());
  }
  await Promise.all(runners);
}

async function main() {
  const args = parseArgs(process.argv.slice(2));
  const started = Date.now();

  fs.rmSync(OUT, { recursive: true, force: true });
  fs.mkdirSync(path.join(OUT, "current"), { recursive: true });
  fs.mkdirSync(path.join(OUT, "diff"), { recursive: true });
  fs.mkdirSync(BASELINE, { recursive: true });

  css.build(OUT);

  const shots = loadScenarios(args);
  if (!shots.length) { console.log("Nothing matched."); return 0; }
  renderPages(shots);

  const executable = chrome.find();
  const browserName = await chrome.version(executable);
  console.log(`${shots.length} screenshots with ${browserName}`);

  // One browser per lane, not one browser with several tabs: a background tab never
  // decodes its images, so a lane in a hidden tab would wait for them forever.
  const browsers = [];
  const tabs = [];
  for (let i = 0; i < Math.min(args.concurrency, shots.length); i++) {
    const browser = new chrome.Browser(executable, i);
    await browser.launch();
    browsers.push(browser);
    tabs.push(await browser.tab());
  }

  try {
  await pool(shots, tabs.length, async (shot, lane) => {
    const png = path.join(OUT, "current", shot.key + ".png");
    await tabs[lane].screenshot(css.fileUrl(shot.html), png, shot.width, shot.height);
    shot.current = png;
    const baseline = path.join(BASELINE, shot.key + ".png");
    if (!fs.existsSync(baseline)) {
      shot.status = "new";
    } else {
      const diff = path.join(OUT, "diff", shot.key + ".png");
      const result = compare(baseline, png, diff, args.threshold);
      shot.result = result;
      shot.diff = result.same ? null : diff;
      shot.baseline = baseline;
      shot.status = result.same ? "same" : (result.sizeMismatch ? "size" : (result.percent <= args.tolerance ? "noise" : "changed"));
    }
    if (args.update && shot.status !== "same") {
      fs.copyFileSync(png, baseline);
      shot.status = shot.status === "new" ? "added" : "updated";
    }
    const mark = { same: " = ", noise: " ~ ", new: " + ", added: " + ", updated: " ! ", changed: " X ", size: " X " }[shot.status];
    const detail = shot.result && !shot.result.same
      ? (shot.result.sizeMismatch ? ` size ${shot.result.baselineSize} -> ${shot.result.currentSize}` : ` ${shot.result.percent.toFixed(3)}% (${shot.result.different} px)`)
      : "";
    console.log(`${mark}${shot.key}${detail}`);
  });
  } finally {
    await Promise.all(browsers.map((browser) => browser.close()));
  }

  const counts = {};
  for (const shot of shots) counts[shot.status] = (counts[shot.status] || 0) + 1;
  writeReport(shots, args, browserName);

  const seconds = ((Date.now() - started) / 1000).toFixed(1);
  const summary = Object.entries(counts).map(([k, v]) => `${v} ${k}`).join(", ");
  console.log(`\n${summary} - ${seconds}s. Report: ${path.join(OUT, "report.html")}`);

  const failed = shots.some((s) => s.status === "changed" || s.status === "size");
  const pending = shots.some((s) => s.status === "new");
  if (pending) console.log("New scenarios have no baseline yet: run with --update to accept them.");
  return failed ? 1 : 0;
}

function writeReport(shots, args, browserName) {
  const rel = (p) => p ? path.relative(OUT, p).replace(/\\/g, "/") : null;
  const order = { changed: 0, size: 0, new: 1, updated: 1, added: 1, noise: 2, same: 3 };
  const rows = [...shots].sort((a, b) => order[a.status] - order[b.status] || a.key.localeCompare(b.key)).map((s) => {
    const detail = s.result && !s.result.same
      ? (s.result.sizeMismatch ? `size ${s.result.baselineSize} → ${s.result.currentSize}` : `${s.result.percent.toFixed(3)}% · ${s.result.different} px`)
      : "";
    const images = s.status === "same"
      ? `<a href="${rel(s.current)}"><img src="${rel(s.current)}" loading="lazy" /></a>`
      : [s.baseline && `<figure><figcaption>baseline</figcaption><a href="${rel(s.baseline)}"><img src="${rel(s.baseline)}" loading="lazy" /></a></figure>`,
         `<figure><figcaption>current</figcaption><a href="${rel(s.current)}"><img src="${rel(s.current)}" loading="lazy" /></a></figure>`,
         s.diff && `<figure><figcaption>diff</figcaption><a href="${rel(s.diff)}"><img src="${rel(s.diff)}" loading="lazy" /></a></figure>`].filter(Boolean).join("");
    return `<section class="shot ${s.status}">
  <h2><span class="badge">${s.status}</span> ${esc(s.key)} <small>${esc(detail)}</small></h2>
  <p>${esc(s.scenario.note || "")}</p>
  <div class="images">${images}</div>
</section>`;
  });
  const html = `<!DOCTYPE html><html lang="en"><head><meta charset="utf-8"><title>Grafit visual report</title>
<style>
body{margin:0;padding:24px;background:#111;color:#ddd;font:14px/1.45 system-ui,sans-serif}
h1{font-size:18px;margin:0 0 16px}h2{font-size:15px;margin:0 0 4px}small{color:#888;font-weight:normal;margin-left:8px}
.shot{padding:16px 0;border-top:1px solid #2a2a2a}p{margin:0 0 10px;color:#9a9a9a}
.badge{display:inline-block;min-width:64px;text-align:center;padding:1px 8px;border-radius:99px;font-size:12px;text-transform:uppercase;letter-spacing:.06em;background:#333}
.changed .badge,.size .badge{background:#b3261e;color:#fff}.new .badge,.added .badge,.updated .badge{background:#7a4dff;color:#fff}.noise .badge{background:#7a5c00;color:#fff}.same .badge{background:#1f5c2e;color:#fff}
.images{display:flex;gap:12px;overflow-x:auto}figure{margin:0;flex:1 1 0;min-width:0}figcaption{font-size:12px;color:#888;margin-bottom:4px}img{width:100%;height:auto;display:block;background:#000;border:1px solid #333}
.same .images img{max-width:480px}
</style></head><body>
<h1>Grafit visual report — ${esc(browserName)} — tolerance ${args.tolerance}% — ${new Date().toISOString()}</h1>
${rows.join("\n")}
</body></html>`;
  fs.writeFileSync(path.join(OUT, "report.html"), html, "utf8");
}

// The exit code is set rather than forced: process.exit() right after console.log can
// cut the last lines off when stdout is a pipe, and the summary is the line that matters.
main().then((code) => { process.exitCode = code; }, (error) => { console.error(error.stack || error.message); process.exitCode = 2; });
