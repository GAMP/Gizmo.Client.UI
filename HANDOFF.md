# Grafit - handoff notes for Gizmo

Grafit is a second skin for the Gizmo V3 client. This repository is a fork of
`GAMP/Gizmo.Client.UI` (branch `version-3`, commit `e95eded`, the sources of Gizmo
3.0.92) with the skin built on top. `GRAFIT.md` inside `Gizmo.Client.UI\` is the
maintainer's map of the code; this file is about what is in the repository and how it
relates to yours.

## What ships and what does not

A skin is a folder in `<Gizmo Server>\skins\`, mirrored to every client PC by the server.
Grafit's folder holds exactly what the stock `Next` skin holds:

| In `skins\grafit\` | Built from |
|---|---|
| `Gizmo.Client.UI.dll` | `Gizmo.Client.UI\` in this repository - the pages, components, styles and scripts |
| `Gizmo.Web.Components.dll` | `Submodules\Gizmo.Web.Components` - with one fix, see below |
| `wwwroot\index.html`, `wwwroot\_content\Gizmo.Client.UI\...` | the webpack bundle from `Gizmo.Client.UI\src\` |
| `wwwroot\_framework\` | copied from the server's own `Next` skin at install |
| `composition.json` | written by `deploy\stage.ps1`; same content as `Next`'s |

Everything else the client runs - `Gizmo.Client.UI.Services`, `Gizmo.UI`, the resource
assembly, the WPF host - is the client's own and is not shipped. Two consequences:

- the skin must be rebuilt against every Gizmo release (the host's assemblies are what
  it links to at run time); `GizmoVersion` in the csproj says which release a build is
  for and the package name carries it;
- strings cannot be added to the resource assembly from a skin. Strings the shell adds
  or rewords live in `Gizmo.Client.UI\Localization\ShellStringOverrides.cs` and are
  compiled into the shell's DLL (`SHELL_` keys: ru + en, English fallback; vendor
  `GIZ_` keys are reused wherever one fits, so they arrive in all 11 cultures).

## Relation to the upstream repositories

Branch `grafit`. History: `e95eded` (your `version-3`) -> a snapshot of the skin as it
was before the 3.0.92 merge -> the merge -> the current state. `vendor/version-3` is
your branch as fetched, for diffing:

```
git diff vendor/version-3 grafit --stat -- Gizmo.Client.UI
```

Against `e95eded` the shell project is 396 files: 141 vendor files modified, 72 files
added (own pages, components, styles, the `visual\` harness, `deploy\`), 32 vendor
files deleted (the profile header/navigation components, the DataGrid account pages,
the unfinished Deposits page - replaced by `AccountFrame` and own row markup).

Submodules are your repositories at the commits `e95eded` pins - unchanged:

| Submodule | Commit | Checked 2026-09-13 |
|---|---|---|
| Gizmo.Client.Shared | `68359d8` | identical |
| Gizmo.Client.UI.Resources | `f45f8c4` | identical |
| Gizmo.Client.UI.Services | `5f753ac` | identical |
| Gizmo.Server.Shared | `bc6ecbd` | identical |
| Gizmo.Shared | `588b410` | identical |
| Gizmo.UI | `4df4e0c` | identical |
| Gizmo.Web.Api.Client | `568f104` | identical |
| Gizmo.Web.Api.Models | `b53d431` | identical |
| Gizmo.Web.Components | `2217ec1` | **one file changed** - see below |

Eight of them are git submodules (`git submodule update --init` after cloning).
`Gizmo.Web.Components` is committed as a plain directory because it carries the one
change to shared code:

**`Infrastructure/Components/CustomComponentBase.cs`** - adds `DispatchWorkflow(Func<Task>)`
and routes `DispatchStateHasChanged()` through it. Background: a logged production crash
- an `async void` view-state handler awaited `InvokeAsync` while the WebView2 process was
going away, the faulted dispatcher call surfaced on the thread pool and took the whole
client down. `DispatchWorkflow` runs a handler as one dispatcher work item and absorbs
only teardown exceptions (`OperationCanceledException`, `ObjectDisposedException`,
`InvalidOperationException`). The shell's components use it for every handler subscribed
to something that outlives the component. The patch is in
`deploy\patches\Gizmo.Web.Components-DispatchWorkflow.patch`; it belongs upstream, and once
it is there the directory can go back to being a submodule.

## Build, package, install

Prerequisites: .NET 10 SDK, Node.js (webpack runs from the csproj's pre-build step).

```
dotnet build Gizmo.Client.UI\Gizmo.Client.UI.csproj -c Release
deploy\stage.ps1            # or stage.ps1 -Build: stages deploy\skin\ and packs deploy\dist\grafit-shell-<grafit>-gizmo-<gizmo>.zip
```

`deploy\install.bat` (in the zip) copies the folder to `skins\grafit` with a backup;
the Manager's host group "Skin" field selects it; clients need a full restart (the skin
is handed out at connect and mirrored to `%PROGRAMDATA%\NETProjects\Gizmo Client\Skins\`).
`deploy\README.md` is the operator's page.

Versions: `GrafitVersion` and `GizmoVersion` in `Gizmo.Client.UI.csproj` -> assembly
metadata and `ProductVersion` ("1.0.7 (Gizmo 3.0.92)"), the package name,
`skin\grafit.version.txt`, and one quiet line on the account page's Profile tab.

## Integration points a club uses

All through the Manager's Skin profile -> Custom CSS, no build needed:

```css
:root { --gg-palette: purple; }     /* blue (default), purple, red, orange, amber, green, teal, pink */
:root { --gg-palette: #e11d48; }    /* any colour: every token is derived from it at run time */
:root { --gg-motion: on; }          /* slow moving gradient behind the shell; off by default */
```

The same switches exist as `data-accent` / `data-motion` on `<html>` in `index.html` and
as `window.grafitTheme.set()` / `.motion()` for a future Manager setting. Every colour
in the shell is a CSS custom property derived from the one accent (`_palette.scss`;
the JavaScript twin in `internal.js` is kept in step by `npm run visual:palette`).

## Verification

- `npm run visual` in `Gizmo.Client.UI\` renders 130+ screens from `visual\scenarios.json`
  with the compiled stylesheet and compares them with `visual\baseline\` (pixelmatch).
  `visual\README.md` explains the harness; `--update` accepts a change.
- `npm run visual:palette` - Sass vs JavaScript palette parity.
- `node visual\palette-sheet.js --page login` - one contact sheet of every palette.
- The build has no errors. Its warnings are of the kinds the vendor tree already has
  (nullable annotations CS8632/CS8669 in a project without `<Nullable>`, BL0007 on
  component parameters, NU1902/NU1903 package advisories); there are no RZ warnings -
  an unresolved component tag is only a warning in Razor and renders as an empty
  element, so that class is worth a look on every build.

## Things we know and you should too

- **Idle gate.** The host never suspends the WebView, so the shell pauses its own infinite
  animations and its polling when it loses focus (`internal.js` activity gate, `_idle.scss`).
  Any new infinite animation must be listed in `_idle.scss`.
- **`@key` on purchase rows.** `UserOrderViewState.Id` is never set by the client
  services (always 0); keying rows by it duplicated keys and killed the renderer. The
  shell keys by object. Worth fixing in `Gizmo.Client.UI.Services`.
- **Notifications window** is a separate transparent WebView2 (`NotificationsHost`):
  `backdrop-filter` paints a dark rectangle there and wide soft shadows smear, so cards
  are opaque with tight shadows.
- **`IClientDialogService`** is implemented by the skin (`ClientDialogService`); a new
  interface member in `Gizmo.Client.UI.Services` means a `TypeLoadException` on an old
  skin, not a cosmetic difference. That submodule's diff is the first thing to read on a
  vendor update.
- **No RTL.** The vendor skin has none either; the shell declares `ltr`. Full mirroring
  is ~390 directional declarations - not started.
- **`SHELL_` strings** are Russian and English only (77 keys). Other cultures get English.
- **Skin switching** in the Manager takes effect at the client's next connect, and the
  Manager's host group setting beats the server default (`ClientSettingsForHostAsync`).

## Suggested next steps on your side

1. Take `DispatchWorkflow` into `Gizmo.Web.Components` (patch above), then re-pin the
   submodule here.
2. A Manager setting for the palette and the motion switch, emitting the two CSS lines
   (or calling `grafitTheme`), so a club does not have to type CSS.
3. Fill `UserOrderViewState.Id`.
4. Translations of the `SHELL_` keys into the other nine cultures, if the skin is to be
   offered outside ru/en clubs.
