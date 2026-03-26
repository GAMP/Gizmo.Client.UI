# Codebase Concerns

**Analysis Date:** 2026-03-26

---

## Tech Debt

**Demo-only runtime shipped as default:**
- Issue: `Program.cs` registers `IGizmoClient` as `TestClient` — a hybrid demo/live client that requires `Network:ServerUri` to be set. Without it, 56+ operations throw `InvalidOperationException` at runtime. The WPF host still uses the older `DemoClient`.
- Files: `Gizmo.Client.UI.Host.Web/Program.cs` (line 49), `Gizmo.Client.UI.Host.WPF/App.xaml.cs` (line 40), `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Client/TestClient.cs`
- Impact: Any feature that calls a `ThrowLiveOnlyNotImplementedAsync`-backed method will crash at runtime when `ServerUri` is not configured. Affects: user agreements, user profile update, payment methods, personal files, password update, registration flow, reservations, usage session, online deposit, most profile operations.
- Fix approach: Configure `Network:ServerUri` in `appsettings.json` for any live deployment, OR implement a proper `DemoClient` fallback path for every `ThrowLiveOnlyNotImplementedAsync` stub.

**`IsFullScreen` not implemented on `TestClient`:**
- Issue: `TestClient.IsFullScreen` throws `NotImplementedException` unconditionally.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Client/TestClient.cs` (line 65)
- Impact: Any component that reads `IGizmoClient.IsFullScreen` will throw at runtime.
- Fix approach: Return `false` as a safe stub, or implement via JS interop.

**`WebLocalizationService.GetSupportedCulturesAsync` is a hardcoded stub:**
- Issue: The method has a `// TODO: FOR EXAMPLE ONLY, REMOVE THIS` comment and returns a hardcoded list of 3 cultures (`en-US`, `el-GR`, `ru-RU`).
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Services/WebLocalizationService.cs` (line 29)
- Impact: Supported cultures are not driven by configuration or backend; other locales in `Gizmo.Client.UI.Resources` (Azerbaijan, Danish, Slovenian, Estonian, etc.) are unreachable at runtime.
- Fix approach: Remove stub and implement by reading available cultures from `Gizmo.Client.UI.Resources` assembly or a configuration key.

**DataGrid virtualization is declared but not implemented:**
- Issue: `DataGrid.razor.cs` includes the `Virtualization` namespace and an `IsVirtualized` branch, but the branch body is empty (`//TODO: Virtualization`).
- Files: `Gizmo.Client.UI/Components/Common/DataGrid.razor.cs` (line 545)
- Impact: DataGrid always renders non-virtualized. Purchases, Products, and Deposits pages that display potentially large lists will degrade with many rows.
- Fix approach: Implement the `IsVirtualized` rendering path using `Microsoft.AspNetCore.Components.Web.Virtualization.Virtualize<T>`.

**Cart order error handling is fully commented out:**
- Issue: The entire error-state handling block in `UserCartViewService` (lines 219–256) is commented out. Order failure state is silently swallowed — `ViewState.HasError` is never set to `true`.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/UserCartViewService.cs` (lines 219–256)
- Impact: If a checkout order fails, the user sees no error message or feedback.
- Fix approach: Uncomment and complete the error-state handling block.

**Checkout payment method filter uses magic number `-4`:**
- Issue: `CheckoutDialog.razor.cs` filters out payment methods with `a.Id != -4`. No constant, comment, or documentation explains what ID `-4` represents.
- Files: `Gizmo.Client.UI/Components/Shop/CheckoutDialog.razor.cs` (line 53)
- Impact: Fragile — if the server-side payment method ID convention changes, the filter silently breaks. Maintainers cannot understand the intent.
- Fix approach: Replace `-4` with a named constant or enum value from `Gizmo.Shared`.

**`HostReservationViewService` does not handle payment status change events:**
- Issue: Explicit `TODO` comment states that `ReservationPaymentStatusChangeEventMessage` is unhandled. The `_nextReservation` state remains stale (unpaid) after a payment completes.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/HostReservationViewService.cs` (lines 447–451)
- Impact: After the user pays for a reservation through the payment dialog, the reservation UI may continue to show the reservation as unpaid until a page reload or another event fires.
- Fix approach: Subscribe to and handle `ReservationPaymentStatusChangeEventMessage` (or equivalent) in the event dispatch method.

