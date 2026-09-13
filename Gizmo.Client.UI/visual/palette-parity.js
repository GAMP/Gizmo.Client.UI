// Checks that grafitTheme (src/js/internal.js) derives the same tokens from an accent
// as _palette.scss does at build time - the contract that lets a club name any colour
// and get the shell the build would have produced. Run from Gizmo.Client.UI:
//
//   npm run visual:palette
//
// Prints the largest channel difference over every token of every built-in palette
// (rounding only: 0.5 or less) and exercises set() with a name, junk and a colour.
// Exit code 1 when the two derivations have drifted apart.
"use strict";

const fs = require("fs");
const path = require("path");
const sass = require("sass");

const ROOT = path.join(__dirname, "..");
const css = sass.compileString(`@use "src/scss/themes/client/palette" as p;`, { loadPaths: [ROOT], style: "expanded" }).css;

// Lift the grafitTheme block out of the bundle source and run it against a stub DOM.
const src = fs.readFileSync(path.join(ROOT, "src", "js", "internal.js"), "utf8");
const start = src.indexOf("// ───────────────────────────── accent palette");
const tail = "window.grafitTheme = theme;\n})();";
const end = src.indexOf(tail) + tail.length;
if (start < 0 || end < tail.length) throw new Error("grafitTheme block not found in internal.js");
const code = src.slice(start, end);

const style = { props: {}, setProperty(n, v) { this.props[n] = v; }, removeProperty(n) { delete this.props[n]; } };
const root = { style, attrs: {}, getAttribute(n) { return this.attrs[n]; }, setAttribute(n, v) { this.attrs[n] = v; } };
global.document = { documentElement: root, readyState: "complete", addEventListener() {} };
global.getComputedStyle = () => ({ getPropertyValue: () => "" });
global.window = {};
eval(code);

function hsl2(h, s, l) {
  const c = (1 - Math.abs(2 * l - 1)) * s, x = c * (1 - Math.abs((h / 60) % 2 - 1)), mm = l - c / 2;
  let rr, gg, bb; const hh = ((h % 360) + 360) % 360;
  if (hh < 60) { rr = c; gg = x; bb = 0; } else if (hh < 120) { rr = x; gg = c; bb = 0; } else if (hh < 180) { rr = 0; gg = c; bb = x; }
  else if (hh < 240) { rr = 0; gg = x; bb = c; } else if (hh < 300) { rr = x; gg = 0; bb = c; } else { rr = c; gg = 0; bb = x; }
  return [(rr + mm) * 255, (gg + mm) * 255, (bb + mm) * 255];
}

function toRgb(v) {
  v = String(v).trim();
  let m = v.match(/^#([0-9a-f]{6})$/i); if (m) return [0, 2, 4].map((i) => parseInt(m[1].substr(i, 2), 16));
  m = v.match(/^rgb\(([^)]+)\)/); if (m) return m[1].split(",").map(Number);
  m = v.match(/^hsl\(([^,]+),\s*([^%]+)%,\s*([^%]+)%\)/); if (m) return hsl2(parseFloat(m[1]), parseFloat(m[2]) / 100, parseFloat(m[3]) / 100);
  if (/^\d/.test(v)) return v.split(",").map(Number);
  return null;
}

// The same eight accents as $gg-palettes; keep in step when one changes.
const palettes = { blue: "#4f8cff", purple: "#b06bff", red: "#ff3b46", orange: "#ff8a3d", amber: "#f0b83d", green: "#4ade80", teal: "#22d3ee", pink: "#ff5fa8" };

let worst = 0, worstAt = "", count = 0;
for (const [name, hex] of Object.entries(palettes)) {
  const match = css.match(new RegExp(":root\\[data-accent=" + name + "\\]\\s*\\{([^}]*)\\}"));
  if (!match) throw new Error("no compiled palette block for " + name);
  const rgb = toRgb(hex);
  // rgb() form rather than the name, so the run-time derivation is what gets exercised.
  if (!window.grafitTheme.set("rgb(" + rgb.join(", ") + ")")) throw new Error("set() refused " + name);
  const js = style.props;
  for (const line of match[1].split("\n")) {
    const m = line.match(/(--gg-[\w-]+):\s*([^;]+);/); if (!m) continue;
    const a = toRgb(m[2]), b = toRgb(js[m[1]]);
    if (!a || !b) { console.log("cannot compare", name, m[1], m[2], js[m[1]]); worst = 999; continue; }
    count++;
    const d = Math.max(...a.map((v, i) => Math.abs(v - b[i])));
    if (d > worst) { worst = d; worstAt = name + " " + m[1] + " sass=" + m[2] + " js=" + js[m[1]]; }
  }
}

console.log("tokens compared:", count, "- largest channel difference:", worst.toFixed(2), worstAt ? "(" + worstAt + ")" : "");
console.log("set('teal'):", window.grafitTheme.set("teal"), "->", root.attrs["data-accent"], "with", Object.keys(style.props).length, "inline tokens");
console.log("set('nonsense'):", window.grafitTheme.set("nonsense"));
console.log("set('#e11d48'):", window.grafitTheme.set("#e11d48"), "->", root.attrs["data-accent"], "panel", style.props["--gg-panel"], "soft", style.props["--gg-accent-soft"]);

process.exitCode = worst > 0.51 ? 1 : 0;
