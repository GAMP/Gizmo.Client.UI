# Technology Stack

**Analysis Date:** 2026-03-26

## Languages

**Primary:**
- C# (LangVersion: latest / net10.0) - all .NET projects, Razor components, view services, view states
- SCSS - styling source in `Gizmo.Client.UI/src/scss/`
- JavaScript (ES class syntax) - asset behavior in `Gizmo.Client.UI/src/js/`

**Secondary:**
- XAML - WPF host UI shell (`Gizmo.Client.UI.Host.WPF/*.xaml`)
- XML/RESX - localization resources (`Submodules/Gizmo.Client.UI.Resources/`)
- JSON - configuration (`wwwroot/appsettings.json`, `composition.json`, `options.json`)

## Runtime

**Environment:**
- .NET 10.0 (`net10.0`) - all main projects and all submodules target net10.0 (verified in each csproj)
- Browser (WebAssembly) via Blazor WASM for web host
- Windows 10 (17763+) via WPF for desktop host (compile errors on Linux are expected for WPF project)

**Satellite Cultures (web host):**
- ru-RU, tr-TR, da-DK, pt-BR, es-ES, et, el, sl-SI, en-US (declared in `Gizmo.Client.UI.Host.Web.csproj`)

## Frameworks

**Core (Web):**
- Blazor WebAssembly (`Microsoft.NET.Sdk.BlazorWebAssembly`) - SPA runtime in browser
  - `Microsoft.AspNetCore.Components.WebAssembly` 10.0.3
  - `Microsoft.AspNetCore.Components.WebAssembly.DevServer` 10.0.3 (dev only)
  - `Microsoft.AspNetCore.Components.Web` 10.0.3
  - `Microsoft.AspNetCore.Components.Forms` 10.0.3
  - `Microsoft.AspNetCore.Components.Authorization` 10.0.3
- WASM Webcil disabled: `<WasmEnableWebcil>false</WasmEnableWebcil>` in `Gizmo.Client.UI.Host.Web.csproj`

**Core (Desktop):**
- WPF with BlazorWebView (`Microsoft.NET.Sdk.Razor` + `UseWPF`)
  - `Microsoft.AspNetCore.Components.WebView.Wpf` 9.0.120

**Reactive Programming:**
- Rx.NET `System.Reactive` 6.1.0 - used in view services for event streams (`Gizmo.Client.UI.Services`, `Gizmo.UI`)

**Serialization:**
- `MessagePack` 2.5.198 - binary serialization for API payloads (`Gizmo.Web.Api.Models`, `Gizmo.Shared`, `Gizmo.Web.Api.Client`)
- `Microsoft.AspNetCore.SignalR.Protocols.Json` 10.0.3 - SignalR JSON protocol (`Gizmo.Web.Api.Models`)
- `System.Text.Json` (built-in) - JSON deserialization in services and token parsing

**Real-time:**
- SignalR (via `Microsoft.AspNetCore.SignalR.Protocols.Json`) - declared for real-time endpoint; actual usage depends on live backend connection

**Resilience:**
- Polly via `Microsoft.Extensions.Http.Polly` 10.0.3 - exponential backoff retry policy on API HTTP clients; configurable via `WithRetryPolicyHandler(int retryCount)` in `Gizmo.Web.Api.Client/Infrastructure/Extensions/WebApiClientBuilder.cs`

**QR Code:**
- `QRCoder` 1.6.0 - QR code generation for host display (`Gizmo.Client.UI.Services`)

**Feed Parsing:**
- `System.ServiceModel.Syndication` 10.0.3 - RSS/Atom feed parsing in `FeedsViewService.cs`

**Caching:**
- `Microsoft.Extensions.Caching.Memory` 10.0.3 - in-process caching in `Gizmo.Client.UI.Services`

**Memory:**
- `Microsoft.IO.RecyclableMemoryStream` 3.0.1 - pooled memory stream for large payloads in `Gizmo.Web.Api.Client` and `Gizmo.Client.UI.Services`

## Build Tools

**JavaScript/CSS:**
- Webpack 5.86.0 - asset bundler
- webpack-cli 5.1.4 - CLI runner
- webpack-merge 5.8.0 - config composition (dev/prod/watch split)
- sass 1.63.0 + sass-loader 12.3.0 - SCSS compilation
- css-loader 6.8.1 + style-loader 3.3.1 - CSS processing
- copy-webpack-plugin 9.0.1 - static file copying (html, img → wwwroot)
- file-loader 6.2.0 - font/binary asset output to `font-family/`
- csv-loader 3.0.5 - CSV import support
- express 4.17.2 + webpack-dev-middleware 5.2.1 - dev server support

