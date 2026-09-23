// Builds the stylesheet the test pages load: the shell's SCSS compiled as it is, plus the
// layout rules that live inline in _Layout.razor, plus a few rules that exist only to
// make screenshots repeatable.
"use strict";

const fs = require("fs");
const path = require("path");
const sass = require("sass");

const ROOT = path.resolve(__dirname, "..", "..");
const SRC = path.join(ROOT, "src");

function fileUrl(p) {
  return "file:///" + p.replace(/\\/g, "/").replace(/^\/+/, "");
}

// The shell's stylesheet. Font faces point at ../font-family relative to the bundle;
// here they are turned into absolute file URLs so the fallback fonts resolve too.
function compileShell() {
  const entry = path.join(SRC, "scss", "main.scss");
  const result = sass.compile(entry, { style: "expanded", quietDeps: true, logger: sass.Logger.silent });
  const fonts = fileUrl(path.join(SRC, "font-family")) + "/";
  return result.css.replace(/url\((['"]?)\.\.\/font-family\//g, (_, q) => `url(${q}${fonts}`);
}

// _Layout.razor carries the frame's styling in a <style> block rather than in SCSS.
// It is taken from the file at run time so the harness cannot drift from it. Razor
// escapes "@" as "@@" inside that block; nothing else in it is Razor.
function layoutStyle() {
  const layout = fs.readFileSync(path.join(ROOT, "Shared", "_Layout.razor"), "utf8");
  const match = layout.match(/<style>([\s\S]*?)<\/style>/);
  if (!match) throw new Error("No <style> block found in Shared/_Layout.razor");
  const css = match[1].replace(/@@/g, "@");
  if (/@\(|@[A-Za-z]+\.[A-Za-z]/.test(css.replace(/@(media|keyframes|supports|font-face|import)\b/g, "")))
    throw new Error("The <style> block in _Layout.razor contains Razor expressions; the harness cannot use it as is");
  return css;
}

// Screenshot hygiene. Entrance animations are run to their last frame instead of being
// removed - removing them would freeze everything at frame zero, invisible. Anything
// that spins forever is parked at frame zero so two runs draw the same pixels.
const HARNESS = `
*, *::before, *::after {
  animation-duration: 0.001s !important;
  animation-delay: 0s !important;
  transition-duration: 0s !important;
  transition-delay: 0s !important;
  caret-color: transparent !important;
}
.giz-animate-spinner, .running, .giz-image-loading--activity { animation-play-state: paused !important; }
html, body { height: 100%; }
`;

function build(outDir) {
  fs.mkdirSync(outDir, { recursive: true });
  fs.writeFileSync(path.join(outDir, "style.css"), compileShell(), "utf8");
  fs.writeFileSync(path.join(outDir, "layout.css"), layoutStyle(), "utf8");
  fs.writeFileSync(path.join(outDir, "harness.css"), HARNESS, "utf8");
}

module.exports = { build, fileUrl, ROOT, SRC };
