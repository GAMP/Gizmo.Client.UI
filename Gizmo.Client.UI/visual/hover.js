// Screenshots a rendered scenario with the mouse over an element, for the states the
// baselines cannot hold: hover tints, tooltips, anything that only exists under the
// pointer. The pointer is moved with a real mouse event, so :hover applies.
//
//   node visual/hover.js <scenario-id> "<css selector>" <out.png> [--size 1920x1080] [--pad 120]
//
// The picture is the area around the element (pad pixels each side, twice the pixels)
// so the detail is readable; --pad 0 gives the whole page.
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
  let pad = 120;
  const si = argv.indexOf("--size");
  if (si >= 0) { size = argv[si + 1]; argv.splice(si, 2); }
  const pi = argv.indexOf("--pad");
  if (pi >= 0) { pad = parseInt(argv[pi + 1], 10); argv.splice(pi, 2); }
  const [id, selector, out] = argv;
  if (!id || !selector || !out) { console.error("usage: node visual/hover.js <scenario-id> <selector> <out.png> [--size WxH] [--pad N]"); process.exit(2); }
  const spec = JSON.parse(fs.readFileSync(path.join(__dirname, "scenarios.json"), "utf8"));
  const scenario = spec.scenarios.find((s) => s.id === id);
  if (!scenario) throw new Error("no scenario " + id);
  const template = require("./templates/" + scenario.page);
  fs.mkdirSync(OUT, { recursive: true });
  css.build(OUT);
  const html = path.join(OUT, `hover-${id}.html`);
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
    const expression = `(async () => { await document.fonts.ready; const el = document.querySelector(${JSON.stringify(selector)}); if (!el) return null; const r = el.getBoundingClientRect(); return { x: r.left, y: r.top, w: r.width, h: r.height }; })()`;
    const { result } = await browser.send("Runtime.evaluate", { expression, awaitPromise: true, returnByValue: true }, tab.sessionId);
    const box = result.value;
    if (!box) throw new Error("no element for " + selector);
    const cx = box.x + box.w / 2, cy = box.y + box.h / 2;
    await browser.send("Input.dispatchMouseEvent", { type: "mouseMoved", x: cx, y: cy }, tab.sessionId);
    await new Promise((r) => setTimeout(r, 400));
    const clip = pad > 0
      ? { x: Math.max(0, cx - box.w / 2 - pad), y: Math.max(0, cy - box.h / 2 - pad), width: box.w + 2 * pad, height: box.h + 2 * pad, scale: 2 }
      : undefined;
    const { data } = await browser.send("Page.captureScreenshot", { format: "png", captureBeyondViewport: false, clip }, tab.sessionId);
    fs.writeFileSync(out, Buffer.from(data, "base64"));
    console.log(`${out}  (${selector} at ${cx.toFixed(0)},${cy.toFixed(0)})`);
  } finally {
    await browser.close();
  }
}

main().catch((e) => { console.error(e); process.exitCode = 1; });
