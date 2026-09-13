# Grafit

A second skin for the Gizmo V3 client, built from this fork of `Gizmo.Client.UI`. It
ships as a folder next to the stock `Next` skin and is selected per host group in the
Manager. This file is the map for whoever maintains it; the vendor's own `readme.md`
still describes the original project.

## What it is, in one paragraph

The same Blazor application as the stock skin (`Gizmo.Client.UI.dll` +
`Gizmo.Web.Components.dll`, listed in `composition.json`), with its own pages, styles
and a few services. Everything else - `Gizmo.Client.UI.Services`, the resource
assembly, the host - is the client's own and is not shipped. The skin therefore has to
be rebuilt against every Gizmo release; the folder only works on a server of the version
it was built for.

## Folder layout of the skin (what the installer copies)

```
skins\grafit\
  composition.json                 same content as Next's
  Gizmo.Client.UI.dll              this project
  Gizmo.Web.Components.dll         Submodules\Gizmo.Web.Components
  wwwroot\
    index.html                     src\html\index.html - data-accent + font links
    _framework\                    taken from the server's own Next skin at install
    _content\Gizmo.Client.UI\      the webpack bundle: JS, CSS, images, fonts, licences
```

## Build

```
npm install
npm run build_prod                 webpack: src\ -> wwwroot\
dotnet build Gizmo.Client.UI.csproj -c Release
```

`dotnet build` runs the npm steps itself (see the csproj), so the second line alone is
enough. Output: `bin\Release\net10.0\Gizmo.Client.UI.dll` and `wwwroot\`.
`..\deploy\stage.ps1 -Build` (the `deploy` folder at the repository root) builds, lays
the skin folder out and packs `grafit-shell-<grafit>-gizmo-<gizmo>.zip`;
`deploy\install.bat` puts the folder on a server. `..\HANDOFF.md` explains the
repository's relation to the upstream one.

## Versions

The shell has a version of its own and it is only meaningful next to the Gizmo
release it was built for: "Grafit 1.0.7 · Gizmo 3.0.92". Both numbers live in one
place, `Gizmo.Client.UI.csproj` (`GrafitVersion`, `GizmoVersion`). They reach the
DLL's version info (`ProductVersion` = "1.0.7 (Gizmo 3.0.92)"), the package name and
`grafit.version.txt` in the skin folder. On screen the shell shows its own number in
exactly one place - a quiet "Grafit 1.0.7" under the cards of the account page's
Profile tab (`ShellVersion.Grafit`, read from the assembly metadata) - and never the
Gizmo release or any mismatch warning. Bump `GrafitVersion` for every shell release,
`GizmoVersion` on every vendor merge; `stage.ps1` refuses to pack a DLL whose
version does not match the project file.

## Where things are

| What | Where |
|---|---|
| Versions (shell + Gizmo release) | `Gizmo.Client.UI.csproj` properties; `Code\ShellVersion.cs`; `..\deploy\stage.ps1` |
| Atmosphere (the static background) | `--gg-atmosphere` in `_palette.scss`; contact sheet `node visual\palette-sheet.js --page login` |
| Palette derivation at run time (any colour) | `src\js\internal.js`, `grafitTheme`; parity test `npm run visual:palette` |
| Moving background (sign-in and shell) | `_flow.scss`; markup in `Shared\_Layout_Login.razor` and `Shared\_Layout.razor`; preview `npm run visual:flow` |
| Palette and every colour token | `src\scss\themes\client\_palette.scss` |
| Palette switch at run time | `src\js\internal.js`, `grafitTheme` |
| Palette shared with the notifications window | `Code\Services\ShellTheme.cs` |
| Strings the shell adds or rewords | `Localization\ShellStringOverrides.cs` |
| Idle gate (the shell stops working when unfocused) | `src\js\internal.js` activity gate, `Shared\ShellActivityWatcher.razor`, `src\scss\_idle.scss` |
| Home board | `Pages\Home.razor(.cs)`, `_page-home.scss` |
| Package purchase and shop checkout | `Components\Shop\CartDialogBase.cs`, `PackagePurchaseDialog`, `CheckoutDialog`, `PayWayChooser`, `CartDialogResult` |
| Account page (Profile, Time, Purchases) | `Components\Profile\AccountFrame`, `TimeProductRow`, `Pages\Profile\*`, `_page-account.scss` |
| Product page | `Pages\Shop\ProductDetails.razor(.cs)`, `_page-product-details.scss` |
| Tariff tooltip in the top bar | `Shared\HeaderUserBalanceCurrentTimeProductTooltip.razor`, `_time-tooltip.scss`, `Code\TimeProductText.cs` |
| Avatar (picture or glyph, not a control) | `Shared\UserAvatar.razor`, `_user-avatar.scss` |
| Frame (top bar, rail, wallpaper rules) | `Shared\_Layout.razor` (styles inline in its `<style>`) |
| Sign-in screen | `Shared\_Layout_Login.razor`, `_layout-login.scss`, `_grafit-auth.scss` |
| Visual regression tests | `visual\` - `npm run visual`, see `visual\README.md` |
| Package, installer, patches for upstream | `..\deploy\` (`stage.ps1`, `install.bat`, `patches\`) |
| Third-party licences | `THIRD-PARTY-NOTICES.md`, `src\vendor\...` |

## Rules that are not obvious from the code

- **Colours.** No component file may contain a literal from the accent's family. Use
  the tokens: `var(--gg-accent)`, `rgba(var(--gg-accent-rgb), a)`, `var(--gg-panel)`,
  `var(--gg-ink)`, `rgba(var(--gg-glass-rgb), a)`, … Every token, surfaces and ink
  included, is derived from the palette's one accent by Sass; warm accents get a
  lighter surface tint so their near-blacks stay charcoal. To add a palette, add one
  line to `$gg-palettes` and the name to the list in `grafitTheme`.
- **The avatar is not a control.** The shell cannot change a person's picture, so
  nothing may hint that it can: `UserAvatar` shows the picture or a plain glyph.
- **Choosing the palette.** `data-accent` on `<html>` is the only switch, and only
  `grafitTheme` writes it: from `index.html` (skin default: `blue`), from a club's custom CSS
  in the Manager (`:root { --gg-palette: green; }`, read after `style.css` loads), or
  from `grafitTheme.set(value)`. The value may also be a colour (`#e11d48`,
  `rgb(…)`): then `grafitTheme` derives every token with the rules of
  `_palette.scss`, ported to JavaScript, and writes them inline on `<html>` under
  `data-accent="custom"`. The two derivations must stay identical - change a rule in
  one, change it in the other, and run `npm run visual:palette`. A Manager setting
  for the colour, should one be added, only needs to deliver that value to the page.
