// Small helpers shared by the page templates.
"use strict";

const path = require("path");
const { fileUrl, SRC } = require("./css");

const FIXTURES = path.resolve(__dirname, "..", "fixtures");

function esc(text) {
  return String(text)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

// The shell's own stock pictures (placeholders, wallpaper).
function img(name) {
  return fileUrl(path.join(SRC, "img", name));
}

// Test artwork from visual/fixtures/art - or, when a provider is set (the promo shots,
// see promo.js and lib/artwork.js), whatever the provider draws for that name.
let provider = null;

function setArtProvider(fn) {
  provider = fn;
}

function art(name) {
  if (provider) {
    const drawn = provider(name);
    if (drawn) return drawn;
  }
  return fileUrl(path.join(FIXTURES, "art", name));
}

// The i-th picture of a kind ("app", "exe", "news", "product", "media"). The fixtures
// hold a few of each and repeat; a provider draws every index its own picture, and
// `shift` moves a template's series away from another's so the two do not share
// pictures (it does nothing to the fixtures).
const FIXTURE_COUNT = { app: 5, exe: 3, news: 3, product: 5 };

function artFor(kind, i, shift) {
  const count = FIXTURE_COUNT[kind];
  return art(`${kind}-${provider || !count ? i + 1 + (shift || 0) : (i % count) + 1}.jpg`);
}

// Money the way the client formats it for a Russian club: "1 250,00 ₽". A fixed
// culture rather than the machine's, so a baseline is the same everywhere.
function money(amount) {
  const fixed = Number(amount).toFixed(2);
  const [whole, frac] = fixed.split(".");
  const grouped = whole.replace(/\B(?=(\d{3})+(?!\d))/g, " ");
  return grouped + "," + frac + " ₽";
}

function number(amount) {
  return String(Math.round(Number(amount))).replace(/\B(?=(\d{3})+(?!\d))/g, " ");
}

// Russian plural forms: one, few, many.
function plural(count, forms) {
  const mod100 = count % 100;
  let index;
  if (mod100 >= 11 && mod100 <= 14) index = 2;
  else {
    const mod10 = count % 10;
    index = mod10 === 1 ? 0 : (mod10 >= 2 && mod10 <= 4 ? 1 : 2);
  }
  return forms[index].replace("{0}", number(count));
}

// A complete document around a body, with the shell's fonts, icons and the compiled
// stylesheet. `accent` selects a palette the way the skin's index.html does. Screens under test render with the same root font size rules as the
// client (10px, growing on large monitors - see src/scss/_global.scss).
function page({ title, body, outDir, wallpaper, accent }) {
  const vendor = fileUrl(path.join(SRC, "vendor"));
  const css = (name) => fileUrl(path.join(outDir, name));
  return `<!DOCTYPE html>
<html lang="ru"${accent ? ` data-accent="${esc(accent)}"` : ""}>
<head>
<meta charset="utf-8" />
<title>${esc(title)}</title>
<link rel="stylesheet" href="${vendor}/fonts/fonts.css" />
<link rel="stylesheet" href="${vendor}/phosphor/regular/style.css" />
<link rel="stylesheet" href="${vendor}/phosphor/bold/style.css" />
<link rel="stylesheet" href="${vendor}/phosphor/fill/style.css" />
<link rel="stylesheet" href="${css("style.css")}" />
<link rel="stylesheet" href="${css("layout.css")}" />
<link rel="stylesheet" href="${css("harness.css")}" />
</head>
<body>
<main client-theme="true">
  <div class="giz-background${wallpaper ? " giz-background--club" : ""}">${wallpaper ? "" : `<div class="gg-flow" aria-hidden="true"><span class="gg-flow__sheet gg-flow__sheet--1"></span><span class="gg-flow__sheet gg-flow__sheet--2"></span></div>`}<img src="${wallpaper || img("background.jpg")}" alt="" /></div>
${body}
</main>
</body>
</html>
`;
}

module.exports = { esc, img, art, artFor, setArtProvider, money, number, plural, page };
