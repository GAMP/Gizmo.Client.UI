# Gizmo Client UI — Theme Customization Guide

This document lists **every parameter of the Client UI theme that can be modified**, what
each one controls, and the two supported ways to change them.

---

## 1. How theming works (read this first)

The Client UI ships a single built-in theme (**"client"**), compiled from SCSS into a
JavaScript style bundle (`client_internal_style.js`) that is injected at startup. Every
themed element lives inside a container carrying the attribute `client-theme="true"`
(applied on `<main>`), so theme rules are scoped as `[client-theme] .giz-…`.

There are **two customization surfaces**:

| Surface | Who it's for | Recompile? | Persists to all clients? |
|---|---|---|---|
| **A. Manager custom CSS** (`style.css`) | Operators / admins | No | Yes — pushed to every client |
| **B. SCSS source variables** | Developers building the product | Yes (`npm` build) | Yes — becomes the new default |

### A. Manager custom CSS (the supported runtime override)

An admin authors CSS in the **Manager**; it is delivered to each client through
`ClientInterfaceOptions.StyleSheet` and served in-memory as `style.css`
(`InMemoryStyleSheetFileProvider`). The layouts inject it **after** the compiled theme:

```razor
@if (!string.IsNullOrEmpty(ClientInterfaceOptions.CurrentValue.StyleSheet))
{
    <HeadContent><link href="style.css" rel="stylesheet" /></HeadContent>
}
```

Because it loads last, your rules win the cascade at equal specificity. **This is the
recommended way to re-theme without rebuilding.** Every parameter below can be overridden
here by targeting the listed selector — no SCSS knowledge required. Scope your rules with
`[client-theme]` to match the theme's specificity.

> The stylesheet is referenced in three hosts: main app (`_Layout.razor`), login
> (`_Layout_Login.razor`), and web notifications (`NotificationsHost.razor`). One CSS blob
> covers all three.

### B. SCSS source variables (the compile-time default)

Editing the files under `src/scss/themes/client/` and rebuilding changes the **shipped
default** for everyone. Use this for permanent product changes, not per-venue tweaks.

---

## 2. Color parameters

Source: `_variables.scss`. To override at runtime (Surface A), set the color on the
selector shown in "Where it shows up".

### 2.1 Brand / primary

| SCSS variable | Default | Purpose | Where it shows up (override target) |
|---|---|---|---|
| `$primary-color-theme-client` | `#3F8CFF` | The master brand accent. Buttons, active nav underline, tab indicator, selected chips, checkbox fill, notification icons. | `[client-theme] .giz-button--fill.primary`, `.giz-header__modules-menu-item > a.active`, `.giz-client-tab` indicator, `[client-theme] .giz-check-box`, `[client-theme] .giz-chip-group .selected` |

> Centralized (Surface B): the 9 former hardcoded `#3F8CFF` sites and 5 hardcoded `#0091E6`
> sites now reference `$primary-color-theme-client` / `$typo-brand-theme-client`, so editing
> those two variables re-brands every primary/brand spot in one place. **Still not
> centralized:** `#0078D2` (~29 uses — a distinct secondary blue with no variable) and
> `#0091FF` (~6 uses). Do not blanket-replace those; they are deliberately different shades
> and several sit next to `#3F8CFF` in the same rule. A full re-brand via Surface A still
> needs to cover them per selector (see §6).

### 2.2 Typography colors

| SCSS variable | Default | Purpose |
|---|---|---|
| `$typo-primary-theme-client` | `#FAFAFA` | Default body / primary text |
| `$typo-secondary-theme-client` | `rgba(255,255,255,0.6)` | Secondary / muted text |
| `$typo-brand-theme-client` | `#0091E6` | Brand-colored text (outline buttons, emphasis) |
| `$typo-ghost-theme-client` | `rgba(255,255,255,0.3)` | Ghost / placeholder / disabled text |
| `$typo-link-theme-client` | `#0F9FFF` | Link color |
| `$typo-link-hover-theme-client` | `#6FA5C8` | Link hover |
| `$typo-link-mirror-theme-client` | `#57BCFF` | Mirrored/alt link color |
| `$typo-system-theme-client` | `#636E83` | System / meta text |
| `$typo-normal-theme-client` | `#009BF5` | "Normal" status text |
| `$typo-success-theme-client` | `#10AE79` | Success state (green) |
| `$typo-warning-theme-client` | `#E68200` | Warning state (amber) |
| `$typo-alert-theme-client` | `#F73B3B` | Alert state (red) |
| `$typo-caution-theme-client` | `#F8C735` | Caution (yellow) |
| `$typo-critical-theme-client` | `#62001D` | Critical (dark red) |

