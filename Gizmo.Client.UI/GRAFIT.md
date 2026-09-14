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
skins\Grafit\
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
release it was built for: "Grafit 1.1.0 · Gizmo 3.0.95". Both numbers live in one
place, `Gizmo.Client.UI.csproj` (`GrafitVersion`, `GizmoVersion`). They reach the
DLL's version info (`ProductVersion` = "1.1.0 (Gizmo 3.0.95)"), the package name and
`grafit.version.txt` in the skin folder. On screen the shell shows its own number in
exactly one place - a quiet "Grafit 1.1.0" under the cards of the account page's
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
| Account page (Profile, Time, Purchases, Progress) | `Components\Profile\AccountFrame`, `TimeProductRow`, `Pages\Profile\*`, `_page-account.scss`, `_page-progress.scss` |
| Loyalty: ladder, achievements, challenges | `Code\Services\Loyalty.cs` (data), `LoyaltySnapshot.cs` (derived figures), `Code\LoyaltyText.cs` (words); `Components\Loyalty\*` (home tile, cards, tiles), `Pages\Profile\Progress.razor`, `Shared\LoyaltyAvatarRing`, `Shared\LoyaltyHint`; `_loyalty.scss`, `_page-progress.scss` |
| Product page | `Pages\Shop\ProductDetails.razor(.cs)`, `_page-product-details.scss` |
| Tariff tooltip in the top bar | `Shared\HeaderUserBalanceCurrentTimeProductTooltip.razor`, `_time-tooltip.scss`, `Code\TimeProductText.cs` |
| Avatar (picture or glyph, not a control) | `Shared\UserAvatar.razor`, `_user-avatar.scss` |
| Frame (top bar, rail, wallpaper rules) | `Shared\_Layout.razor` (styles inline in its `<style>`) |
| Sign-in screen | `Shared\_Layout_Login.razor`, `_layout-login.scss`, `_grafit-auth.scss` |
| Visual regression tests | `visual\` - `npm run visual`, see `visual\README.md`; `node visual\measure.js <scenario> <selector>` prints boxes and computed styles |
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
  keys to it. Shell strings live in `ShellStringOverrides` (`SHELL_` keys: ru + en in
  the main file, the client's other eight cultures - az, da, el, es, et, pt, sl, tr -
  in `ShellStringOverrides.Translations.cs`, machine-drafted and unreviewed; English
  is the fallback for anything else); vendor `GIZ_` keys are reused whenever one
  fits. A new `SHELL_` key gets all ten languages at once.
- **Images must fail visibly.** The host answers an image request only while it is
  connected and a user is signed in (it asks the server for the hash before using its
  own cache), so a request made during a dropped connection fails or hangs. `GizImage`
  therefore times out to the error placeholder after 15 s and retries every failed load
  on reconnect / sign-in, and the placeholders (`src\img\no-*-image.svg`,
  `broken-image.svg`, the loading plate) are translucent plates with a light glyph -
  the vendor's dark ones vanished on the rail and read as "the icons disappeared".
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
- **The loyalty data is one static snapshot.** `Loyalty.State` is replaced whole on
  sign-in, on every achievement push event (debounced 1.2 s) and on reconnect after a
  failed load; screens read it and re-render on `Loyalty.Changed`, nothing polls. A
  401/403 means "no user token behind this session" (a guest, or the host has not
  fetched the JWT yet): the first one is retried once after 3 s, then the feature is
  simply absent - no tab, no tile, no ring, the bar back on the home board. A 404 is
  a surface the server does not have. `IsAvailable` = the server returned at least
  one level, achievement or challenge; everything on screen is gated by it.
- **The sign-in pill is offered once per session** (`Loyalty.TakeHint`), for ten
  seconds, and only says what the ring means; the standing itself lives on the home
  tile and the tab. Next to the icons a 1366-wide bar leaves the pill a dozen rem, so
  a container query hides its words and leaves the mark and the arrow.
- **The one next step** on the home tile is chosen in `LoyaltySnapshot.NextActionOf`:
  the running challenge closest to its reward (its first unmet requirement, resolved
  to the achievement's name), else the achievement closest to being earned. Nothing
  else is listed there on purpose: the tile is a summary, the tab has the rest.

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

## Achievements, challenges, ladder (Gizmo 3.0.95)

Researched 14.09.2026 from the 3.0.95 server and client binaries (`GizmoService.dll`,
the client's `gizmoclientsetup.exe` payload) and the GAMP repositories; on screen since
Grafit 1.1.0. The stock skin draws none of it. What the shell shows:

- **Home board** (`LoyaltySummary`): with the feature on, the right column is two tiles
  that line up with the left one - packages level with the hero, the summary level with
  the strip - and the bar leaves the board (it is one click away in the shop). The
  summary: the level with its ring and mark, one line on where the customer stands, the
  bar to the next level, the one next step with what it pays, a button to the tab. On a
  short screen (≤ 820 px high) the next step hides.
- **Progress tab** (`Pages\Profile\Progress`): the level card (emblem in a ring, state
  chip, one sentence, the rail of levels with the current and the next threshold, perks),
  challenge cards (steps done, the one step left, rewards, pool left, "reward waiting at
  the counter"), achievement tiles (ring of this period's progress, earned check, secrets),
  the last level change.
- **Top bar**: a ring around the avatar (progress to the next level) with the level's
  mark, and the pill next to it after sign-in.
- **Notifications**: achievement earned, challenge completed, level changed, reward
  waiting / given - one alert each through `IClientNotificationService`, so a missed one
  lands in the bell.

The harness mirrors all of it: scenarios `home-loyalty`, `home-loyalty-secured`,
`progress`, `progress-secured`, `progress-ladder-only`.

**Data - REST, user surface, already authenticated.** `Gizmo.Client.UI.Services`
registers every client class of `Gizmo.Web.Api.Client` as a typed `HttpClient` with the
server's base address (`ClientNetworkOptions.ServerUri`) and a delegating handler that
adds the signed-in user's bearer token (the host receives the JWT at login and hands it
to `UserAccessTokenHandler`; the stock cart checkout relies on the same handler). The
user surface shares its class names with the operator surface, so `Loyalty.cs` aliases
`Gizmo.Web.Api.User.Clients.*` and resolves them from a scope:

| Client (`Gizmo.Web.Api.User.Clients`) | Route | Returns |
|---|---|---|
| `AchievementsWebApiClient.GetAchievementsAsync()` | `GET api/user/v3/achievements` | `UserAchievementsModel` - every achievement with state, target, current value, progress, completions, period window, `IsHidden`, image guid |
| `AchievementsWebApiClient.GetChallengesAsync()` | `GET api/user/v3/achievements/challenges` | `UserAchievementChallengesModel` - challenges with requirements + progress, rewards (points / time / product), own completions and reward statuses, state, pool remaining |
| `AchievementLadderWebApiClient.GetStandingAsync()` | `GET api/user/v3/ladder/standing` | `LadderStandingModel` - mode (points / requirements), state (Earning / Secured / Awaiting), period, score, current and projected rank, every level with threshold, perks (discount, waiting-line priority) and emblem guid, transitions |
| `AchievementLadderWebApiClient.GetEventsAsync(filter)` | `GET api/user/v3/ladder/events` | paged level history |
| (no typed method in the shipped client) | `GET api/user/v3/achievements/rewards` | paged `UserAchievementRewardModel` - use `IHttpClientFactory.CreateClient("Gizmo.Web.Api.Clients.Secure")` |

Models: `Submodules\Gizmo.Web.Api.Models\Models\API\Request\Achievement*`,
`AchievementLadder`, `AchievementChallenge`; enums `UserAchievementState`,
`UserAchievementChallengeState`, `LadderStandingState`, `AchievementChallengeRewardStatus`,
`SignalUnit` (count / currency / duration / points / days), `CalendarPeriod`.

**Live updates - push, through the host.** The server evaluates achievements on entity
events (session, order, deposit...) with a one-second buffer and a five-minute sweep,
and sends the user's client `UserAchievementCompletedEventMessage`,
`UserAchievementChallengeCompletedEventMessage`, `UserAchievementLevelChangedEventMessage`
and `UserAchievementRewardStatusChangedEventMessage` (`Gizmo.Web.Api.Messaging`). The host
raises `IGizmoClient.OnAPIEventMessage` for every API event it receives, so the skin
subscribes there (the vendor's `AssistanceRequestViewService` is the pattern), refreshes
the cached models and shows a notification. No polling needed.

**Images.** `{ServerUri}/files/{guid}` serves any uploaded file by guid - achievement and
challenge pictures (`ImageGuid`), level emblems (`EmblemGuid`) - public, correct
content-type, `Cache-Control: immutable`, SVG allowed. A plain `<img>` works.

**Caveats.** Guests have no user token: the feature is absent for them (`IsGuest` at
sign-in, and 401 as the backstop). A club with no ladder / no achievements gets empty
models - nothing is rendered, not an empty tab. `IsHidden` achievements are secret until
earned. Level names are user groups (`ToUserGroupId`): a level change is also a
pricing/perks change, which the balance and tariff already reflect through their own
events. The progress figures (`Progress` on levels, achievements, challenges) are read
as a fraction, a value above one as a percentage (`LoyaltySnapshot.Fraction`), because
the contract does not say which; `CurrentValue / TargetValue` is preferred whenever the
server sends both.

**Not verified on a live server yet** (no club with the feature configured was reachable
at build time; the routes answer 401 without a token as expected): the exact ladder
semantics - retain threshold vs `ProjectedRank`, `PromoteOnSettleOnly`, `IsStepwise` -
and the wording that follows from them (`LoyaltyLines`). First live run: sign in as a
member of a group with a ladder, check the tile, the tab and the pill, earn one
achievement and watch for the toast and the refresh; the client log carries
`Grafit.Loyalty` warnings for any read that failed.

**Prerequisite done 14.09:** submodules at the 3.0.95 client's commits
(`Gizmo.Web.Api.Client` 279fea2 has the user clients, `Gizmo.Web.Api.Models` e9fe1536 the
models).

## Not in the box

Behaviour is tested by hand on a live client; the visual tests cover layout only. The
`SHELL_` strings are written in Russian and English; the client's other eight cultures
carry machine-drafted translations that nobody has reviewed (see "Strings" above).