- **Motion is transform-only, gated, and off by default.** The moving background
  (`.gg-flow`) exists only when a club turns it on (`:root { --gg-motion: on; }`,
  `grafitTheme.motion("on")`, or `data-motion="on"` on `<html>`); it and every other
  infinite animation animate `transform`/`opacity` only, are listed in
  `src\scss\_idle.scss` and stop when the shell loses focus. Nothing with
  `backdrop-filter` may cover the whole background (it would be re-blurred every
  frame). No video backgrounds: a decoder is a standing cost, a composited layer is not.
- **`@key` needs a real key.** The vendor leaves ids at zero on some view states
  (orders, for one); two siblings with the same key make Blazor throw on the next
  re-render and the renderer dies - animations keep running, nothing reacts. Key by the
  object when in doubt.
- **The notifications window is transparent.** No `backdrop-filter` and no wide soft
  shadows in anything rendered there: the first paints a dark rectangle, the second a
  smudge over whatever is on screen. Cards are opaque.
- **Wallpapers.** A club picture (`ClientInterfaceOptions.Background` /
  `LoginBackground`) is shown when configured; otherwise the shell paints its own
  atmosphere (`--gg-atmosphere`) rather than the vendor's stock photograph. While the
  sign-in card is up, the shell's own gradient covers any club picture.
- **The atmosphere is one hue.** Only the accent is painted into the background
  (corner washes and a diagonal sweep, no discs); the companion hues
  `--gg-accent-2/3-rgb` are derived for a club's own CSS but the shell does not use
  them. Two hues in the field read as foreign colours ("red and yellow on a purple
  shell"); accent-only was the accepted answer. Judge a change to it on the whole
  set: `node visual\palette-sheet.js`.
- **Progress has no moving edge.** The deployment banner reads progress on a ring
  around its badge plus a feathered wash; while the size is unknown the ring turns.
  A hard-edged fill with a rim, and a segment sliding along the pill, read as "a
  square moved along a strip" and were replaced.
- **Strings.** The resource assembly belongs to the client, so the skin cannot add
  keys to it. Shell strings live in `ShellStringOverrides` (`SHELL_` keys, ru + en,
  English fallback); vendor `GIZ_` keys are reused whenever one fits.
- **`async void` is fatal.** An unobserved exception on a handler exits the whole
  client. Anything subscribed to a view state or a static event goes through
  `DispatchWorkflow`, and every `Dispose` unsubscribes.
- **The host never suspends the WebView.** Everything that runs on a timer or an
  infinite animation must stop when the shell loses focus - `ShellActivity` for timers,
  `_idle.scss` for animations.
- **Only the two listed assemblies load from the skin.** Anything reached through
  `Gizmo.Client.UI.Services` is the host's copy; the skin cannot add members to
  interfaces there (`IClientDialogService` in particular - the concrete
  `ClientDialogService` carries the shell's own dialogs).

## Updating to a new Gizmo release

The vendor repository is `github.com/GAMP/Gizmo.Client.UI`, branch `version-3`, fetched
here as `vendor/version-3`. Merge the new release commit into `grafit` (eight
`Submodules` are gitlinks and follow the vendor's pins; `Gizmo.Web.Components` is a
plain directory carrying one patch - bring its files to the new pin by hand and re-apply
`deploy\patches\...`), build with `--no-incremental`, run `npm run visual`, look at the
report, accept the baselines that moved on purpose. The area that actually breaks
between releases is `Gizmo.Client.UI.Services`: check its diff first. Read the build's
warnings for the shell's own files: an unresolved component tag is only a warning
(RZ10012) and renders as an empty HTML element.

## Not in the box

Behaviour is tested by hand on a live client; the visual tests cover layout only. The
`SHELL_` strings are translated to Russian and English; the client's other nine
cultures fall back to English for them.
