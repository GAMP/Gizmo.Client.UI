# Codebase Structure

**Analysis Date:** 2026-03-26

---

## Top-Level Repository Layout

```
Gizmo.Client.UI/                        # Repository root
├── Gizmo.Client.UI/                    # Razor Class Library — UI core
├── Gizmo.Client.UI.Host.Web/           # Blazor WASM host (primary target)
├── Gizmo.Client.UI.Host.WPF/           # WPF/BlazorWebView host (Windows-only)
├── Gizmo.Client.UI.Modules/            # Composition page modules
├── Submodules/                         # Git submodules
├── Gizmo.Client.UI.sln                 # Solution file
├── CLAUDE.md                           # Project instructions for Claude
├── README.md
└── doc/llm/                            # LLM reference maps
```

---

## Project Directories

### `Gizmo.Client.UI/` — UI Core (Razor Class Library)

The main working directory. All pages, components, layouts, SCSS source, JS source, and webpack configuration live here.

```
Gizmo.Client.UI/
├── App.razor                   # Router root component
├── App.razor.cs                # Code-behind: injects IUICompositionService, JSRuntime init
├── _Imports.razor              # Global namespace imports for all Razor files
├── Gizmo.Client.UI.csproj      # Project file; triggers npm + webpack on build
├── package.json                # npm dependencies (webpack, sass-loader, etc.)
├── package-lock.json
│
├── Pages/                      # Page components (routable)
│   ├── Home.razor / .cs
│   ├── CriticalErrorPage.razor / .cs
│   ├── NotFoundPage.razor / .cs
│   ├── Apps/                   # App catalog pages
│   ├── Login/                  # Auth pages
│   ├── Profile/                # User profile pages
│   └── Shop/                   # Shop / product pages
│
├── Shared/                     # Layouts and shell components
│   ├── _Layout.razor / .cs         # Main authenticated shell
│   ├── _Layout_Login.razor / .cs   # Login/registration shell
│   ├── _EmptyLayout.razor          # Minimal shell for utility pages
│   ├── Header.razor / .cs
│   ├── Header*.razor / .cs         # All header sub-components
│   ├── Menu*.razor / .cs           # Dropdown menu panels
│   ├── Layout_Login*.razor / .cs   # Login shell helpers (rotator, lock, etc.)
│   ├── DialogHost.razor / .cs
│   ├── UserLock.razor / .cs
│   ├── UserBan.razor / .cs
│   └── GracePeriod.razor / .cs
│
├── Components/                 # Reusable non-routable components
│   ├── Apps/                   # App cards, filters, launch buttons, executables
│   ├── Base/                   # Base classes: ButtonBase, GizInputBase, etc.
│   ├── Common/                 # Shared primitives: Button, TextInput, DataGrid, etc.
│   ├── Dialogs/                # (implicit — dialogs inside Common and feature dirs)
│   ├── Icons/                  # Icon assets
│   ├── Interfaces/             # C# interfaces: IGizInput, ISelect, etc.
│   ├── Login/                  # Login-specific: LoginCard, ClientLanguageMenu, UserAgreementDialog
│   ├── Notifications/          # NotificationsHost
│   ├── Profile/                # Profile header, dialogs, navigation
│   └── Shop/                   # Product cards, order, checkout, quantity picker
│
├── Code/                       # C# helpers, utilities, enums
│   ├── Enumerations/           # UI enums: ButtonColors, Icons, InputSizes, etc.
│   ├── Services/               # Host-side service implementations
│   │   ├── ClientDialogService.cs
│   │   ├── ClientNotificationService.cs
│   │   └── IconSelectCountry.cs
│   └── ViewModels/             # (currently contains only IconSelectCountry.cs)
│
├── src/                        # Asset sources — NEVER edit wwwroot directly
│   ├── js/
│   │   ├── internal.js         # DOM/browser behavior, Blazor interop
│   │   ├── external.js         # White-label extensions (ExternalFunctions class)
│   │   └── api.js              # JS bridge for calls from Blazor (ClientAPI class)
│   ├── scss/
│   │   ├── main.scss           # Webpack entrypoint → imports theme + globals
│   │   ├── external.scss       # Separate bundle for external overrides
│   │   ├── _global.scss        # html/body reset, base form helpers
│   │   ├── components/
│   │   │   └── _layout.scss    # Base shell grid
│   │   └── themes/
│   │       └── client/         # Active client theme
│   │           ├── main.scss       # Imports all partials in order
│   │           ├── _variables.scss # Color tokens, z-index, shadows
│   │           ├── _typography.scss
│   │           ├── _global.scss    # Base body/theme look
│   │           ├── gizmo.web.components/  # Adapts shared primitives to this theme
│   │           └── components/     # Client-specific component + page partials
│   ├── img/                    # Source images (copied to wwwroot/img by webpack)
│   ├── html/                   # Source HTML fragments (copied to wwwroot by webpack)
│   ├── font-family/            # Font files (ttf) used by SCSS
│   └── theme/                  # (legacy/external theme files)
│
├── webpack/                    # Webpack configuration
│   ├── webpack.common.js       # Entry points + output config + loaders
│   ├── webpack.dev.js          # Dev build (unminified)
│   ├── webpack.dev.watch.js    # Dev build with watch mode
│   └── webpack.prod.js         # Production build (minified)
│
└── wwwroot/                    # Generated output — do not edit manually
    ├── client_internal_code.js
    ├── client_external_code.js
    ├── client_api_code.js
    ├── client_internal_style.js
    ├── client_external_style.js
    ├── font-family/            # Bundled fonts
    ├── img/                    # Copied images
    └── notifications.html      # Embedded resource
```

