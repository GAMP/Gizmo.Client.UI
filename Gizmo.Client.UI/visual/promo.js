#!/usr/bin/env node
// Promo shots: every screen of the shell, in every palette, with generated artwork
// instead of the harness's test pictures, at twice the pixel density for video.
//
//   node visual/promo.js                          all scenes, all eight palettes
//   node visual/promo.js --palette purple --palette teal
//   node visual/promo.js --hex coral=#ff6b57 --only home-news-plain   a club's own colour
//   node visual/promo.js --only shop              scenes whose id contains "shop"
//   node visual/promo.js --scale 1                1920x1080 pixels instead of 3840x2160
//   node visual/promo.js --out D:\shots           default: <fork>\deploy\dist\promo
//   node visual/promo.js --sheets                 only rebuild the contact sheets and the list
//
// Output: <out>\<palette>\<nn>-<scene>.png, a contact sheet per palette in <out>\_sheets
// and <out>\README.txt with the list of scenes. The scenes are in promo-scenes.js.
"use strict";

const fs = require("fs");
const path = require("path");
const { PNG } = require("pngjs");

const css = require("./lib/css");
const chrome = require("./lib/chrome");
const html = require("./lib/html");
const artwork = require("./lib/artwork");
const theme = require("./lib/theme");
const { PALETTES, SCENES, PROFILE } = require("./promo-scenes");

const VISUAL = __dirname;
// Its own working folder, not visual/out: a test run wipes that folder at its start,
// and a promo run takes a quarter of an hour.
const WORK = path.join(VISUAL, "out-promo");
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
  apps: require("./templates/apps"),
  shop: require("./templates/shop"),
  appdetails: require("./templates/appdetails"),
  topup: require("./templates/topup"),
  lock: require("./templates/lock"),
};

