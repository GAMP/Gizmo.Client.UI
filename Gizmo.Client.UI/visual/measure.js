// Measures elements on a rendered scenario: bounding boxes and a few computed styles,
// for the cases where a screenshot shows "something is off" but not why.
//
//   node visual/measure.js <scenario-id> "<css selector>" [more selectors...] [--size 1920x1080]
//
// Prints one line per matched element: rect, then white-space / max-width / text-align /
// padding / margin as computed.
"use strict";

const fs = require("fs");
const path = require("path");
const css = require("./lib/css");
const chrome = require("./lib/chrome");
const { page } = require("./lib/html");

const OUT = path.join(__dirname, "out");

async function main() {
  const argv = process.argv.slice(2);
  let size = "1920x1080";
  const si = argv.indexOf("--size");
  if (si >= 0) { size = argv[si + 1]; argv.splice(si, 2); }
  const [id, ...selectors] = argv;
  if (!id || !selectors.length) { console.error("usage: node visual/measure.js <scenario-id> <selector> [...] [--size WxH]"); process.exit(2); }
  const spec = JSON.parse(fs.readFileSync(path.join(__dirname, "scenarios.json"), "utf8"));
  const scenario = spec.scenarios.find((s) => s.id === id);
  if (!scenario) throw new Error("no scenario " + id);
  const template = require("./templates/" + scenario.page);
  fs.mkdirSync(OUT, { recursive: true });
  css.build(OUT);
  const html = path.join(OUT, `measure-${id}.html`);
  fs.writeFileSync(html, page({ title: id, body: template.render(scenario.data || {}), outDir: OUT, accent: scenario.accent }), "utf8");

  const [w, h] = size.split("x").map((n) => parseInt(n, 10));
  const browser = new chrome.Browser(chrome.find(), 7);
  await browser.launch();
  const tab = await browser.tab();
  try {
    await browser.send("Emulation.setDeviceMetricsOverride", { width: w, height: h, deviceScaleFactor: 1, mobile: false }, tab.sessionId);
    const loaded = browser.once("Page.loadEventFired", tab.sessionId);
    await browser.send("Page.navigate", { url: css.fileUrl(html) }, tab.sessionId);
    await loaded;
    const expression = `(async () => { await document.fonts.ready; const out = []; for (const sel of ${JSON.stringify(selectors)}) { document.querySelectorAll(sel).forEach((el, i) => { const r = el.getBoundingClientRect(); const cs = getComputedStyle(el); out.push(sel + "[" + i + "] x=" + r.left.toFixed(1) + " y=" + r.top.toFixed(1) + " w=" + r.width.toFixed(1) + " h=" + r.height.toFixed(1) + " centre=" + (r.left + r.width / 2).toFixed(1) + " | white-space=" + cs.whiteSpace + " max-width=" + cs.maxWidth + " text-align=" + cs.textAlign + " padding=" + cs.padding + " margin=" + cs.margin + " overflow=" + cs.overflow + " scrollW=" + el.scrollWidth); }); } return out.join("\\n"); })()`;
    const { result } = await browser.send("Runtime.evaluate", { expression, awaitPromise: true, returnByValue: true }, tab.sessionId);
    console.log(result.value);
  } finally {
    await browser.close();
  }
}

main().catch((e) => { console.error(e); process.exitCode = 1; });