---

### `Gizmo.Client.UI.Host.Web/` — Blazor WebAssembly Host

The web runtime entry point. Thin by design — only startup wiring.

```
Gizmo.Client.UI.Host.Web/
├── Program.cs                  # WebAssemblyHostBuilder, DI registration, app startup
├── Gizmo.Client.UI.Host.Web.csproj
├── Properties/
├── wwwroot/
│   ├── index.html              # SPA shell: loads Blazor + all JS bundles from _content/
│   ├── appsettings.json        # UIComposition, ClientInterface, Network, and all options
│   ├── css/                    # Host-level CSS (minimal)
│   └── static/                 # Static assets served directly (images, XML feeds)
├── docker-compose.dev.yml      # Dev backend: postgres + gizmo-server
└── docker/                     # Docker data directories
```

Key file: `Program.cs` registers all services:
1. `AddClientConfigurationSource()` — reads `appsettings.json` sections
2. `AddClientOptions()` — binds options classes
3. `AddClientServices()` — registers ViewServices + ViewStates by convention
4. Host-specific singletons: `IClientDialogService`, `IClientNotificationService`, `IInputLanguageService`
5. `IGizmoClient` → currently `TestClient` (demo/mock mode)
6. `IImageService`, `INotificationsHost`
7. `InitializeClientServices()` — async initialization

---

### `Gizmo.Client.UI.Host.WPF/` — WPF Host

Windows-only. Uses `BlazorWebView` to host the same Razor UI in a desktop app. Compile errors on Linux are expected and normal. Not part of the primary web analysis.

---

### `Gizmo.Client.UI.Modules/` — Page Modules

Contains page extensions loaded dynamically via the composition system.

```
Gizmo.Client.UI.Modules/
├── Gizmo.Client.UI.Modules.csproj
└── Pages/
    └── CustomPageModule.razor / .cs   # Base for dynamically composed page modules
```

Module pages are identified by: `ModuleGuid`, `PageUIModule`, `ModuleDisplayOrder`, `DefaultRoute`.

---

## Submodules

Located under `Submodules/`. All are git submodules — update with `git submodule update --init --recursive`.

### `Submodules/Gizmo.Client.UI.Services/` — View Services + View States

The application logic layer. Contains all `*ViewService.cs` and `*ViewState.cs` files plus the DemoClient.

