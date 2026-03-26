# Integrations & External Systems

**Analysis Date:** 2026-03-25

## APIs & External Services

**Gizmo Server (primary backend):**
- Gizmo Server REST API - all application data, user management, products, shop, sessions
  - Client: `Gizmo.Web.Api.Client` submodule (`Submodules/Gizmo.Web.Api.Client/`)
  - Builder: `AddSecureWebApiClients()` / `AddUnsecureWebApiClients()` in `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Extensions/ServiceCollectionExtensions.cs`
  - Base URL: `Network:ServerUri` from `appsettings.json` (default: `http://localhost:8080`)
  - Serialization: MessagePack (primary) + JSON fallback
  - Auth: Bearer token via `UserAccessTokenHandler` (auto-refresh with 2-minute skew)
  - Polly resilience: `Microsoft.Extensions.Http.Polly` 10.0.3
  - **Current state:** Not active in default web runtime. `IGizmoClient` is registered as `TestClient` (mock). Infrastructure is wired but the live client is not connected.

**Gizmo Server SignalR / Real-time:**
- Real-time event endpoint for server-pushed events (balance changes, session events, notifications)
  - Endpoint: `UIComposition:RealTimeEndpoint` from `appsettings.json` (default: `http://localhost/rt`)
  - Protocol: `Microsoft.AspNetCore.SignalR.Protocols.Json` 10.0.3
  - **Current state:** Declared in config but not active in demo/test runtime.

**REST Countries API:**
- External: `https://restcountries.com/v3.1/all`
- Purpose: Country list for registration forms (name, flag, calling code)
- Client: Named `HttpClient` (`HttpClientRestCountries`) registered in `ServiceCollectionExtensions.cs`
- Timeout: 15 seconds
- Fallback: Embedded `countries.json` resource in `Gizmo.Client.UI.Services` assembly

**GeoPlugin API:**
- External: `http://www.geoplugin.net/json.gp`
- Purpose: Auto-detect user's country during registration
- Client: Named `HttpClient` (`HttpClientGeoPlugin`) registered in `ServiceCollectionExtensions.cs`
- Timeout: 15 seconds
- No fallback if geo-detection fails (returns `GetCountryCodeResult.Failure`)

**YouTube Embed:**
- External: `https://www.youtube.com/embed/<videoId>` and `https://img.youtube.com/vi/<videoId>/hqdefault.jpg`
- Purpose: Video media items in App Details page (`AppDetailsMediaItem` component)
- Implementation: YouTube video IDs extracted from watch URLs and embedded as iframes; thumbnails loaded from `img.youtube.com`
- Client-side only (browser iframe / img src)

**RSS/Atom Feeds:**
- External: Configurable URLs from `Feeds` section in `appsettings.json`
- Purpose: News rotator on the Home page
- Client: `System.ServiceModel.Syndication` 10.0.3 for feed parsing
- Service: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/View/Services/FeedsViewService.cs`
- Rotation interval: `Feeds:RotateEvery` (default: 5 seconds)

**Image Service:**
- Internal HTTP client for loading images (app screenshots, product images, ads)
- Named `HttpClient` (`ImageService`) registered in `ServiceCollectionExtensions.cs`
- Service: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Client/ImageService.cs`

**Runtime CSS Override:**
- External static host: `https://static/<StyleSheet>`
- Purpose: White-label theme override loaded at runtime without rebuild
- Config: `ClientInterface:StyleSheet` in `appsettings.json` (default: `main.css`)
- Applied in `Gizmo.Client.UI/Shared/_Layout.razor` and `_Layout_Login.razor`

## Databases

**PostgreSQL (dev backend only):**
- Image: `postgres:16-alpine`
- Container: `gizmo-postgres-dev` (via `docker-compose.dev.yml`)
- Database name: `Gizmo` (env: `POSTGRES_DB`)
- Port: 5432 (env: `POSTGRES_PORT`)
- Used by: `gizmo-server` container (not directly by the UI)
- `Microsoft.EntityFrameworkCore` 10.0.3 is present in `Gizmo.Server.Shared` (server-side shared types pulled transitively — not used directly in UI code)

**No direct database access from UI:**
- The UI projects do not connect to any database directly
- All data flows through `IGizmoClient` interface → Gizmo Server REST API

## Auth & Identity

**Token-based auth (JWT):**
- Provider: Gizmo Server (`AuthWebApiClient`)
- Handler: `UserAccessTokenHandler` in `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Services/UserAccessTokenHandler.cs`
- Flow: Access token + refresh token; auto-refresh when within 2 minutes of expiry
- Token attached to requests via `UserApiClientDelegatingHandler`
- `Microsoft.AspNetCore.Components.Authorization` 10.0.3 present for authorization primitives
- `Microsoft.AspNetCore.Authorization` 10.0.3 present in `Gizmo.Shared`