**Config files:**
- `Gizmo.Client.UI/webpack/webpack.common.js` - shared entry points and rules
- `Gizmo.Client.UI/webpack/webpack.dev.js` - development build (inline-source-map)
- `Gizmo.Client.UI/webpack/webpack.dev.watch.js` - watch mode
- `Gizmo.Client.UI/webpack/webpack.prod.js` - production build (minified, no source maps)

**.NET:**
- MSBuild - invokes `npm install` + `npm run build_dev/build_prod` as `PreBuild` target in `Gizmo.Client.UI/Gizmo.Client.UI.csproj`
- `dotnet publish -c Release` for web; `dotnet publish -c Release -r win-x64 --self-contained` for WPF

## Package Managers

**JavaScript:**
- npm 11.5.2 (no `.nvmrc` pinning Node version)
- `package.json` location: `Gizmo.Client.UI/package.json`
- Runtime dependency: `axios` 1.4.0 (HTTP client available in JS bundle via `src/js/`)
- All other JS packages are `devDependencies` (build-time only)

**.NET:**
- NuGet (via `PackageReference` in `.csproj` files)
- No `Directory.Packages.props` - each project pins its own versions independently

## Key Dependencies (with versions)

| Package | Version | Project | Purpose |
|---|---|---|---|
| `Microsoft.AspNetCore.Components.WebAssembly` | 10.0.3 | Host.Web, Services | Blazor WASM runtime |
| `Microsoft.AspNetCore.Components.WebView.Wpf` | 9.0.120 | Host.WPF | Blazor inside WPF window |
| `System.Reactive` | 6.1.0 | Services, Gizmo.UI | Rx.NET event streams in view services |
| `MessagePack` | 2.5.198 | Web.Api.Models, Shared, Web.Api.Client | Binary API serialization |
| `Microsoft.AspNetCore.SignalR.Protocols.Json` | 10.0.3 | Web.Api.Models | SignalR JSON protocol |
| `QRCoder` | 1.6.0 | Services | QR code generation for host display |
| `System.ServiceModel.Syndication` | 10.0.3 | Services | RSS/Atom feed parsing |
| `Microsoft.Extensions.Caching.Memory` | 10.0.3 | Services | In-memory caching |
| `Microsoft.Extensions.Http` | 10.0.3 | Host.Web, Host.WPF, Services | IHttpClientFactory |
| `Microsoft.Extensions.Http.Polly` | 10.0.3 | Web.Api.Client | Polly resilience for HTTP clients |
| `Microsoft.IO.RecyclableMemoryStream` | 3.0.1 | Services, Web.Api.Client | Pooled memory streams |
| `Microsoft.Extensions.Localization` | 10.0.3 | Services, Gizmo.UI, Gizmo.Shared | RESX-based localization |
| `Microsoft.AspNetCore.Authorization` | 10.0.3 | Gizmo.Shared | Authorization primitives |
| `Microsoft.Extensions.FileProviders.Embedded` | 10.0.3 | Gizmo.Client.UI | Embedded static file serving from RCL |
| `System.ComponentModel.Annotations` | 5.0.0 | Web.Api.Models, Gizmo.Shared | Data annotation validators |
| `axios` | 1.4.0 (JS) | Gizmo.Client.UI | HTTP client available in JS bundles |

## Configuration

**Environment (web):**
- `Gizmo.Client.UI.Host.Web/wwwroot/appsettings.json` - primary runtime config (read by Blazor WASM at startup)
- Configured via `AddClientConfigurationSource()` + `AddClientOptions()` in `Program.cs`
- Key sections: `UIComposition`, `ClientInterface`, `Currency`, `Network`, `Feeds`, `Shop`, `UserLogin`, `Validation`

**Environment (WPF):**
- `Gizmo.Client.UI.Host.WPF/composition.json` - composition config
- `Gizmo.Client.UI.Host.WPF/options.json` - options config

**Build:**
- `Gizmo.Client.UI/webpack/webpack.common.js` - shared Webpack config
- MSBuild `PreBuild` target in `Gizmo.Client.UI/Gizmo.Client.UI.csproj` triggers `npm install` and webpack

## Platform Requirements

**Development:**
- Node.js 22+ / npm 11+ (no version pinned in project)
- .NET SDK 10.0
- Docker + Docker Compose (optional, for local backend: `docker-compose.dev.yml`)

**Production (Web):**
- Static file host with SPA fallback (Nginx or IIS with fallback to `index.html`)
- Published via `dotnet publish -c Release` from `Gizmo.Client.UI.Host.Web`

**Production (Desktop):**
- Windows 10 build 17763 or later
- Self-contained publish: `dotnet publish -c Release -r win-x64 --self-contained`

---

*Stack analysis: 2026-03-26*