```
Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/
├── View/
│   ├── Services/               # All ViewService implementations
│   │   ├── Lookup/             # Lookup services (caching layer over IGizmoClient)
│   │   └── *.ViewService.cs
│   └── States/                 # All ViewState classes
│       ├── App/
│       ├── Base/
│       └── *.ViewState.cs
├── Client/                     # IGizmoClient implementations
│   ├── DemoClient.cs           # Demo/mock data client
│   ├── TestClient.cs           # Currently registered in Program.cs
│   ├── ImageService.cs
│   └── DemoUserProfile.cs, etc.
├── Services/                   # Infrastructure services
│   ├── WebAssemblyUICompositionService.cs
│   ├── DesktopUICompositionService.cs
│   ├── WebInputLenguageService.cs
│   ├── WebLocalizationService.cs
│   ├── UserAccessTokenHandler.cs       # HTTP handler for auth tokens
│   ├── UserApiClientDelegatingHandler.cs
│   └── JSInteropService.cs
├── Extensions/
│   └── ServiceCollectionExtensions.cs  # AddClientServices(), AddClientViewServices(), etc.
├── Code/
│   └── Constants.cs
└── ClientInMemoryConfiurationSource.cs
```

Registration is convention-based: any class ending in `*ViewService` is registered as a service, any `*ViewState` as a singleton state. **Naming convention is mandatory** — breaking the suffix breaks automatic DI registration.

### `Submodules/Gizmo.UI/` — Base UI Services

```
Gizmo.UI/Gizmo.UI/
├── Code/           # Base helpers
├── Extensions/     # Service extension methods
├── Resources/      # Localization base
├── Services/       # NavigationService, JSRuntimeService, JSInteropService
├── Shared/         # Shared base types
└── View/           # UICompositionServiceBase, base view abstractions
```

Provides: `IUICompositionService`, `NavigationService`, `JSRuntimeService`, `JSInteropService`.

### `Submodules/Gizmo.Web.Components/` — Shared Razor Primitives

Reusable low-level Razor components used across Gizmo projects.

```
Gizmo.Web.Components/
├── src/            # Component-level JS and SCSS (included in webpack via imports)
├── Infrastructure/ # Component infrastructure
└── Extensions/     # Component extension methods
```

Examples: `Button`, `TextInput`, `Select`, `DataGrid`, `Tooltip`, `Popup`, `Avatar`, `Icon`.
**Do not edit these directly.** Override via `src/scss/themes/client/gizmo.web.components/`.

### `Submodules/Gizmo.Client.Shared/` — Domain Contracts

```
Gizmo.Client.Shared/Gizmo.Client.Shared/
├── Code/           # ClientRoutes.cs — all route constants
├── Interfaces/     # IGizmoClient interface
├── Models/
├── Enumerations/
├── EventArgs/
└── Resources/
```

`ClientRoutes` is the canonical source of all route path strings. `IGizmoClient` is the interface all data access flows through.

### `Submodules/Gizmo.Web.Api.Models/` — API DTOs

Request/response models, filters, and enumerations for the REST API and SignalR.

```
Gizmo.Web.Api.Models/
├── Models/         # DTOs
├── Abstractions/
├── Enumerations/
├── Infrastructure/
└── Services/
```

### `Submodules/Gizmo.Client.UI.Resources/` — RESX Localization

Contains `.resx` resource files for UI string localization.

### `Submodules/Gizmo.Shared/` — Shared Enums and Base Types

Cross-project shared enumerations, base abstractions, and infrastructure types (netstandard2.0).

### `Submodules/Gizmo.Web.Api.Client/` — API HTTP Client

**Currently empty.** Directory exists but no project files. Referenced in `Gizmo.Client.UI.Services.csproj`. Required for live backend connectivity. This is an active architectural gap.

### `Submodules/Gizmo.Server.Shared/` — Server Shared Types

**Currently empty.** Directory exists but no project files. Same status as `Gizmo.Web.Api.Client`.

---

## Key Files Quick Reference

