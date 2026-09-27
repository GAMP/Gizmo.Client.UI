# Grafit skin - package and installer

This folder is what a server operator receives: the built skin (`skin\`) and an
installer. It is also where the skin is built into a package (`stage.ps1`).

## Install on a Gizmo Server

1. Unpack the zip anywhere on the server machine.
2. Run `install.bat` (it asks for administrator rights - the skins folder is under
   Program Files). It puts the skin together next to itself - `skin\` plus
   `wwwroot\_framework` and `static\` from the server's own `Next` skin (an empty
   `static\` if `Next` has none) - checks it, and only then replaces
   `<Gizmo Server>\skins\Grafit\`. Whatever was there before is kept under
   `backup\<timestamp>\`; if the copy fails half-way, it is put back.
3. In the Manager, point a host group at the skin: Host groups -> the group -> Skin =
   `Grafit`. (A Skin profile with custom CSS is optional - see "Colour" below.)
4. Restart the client on the affected PCs - fully, not a re-login. The server hands the
   skin to a client when it connects and mirrors it to
   `%PROGRAMDATA%\NETProjects\Gizmo Client\Skins\` on the PC; a running client keeps the
   skin it started with.

- `install.bat rollback` goes back to the version installed before the current one (run
  it again to go further back).
- `install.bat uninstall` goes back to what was in `skins\Grafit` before Grafit was first
  installed - usually nothing, so the folder is removed.

The stock `Next` skin is never touched.

## Versions

- **Server.** The package is built for one Gizmo release: `skin\grafit.version.txt` says
  which. A skin built for another release fails to load on the client (the host's
  assemblies differ). The installer compares it with the server's `GizmoService.dll` and
  warns on a mismatch.
- **Client.** The shell needs Gizmo Client **3.0.94 or newer** on the PCs (the `Client`
  line of `grafit.version.txt`). An older client does not start it.
- **After every Gizmo Server update, run `install.bat` again.** `wwwroot\_framework` (the
  Blazor runtime of the server version) and `static\` (the club's pictures served as
  `https://static/`, including the sign-in rotator's fallback images) are copied from
  `Next` at install time. The update refreshes `Next`, not Grafit; without a reinstall
  Grafit keeps the old `blazor.webview.js`.

## Colour and motion (Manager -> Skin profile -> Custom CSS)

Set these in the Manager, not in the installed files: a reinstall (after every server
update) replaces `skins\Grafit`, the Manager's custom CSS stays.

```css
:root { --giz-palette: purple; }     /* blue (default), purple, red, orange, amber, green, teal, pink */
:root { --giz-palette: #e11d48; }    /* or any colour - the whole palette is derived from it */
:root { --giz-motion: on; }          /* slow moving gradient behind the shell; off by default */
```

## Build a package (developers)

```powershell
.\stage.ps1 -Build      # dotnet build -c Release (runs npm/webpack), then stage + zip
.\stage.ps1             # stage + zip from an existing Release build
```

Output: `skin\` and `dist\grafit-shell-<grafit>-gizmo-<gizmo>.zip`, with the versions
taken from `Gizmo.Client.UI\Gizmo.Client.UI.csproj` (`GrafitVersion`, `GizmoVersion`,
`GizmoClientMinVersion`). `stage.ps1` refuses a DLL whose version does not match the
project file, so a stale build cannot be packed by mistake.
