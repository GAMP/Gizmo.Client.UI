// The run-time palette derivation (grafitTheme in src/js/internal.js) run in Node: the
// same code a club's `--gg-palette: #hex` goes through, against a stub document, so a
// page can be rendered in any colour and not only the eight the build compiled.
//
//   const theme = require("./theme");
//   theme.derive("#e11d48")   -> { "--gg-accent": "#e11d48", "--gg-panel": "...", ... }
"use strict";

const fs = require("fs");
const path = require("path");

let set = null;
let props = null;

function load() {
  if (set) return;
  const src = fs.readFileSync(path.join(__dirname, "..", "..", "src", "js", "internal.js"), "utf8");
  const start = src.indexOf("// ───────────────────────────── accent palette");
  const tail = "window.grafitTheme = theme;\n})();";
  const end = src.indexOf(tail) + tail.length;
  if (start < 0 || end < tail.length) throw new Error("grafitTheme block not found in internal.js");
  const code = src.slice(start, end);

  // The block writes the tokens on document.documentElement.style; that is all the
  // document it needs.
  const style = { props: {}, setProperty(n, v) { this.props[n] = v; }, removeProperty(n) { delete this.props[n]; } };
  const root = { style, attrs: {}, getAttribute(n) { return this.attrs[n]; }, setAttribute(n, v) { this.attrs[n] = v; }, removeAttribute(n) { delete this.attrs[n]; } };
  const sandbox = {
    document: { documentElement: root, readyState: "complete", addEventListener() {} },
    getComputedStyle: () => ({ getPropertyValue: () => "" }),
    window: {},
  };
  // eslint-disable-next-line no-new-func
  new Function("document", "getComputedStyle", "window", code)(sandbox.document, sandbox.getComputedStyle, sandbox.window);
  set = sandbox.window.grafitTheme.set;
  props = style.props;
}

// Every --gg-* token for a colour ("#rrggbb", "#rgb" or "rgb(r, g, b)"), as the shell
// would write them inline on <html>. Throws when the colour is not one.
function derive(colour) {
  load();
  for (const key of Object.keys(props)) delete props[key];
  if (!set(colour)) throw new Error("Not a colour grafitTheme accepts: " + colour);
  return { ...props };
}

module.exports = { derive };