**`UserLoginStatusViewService` navigation uses a "temporary fix" for mouse buttons:**
- Issue: A comment marks a nav workaround as `//TODO temprary fix, we need to fix the mouse buttons problem`. The exact nature of the underlying mouse button bug is undocumented.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/UserLoginStatusViewService.cs` (line 232)
- Impact: Workaround may interact unexpectedly with new module additions or layout changes.
- Fix approach: Investigate and resolve the original input event issue rather than patching navigation.

**`ClientServerCartViewService` is missing debounce and promotion code caching:**
- Issue: Two adjacent `TODO` comments note that (a) `ViewState.RaiseChanged()` calls should be debounced and (b) promotion code state should be cached to avoid redundant API calls on every cart refresh.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/ClientServerCartViewService.cs` (lines 382–434)
- Impact: Every cart state change triggers unbounded re-renders and potentially repeated promo-code lookups against the API.
- Fix approach: Introduce a debounce timer before `RaiseChanged()` and cache promo code state keyed by `PromoCodeId`.

**`GlobalSearchViewService` has unexplained `RaiseChanged` call:**
- Issue: A `//TODO: Pestunov: Why is this here?` comment on `ViewState.RaiseChanged()` in the "view all" navigation handler indicates unreviewed behaviour.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/GlobalSearchViewService.cs` (line 88)
- Impact: May cause redundant renders or subtle state timing issues.
- Fix approach: Review and either document the intent or remove the call.

---

## Known Bugs

**Registration and password recovery flows do not show success messages:**
- Symptoms: After a successful password recovery or registration confirmation, the service navigates to Login without displaying a success notification or toast.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/UserPasswordRecoverySetNewPasswordViewService.cs` (line 81), `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/UserRegistrationAdditionalFieldsViewService.cs` (line 174)
- Trigger: Complete the password recovery set-new-password step or the registration confirmation step successfully.
- Workaround: None; user must infer success from the navigation to the login page.

**Product card hover rendering broken after `content-visibility` events:**
- Symptoms: `ProductTimeCard`, `ProductSimpleCard`, and `ProductBundleCard` have explicit `//TODO` comments noting a rendering problem triggered by `mouseover`.
- Files: `Gizmo.Client.UI/Components/Shop/ProductTimeCard.razor.cs` (line 13), `Gizmo.Client.UI/Components/Shop/ProductSimpleCard.razor.cs` (line 13), `Gizmo.Client.UI/Components/Shop/ProductBundleCard.razor.cs` (line 14)
- Trigger: Hover over product cards in the shop.
- Workaround: Unknown.

**`UserProfileViewState.RegistrationDate` is never populated:**
- Symptoms: The profile page does not display the user's registration date.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/UserProfileViewService.cs` (line 39)
- Trigger: Visit `/profile` when logged in.
- Workaround: None; field is silently omitted.

**`AppsPageViewService` filter recreation on localization change is inefficient:**
- Symptoms: Filters are fully recreated (not just translated) on each localization change because the localized string is baked into filter objects.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/AppsPageViewService.cs` (line 87)
- Trigger: Changing app language while on the apps page.
- Workaround: None currently.

---

## Security Considerations

**Trace-level logging enabled globally in production builds:**
- Risk: `SetMinimumLevel(LogLevel.Trace)` is set unconditionally in both `Program.cs` (web) and `App.xaml.cs` (WPF). Trace logging in WASM sends all log output to browser DevTools, potentially exposing request details, tokens, or user data.
- Files: `Gizmo.Client.UI.Host.Web/Program.cs` (line 34), `Gizmo.Client.UI.Host.WPF/App.xaml.cs` (line 51)
- Current mitigation: None.
- Recommendations: Set minimum level to `LogLevel.Warning` for Release builds; use conditional compilation or environment checks.

**`appsettings.json` contains localhost endpoint defaults checked into source:**
- Risk: `ApiEndpoint`, `RealTimeEndpoint`, and `Network:ServerUri` are all set to `http://localhost` variants in the committed file. An operator who forgets to override these settings will silently connect to localhost.
- Files: `Gizmo.Client.UI.Host.Web/wwwroot/appsettings.json`
- Current mitigation: `.env` file is gitignored for docker compose secrets.
- Recommendations: Use clearly invalid placeholder values (e.g., `https://CONFIGURE_THIS_VALUE`) in the committed config to prevent silent misconfiguration.