**Current active state:**
- In the default web runtime, `IGizmoClient` is registered as `TestClient` (mock)
- No actual JWT auth flow is active until `IGizmoClient` is replaced with a live client

**WPF (desktop):**
- Certificate validation disabled for non-browser environments (self-signed cert support):
  ```
  ServerCertificateCustomValidationCallback = (...) => true
  ```
  Configured in `ServiceCollectionExtensions.cs`

## Infrastructure & Deployment

**Dev backend (Docker):**
- `Gizmo.Client.UI.Host.Web/docker-compose.dev.yml`
- Services:
  - `postgres:16-alpine` - database for Gizmo Server
  - `gizmopowered/gizmo-server` - proprietary Gizmo application server
- Network: `gizmo-dev-network` (bridge)
- Gizmo Server ports: HTTP 8080, HTTPS 8081, TCP 44966
- Requires `.env` file for credentials (not committed; see `docker-compose.dev.yml` for var names)

**Web publish target:**
- Static files output from `dotnet publish -c Release` in `Gizmo.Client.UI.Host.Web`
- Deploy targets configured in `Properties/ServiceDependencies/` (Web Deploy, Zip Deploy, FTP profiles for Azure App Service)
- Expected hosting: Nginx or IIS with SPA fallback to `index.html`
- Dev server: `https://localhost:5001` (IIS Express: port 44310)

**WPF publish target:**
- Self-contained Windows executable: `dotnet publish -c Release -r win-x64 --self-contained`
- Output cleaned post-publish: only `Gizmo.Client.UI.dll`, `Gizmo.Web.Components.dll`, `composition.json`, `wwwroot/`, `static/` are retained

**Localization satellite assemblies (web):**
- Built into the WASM publish output for: ru-RU, tr-TR, da-DK, pt-BR, es-ES, et, el, sl-SI, en-US
- Configured via `SatelliteCulture` items in `Gizmo.Client.UI.Host.Web.csproj`

## Third-party Libraries with External Calls

| Library | Call direction | Target | File |
|---|---|---|---|
| `IHttpClientFactory` (REST Countries) | Outbound | `https://restcountries.com/v3.1/all` | `CountryInformationService.cs` |
| `IHttpClientFactory` (GeoPlugin) | Outbound | `http://www.geoplugin.net/json.gp` | `CountryInformationService.cs` |
| `System.ServiceModel.Syndication` | Outbound | RSS/Atom feed URLs from config | `FeedsViewService.cs` |
| `IHttpClientFactory` (ImageService) | Outbound | Image URLs from server | `ImageService.cs` |
| `Gizmo.Web.Api.Client` | Outbound | `Network:ServerUri` REST API | `ServiceCollectionExtensions.cs` |
| SignalR client (prepared) | Outbound | `UIComposition:RealTimeEndpoint` | Not active in current runtime |
| Runtime stylesheet `<link>` | Outbound (browser fetch) | `https://static/<StyleSheet>` | `_Layout.razor`, `_Layout_Login.razor` |
| `axios` (JS) | Outbound | Not determined from source read | `src/js/` bundle |

## Configuration & Secrets Management

**Web host config file:**
- `Gizmo.Client.UI.Host.Web/wwwroot/appsettings.json` - committed, contains only defaults (no secrets)
- Read at browser startup by Blazor WASM before app initialization

**Sensitive config (not committed):**
- `.env` file required for `docker-compose.dev.yml` - contains `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_PORT`, `GIZMO_HTTP_PORT`, `GIZMO_HTTPS_PORT`, `GIZMO_TCP_PORT`
- `.env` is listed in `.gitignore` (not verified, standard practice)

**Options classes (C#):**
- `ClientNetworkOptions` - `Network:ServerUri` (Gizmo Server base URL)
- `UICompositionOptions` - `UIComposition:ApiEndpoint`, `RealTimeEndpoint`, assembly list
- `ClientInterfaceOptions` - `ClientInterface:StyleSheet`, UI feature flags
- Bound via `AddClientOptions()` in `Program.cs` using `IOptions<T>` / `IOptionsMonitor<T>`

**No secret management service detected:**
- No Azure Key Vault, AWS Secrets Manager, HashiCorp Vault, or similar integration is present
- All runtime configuration is file-based or environment variable-based via Docker Compose

---

*Integration audit: 2026-03-26*