### 2.3 Backgrounds

| Value | Default | Purpose | Override target |
|---|---|---|---|
| `body` background | `#0C0F11` | App background base color (`_global.scss`) | `html, body` |
| `$bg-alert-theme-client` | `#F73B3B` | Alert background fill | alert/toast components |
| Wallpaper `<img>` | (dynamic) | Full-bleed background image behind the app | `.giz-background img` |

---

## 3. Elevation / shadow parameters

Source: `_variables.scss`. Control drop shadows / depth of cards, dialogs, popups.

| SCSS variable | Default | Purpose |
|---|---|---|
| `$elevation-0-theme-client` | `none` | Flat, no shadow |
| `$elevation-1-theme-client` | `0 1px 12px rgba(37,140,246,.5)` | Subtle brand glow (low elevation) |
| `$elevation-2-theme-client` | `0 5px 10px rgba(8,8,13,.3)` | Cards / raised surfaces |
| `$elevation-3-theme-client` | `0 4px 8px rgba(4,5,6,.4)` | Menus / dropdowns |
| `$elevation-4-theme-client` | `0 5px 10px rgba(8,8,13,.3)` | Dialogs |
| `$elevation-5-theme-client` | `0 20px 15px 5px rgba(4,5,6,.4)` | Modal / top-most overlays |

---

## 4. Typography parameters

Source: `_typography.scss`.

### 4.1 Font families & weights

| SCSS variable | Default | Purpose | Override target |
|---|---|---|---|
| `$font-family-header-theme-client` | `Rubik, sans-serif` | Headings (h1/h5 mixins) | `[client-theme] h1, h2, h3, h4, h5` |
| `$font-family-text-theme-client` | `'Noto Sans', sans-serif` | Body text | `body` |
| `$font-weight-light-theme-client` | `400` | Light weight |
| `$font-weight-regular-theme-client` | `500` | Regular (default) weight |
| `$font-weight-bold-theme-client` | `700` | Bold weight |

> Bundled fonts (`Rubik`, `Noto Sans`) are `@font-face`-loaded from `../font-family/*.ttf`.
> To use a different font at runtime (Surface A), declare your own `@font-face` in the
> custom CSS and set `font-family` on `body` / headings.

### 4.2 Type scale (mixins → fixed sizes)

Defined as SCSS mixins; each sets `font-size` / `line-height`. Font sizes are in `rem`,
and `1rem = 10px` at the base (see §5), so multiply by 10 for px.

| Mixin | font-size | line-height | Typical use |
|---|---|---|---|
| `font-h1` | 3.2rem (32px) | 4.0rem | Page titles |
| `font-h5` | 2.4rem (24px) | 3.0rem | Section headers |
| `font-xxl` | 2.0rem (20px) | 3.0rem | Large emphasis |
| `font-xl` | 1.8rem (18px) | 2.8rem | Sub-headers |
| `font-l` | 1.6rem (16px) | 2.6rem | Buttons / large body |
| `font-m` | 1.4rem (14px) | 2.2rem | Default body |
| `font-s` | 1.2rem (12px) | 1.8rem | Captions / meta |

> These are compile-time mixins (Surface B only). At runtime you can still resize text per
> selector via custom CSS, but the scale itself is not a single knob.

---

## 5. Global layout / root sizing

Source: `src/scss/_global.scss` (outside the theme; applies app-wide).

| Parameter | Default | Purpose | Override target |
|---|---|---|---|
| Root font-size | `10px` | Base for all `rem` units — **change this to scale the entire UI** | `html` |
| Root font-size @ ≥1921px | `11px` | Auto-upscale on large displays | `@media (min-width:1921px) html` |
| Root font-size @ ≥2561px | `13px` | Auto-upscale on 4K+ displays | `@media (min-width:2561px) html` |
| `line-height` | `1.4` | Global default line height | `html` |
| `body { user-select }` | `none` | Text selection disabled globally | `body` |

> Changing the root `font-size` is the single most powerful sizing lever: everything is in
> `rem`, so bumping `html { font-size }` proportionally scales the whole interface.

---

## 6. Component-level override targets

Every visible component has a dedicated stylesheet and a stable `giz-*` class you can
target from Manager custom CSS. Highest-value targets for re-theming:

| Component | Root class | Notable modifiers / accents |
|---|---|---|
| Button | `.giz-button` | `--fill` (commit), `--outline` (non-final), `--text`, `--progress`, `.primary`, `.disabled` |
| Card | `.giz-card` | background, border, elevation |
| App / Executable card | `.giz-app-card`, `.giz-executable-card` | grid tiles on Home/Apps |
| Header | `.giz-header` | `__logo`, `__modules-menu-item > a.active` (active nav accent + underline) |
| Tabs | `.giz-client-tab` | active color + indicator bar |
| Chip | `.giz-chip`, `.giz-chip-group .selected` | filter chips |
| Checkbox | `.giz-check-box` | checked fill + border |
| Dialog | `.giz-client-dialog`, `.giz-dialog` | modal card surface |
| Popup / Menu | `.giz-client-popup`, `.giz-menu`, `.giz-dropdown-menu` | dropdown surfaces |
| Toast / Notifications | `.giz-toast`, `.giz-menu-notifications` | status accents |
| Global search | `.giz-global-search` | search bar surface |
| Scrollbars | `.giz-scrollbar--v`, `.giz-scrollbar-slim--v`, `.giz-scrollbar--h` | track + thumb color/width |
| Background | `.giz-background`, `.giz-container` | wallpaper + main frame |

Full list of themed components (each has a `_name.scss` partial and a `giz-name` class):
avatar, badge, button, button-group, card, check-box, chip, chip-group,
circular-progress-bar, collapse, combo-button, data-grid, dialog, divider,
expansion-panel, file-input, giz-input, icon, icon-button, icon-select, list,
masked-date/phone/text-input, menu, multi-select, numeric-up-down, overlay,
password-input, percentage-stack-bar, popup, progress-bar, radio-button, select,
spinner, text-input, tooltip — plus higher-level pieces: ads-carousel, alert, app-card,
app-filters, bundled-product, carousel, checkout-dialog, dock, dropdown-menu,
executable-card, global-search, header, order, product-card, quantity-picker,
quick-launcher, section, toast, user-lock, and per-page styles (`page-home`, `page-apps`,
`page-shop`, `page-user-profile`, etc.).

---

## 7. Z-index (stacking order) parameters

Source: `_variables.scss`. Change only if overlays stack incorrectly.

| Variable | Value | Layer |
|---|---|---|
| `$info-tooltip-index` / `$app-section-header-index` | `10` | Inline tooltips, sticky headers |
| `$quick-launch-index` | `20` | Quick launcher |
| `$notifications-index` | `50` | Notifications |
| `$header-dropdown-index` / `$global-search-index` | `100` | Header dropdowns, search |
| `$tooltip-index` | `200` | Tooltips |
| `$dialog-index` / `$dialog-card-index` | `1001` / `1002` | Dialogs |
| `$lock-overlay-index` | `2000` | Lock screen |
| `$login-overlay-index` | `3000` | Login (top-most) |

---

## 8. Worked example — Manager custom CSS re-brand

Paste into the Manager's custom-CSS field. This is pure CSS (Surface A) — no rebuild.

```css
/* Re-brand from stock blue to teal. Loaded after the theme, so it wins. */
[client-theme] .giz-button--fill.primary:not(.disabled) { background-color: #0FB5A6 !important; }
[client-theme] .giz-button--outline.primary { border-color: #0FB5A6 !important; color: #4FE3D6 !important; }
.giz-header__modules-menu-item > a.active,
.giz-header__modules-menu-item > a.active::before { color: #0FB5A6; background-color: #0FB5A6; }
.giz-client-tab .active { color: #0FB5A6; }
[client-theme] .giz-chip-group .selected { background-color: #0FB5A6 !important; }
a { color: #4FE3D6; }

/* Darker surface + bigger UI */
html, body { background-color: #06110F; }
html { font-size: 11px; }               /* scale entire UI up ~10% */

/* Custom scrollbars */
.giz-scrollbar--v::-webkit-scrollbar-thumb { background: rgba(15,181,166,0.5); }
```

> **Tip — one-line global recolor:** to preview a whole different hue instantly, put
> `main[client-theme="true"] { filter: hue-rotate(160deg) saturate(1.2); }` in the custom
> CSS. Good for demos; for production prefer explicit per-component colors above.

---

## 9. Quick reference — what to change for common goals

| Goal | Change | Surface |
|---|---|---|
| Swap the brand accent color | Override `.giz-button--fill.primary` + accent selectors (§6) / edit `$primary-color-theme-client` | A / B |
| Make the whole UI bigger/smaller | `html { font-size }` | A or B |
| Change the app background | `html, body { background }` + `.giz-background` | A |
| Change fonts | `@font-face` + `body` / heading `font-family` | A / B |
| Adjust card/dialog shadows | `$elevation-*` vars or per-component `box-shadow` | A / B |
| Recolor scrollbars | `.giz-scrollbar*` thumb/track | A |
| Fix overlay stacking | `$*-index` vars | B |
```