| File | Purpose |
|------|---------|
| `Gizmo.Client.UI.Host.Web/Program.cs` | App entrypoint; all DI registration |
| `Gizmo.Client.UI.Host.Web/wwwroot/appsettings.json` | Runtime config: UIComposition, ClientInterface, Network, etc. |
| `Gizmo.Client.UI.Host.Web/wwwroot/index.html` | SPA HTML shell; loads Blazor + JS bundles |
| `Gizmo.Client.UI/App.razor` | Router root; uses `IUICompositionService` for assembly discovery |
| `Gizmo.Client.UI/App.razor.cs` | Initializes JSRuntime and NavigationManager associations |
| `Gizmo.Client.UI/_Imports.razor` | Global Razor `@using` directives |
| `Gizmo.Client.UI/Gizmo.Client.UI.csproj` | Triggers `npm install` + webpack on dotnet build |
| `Gizmo.Client.UI/webpack/webpack.common.js` | Webpack entry points and output path configuration |
| `Gizmo.Client.UI/src/scss/main.scss` | SCSS root entrypoint (imported by webpack) |
| `Gizmo.Client.UI/src/scss/themes/client/main.scss` | Client theme: imports all partials in correct order |
| `Gizmo.Client.UI/src/scss/themes/client/_variables.scss` | Theme color tokens, spacing, z-index — edit here for theme changes |
| `Gizmo.Client.UI/src/scss/external.scss` | External override bundle (white-label layer) |
| `Gizmo.Client.UI/src/js/internal.js` | Browser behavior, DOM interop |
| `Gizmo.Client.UI/src/js/external.js` | External integrations (`ExternalFunctions` class) |
| `Gizmo.Client.UI/src/js/api.js` | Blazor↔JS bridge (`ClientAPI` class) |
| `Submodules/Gizmo.Client.UI.Services/.../ServiceCollectionExtensions.cs` | `AddClientServices()` — ViewService/ViewState registration |
| `Submodules/Gizmo.Client.UI.Services/.../DemoClient.cs` | Mock `IGizmoClient` used in current runtime |
| `Submodules/Gizmo.Client.Shared/.../ClientRoutes.cs` | All route path constants |

---

## Pages Directory

Located at `Gizmo.Client.UI/Pages/`. Each page has a `.razor` markup file and a `.razor.cs` code-behind.

| Page File | Route | Layout |
|-----------|-------|--------|
| `Pages/Login/Login.razor` | `/` | `_Layout_Login` |
| `Pages/Login/PasswordRecovery.razor` | `/passwordrecovery` | `_Layout_Login` |
| `Pages/Login/PasswordRecoveryConfirmation.razor` | `/passwordrecoveryconfirmation` | `_Layout_Login` |
| `Pages/Login/PasswordRecoverySetNewPassword.razor` | `/passwordrecoverysetnewpassword` | `_Layout_Login` |
| `Pages/Login/RegistrationIndex.razor` | `/registrationindex` | `_Layout_Login` |
| `Pages/Login/RegistrationBasicFields.razor` | `/registrationbasicfields` | `_Layout_Login` |
| `Pages/Login/RegistrationAdditionalFields.razor` | `/registrationadditionalfields` | `_Layout_Login` |
| `Pages/Login/RegistrationConfirmationMethod.razor` | `/registrationconfirmationmethod` | `_Layout_Login` |
| `Pages/Login/RegistrationConfirmation.razor` | `/registrationconfirmation` | `_Layout_Login` |
| `Pages/Apps/AppsIndex.razor` | `/apps` | `_Layout` |
| `Pages/Apps/AppDetails.razor` | `/appdetails?ApplicationId=...` | `_Layout` |
| `Pages/Shop/ProductsIndex.razor` | `/shop` | `_Layout` |
| `Pages/Shop/ProductDetails.razor` | `/productdetails?ProductId=...` | `_Layout` |
| `Pages/Profile/Profile.razor` | `/profile` | `_Layout` |
| `Pages/Profile/Purchases.razor` | `/profile/purchases` | `_Layout` |
| `Pages/Profile/Products.razor` | `/profile/products` | `_Layout` |
| `Pages/Profile/Deposits.razor` | `/profile/deposits` | `_Layout` |
| `Pages/Home.razor` | `/home` | `_Layout` |
| `Pages/NotFoundPage.razor` | catch-all | Router default |
| `Pages/CriticalErrorPage.razor` | error boundary | `_Layout` error content |

