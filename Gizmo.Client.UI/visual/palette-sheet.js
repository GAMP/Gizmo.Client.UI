// Renders one page in every palette and tiles the shots into a contact sheet, so a
// change to the atmosphere or the token rules is judged across the whole set at once
// rather than one accent at a time.
//
//   node visual/palette-sheet.js [--page login|home] [--accent red --accent blue ...] [--scale 3]
//
// Output: visual/out/palettes-<page>.png. Without --accent every palette is rendered.
"use strict";

const fs = require("fs");
const path = require("path");
const { PNG } = require("pngjs");
const css = require("./lib/css");
const chrome = require("./lib/chrome");
const { page } = require("./lib/html");

const OUT = path.join(__dirname, "out");
const ALL = ["blue", "purple", "red", "orange", "amber", "green", "teal", "pink"];

function args() {
  const a = { accents: [], page: "login", scale: 3 };
  const argv = process.argv.slice(2);
  for (let i = 0; i < argv.length; i++) {
    if (argv[i] === "--accent") a.accents.push(argv[++i]);
    else if (argv[i] === "--page") a.page = argv[++i];
    else if (argv[i] === "--scale") a.scale = parseInt(argv[++i], 10) || 3;
  }
  if (!a.accents.length) a.accents = ALL;
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

  const browser = new chrome.Browser(chrome.find(), 8);
  await browser.launch();
  const tab = await browser.tab();
  const width = 1920, height = 1080;
  const frames = [];

  try {
    for (const accent of a.accents) {
      const html = path.join(OUT, `palette-${a.page}-${accent}.html`);
      fs.writeFileSync(html, page({ title: accent, body: template.render(data), outDir: OUT, accent }), "utf8");
      const png = path.join(OUT, `palette-${a.page}-${accent}.png`);
      await tab.screenshot(css.fileUrl(html), png, width, height);
      frames.push(PNG.sync.read(fs.readFileSync(png)));
    }
  } finally {
    await browser.close();
  }

  // Nearest-neighbour downscale by an integer factor; a fractional one produced garbage.
  const s = a.scale;
  const w = Math.floor(width / s), h = Math.floor(height / s);
  const cols = frames.length > 4 ? 4 : frames.length;
  const rows = Math.ceil(frames.length / cols);
  const sheet = new PNG({ width: w * cols, height: h * rows });
  frames.forEach((im, i) => {
    const ox = (i % cols) * w, oy = Math.floor(i / cols) * h;
    for (let y = 0; y < h; y++) for (let x = 0; x < w; x++) {
      const si = ((y * s) * im.width + (x * s)) * 4, di = ((oy + y) * sheet.width + (ox + x)) * 4;
      sheet.data[di] = im.data[si]; sheet.data[di + 1] = im.data[si + 1]; sheet.data[di + 2] = im.data[si + 2]; sheet.data[di + 3] = 255;
    }
  });
  const out = path.join(OUT, `palettes-${a.page}.png`);
  fs.writeFileSync(out, PNG.sync.write(sheet));
  console.log(out);
}

main().catch((e) => { console.error(e); process.exitCode = 1; });