**JWT parsing implemented manually without signature verification:**
- Risk: `TestClient.TryGetUserIdFromJwt()` manually Base64-decodes the JWT payload to extract the user ID without verifying the token signature.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Client/TestClient.cs` (lines 792–826)
- Current mitigation: This is used internally to resolve `_currentUserId` from an access token already obtained from the server. Server-side validation still applies.
- Recommendations: Document the intent and scope clearly, or switch to `System.IdentityModel.Tokens.Jwt` for structured parsing if this pattern is extended.

---

## Performance Bottlenecks

**`DemoClient` is 2103 lines, `TestClient` is 1029 lines — monolithic client files:**
- Problem: Both client implementations are giant partial classes with all methods inline. Finding and modifying individual operations requires navigating large files.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Client/DemoClient.cs`, `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Client/TestClient.cs`
- Cause: No domain-based file splitting.
- Improvement path: Split into partial class files per domain area (applications, products, users, reservations, etc.).

**`DataGrid.razor.cs` is 1124 lines without virtualization:**
- Problem: Large list rendering in Purchases, Products, Deposits pages has no row virtualization.
- Files: `Gizmo.Client.UI/Components/Common/DataGrid.razor.cs`
- Cause: `IsVirtualized` path is a TODO stub.
- Improvement path: Implement the virtualized rendering path before deploying to environments with many rows.

**Cart server polling lacks debounce:**
- Problem: `ClientServerCartViewService` calls `ViewState.RaiseChanged()` and potentially re-fetches promo code state on every cart mutation without any debounce.
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/ClientServerCartViewService.cs` (lines 382–434)
- Cause: Acknowledged in TODO comments.
- Improvement path: Add a debounce timer (e.g., 250ms) before `RaiseChanged()` in the cart refresh loop.

---

## Fragile Areas

**ViewService/ViewState naming convention is load-bearing:**
- Files: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/ServiceCollectionExtensions.cs`
- Why fragile: `AddClientViewServices()` and `AddClientViewStates()` register types by scanning for the `*ViewService` / `*ViewState` suffix. Renaming, moving, or creating types without these suffixes silently breaks DI registration.
- Safe modification: Always append `ViewService` or `ViewState` to any new service/state class in this pattern. Never rename existing classes without updating the suffix.
- Test coverage: No automated tests exist for DI registration correctness.

**`appsettings.json` comment syntax is non-standard JSON:**
- Files: `Gizmo.Client.UI.Host.Web/wwwroot/appsettings.json` (line 20: `//https://learn.microsoft.com/...`)
- Why fragile: JSON spec does not allow `//` comments; this works only because the .NET `Microsoft.Extensions.Configuration` JSON parser allows them. Standard JSON validators and linters will reject this file, and some CI tools may fail.
- Safe modification: Replace inline comments with documentation outside the JSON file or use the `//` comment pattern consistently, knowing it is a .NET extension.

**Webpack prebuild runs `npm install` on every MSBuild invocation:**
- Files: `Gizmo.Client.UI/Gizmo.Client.UI.csproj` (PreBuild target)
- Why fragile: Every `dotnet build` triggers `npm install`, slowing CI builds and introducing network dependency for offline builds. If npm registry is unavailable, the build fails entirely.
- Safe modification: Add a lockfile check (`npm ci` instead of `npm install`) or gate the npm step on a file change sentinel.

**Module composition depends on `AdditionalAssemblies` in `appsettings.json`:**
- Files: `Gizmo.Client.UI.Host.Web/wwwroot/appsettings.json` (`UIComposition.AdditionalAssemblies`)
- Why fragile: If an assembly is renamed or removed but the config is not updated, the router silently ignores the missing assembly. No validation or startup error is raised.
- Safe modification: When adding or renaming assemblies referenced in `AdditionalAssemblies`, always update this config.

---

## Scaling Limits

**WASM payload includes all assemblies on first load:**
- Current capacity: All route assemblies and submodule DLLs are loaded at startup.
- Limit: The more assemblies referenced in `AdditionalAssemblies`, the larger the initial WASM download for the client. Currently includes `Gizmo.Web.Components.dll` plus all transitive dependencies.
- Scaling path: Lazy assembly loading via `LazyAssemblyLoader` in Blazor WASM for module-specific pages.

---

## Dependencies at Risk

**`sass-loader` version `^12.3.0` is outdated (current is 16.x):**
- Risk: `sass-loader` 12 targets Webpack 5 but is several major versions behind. It may have known vulnerabilities and lacks compatibility with current `sass` APIs.
- Files: `Gizmo.Client.UI/package.json`
- Impact: Build warnings or future incompatibilities with `sass` ^1.63.0 (which continues to evolve its API).
- Migration plan: Update `sass-loader` to 14+ and test for breaking changes in SCSS compilation output.

