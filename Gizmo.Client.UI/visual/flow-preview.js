// Samples the moving background at several moments of its cycle and writes them side by
// side, so a change to _flow.scss can be judged as motion rather than as one frame.
//
//   node visual/flow-preview.js [--accent red] [--page login|home]
//
// Output: visual/out/flow-<page>-<accent>.png (four frames at 0 / 20 / 40 / 60 s).
"use strict";

const fs = require("fs");
const path = require("path");
const { PNG } = require("pngjs");
const css = require("./lib/css");
const chrome = require("./lib/chrome");
const { page } = require("./lib/html");

const OUT = path.join(__dirname, "out");
const TIMES = [0, 20000, 40000, 60000];

function args() {
  const a = { accent: null, page: "login" };
  const argv = process.argv.slice(2);
  for (let i = 0; i < argv.length; i++) {
    if (argv[i] === "--accent") a.accent = argv[++i];
    else if (argv[i] === "--page") a.page = argv[++i];
  }
  return a;
}

async function main() {
  const a = args();
  fs.mkdirSync(OUT, { recursive: true });
  css.build(OUT);

  const template = a.page === "home" ? require("./templates/home") : require("./templates/login");
  const data = a.page === "home"
    ? { hero: "news", apps: 8, packs: 6, bar: 4, balance: 1250, points: 3400, time: "2ч 15м", pinned: 3 }
    : { pc: 12 };
  const html = path.join(OUT, `flow-${a.page}.html`);
  fs.writeFileSync(html, page({ title: "flow", body: template.render(data), outDir: OUT, accent: a.accent || undefined }), "utf8");

  const browser = new chrome.Browser(chrome.find(), 9);
  await browser.launch();
  const tab = await browser.tab();
  const b = browser;
  const width = 1920, height = 1080;
  const frames = [];

  try {
    await b.send("Emulation.setDeviceMetricsOverride", { width, height, deviceScaleFactor: 1, mobile: false }, tab.sessionId);
    const loaded = b.once("Page.loadEventFired", tab.sessionId);
    await b.send("Page.navigate", { url: css.fileUrl(html) }, tab.sessionId);
    await loaded;
    await b.send("Runtime.evaluate", { expression: `(async () => { await document.fonts.ready; for (const an of document.getAnimations()) { try { an.pause(); } catch (_) {} } return true; })()`, awaitPromise: true }, tab.sessionId);

    for (const t of TIMES) {
      await b.send("Runtime.evaluate", { expression: `(() => { for (const an of document.getAnimations()) { try { const tm = an.effect.getTiming(); an.currentTime = tm.iterations === Infinity ? ${t} : (tm.duration || 0) + (tm.delay || 0); } catch (_) {} } return document.getAnimations().length; })()` }, tab.sessionId);
      const { data: png } = await b.send("Page.captureScreenshot", { format: "png" }, tab.sessionId);
      frames.push(PNG.sync.read(Buffer.from(png, "base64")));
    }
  } finally {
    await browser.close();
  }

  const s = 2;
  const w = Math.floor(width / s), h = Math.floor(height / s);
  const sheet = new PNG({ width: w * 2, height: h * 2 });
  frames.forEach((im, i) => {
    const ox = (i % 2) * w, oy = Math.floor(i / 2) * h;
    for (let y = 0; y < h; y++) for (let x = 0; x < w; x++) {
      const si = ((y * s) * im.width + (x * s)) * 4, di = ((oy + y) * sheet.width + (ox + x)) * 4;
      sheet.data[di] = im.data[si]; sheet.data[di + 1] = im.data[si + 1]; sheet.data[di + 2] = im.data[si + 2]; sheet.data[di + 3] = 255;
    }
  });
  const out = path.join(OUT, `flow-${a.page}-${a.accent || "default"}.png`);
  fs.writeFileSync(out, PNG.sync.write(sheet));
  console.log(out);
}

main().catch((e) => { console.error(e); process.exitCode = 1; });
