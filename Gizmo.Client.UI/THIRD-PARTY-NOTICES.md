# Third-party notices

This skin redistributes the following third-party components. Each licence requires its
text to travel with the files, so every licence below is also shipped inside the skin at
the path given.

| Component | Version | Licence | Licence text ships at |
|---|---|---|---|
| [Manrope](https://github.com/sharanda/manrope) | 4.505 (@fontsource build) | SIL Open Font License 1.1 | `wwwroot/vendor/fonts/OFL-Manrope.txt` |
| [Space Grotesk](https://github.com/floriankarsten/space-grotesk) | 2.0 (@fontsource build) | SIL Open Font License 1.1 | `wwwroot/vendor/fonts/OFL-SpaceGrotesk.txt` |
| [Phosphor Icons](https://github.com/phosphor-icons/web) | 2.x web font | MIT | `wwwroot/vendor/phosphor/LICENSE-Phosphor.txt` |
| [flag-icons](https://github.com/lipis/flag-icons) | 7.5.0 | MIT | `wwwroot/img/flags/LICENSE-flag-icons.txt` |

## Where the files come from

Fonts and icon fonts are **self-hosted**, not fetched from a CDN: club machines can be
offline, and a shell that loses its icons because a network is down is not shippable. The
source of truth is `src/vendor/`, copied to `wwwroot/vendor/` by the webpack build, so a
clean build produces a complete skin.

The country flags are copied out of the `flag-icons` npm package at build time
(`node_modules/flag-icons/flags/4x3` → `wwwroot/img/flags`), and its licence is copied
alongside them.

## Obligations, in short

**SIL Open Font License 1.1** (Manrope, Space Grotesk) — the fonts may be bundled and
sold as part of a larger work. The licence text and copyright notice must be included,
which they are. The fonts must not be sold on their own, and a modified version may not
use the reserved font name.

**MIT** (Phosphor Icons, flag-icons) — the copyright notice and permission notice must be
included with the distribution, which they are.

Nothing here restricts commercial redistribution as part of the client shell.

## When updating a font or icon set

Replace the files under `src/vendor/`, and replace the licence text next to them from the
same release. A newer release can carry a different copyright line, and shipping the old
text is the same failure as shipping none.
