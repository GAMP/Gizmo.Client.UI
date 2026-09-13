// Cuts a region out of a screenshot, optionally enlarged, for looking at one control
// closely or for sending a detail rather than a whole screen.
//
//   node visual/crop.js <in.png> <out.png> <x> <y> <width> <height> [zoom]
//
// zoom is an integer nearest-neighbour factor (2 = twice the size); no smoothing, so
// a one-pixel hairline stays a crisp two-pixel one.
"use strict";

const fs = require("fs");
const { PNG } = require("pngjs");

const [inp, out, x, y, w, h, z] = process.argv.slice(2);
if (!inp || !out || !w || !h) {
  console.error("usage: node visual/crop.js <in.png> <out.png> <x> <y> <width> <height> [zoom]");
  process.exit(2);
}
const src = PNG.sync.read(fs.readFileSync(inp));
const X = +x || 0, Y = +y || 0, Z = Math.max(1, parseInt(z || "1", 10));
const W = Math.min(+w, src.width - X), H = Math.min(+h, src.height - Y);
const dst = new PNG({ width: W * Z, height: H * Z });
for (let j = 0; j < H * Z; j++) {
  for (let i = 0; i < W * Z; i++) {
    const si = ((Y + Math.floor(j / Z)) * src.width + (X + Math.floor(i / Z))) * 4;
    const di = (j * W * Z + i) * 4;
    dst.data[di] = src.data[si];
    dst.data[di + 1] = src.data[si + 1];
    dst.data[di + 2] = src.data[si + 2];
    dst.data[di + 3] = 255;
  }
}
fs.writeFileSync(out, PNG.sync.write(dst));
console.log(out);