function parseArgs(argv) {
  const args = { palettes: [], custom: {}, only: [], scale: 2, width: 1920, height: 1080, out: path.resolve(VISUAL, "..", "..", "deploy", "dist", "promo"), concurrency: 3 };
  for (let i = 0; i < argv.length; i++) {
    const a = argv[i];
    if (a === "--palette") args.palettes.push(argv[++i]);
    else if (a === "--hex") {
      // A club's own colour, as `--gg-palette: #hex` gives it: "coral=#ff6b57" names
      // the folder, a bare "#ff6b57" is filed under hex-ff6b57.
      const m = /^(?:([a-z0-9-]+)=)?(#[0-9a-f]{6})$/i.exec(argv[++i] || "");
      if (!m) throw new Error("--hex wants [name=]#rrggbb");
      const name = (m[1] || "hex-" + m[2].slice(1)).toLowerCase();
      args.custom[name] = m[2].toLowerCase();
      args.palettes.push(name);
    }
    else if (a === "--only") args.only.push(argv[++i]);
    else if (a === "--scale") args.scale = parseFloat(argv[++i]);
    else if (a === "--size") { const [w, h] = argv[++i].split("x").map((n) => parseInt(n, 10)); args.width = w; args.height = h; }
    else if (a === "--out") args.out = path.resolve(argv[++i]);
    else if (a === "--concurrency") args.concurrency = parseInt(argv[++i], 10) || 3;
    else if (a === "--sheets") args.sheetsOnly = true;
    else throw new Error("Unknown argument: " + a);
  }
  if (!args.palettes.length) args.palettes = Object.keys(PALETTES);
  for (const p of args.palettes) if (!PALETTES[p] && !args.custom[p]) throw new Error("Unknown palette: " + p);
  return args;
}

async function pool(items, limit, worker) {
  let next = 0;
  await Promise.all(Array.from({ length: Math.min(limit, items.length) }, async (_, lane) => {
    while (next < items.length) await worker(items[next++], lane);
  }));
}

// Nearest-neighbour downscale of a run of shots into a grid, as palette-sheet.js does.
function sheet(files, out, factor, cols) {
  const frames = files.map((f) => PNG.sync.read(fs.readFileSync(f)));
  if (!frames.length) return;
  const w = Math.floor(frames[0].width / factor), h = Math.floor(frames[0].height / factor);
  const rows = Math.ceil(frames.length / cols);
  const gap = 8;
  const png = new PNG({ width: w * cols + gap * (cols - 1), height: h * rows + gap * (rows - 1) });
  png.data.fill(16);
  for (let i = 3; i < png.data.length; i += 4) png.data[i] = 255;
  frames.forEach((im, i) => {
    const ox = (i % cols) * (w + gap), oy = Math.floor(i / cols) * (h + gap);
    for (let y = 0; y < h; y++) for (let x = 0; x < w; x++) {
      const si = ((y * factor) * im.width + (x * factor)) * 4, di = ((oy + y) * png.width + (ox + x)) * 4;
      png.data[di] = im.data[si]; png.data[di + 1] = im.data[si + 1]; png.data[di + 2] = im.data[si + 2]; png.data[di + 3] = 255;
    }
  });
  fs.writeFileSync(out, PNG.sync.write(png));
}

async function main() {
  const args = parseArgs(process.argv.slice(2));
  const started = Date.now();
  const scenes = SCENES.filter((s) => !args.only.length || args.only.some((o) => s.id.includes(o)));
  if (!scenes.length) { console.log("Nothing matched."); return; }
  if (args.sheetsOnly) { finish(args, args.palettes); console.log(`sheets -> ${path.join(args.out, "_sheets")}`); return; }

  fs.rmSync(WORK, { recursive: true, force: true });
  fs.mkdirSync(WORK, { recursive: true });
  css.build(WORK);

  // The pages, per palette: the artwork provider is set for the palette, the pages are
  // written, then the next palette. Rendering is a pure function of the data, so the
  // pages can all be written up front and shot in any order.
  const shots = [];
  for (const palette of args.palettes) {
    const accent = PALETTES[palette] || args.custom[palette];
    // A built-in palette is the compiled one (data-accent); a colour of the club's own
    // goes through the run-time derivation and is written inline, as in the client.
    const tokens = PALETTES[palette] ? null : theme.derive(accent);
    html.setArtProvider((name) => artwork.forName(name, accent));
    const profile = { ...PROFILE, picture: artwork.make({ kind: "avatar", seed: "me", accent }) };
    const dir = path.join(args.out, palette);
    fs.mkdirSync(dir, { recursive: true });
    scenes.forEach((scene, index) => {
      const template = TEMPLATES[scene.page];
      if (!template) throw new Error(`Scene ${scene.id}: unknown page "${scene.page}"`);
      const data = scene.page === "login" || scene.page === "notifications" ? { ...scene.data } : { ...profile, ...scene.data };
      // A dialog's board shows the figures the dialog reckons with.
      if (data.board) data.board = { ...profile, ...data.board, balance: data.balance == null ? profile.balance : data.balance, points: data.points == null ? profile.points : data.points };
      const body = template.render(data);
      const file = path.join(WORK, `${palette}-${scene.id}.html`);
      fs.writeFileSync(file, html.page({ title: scene.id, body, outDir: WORK, accent: tokens ? null : palette, tokens }), "utf8");
      shots.push({ palette, scene, index, html: file, png: path.join(dir, `${String(index + 1).padStart(2, "0")}-${scene.id}.png`) });
    });
  }
  html.setArtProvider(null);

  const executable = chrome.find();
  console.log(`${shots.length} shots (${scenes.length} scenes x ${args.palettes.length} palettes) at ${args.width}x${args.height} @${args.scale}x with ${await chrome.version(executable)}`);
  const browsers = [];
  const tabs = [];
  for (let i = 0; i < Math.min(args.concurrency, shots.length); i++) {
    const browser = new chrome.Browser(executable, 20 + i);
    await browser.launch();
    browsers.push(browser);
    tabs.push(await browser.tab());
  }
  try {
    await pool(shots, tabs.length, async (shot, lane) => {
      await tabs[lane].screenshot(css.fileUrl(shot.html), shot.png, args.width, args.height, { scale: args.scale, hover: shot.scene.hover || null });
      console.log(` + ${shot.palette}/${path.basename(shot.png)}`);
    });
  } finally {
    await Promise.all(browsers.map((b) => b.close()));
  }

  finish(args, args.palettes);
  console.log(`\n${shots.length} shots in ${((Date.now() - started) / 1000).toFixed(0)}s -> ${args.out}`);
}

// The contact sheets and the list, from what is on disk: a run for one palette or a
// few scenes refreshes those files and leaves the rest, and the sheets show it all.
function finish(args, palettes) {
  const sheets = path.join(args.out, "_sheets");
  fs.mkdirSync(sheets, { recursive: true });
  const present = fs.readdirSync(args.out).filter((f) => !f.startsWith("_") && fs.statSync(path.join(args.out, f)).isDirectory());
  for (const palette of present) {
    if (!palettes.includes(palette)) continue;
    const dir = path.join(args.out, palette);
    const files = fs.readdirSync(dir).filter((f) => f.endsWith(".png")).sort().map((f) => path.join(dir, f));
    sheet(files, path.join(sheets, `${palette}.png`), args.scale * 4, 4);
  }
  const list = SCENES.map((s, i) => `${String(i + 1).padStart(2, "0")}-${s.id}.png — ${s.note}`).join("\n");
  fs.writeFileSync(path.join(args.out, "README.txt"), `Grafit — промо-скриншоты\n\nПапки: ${present.join(", ")} — одна на палитру (data-accent). В каждой одинаковый набор сцен, ${args.width}x${args.height} @${args.scale}x.\n_sheets\\<палитра>.png — контактные листы.\n\n${list}\n`, "utf8");
}

main().catch((error) => { console.error(error.stack || error.message); process.exitCode = 2; });
