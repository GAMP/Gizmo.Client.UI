// Pixel comparison of a fresh screenshot against its committed reference.
"use strict";

const fs = require("fs");
const { PNG } = require("pngjs");
const pixelmatch = require("pixelmatch");

function read(file) {
  return PNG.sync.read(fs.readFileSync(file));
}

// Returns { same, different, total, percent, sizeMismatch } and writes a diff image
// (changed pixels in red over a faded copy) when there is anything to show.
function compare(baselineFile, currentFile, diffFile, threshold) {
  const a = read(baselineFile);
  const b = read(currentFile);

  if (a.width !== b.width || a.height !== b.height) {
    return { same: false, different: -1, total: b.width * b.height, percent: 100, sizeMismatch: true,
             baselineSize: a.width + "x" + a.height, currentSize: b.width + "x" + b.height };
  }

  const diff = new PNG({ width: a.width, height: a.height });
  // Anti-aliased edges are left out of the count: they move with font rasterisation
  // from one Chrome build to the next and never mean a layout change.
  const different = pixelmatch(a.data, b.data, diff.data, a.width, a.height,
    { threshold, includeAA: false, alpha: 0.35, diffColor: [255, 60, 60], diffColorAlt: [255, 200, 60] });
  const total = a.width * a.height;
  const percent = (different / total) * 100;

  if (different > 0) fs.writeFileSync(diffFile, PNG.sync.write(diff));

  return { same: different === 0, different, total, percent, sizeMismatch: false };
}

module.exports = { compare };