**`copy-webpack-plugin` version `^9.0.1` is outdated (current is 12.x):**
- Risk: Three major versions behind; may lack security patches.
- Files: `Gizmo.Client.UI/package.json`
- Impact: Non-blocking currently but accumulates upgrade cost over time.
- Migration plan: Upgrade alongside `sass-loader` in a single pass.

**`file-loader` is deprecated:**
- Risk: `file-loader ^6.2.0` is deprecated in favour of Webpack 5 built-in Asset Modules.
- Files: `Gizmo.Client.UI/package.json`
- Impact: Will log deprecation warnings; may stop receiving security updates.
- Migration plan: Replace `file-loader` with `type: 'asset/resource'` in webpack rule config.

---

## Missing Critical Features

**Real-time / SignalR connection not implemented:**
- Problem: `UIComposition.RealTimeEndpoint` is declared in `appsettings.json` and wired into `UICompositionOptions`, but no SignalR `HubConnection` client exists in the codebase. Server-push events (balance changes, reservation changes, etc.) are only simulated via `DemoClient`/`TestClient` event properties.
- Blocks: Live balance updates, live reservation state changes, live app execution context updates — all features that depend on `IGizmoClient` events being fired from server push.

**`Leaderboard` component is disabled in App Details:**
- Problem: `<Leaderboard />` is commented out in `Pages/Apps/AppDetails.razor` (line 155).
- Blocks: The leaderboard feature is non-functional; a `<div class="giz-app-details__leaderboard">` placeholder remains in the DOM with no content.

**Online deposit dropdown panel is disabled:**
- Problem: `<MenuUserOnlineDepositContainer />` is commented out in `Shared/HeaderUserMenuUserOnlineDeposit.razor` (line 9). The button renders and opens the container wrapper with the `open` CSS class, but the dropdown panel itself is absent.
- Blocks: Users who click the deposit icon in the header see an empty open container.

**Profile edit buttons are disabled:**
- Problem: Edit buttons for Profile, Email, and Mobile fields are all commented out in `Pages/Profile/Profile.razor` (lines 34, 57, 72).
- Blocks: Users cannot trigger `ChangeProfileDialog`, `ChangeEmailDialog`, or `ChangeMobileDialog` from the Profile page UI, even though the dialog implementations exist.

**9-digit phone numbers are unhandled in recovery and registration:**
- Problem: Acknowledged `TODO` notes that 9-digit phone numbers may not validate correctly in `UserPasswordRecoveryViewService` (line 144) and `UserRegistrationConfirmationMethodViewService` (line 159).
- Blocks: Users with 9-digit phone numbers may be unable to complete password recovery or registration by phone.

---

## Test Coverage Gaps

**No test projects exist in this repository:**
- What's not tested: All pages, all view services, all view states, lookup services, client implementations, component logic, and the asset pipeline.
- Files: Entire `Gizmo.Client.UI/`, `Gizmo.Client.UI.Host.Web/`, and `Submodules/Gizmo.Client.UI.Services/` directories.
- Risk: Any refactoring, ViewState naming change, DI registration change, or client implementation change can silently break UI behaviour with no automated detection.
- Priority: High — the only current verification is manual browser testing.

**`Gizmo.Web.Api.Client.Tests` project exists but is minimal:**
- What's not tested: `Submodules/Gizmo.Web.Api.Client/Gizmo.Web.Api.Client.Tests/` contains only a `Program.cs` with no test classes.
- Files: `Submodules/Gizmo.Web.Api.Client/Gizmo.Web.Api.Client.Tests/Program.cs`
- Risk: HTTP client wrappers for all API endpoints are untested.
- Priority: Medium.

---

## Hardcoded / Non-Localised UI Strings

**Multiple validation error messages are hardcoded in English:**
- Files and locations:
  - `Gizmo.Client.UI/Components/Base/MaskedNumericInputBase.cs` (line 254): `"The field should be a number."`
  - `Gizmo.Client.UI/Components/Common/MaskedDateInput.razor.cs` (line 288): `"The field should be a date."`
  - `Gizmo.Client.UI/Components/Common/IconSelect.razor.cs` (line 409): `"The field is invalid."`
  - `Gizmo.Client.UI/Components/Common/Select.razor.cs` (lines 263, 298): `"The field is required!"`
  - `Gizmo.Client.UI/Code/StringConverter.cs` (multiple lines): `"The field should be boolean/an integer number/a number."`
- Impact: These validation messages are always displayed in English regardless of the selected UI language.
- Fix approach: Replace hardcoded strings with `LocalizationService.GetString(...)` calls using appropriate resource keys.

---

*Concerns audit: 2026-03-26*
