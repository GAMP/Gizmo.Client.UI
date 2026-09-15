// Abstract artwork for the promo shots: SVG data URIs, deterministic per name, in the
// palette's hues. Nothing photographic and nothing that could be taken for a real game
// or a real product - covers, icons, banners and photos are all fields of light: soft
// washes, folded ridges, a ribbon of silk, a shaft of light. No discs, no lines, no
// type. The fixtures in visual/fixtures/art stay the harness's test artwork; this is
// for pictures meant to be looked at.
//
//   const art = require("./artwork");
//   art.make({ kind: "cover", seed: "app-3", accent: "#b06bff" })   -> "data:image/svg+xml,..."
//
// kind: cover (3:4 poster), banner (wide), photo (goods, 4:3), icon (square, bolder),
// avatar (square, quiet), media (16:9 screenshot).
"use strict";

// ── deterministic randomness ──────────────────────────────────────────────

function hash(text) {
  let h = 2166136261;
  for (const ch of String(text)) {
    h ^= ch.charCodeAt(0);
    h = Math.imul(h, 16777619) >>> 0;
  }
  return h >>> 0;
}

function prng(seed) {
  let a = seed >>> 0;
  return () => {
    a = (a + 0x6d2b79f5) >>> 0;
    let t = a;
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

// ── colour ────────────────────────────────────────────────────────────────

function hexToHue(hex) {
  const m = /^#?([0-9a-f]{6})$/i.exec(hex || "");
  if (!m) return 268;
  const n = parseInt(m[1], 16);
  const r = ((n >> 16) & 255) / 255, g = ((n >> 8) & 255) / 255, b = (n & 255) / 255;
  const max = Math.max(r, g, b), min = Math.min(r, g, b), d = max - min;
  if (d === 0) return 268;
  let h;
  if (max === r) h = ((g - b) / d) % 6;
  else if (max === g) h = (b - r) / d + 2;
  else h = (r - g) / d + 4;
  h = Math.round(h * 60);
  return h < 0 ? h + 360 : h;
}

function hsl(h, s, l, a) {
  const hh = ((h % 360) + 360) % 360;
  return a == null ? `hsl(${hh.toFixed(0)}, ${s.toFixed(0)}%, ${l.toFixed(0)}%)` : `hsla(${hh.toFixed(0)}, ${s.toFixed(0)}%, ${l.toFixed(0)}%, ${a.toFixed(2)})`;
}

const SIZES = {
  cover: [600, 800],
  banner: [1600, 640],
  photo: [640, 480],
  icon: [256, 256],
  avatar: [256, 256],
  media: [640, 360],
};

// ── the picture ───────────────────────────────────────────────────────────

// Every picture is drawn from one hue family: the palette's accent, drifted by a few
// dozen degrees per seed, with one picture in eight almost grey so a wall of them keeps
// a rhythm instead of one colour.
function make({ kind = "cover", seed = "", accent = "#4f8cff" } = {}) {
  const [W, H] = SIZES[kind] || SIZES.cover;
  const rnd = prng(hash(kind + ":" + seed));
  const base = hexToHue(accent);
  const neutral = rnd() < 0.12 && kind !== "icon" && kind !== "avatar";
  // How far a picture may drift from the accent: a wall of covers wants variety, the
  // one banner and the small marks (icons, the avatar) should stay the palette's.
  const drift = kind === "banner" ? 20 : kind === "icon" || kind === "avatar" ? 15 : 32;
  // A warm accent (orange, amber) drifts towards red only: a step towards green turns
  // it olive. The companion hues lean the same way.
  const warm = base >= 15 && base <= 75;
  const hue = warm ? base - rnd() * Math.min(drift * 1.3, base - 12) : base + (rnd() * 2 - 1) * drift;
  const dir = warm ? -1 : 1;
  // Warm hues need more saturation: a half-saturated amber over black is olive.
  const sat = neutral ? 10 : warm ? 74 + rnd() * 16 : 58 + rnd() * 22;
  const motif = kind === "icon" ? ["aurora", "beam", "silk"][Math.floor(rnd() * 3)]
    : kind === "avatar" ? "aurora"
    : ["aurora", "dunes", "silk", "beam"][Math.floor(rnd() * 4)];
  const id = "a" + (hash(kind + seed) % 100000).toString(36);
  const parts = [];
  const defs = [];

  // The base: near-black in the hue, lit from one corner. An icon is a lit tile
  // through and through - at 48 pixels a dark corner reads as a missing picture.
  const angle = Math.floor(rnd() * 360);
  const small = kind === "icon" || kind === "avatar";
  const baseLight = small ? [38, 22] : [11, 4];
  // The near-black base of a warm picture is browner and redder than its light, the
  // way the palette's own surfaces are: a saturated dark amber is olive.
  const baseSat = neutral ? 8 : warm ? 22 : 34;
  const baseHue = warm ? hue - 12 : hue;
  defs.push(`<linearGradient id="${id}b" gradientTransform="rotate(${angle} .5 .5)"><stop offset="0" stop-color="${hsl(baseHue, baseSat + (small ? 20 : 0), baseLight[0])}"/><stop offset="1" stop-color="${hsl(baseHue + dir * 18, baseSat + 6 + (small ? 16 : 0), baseLight[1])}"/></linearGradient>`);
  // A banner is the biggest picture on the screen and carries text: lit a little more.
  const lift = kind === "banner" ? 8 : 0;
  parts.push(`<rect width="${W}" height="${H}" fill="url(#${id}b)"/>`);

  // The filter region is the whole canvas and a margin, not the element's own box: a
  // thin wash with a wide blur clipped at its box came out with a hard edge.
  const blur = (n, std) => `<filter id="${id}f${n}" filterUnits="userSpaceOnUse" x="${-W}" y="${-H}" width="${W * 3}" height="${H * 3}"><feGaussianBlur stdDeviation="${std.toFixed(1)}"/></filter>`;
  const S = Math.max(W, H);

  // A wash of light entering the frame from one edge: wide, flat, blurred past having
  // an edge - the shared vocabulary of every motif.
  const wash = (filter, h, l, opacity, side) => {
    const left = side == null ? rnd() < 0.5 : side;
    const cx = left ? W * (rnd() * 0.3 - 0.1) : W * (0.8 + rnd() * 0.3);
    const cy = H * (rnd() * 1.1 - 0.05);
    const rx = W * (0.7 + rnd() * 0.4), ry = H * (0.14 + rnd() * 0.2);
    const tilt = rnd() * 50 - 25;
    return `<ellipse cx="${cx.toFixed(0)}" cy="${cy.toFixed(0)}" rx="${rx.toFixed(0)}" ry="${ry.toFixed(0)}" fill="${hsl(h, sat + 12, l + lift)}" opacity="${Math.min(1, opacity + lift / 40).toFixed(2)}" transform="rotate(${tilt.toFixed(0)} ${cx.toFixed(0)} ${cy.toFixed(0)})" filter="url(#${filter})"/>`;
  };

  if (motif === "aurora") {
    // Three washes from the edges, two hues apart, meeting in the middle.
    defs.push(blur(1, S * (small ? 0.12 : 0.11)));
    const count = kind === "icon" ? 2 : 3;
    for (let i = 0; i < count; i++) {
      parts.push(wash(`${id}f1`, hue + (i === 0 ? 0 : warm ? -rnd() * 22 : rnd() * 70 - 35), 46 + rnd() * 18, 0.6 + rnd() * 0.3, i === 0 ? true : i === 1 ? false : null));
    }
  } else if (motif === "dunes") {
    // A glow in the sky, then three ridges from the back to the front, each lit along
    // its fold and falling into shadow below it.
    defs.push(blur(1, S * 0.12));
    defs.push(blur(2, S * 0.008));
    parts.push(wash(`${id}f1`, hue + dir * 10, 52, 0.7));
    // In a wide frame the ridges also fall from one side to the other, or they read as
    // flat bands.
    const drift = W > H * 1.6 ? H * (rnd() < 0.5 ? 0.5 : -0.5) : 0;
    for (let i = 0; i < 3; i++) {
      const y0 = H * (0.3 + i * 0.2 + rnd() * 0.08);
      const amp = H * (0.16 + rnd() * 0.12);
      const p = [y0 - drift / 2 + (rnd() - 0.5) * amp, y0 + (rnd() - 0.5) * amp * 2, y0 + (rnd() - 0.5) * amp * 2, y0 + drift / 2 + (rnd() - 0.5) * amp];
      const light = 48 - i * 12 + rnd() * 6;
      defs.push(`<linearGradient id="${id}d${i}" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="${hsl(hue + dir * i * 5, sat, light + 14)}"/><stop offset="0.12" stop-color="${hsl(hue + dir * i * 5, sat, light)}"/><stop offset="0.7" stop-color="${hsl(hue + dir * i * 5, sat - 8, Math.max(4, light - 22))}"/><stop offset="1" stop-color="${hsl(hue + dir * i * 5, sat - 12, Math.max(2, light - 34))}"/></linearGradient>`);
      const d = `M-20,${p[0].toFixed(0)} C${(W * 0.25).toFixed(0)},${p[1].toFixed(0)} ${(W * 0.5).toFixed(0)},${p[2].toFixed(0)} ${(W * 0.72).toFixed(0)},${p[3].toFixed(0)} S${(W * 0.95).toFixed(0)},${(y0 + drift / 2 + (rnd() - 0.5) * amp).toFixed(0)} ${W + 20},${(y0 + drift / 2 + (rnd() - 0.5) * amp).toFixed(0)} L${W + 20},${H + 20} L-20,${H + 20} Z`;
      // A shadow the ridge throws on the one behind it.
      parts.push(`<path d="${d}" fill="${hsl(hue, 30, 3, 0.55)}" transform="translate(0 ${(-S * 0.03).toFixed(0)})" filter="url(#${id}f1)"/>`);
      parts.push(`<path d="${d}" fill="url(#${id}d${i})" filter="url(#${id}f2)"/>`);
    }
  } else if (motif === "silk") {
    // A ribbon across the frame, lit along its length, a thinner one behind it, air
    // behind both.
    defs.push(blur(1, S * 0.12));
    defs.push(blur(3, S * 0.012));
    parts.push(wash(`${id}f1`, hue + dir * 20, 42, 0.5));
    const drift = W > H * 1.6 ? H * (rnd() < 0.5 ? 0.7 : -0.7) : 0;
    const curve = () => {
      const y0 = H * (0.2 + rnd() * 0.6) - drift / 2, amp = H * (0.25 + rnd() * 0.25);
      const y1 = y0 + (rnd() - 0.5) * amp * 2, y2 = y0 + drift + (rnd() - 0.5) * amp * 2, y3 = y0 + drift + (rnd() - 0.5) * amp;
      return `M${(-W * 0.2).toFixed(0)},${y0.toFixed(0)} C${(W * 0.3).toFixed(0)},${y1.toFixed(0)} ${(W * 0.6).toFixed(0)},${y2.toFixed(0)} ${(W * 1.2).toFixed(0)},${y3.toFixed(0)}`;
    };
    for (let i = 1; i >= 0; i--) {
      const width = i === 0 ? S * (0.22 + rnd() * 0.12) : S * (0.07 + rnd() * 0.06);
      const d = curve();
      defs.push(`<linearGradient id="${id}s${i}" x1="0" y1="0" x2="1" y2="0"><stop offset="0" stop-color="${hsl(hue - dir * 16, sat + 6, 58)}"/><stop offset="0.5" stop-color="${hsl(hue + dir * 4, sat, 40)}"/><stop offset="1" stop-color="${hsl(hue + dir * 24, sat - 4, 26)}"/></linearGradient>`);
      parts.push(`<path d="${d}" fill="none" stroke="url(#${id}s${i})" stroke-width="${width.toFixed(0)}" stroke-linecap="round" opacity="${i === 0 ? 0.96 : 0.5}" filter="url(#${id}f3)"/>`);
      // The lit edge of the fold.
      if (i === 0) parts.push(`<path d="${d}" fill="none" stroke="${hsl(hue - dir * 10, 40, 94, 0.18)}" stroke-width="${(width * 0.16).toFixed(0)}" stroke-linecap="round" transform="translate(0 ${(-width * 0.3).toFixed(0)})" filter="url(#${id}f3)"/>`);
    }
  } else {
    // beam: one wide shaft of light through the frame, a second parallel and fainter,
    // and a glow where they come from.
    defs.push(blur(5, S * 0.06));
    defs.push(blur(6, S * 0.14));
    const tilt = (22 + rnd() * 24) * (rnd() < 0.5 ? 1 : -1);
    const count = 1 + Math.floor(rnd() * 2);
    for (let i = 0; i < count; i++) {
      const x = W * (0.2 + rnd() * 0.6);
      const w = S * (i === 0 ? 0.2 + rnd() * 0.16 : 0.06 + rnd() * 0.06);
      const l = 60 + rnd() * 10;
      defs.push(`<linearGradient id="${id}m${i}" x1="0" y1="0" x2="1" y2="0"><stop offset="0" stop-color="${hsl(hue + dir * i * 14, sat + 10, l)}" stop-opacity="0"/><stop offset="0.5" stop-color="${hsl(hue + dir * i * 14, sat + 10, l)}" stop-opacity="${(i === 0 ? 0.6 + rnd() * 0.25 : 0.35).toFixed(2)}"/><stop offset="1" stop-color="${hsl(hue + dir * i * 14, sat + 10, l)}" stop-opacity="0"/></linearGradient>`);
      parts.push(`<rect x="${(x - w / 2).toFixed(0)}" y="${(-H).toFixed(0)}" width="${w.toFixed(0)}" height="${(H * 3).toFixed(0)}" fill="url(#${id}m${i})" transform="rotate(${tilt.toFixed(0)} ${x.toFixed(0)} ${(H / 2).toFixed(0)})" filter="url(#${id}f5)"/>`);
    }
    parts.splice(1, 0, wash(`${id}f6`, hue - dir * 15, 45, 0.65));
  }

  // A sheen from the lit corner and a vignette that settles everything into the frame.
  defs.push(`<linearGradient id="${id}h" gradientTransform="rotate(${angle} .5 .5)"><stop offset="0" stop-color="#fff" stop-opacity="0.09"/><stop offset="0.55" stop-color="#fff" stop-opacity="0"/></linearGradient>`);
  defs.push(`<radialGradient id="${id}v" cx="0.5" cy="0.5" r="0.75"><stop offset="0.45" stop-color="#000" stop-opacity="0"/><stop offset="1" stop-color="#000" stop-opacity="${kind === "icon" ? 0.3 : 0.55}"/></radialGradient>`);
  parts.push(`<rect width="${W}" height="${H}" fill="url(#${id}h)"/>`);
  parts.push(`<rect width="${W}" height="${H}" fill="url(#${id}v)"/>`);

  const svg = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ${W} ${H}" width="${W}" height="${H}" preserveAspectRatio="xMidYMid slice"><defs>${defs.join("")}</defs>${parts.join("")}</svg>`;
  return "data:image/svg+xml;charset=utf-8," + encodeURIComponent(svg).replace(/%20/g, " ").replace(/%3D/g, "=").replace(/%3A/g, ":").replace(/%2F/g, "/").replace(/%22/g, "'");
}

// The fixture names the templates ask for, mapped to a kind and a seed: "app-3.jpg" is
// the third cover, "exe-1.jpg" an icon, "news-2.jpg" a banner, "product-4.jpg" a photo.
const KINDS = { app: "cover", exe: "icon", news: "banner", product: "photo", media: "media", avatar: "avatar" };

function forName(name, accent) {
  const m = /^([a-z]+)-(\d+)/.exec(name);
  if (!m) return null;
  return make({ kind: KINDS[m[1]] || "cover", seed: m[1] + "-" + m[2], accent });
}

module.exports = { make, forName, hash };
