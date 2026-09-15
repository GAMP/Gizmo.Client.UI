// A contact sheet of the generated artwork (lib/artwork.js) in one palette, for judging
// the pictures on their own before they go on a screen.
//
//   node visual/art-sheet.js [--accent purple] [--out path.png]
//
// Output: visual/out/art-<accent>.png by default (visual/out is wiped by every harness
// run - pass --out for anything worth keeping).
"use strict";

const fs = require("fs");
const path = require("path");
const chrome = require("./lib/chrome");
const css = require("./lib/css");
const art = require("./lib/artwork");

const PALETTES = { blue: "#4f8cff", purple: "#b06bff", red: "#ff3b46", orange: "#ff8a3d", amber: "#f0b83d", green: "#4ade80", teal: "#22d3ee", pink: "#ff5fa8" };

function args() {
  const a = { accent: "purple", out: null };
  const argv = process.argv.slice(2);
  for (let i = 0; i < argv.length; i++) {
    if (argv[i] === "--accent") a.accent = argv[++i];
    else if (argv[i] === "--out") a.out = argv[++i];
  }
  return a;
}

async function main() {
  const a = args();
  const accent = PALETTES[a.accent] || a.accent;
  const OUT = path.join(__dirname, "out");
  fs.mkdirSync(OUT, { recursive: true });

  const row = (kind, count, w, h) => Array.from({ length: count }, (_, i) =>
    `<img src="${art.make({ kind, seed: kind + "-" + (i + 1), accent })}" style="width:${w}px;height:${h}px;border-radius:14px;object-fit:cover" />`).join("");
  const html = `<!DOCTYPE html><html><head><meta charset="utf-8"><style>
body{margin:0;padding:24px;background:#0b0b0e;font:12px system-ui;color:#888}
.r{display:flex;gap:12px;margin-bottom:18px;flex-wrap:wrap}h2{font-size:12px;margin:0 0 6px;text-transform:uppercase;letter-spacing:.1em}
</style></head><body>
<h2>cover</h2><div class="r">${row("cover", 12, 150, 200)}</div>
<h2>banner</h2><div class="r">${row("banner", 3, 600, 240)}</div>
<h2>photo</h2><div class="r">${row("photo", 8, 200, 150)}</div>
<h2>icon</h2><div class="r">${row("icon", 12, 64, 64)}</div>
<h2>media</h2><div class="r">${row("media", 5, 320, 180)}</div>
</body></html>`;
  const file = path.join(OUT, `art-${a.accent}.html`);
  fs.writeFileSync(file, html, "utf8");

  const browser = new chrome.Browser(chrome.find(), 11);
  await browser.launch();
  try {
    const tab = await browser.tab();
    const out = a.out || path.join(OUT, `art-${a.accent}.png`);
    await tab.screenshot(css.fileUrl(file), out, 2000, 1250);
    console.log(out);
  } finally {
    await browser.close();
  }
}

main().catch((e) => { console.error(e); process.exitCode = 1; });
