# Visual regression

Screenshots of the shell's screens in fixed situations, compared pixel by pixel with
the references committed in `baseline/`. The point is the merge onto every new Gizmo
release: one command shows what moved, instead of an afternoon of clicking through
screens and hoping to notice.

```
npm run visual                    render everything, compare with baseline/, write out/report.html
npm run visual -- --only buy      only scenarios whose id contains "buy" (repeat for several)
npm run visual -- --size 1366x768 only that viewport
npm run visual:update             accept what is rendered now as the new baseline
npm run visual -- --tolerance 0.1    percent of pixels allowed to differ (default 0.05)
npm run visual -- --threshold 0.05   per-pixel colour sensitivity, 0..1 (default 0.02)
```

Exit code 1 when anything differs beyond the tolerance, so a CI job can fail on it.
`out/` is git-ignored: it holds the rendered pages, the fresh screenshots, a diff image
for every changed shot, and `report.html` with baseline, current and diff side by side.

## What it needs

- Node 22 or newer (the built-in WebSocket drives Chrome; the webpack build itself
  runs on older Node) and the dev dependencies from `package.json` (`sass`,
  `pixelmatch`, `pngjs`).
- Chrome or Edge on the machine. Found automatically; point `GRAFIT_CHROME` at the
  executable if it lives somewhere unusual.

Nothing else. No browser automation package, no server, no client.

## How it works

1. `lib/css.js` compiles `src/scss/main.scss` with the same Sass the bundle uses and
   takes the `<style>` block out of `Shared/_Layout.razor`, so the pages are drawn with
   the stylesheet as it is right now.
2. `templates/*.js` build the HTML for a scenario: the shell frame (`frame.js`), the
   home board (`home.js`), the package purchase dialog (`purchase.js`), the shop
   checkout (`checkout.js`), the tariff tooltip open over the board (`tooltip.js`),
   the account page with its three tabs (`account.js`), the product page with the
   cart beside it (`product.js`), the idle sign-in screen (`login.js`). They emit the
   same class names and nesting as the Razor components - that is the whole contract.
3. Headless Chrome renders each page at each viewport (`lib/chrome.js`). Entrance
   animations are run to their last frame, anything that spins forever is parked at
   frame zero, text is rasterised without hinting or subpixel colour, so two runs on
   two machines draw the same pixels.
4. `pixelmatch` counts the pixels that differ from the baseline, anti-aliased edges
   excluded (`lib/compare.js`). The per-pixel threshold is deliberately low (0.02): a
   dark panel drifting from charcoal to a purple tint is a few units per channel, and
   at pixelmatch's default it would pass unseen.

## Scenarios

`scenarios.json` is the table of cases. One entry is:

```json
{ "id": "buy-points-short", "page": "purchase", "viewports": "dialog",
  "note": "Not enough points and no way to earn them here: the shortfall is stated, the button is dead.",
  "data": { "ways": ["balance", "points"], "selected": "points", "price": 650, "pointsPrice": 1200, "points": 800 } }
```

- `page` picks the template; `data` is what the template renders from. The data is
  the situation (prices, balances, counts, name lengths), not strings to paste - the
  template works the figures out the way the code-behind does.
- `viewports` is a named set from the top of the file (`all` = 1366×768, 1920×1080,
  2560×1440, 3440×1440; `dialog` = the two small ones, enough for a fixed-size card)
  or an explicit list.
- `note` says what the case is for. Keep it: it is printed in the report next to
  the pictures.
- `accent` (optional) renders the page in another palette, the way `data-accent` on
  `<html>` does in the skin's index.html: `"accent": "green"`.

The list leans on the edges on purpose - empty, too many, too long, nothing to pay
with - because that is where layouts break. The typical case is there for the
record; it has never been the one that failed.

To add a case: add a line. If it needs data the template does not know, teach the
template (a new key in `data`), and keep the template's markup identical to the Razor.

## When a Razor file changes

The templates mirror the markup by hand. When `Home.razor`, `PackagePurchaseDialog.razor`,
`AccountFrame.razor`, `ProductDetails.razor`, the tariff tooltip or the frame in
`_Layout.razor` changes shape, change the template with it, run
`npm run visual`, look at the report, and `npm run visual:update` once it is right.
A template that has drifted from its Razor tests nothing, so this is not optional.

The SCSS needs no mirroring: it is compiled from the source every run.

## What this does not test

It checks how things are drawn, not what they do. That the Buy button actually puts a
package in the cart, that a top-up returns to the purchase, that a slide changes every
seven seconds - none of that is here; it needs a running client and server. Vendor
components whose markup lives in compiled assemblies (`TextInput`, `Select`, `Button`)
are approximated from their class names, so a change inside them is not caught either.

Baselines are rendered with a particular Chrome build. A new build can move text by a
pixel; `includeAA` is off and there is a tolerance for exactly that, and the report
shows the diff so a human can tell a font from a layout. Text drawn over
`backdrop-filter` glass is the one thing Chrome does not rasterise identically from run
to run - a few hundred pixels on a 1366 panel - which is what the 0.05 % tolerance
absorbs. A real layout shift is ten times that at least.

## The palette parity check

`npm run visual:palette` (`visual/palette-parity.js`) compiles `_palette.scss`, runs the
`grafitTheme` block of `internal.js` against a stub DOM and compares every token of every
built-in palette between the two. They must agree to rounding: a club that names its own
colour gets the run-time derivation, a club on a built-in palette gets the compiled one,
and the shell must not look different for that reason. Run it after touching either.

## The moving background

The harness parks infinite animations at frame zero, so a baseline shows the moving
background (`_flow.scss`) at its start only. `npm run visual:flow -- --page home` (or
`--page login`, `--accent red`) renders four moments of the cycle side by side into
`visual/out/flow-*.png` - the way to judge a change to the flow as motion.

## Contact sheets and crops

- `node visual/palette-sheet.js [--page login|home] [--accent red ...] [--scale 3]` - one
  page in every palette, tiled into `out/palettes-<page>.png`. For judging the
  atmosphere or a token rule across the whole set.
- `node visual/crop.js <in.png> <out.png> <x> <y> <w> <h> [zoom]` - a region of a
  screenshot, optionally enlarged, for looking at one control or sending a detail.
  `visual/out` is wiped by every harness run, so write elsewhere anything worth keeping.