---

## Components Directory

Located at `Gizmo.Client.UI/Components/`. Organized by feature area.

| Directory | Contents |
|-----------|---------|
| `Components/Apps/` | `ApplicationCard`, `AppFilters`, `ExecutableLaunchButton`, `UniversalExecutable`, `Leaderboard`, `AdsCarousel` |
| `Components/Common/` | All shared UI primitives: `Button`, `TextInput`, `Select`, `DataGrid`, `Tooltip`, `Popup`, `Avatar`, `Spinner`, `NewsRotator`, `QuickLauncher`, `QuantityPicker`, `GizDock`, `MediaDialog`, etc. |
| `Components/Base/` | Base C# classes: `ButtonBase`, `GizInputBase`, `MaskedInputBase`, `MaskedNumericInputBase` |
| `Components/Interfaces/` | C# interfaces: `IGizInput`, `ISelect`, `ISelectItem`, `IconSelectItem` |
| `Components/Login/` | `LoginCard`, `ClientLanguageMenu`, `PasswordTooltip`, `UserAgreementDialog` |
| `Components/Notifications/` | `NotificationsHost` |
| `Components/Profile/` | `ProfileHeader`, `ProfileNavigation`, change dialogs, `ProductsProductType` |
| `Components/Shop/` | `ProductCard`, `ProductBundleCard`, `ProductSimpleCard`, `ProductTimeCard`, `GizOrder`, `CheckoutDialog`, `ProductQuantityPicker`, `ProductVirtualizedCards` |

---

## SCSS Structure

All SCSS sources are under `Gizmo.Client.UI/src/scss/`. Output goes to `Gizmo.Client.UI/wwwroot/` via webpack. **Never edit `wwwroot/` directly.**

### Import chain

```
webpack → src/scss/main.scss
            ↓
          themes/client/main.scss
            ↓ imports in order:
            _variables.scss         (tokens)
            _typography.scss
            _global.scss
            gizmo.web.components/*  (37 shared primitive partials)
            components/*            (57 client-specific partials)
          ↓
          src/scss/_global.scss     (reset)
          src/scss/components/_layout.scss  (shell grid)

webpack → src/scss/external.scss    (separate bundle, white-label)
```

### Theme partials location

| What to style | File |
|---------------|------|
| Color tokens, spacing, z-index | `src/scss/themes/client/_variables.scss` |
| Typography | `src/scss/themes/client/_typography.scss` |
| App shell / layout grid | `src/scss/themes/client/components/_layout.scss` |
| Login shell | `src/scss/themes/client/components/_layout-login.scss` |
| Header | `src/scss/themes/client/components/_header.scss` |
| Global search | `src/scss/themes/client/components/_global-search.scss` |
| Dialog wrapper | `src/scss/themes/client/components/_client-dialog.scss` |
| Dropdown menus | `src/scss/themes/client/components/_dropdown-menu.scss` |
| Home page | `src/scss/themes/client/components/_page-home.scss` |
| Apps page | `src/scss/themes/client/components/_page-apps.scss` |
| App details | `src/scss/themes/client/components/_page-app-details.scss` |
| Shop page | `src/scss/themes/client/components/_page-shop.scss` |
| Product details | `src/scss/themes/client/components/_page-product-details.scss` |
| Profile page | `src/scss/themes/client/components/_page-user-profile.scss` |
| Shared primitive override | `src/scss/themes/client/gizmo.web.components/_<component>.scss` |
| External override | `src/scss/external.scss` |

---

## JS Structure

All JS sources are under `Gizmo.Client.UI/src/js/`.

| File | Bundle Output | Purpose |
|------|--------------|---------|
| `src/js/internal.js` | `client_internal_code.js` | Browser behavior, DOM interop, internal helpers |
| `src/js/external.js` | `client_external_code.js` | External integrations; `ExternalFunctions` class; call as `ExternalFunctions.methodName()` |
| `src/js/api.js` | `client_api_code.js` | JS bridge for Blazor calls; `ClientAPI` class |

