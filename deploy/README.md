# Grafit skin - package and installer

This folder is what a server operator receives: the built skin (`skin\`) and an
installer. It is also where the skin is built into a package (`stage.ps1`).

## Install on a Gizmo Server

1. Unpack the zip anywhere on the server machine.
2. Run `install.bat` (it asks for administrator rights - the skins folder is under
   Program Files). It copies `skin\` to `<Gizmo Server>\skins\grafit\`, taking
   `wwwroot\_framework` from the server's own `Next` skin, and keeps a backup of whatever
   was in `skins\grafit` before under `backup\<timestamp>\`.
3. In the Manager, point a host group at the skin: Host groups -> the group -> Skin =
   `grafit`. (A Skin profile with custom CSS is optional - see "Colour" below.)
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
```

## Build a package (developers)

```powershell
.\stage.ps1 -Build      # dotnet build -c Release (runs npm/webpack), then stage + zip
.\stage.ps1             # stage + zip from an existing Release build
```

Output: `skin\` and `dist\grafit-shell-<grafit>-gizmo-<gizmo>.zip`, with the two
versions taken from `Gizmo.Client.UI\Gizmo.Client.UI.csproj` (`GrafitVersion`,
`GizmoVersion`). `stage.ps1` refuses a DLL whose version does not match the project
file, so a stale build cannot be packed by mistake.
