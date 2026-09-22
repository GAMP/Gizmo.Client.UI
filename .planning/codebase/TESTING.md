# Testing Patterns

**Analysis Date:** 2026-03-26

## Current State

**No automated tests exist** in this repository. This is explicitly documented in `CLAUDE.md`:

> **No tests exist** in this repository.

The only test-like project in the entire workspace is `Submodules/Gizmo.Web.Api.Client/Gizmo.Web.Api.Client.Tests/`, but this is:
- A console `OutputType=Exe` program, not a test framework project
- Targets `net6.0` while the main projects target `net10.0`
- An integration smoke tool that manually calls the API client against a running server
- Not part of the main solution's test suite — it belongs to the `Gizmo.Web.Api.Client` submodule

## Test Framework

**None.** No xUnit, NUnit, MSTest, bUnit, or Playwright/Selenium projects exist in the solution.

The submodule console test uses:
- `Microsoft.Extensions.Hosting` for DI bootstrap
- `Gizmo.Web.Api.Client` and `Gizmo.Web.Api.Clients.Builder` directly
- `MessagePack` for serialization
- Manual assertions via `Console.WriteLine` on exception

## Build Verification

The primary automated verification is a clean build. From `CLAUDE.md`:

```bash
# Install JS assets first
cd Gizmo.Client.UI
npm install
npm run build_dev

# Build .NET solution
cd Gizmo.Client.UI.Host.Web
dotnet run
```

MSBuild auto-runs `npm install` and the webpack script on `dotnet build` / `dotnet publish`, so a full dotnet build implicitly verifies the JS/SCSS pipeline.

**Build correctness checks:**
- `dotnet build` — compiles all C# projects
- Webpack — compiles SCSS and JS bundles (fails on syntax errors)
- `dotnet publish -c Release` — release build with full optimization

**Expected build limitation on Linux:** `Gizmo.Client.UI.Host.WPF` will fail to compile on Linux — this is expected and documented. The web host builds cleanly cross-platform.

## Manual Verification Approach

Given no automated tests, all functional verification is manual:

### Startup Smoke Test

```bash
cd Gizmo.Client.UI.Host.Web
dotnet run
# Open https://localhost:5001
```

The app starts in demo mode (`IGizmoClient` is registered as `TestClient`/`DemoClient`). All pages and components are populated from mock data — no live server required.

### Page-Level Manual Checks

Walk through all routes to verify no runtime errors:

| Route | What to check |
|---|---|
| `/` | Login form renders, method selector works |
| `/home` | Ads carousel, news rotator, quick launcher all render |
| `/apps` | App cards grid renders, filters work, search works |
| `/appdetails?ApplicationId=<id>` | App detail page renders, media gallery works |
| `/shop` | Product cards render (simple, time, bundle variants) |
| `/productdetails?ProductId=<id>` | Product detail page renders |
| `/profile` | Profile header with stats, navigation tabs |
| `/profile/purchases` | Data grid renders |
| `/profile/products` | Products list renders |
| `/profile/deposits` | Deposits list renders |
| `/passwordrecovery` | Recovery form renders |
| `/registrationindex` | Registration entry renders |

### Overlay and Dialog Checks

These require specific demo state to be triggered:
- `UserLock` overlay — trigger via header user menu lock action
- `GracePeriod` overlay — triggered by `GracePeriodViewState.IsInGracePeriod`
- `DialogHost` — triggered by any dialog-opening action (e.g., checkout)
- `NotificationsHost` — browser-only, verify no JS errors in console

### Style Verification

After any SCSS change, verify:
1. Run `npm run build_dev` in `Gizmo.Client.UI/` (or use `npm run watch_dev` during development)
2. Reload the browser — no missing styles, no layout breaks
3. Check both `_Layout` (authenticated pages) and `_Layout_Login` (auth pages) look correct

## Backend Integration Testing

The app ships with `TestClient` / `DemoClient` as the default `IGizmoClient` implementation. Testing against a live backend requires Docker:

```bash
cd Gizmo.Client.UI.Host.Web
docker compose --env-file .env -f docker-compose.dev.yml up -d
# Starts: postgres:16-alpine + gizmopowered/gizmo-server
```

Then switch `Program.cs` registration from `TestClient` to the live client implementation and update `wwwroot/appsettings.json`:
- `UIComposition:ApiEndpoint`
- `UIComposition:RealTimeEndpoint`
- `Network:ServerUri`

## Recommended Testing Additions

If automated testing is introduced, the natural fit given the tech stack:

**Unit testing view services:** xUnit with mocked `IGizmoClient` — test filtering/sorting logic in view services like `AppsPageViewService.RefilterRequest()`.

**Component testing:** bUnit — test individual Razor components in isolation with mocked ViewState dependencies injected via DI.

**E2E testing:** Playwright — can drive the Blazor WebAssembly app in a real browser against the demo mode server.

**What to test first (highest risk, no coverage):**
- `AppsPageViewService` filter/sort logic — pure C# business logic in `RefilterRequest()`
- `UserCartViewService` cart state mutations
- Login flow state transitions in `UserLoginViewService`
- Component rendering guard conditions (e.g., `DisableAppDetails` flag on `ApplicationCard`)

## CI/CD Pipeline

No CI pipeline configuration was found (no `.github/workflows/`, no `azure-pipelines.yml`, no `Jenkinsfile`). Verification is currently entirely local/manual.

---

*Testing analysis: 2026-03-26*
