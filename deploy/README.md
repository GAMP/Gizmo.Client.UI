# Grafit skin - package and installer

This folder is what a server operator receives: the built skin (`skin\`) and an
installer. It is also where the skin is built into a package (`stage.ps1`).

## Install on a Gizmo Server

1. Unpack the zip anywhere on the server machine.
2. Run `install.bat` (it asks for administrator rights - the skins folder is under
   Program Files). It copies `skin\` to `<Gizmo Server>\skins\Grafit\`, taking
   `wwwroot\_framework` from the server's own `Next` skin, and keeps a backup of whatever
   was in `skins\Grafit` before under `backup\<timestamp>\`.
3. In the Manager, point a host group at the skin: Host groups -> the group -> Skin =
   `Grafit`. (A Skin profile with custom CSS is optional - see "Colour" below.)
4. Restart the client on the affected PCs - fully, not a re-login. The server hands the
   skin to a client when it connects and mirrors it to
   `%PROGRAMDATA%\NETProjects\Gizmo Client\Skins\` on the PC; a running client keeps the
   skin it started with.

`install.bat uninstall` puts back the last backup, or removes the folder if there was
nothing before. The stock `Next` skin is never touched.

The package is built for one Gizmo release: `skin\grafit.version.txt` says which. A skin
built for another release fails to load on the client (the host's assemblies differ).

## Colour and motion (Manager -> Skin profile -> Custom CSS)

```css
:root { --gg-palette: purple; }     /* blue (default), purple, red, orange, amber, green, teal, pink */
:root { --gg-palette: #e11d48; }    /* or any colour - the whole palette is derived from it */
:root { --gg-motion: on; }          /* slow moving gradient behind the shell; off by default */
:root { --gg-avatars: on; }         /* customers' own pictures; off by default, needs the service below */
```

## Customers' own pictures (optional)

Off unless `--gg-avatars` says otherwise, and it needs one small service on the server
machine: Gizmo 3.0.95 has no self-service picture, and reading or writing one goes
through the management API, which a station must never hold credentials for. `avatar-proxy`
in this folder keeps those credentials on the server and offers the stations a plain
`/avatar/{id}`; its own README has the three steps to install it.

```css
:root { --gg-avatars: on; }                        /* service on the Gizmo server's machine, port 8765 */
:root { --gg-avatars: "http://10.0.0.5:8765"; }    /* or wherever it runs */
```

With it on, the customer's picture appears on the profile card, the account page and the
bar, the picture itself is the way into the editor (crop, paste from the clipboard, a
link), and a customer without one is invited to add it once in a while.

## Build a package (developers)

```powershell
.\stage.ps1 -Build      # dotnet build -c Release (runs npm/webpack), then stage + zip
.\stage.ps1             # stage + zip from an existing Release build
```

Output: `skin\` and `dist\grafit-shell-<grafit>-gizmo-<gizmo>.zip`, with the two
versions taken from `Gizmo.Client.UI\Gizmo.Client.UI.csproj` (`GrafitVersion`,
`GizmoVersion`). `stage.ps1` refuses a DLL whose version does not match the project
file, so a stale build cannot be packed by mistake.
