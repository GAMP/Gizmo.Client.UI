// Stacks PNGs vertically (same width assumed; narrower ones are left-aligned on black)
// into one image - for sending several states of one control as a single picture.
//
//   node visual/stack.js <out.png> <in1.png> <in2.png> ... [--gap 12]
"use strict";

const fs = require("fs");
const { PNG } = require("pngjs");

const argv = process.argv.slice(2);
let gap = 12;
const gi = argv.indexOf("--gap");
if (gi >= 0) { gap = parseInt(argv[gi + 1], 10) || 0; argv.splice(gi, 2); }
const [out, ...inputs] = argv;
if (!out || inputs.length < 1) {
  console.error("usage: node visual/stack.js <out.png> <in1.png> [in2.png ...] [--gap 12]");
  process.exit(2);
}
const images = inputs.map((p) => PNG.sync.read(fs.readFileSync(p)));
const width = Math.max(...images.map((i) => i.width));
const height = images.reduce((h, i) => h + i.height, 0) + gap * (images.length - 1);
const sheet = new PNG({ width, height });
sheet.data.fill(0);
for (let i = 3; i < sheet.data.length; i += 4) sheet.data[i] = 255;
let y = 0;
for (const im of images) {
  for (let j = 0; j < im.height; j++) {
    im.data.copy(sheet.data, ((y + j) * width) * 4, (j * im.width) * 4, (j * im.width + im.width) * 4);
  }
  y += im.height + gap;
}
fs.writeFileSync(out, PNG.sync.write(sheet));
console.log(out);
