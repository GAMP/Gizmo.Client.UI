#!/usr/bin/env node
// The pictures of the repository's README (docs/img): the home board, four screens in a
// grid and the home in every built-in palette - rendered by promo.js from the current
// sources, so they never lag behind the shell.
//
//   node visual/readme-shots.js            writes ../docs/img/*.png (about 2 MB in all)
"use strict";

const fs = require("fs");
const path = require("path");
const { execFileSync } = require("child_process");
const { PNG } = require("pngjs");
const { PALETTES } = require("./promo-scenes");

const VISUAL = __dirname;
const WORK = path.join(VISUAL, "out-readme");
const OUT = path.resolve(VISUAL, "..", "..", "docs", "img");
const BRAND = "purple";

// Area-averaging resample to a width: crisp at any factor, unlike the nearest-neighbour
// contact sheets.
function resize(png, width) {
  const scale = png.width / width;
  const height = Math.round(png.height / scale);
  const out = new PNG({ width, height });
  for (let y = 0; y < height; y++) {
    const y0 = Math.floor(y * scale), y1 = Math.min(png.height, Math.ceil((y + 1) * scale));
    for (let x = 0; x < width; x++) {
      const x0 = Math.floor(x * scale), x1 = Math.min(png.width, Math.ceil((x + 1) * scale));
      let r = 0, g = 0, b = 0, n = 0;
      for (let sy = y0; sy < y1; sy++) for (let sx = x0; sx < x1; sx++) {
        const i = (sy * png.width + sx) * 4;
        r += png.data[i]; g += png.data[i + 1]; b += png.data[i + 2]; n++;
      }
      const o = (y * width + x) * 4;
      out.data[o] = Math.round(r / n); out.data[o + 1] = Math.round(g / n); out.data[o + 2] = Math.round(b / n); out.data[o + 3] = 255;
    }
  }
  return out;
}

// Frames in a grid on the shell's black, with a gap and rounded corners on each frame.
function grid(frames, cols, gap, radius) {
  const w = frames[0].width, h = frames[0].height;
  const rows = Math.ceil(frames.length / cols);
  const out = new PNG({ width: w * cols + gap * (cols - 1), height: h * rows + gap * (rows - 1) });
  for (let i = 0; i < out.data.length; i += 4) { out.data[i] = 11; out.data[i + 1] = 12; out.data[i + 2] = 14; out.data[i + 3] = 255; }
  frames.forEach((f, k) => {
    const ox = (k % cols) * (w + gap), oy = Math.floor(k / cols) * (h + gap);
    for (let y = 0; y < h; y++) for (let x = 0; x < w; x++) {
      // Outside the rounded corner: leave the background.
      const cx = x < radius ? radius - x : x >= w - radius ? x - (w - radius - 1) : 0;
      const cy = y < radius ? radius - y : y >= h - radius ? y - (h - radius - 1) : 0;
      if (cx && cy && cx * cx + cy * cy > radius * radius) continue;
      const si = (y * f.width + x) * 4, di = ((oy + y) * out.width + (ox + x)) * 4;
      out.data[di] = f.data[si]; out.data[di + 1] = f.data[si + 1]; out.data[di + 2] = f.data[si + 2]; out.data[di + 3] = 255;
    }
  });
  return out;
}

function read(file) {
  return PNG.sync.read(fs.readFileSync(file));
}

function shot(palette, scene) {
  const dir = path.join(WORK, palette);
  const file = fs.readdirSync(dir).find((f) => f.endsWith(`-${scene}.png`));
  if (!file) throw new Error(`No shot for ${palette}/${scene}`);
  return read(path.join(dir, file));
}

function main() {
  fs.rmSync(WORK, { recursive: true, force: true });
  const promo = path.join(VISUAL, "promo.js");
  const scenes = ["home-news", "shop-all", "apps-popular", "progress", "buy-balance"];
  execFileSync(process.execPath, [promo, "--scale", "1", "--palette", BRAND, ...scenes.flatMap((s) => ["--only", s]), "--out", WORK], { stdio: "inherit" });
  execFileSync(process.execPath, [promo, "--scale", "1", "--only", "home-news-plain", "--out", WORK], { stdio: "inherit" });

  fs.mkdirSync(OUT, { recursive: true });
  fs.writeFileSync(path.join(OUT, "home.png"), PNG.sync.write(resize(shot(BRAND, "home-news"), 1600)));
  fs.writeFileSync(path.join(OUT, "screens.png"), PNG.sync.write(grid(["shop-all", "apps-popular", "progress", "buy-balance"].map((s) => resize(shot(BRAND, s), 790)), 2, 20, 12)));
  fs.writeFileSync(path.join(OUT, "palettes.png"), PNG.sync.write(grid(Object.keys(PALETTES).map((p) => resize(shot(p, "home-news-plain"), 385)), 4, 20, 8)));
  for (const f of fs.readdirSync(OUT)) console.log(` ${f}  ${(fs.statSync(path.join(OUT, f)).size / 1024).toFixed(0)} KB`);
  fs.rmSync(WORK, { recursive: true, force: true });
}

main();