Custom JS functions go in `src/js/external.js` inside `ExternalFunctions`. To call Blazor from JS, use `src/js/api.js`.

---

## Where to Add New Code

### New page

1. Create `Pages/<Module>/PageName.razor` + `PageName.razor.cs`
2. Add `[Route(ClientRoutes.YourRoute)]` attribute using a constant from `Submodules/Gizmo.Client.Shared/.../ClientRoutes.cs`
3. Specify layout: `@layout _Layout` (authenticated) or `@layout _Layout_Login`
4. For module pages, also add: `ModuleGuid`, `PageUIModule`, `ModuleDisplayOrder`, `DefaultRoute`
5. Create corresponding ViewService + ViewState in `Submodules/Gizmo.Client.UI.Services/...View/Services/` and `.../View/States/` — suffix `*ViewService` / `*ViewState` is mandatory
6. Add page SCSS partial `src/scss/themes/client/components/_page-<name>.scss` and import it in `src/scss/themes/client/main.scss`

### New component

1. Create `Components/<Area>/ComponentName.razor` + `ComponentName.razor.cs`
2. Code-behind inherits `CustomDOMComponentBase` (or an appropriate base from `Components/Base/`)
3. Inject dependencies with `[Inject]`; subscribe to ViewState in `OnInitialized`/`OnInitializedAsync`; unsubscribe in `Dispose`
4. If the component needs its own SCSS, add a partial in `src/scss/themes/client/components/_<component-name>.scss` and import it in `themes/client/main.scss`

### New ViewService + ViewState

1. Create `*ViewService.cs` in `Submodules/Gizmo.Client.UI.Services/.../View/Services/`
2. Create `*ViewState.cs` in `Submodules/Gizmo.Client.UI.Services/.../View/States/`
3. Both are auto-registered by `AddClientViewServices()` / `AddClientViewStates()` via name convention scanning
4. **Do not use any other suffix** — this breaks automatic registration

### New lookup service

1. Create `*ViewStateLookupService.cs` in `Submodules/Gizmo.Client.UI.Services/.../View/Services/Lookup/`
2. Register manually in `ServiceCollectionExtensions.cs` if not already convention-scanned

### New SCSS component partial

1. Create `_component-name.scss` in `src/scss/themes/client/components/`
2. Add `@import "components/_component-name";` in `src/scss/themes/client/main.scss`
3. Use `giz-` prefixed class names following BEM-like pattern: `giz-block`, `giz-block__element`, `giz-block--modifier`

### New JS utility

- Browser/DOM behavior → add to `src/js/internal.js`
- External integration → add method to `ExternalFunctions` class in `src/js/external.js`
- Blazor-callable bridge → add to `ClientAPI` class in `src/js/api.js`

### New theme

1. Copy `src/scss/themes/client/` to `src/scss/themes/<theme-name>/`
2. Set `$theme-attr-name` in the new `_variables.scss`
3. Import new `main.scss` in `src/scss/main.scss`

### New configuration option

1. Add property to the appropriate options class in `Gizmo.Client.UI.Services`
2. Bind the section in `ServiceCollectionExtensions.AddClientOptions()`
3. Set value in `Gizmo.Client.UI.Host.Web/wwwroot/appsettings.json`

---

## Naming Conventions Summary

| Artifact | Pattern | Example |
|----------|---------|---------|
| Page files | `PascalCase.razor` / `.razor.cs` | `AppDetails.razor` |
| Component files | `PascalCase.razor` / `.razor.cs` | `ApplicationCard.razor` |
| ViewService files | `*ViewService.cs` (suffix mandatory) | `AppsPageViewService.cs` |
| ViewState files | `*ViewState.cs` (suffix mandatory) | `AppsPageViewState.cs` |
| SCSS partials | `_kebab-case.scss` | `_page-apps.scss` |
| CSS classes | `giz-` prefix, BEM-like | `giz-app-card__title` |
| JS public API | Class methods, PascalCase methods | `ExternalFunctions.ShowBanner()` |
| Route constants | All in `ClientRoutes.cs` | `ClientRoutes.Apps = "/apps"` |

---

*Structure analysis: 2026-03-26*
