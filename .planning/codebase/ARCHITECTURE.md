# Architecture

**Analysis Date:** 2026-03-26

## Pattern Overview

**Overall:** Blazor WebAssembly SPA with a plugin/composition host model and a centralized ViewService/ViewState UI state pattern.

**Key Characteristics:**
- All UI state is owned by singleton `*ViewState` objects, never held in Razor components directly
- `*ViewService` classes drive state mutations and call `IGizmoClient` for data
- Razor components are stateless renderers that inject and subscribe to `ViewState` instances
- The Router is assembled at runtime from config-specified assemblies via `IUICompositionService`
- DI registration is entirely attribute-driven via `[Register]` — no manual `services.AddSingleton<T>()` for view layer

---

## Layers

**Host (Entry Point):**
- Purpose: Bootstraps the Blazor WASM app, registers DI, sets runtime mode
- Location: `Gizmo.Client.UI.Host.Web/`
- Contains: `Program.cs`, `wwwroot/appsettings.json`, `index.html`
- Depends on: `Gizmo.Client.UI` (single project reference)
- Used by: Browser runtime via `dotnet.js`

**Presentation (UI Core):**
- Purpose: All Razor pages, layouts, shell components, assets, Webpack pipeline
- Location: `Gizmo.Client.UI/`
- Contains: Pages, Components, Shared layouts, `src/scss`, `src/js`, `webpack/`
- Depends on: `Gizmo.Client.UI.Services`, `Gizmo.Web.Components`
- Used by: Host.Web (and Host.WPF)

**Application State / Services:**
- Purpose: ViewService/ViewState pairs for every UI concern; composition service; DemoClient / TestClient; web API infrastructure
- Location: `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/`
- Contains: `View/Services/`, `View/States/`, `Services/`, `Client/`, `Extensions/`
- Depends on: `Gizmo.UI`, `Gizmo.Client.Shared`, `Gizmo.Web.Api.Client`, `Gizmo.Server.Shared`, `Gizmo.Client.UI.Resources`
- Used by: `Gizmo.Client.UI`

**UI Primitives:**
- Purpose: Shared reusable Razor components (buttons, inputs, grids, etc.) used across projects
- Location: `Submodules/Gizmo.Web.Components/`
- Contains: Razor component library with its own `src/js` and `src/scss` consumed by Webpack
- Depends on: `Gizmo.Shared`
- Used by: `Gizmo.Client.UI`, `Gizmo.Client.UI.Modules`

**Base UI Abstractions:**
- Purpose: Base classes for ViewService/ViewState, JS interop utilities, navigation helpers, Rx.NET helpers
- Location: `Submodules/Gizmo.UI/Gizmo.UI/`
- Contains: `View/Services/ViewServiceBase.cs`, `View/Services/ViewStateServiceBase.cs`, `View/Services/ViewStateLookupServiceBase.cs`, `UICompositionServiceBase.cs`, `ServiceCollectionExtensions.cs`
- Depends on: (minimal — ASP.NET Core Components, System.Reactive)
- Used by: `Gizmo.Client.UI.Services`

**Domain Contracts:**
- Purpose: `IGizmoClient` interface, `ClientRoutes` constants, domain event args
- Location: `Submodules/Gizmo.Client.Shared/Gizmo.Client.Shared/`
- Contains: `Interfaces/IGizmoClient.cs`, `Code/ClientRoutes.cs`
- Depends on: `Gizmo.Web.Api.Models`, `Gizmo.Shared`

**DTOs / API Models:**
- Purpose: Request/response models for REST API and SignalR messaging
- Location: `Submodules/Gizmo.Web.Api.Models/`

**Page Modules (Extension Assembly):**
- Purpose: Example of a loadable page module loaded at runtime via `UIComposition:AdditionalAssemblies`
- Location: `Gizmo.Client.UI.Modules/`
- Contains: `Pages/CustomPageModule.razor` + `.razor.cs`

---

## Data Flow

**Normal page render flow:**

1. Razor component initializes → `OnInitialized()` calls `this.SubscribeChange(ViewState)` (via `CustomDOMComponentBase`)
2. Component injects `*ViewService` and calls a load/refresh method (e.g., `HomePageService.RefilterAsync()`)
3. `ViewService` calls `IGizmoClient` methods (e.g., `UserPopularProductsGetAsync()`) and lookup services
4. `ViewService` writes results into its `ViewState` properties
5. `ViewState` change notification fires → Blazor re-renders subscribed components
6. Component reads from `ViewState` in the Razor markup

**Event-driven updates:**

1. `IGizmoClient` fires domain events (e.g., `LoginStateChange`, `UserBalanceChange`)
2. Relevant `ViewService` subscribes to these events in `OnInitializing()`
3. Event handler updates `ViewState` → triggers re-render

**Lookup service flow:**

1. `PageViewService` injects one or more `*ViewStateLookupService` instances
2. Lookup service calls `IGizmoClient` to fetch a collection (e.g., `UserProductsGetAsync()`)
3. Lookup service maintains a keyed cache of `ViewState` objects
4. Page/component queries the lookup service rather than calling `IGizmoClient` directly

**State Management:**
- All state is held in singleton `*ViewState` classes registered in DI
- Components never hold mutable state — all data flows through injected `ViewState`
- `ValidatingViewStateBase` adds DataAnnotations validation support for form states
- No client-side Flux/Redux pattern; state is directly mutated by `ViewService` then notified

---

## Key Abstractions

**IGizmoClient:**
- Purpose: Single gateway to all backend data and commands; currently implemented by `TestClient` (web) and `DemoClient`
- Files: `Submodules/Gizmo.Client.Shared/Gizmo.Client.Shared/Interfaces/IGizmoClient.cs`, `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Client/TestClient.cs`
- Pattern: Interface registered as singleton in `Program.cs` — swap implementation to go live

**ViewStateServiceBase\<TViewState\>:**
- Purpose: Base for all page-level `*ViewService` classes; wires ViewState dependency, provides `InitializeAsync`, `OnInitializing`
- Files: `Submodules/Gizmo.UI/Gizmo.UI/View/Services/ViewStateServiceBase.cs`
- Pattern: `[Register()]` attribute on concrete class → auto-registered as singleton by reflection

**ViewStateLookupServiceBase:**
- Purpose: Base for services that cache collections of entity-level `ViewState` objects keyed by ID
- Files: `Submodules/Gizmo.UI/Gizmo.UI/View/Services/ViewStateLookupServiceBase.cs`
- Pattern: Used by page ViewServices to avoid redundant `IGizmoClient` calls across components

**IUICompositionService / UICompositionServiceBase:**
- Purpose: Resolves which assemblies and root component to load at runtime from config
- Files: `Submodules/Gizmo.UI/Gizmo.UI/Services/UICompositionServiceBase.cs`, `Submodules/Gizmo.Client.UI.Services/Gizmo.Client.UI.Services/Services/WebAssemblyUICompositionService.cs`
- Pattern: Web implementation fetches additional assemblies via HTTP from `/_framework/{assemblyName}`

**CustomDOMComponentBase:**
- Purpose: Razor component base class; provides `SubscribeChange(viewState)` / `UnsubscribeChange(viewState)` for reactive re-render wiring
- Files: `Submodules/Gizmo.Web.Components/` (shared component library)
- Pattern: Pages and components inherit this; subscribe in `OnInitialized`, unsubscribe in `Dispose`

**[Register] attribute:**
- Purpose: Marks a class for automatic DI registration; `Scope` defaults to `Singelton`
- Files: `Submodules/Gizmo.Shared/Shared/Microsoft/Extensions/DependencyInjection/RegisterAttribute.cs`
- Pattern: Applied to all `*ViewService` and `*ViewState` concrete classes; scanned by `AddViewServices(assembly)` / `AddViewStates(assembly)`

---

## Entry Points

**Web Host:**
- Location: `Gizmo.Client.UI.Host.Web/Program.cs`
- Triggers: Browser loads `index.html` → Blazor WASM bootstrap → `Program.Main`
- Responsibilities:
  1. `WebAssemblyHostBuilder.CreateDefault(args)`
  2. Registers root components `App` and `HeadOutlet`
  3. `AddClientConfigurationSource()` — loads `appsettings.json`
  4. `AddClientOptions()` — binds all options sections (`UIComposition`, `ClientInterface`, `Currency`, etc.)
  5. `AddClientServices()` — registers: UI services, ViewServices, ViewStates, web API infrastructure; platform-detection selects `WebAssemblyUICompositionService` vs `DesktopUICompositionService`
  6. Host-specific singletons: `ClientDialogService`, `ClientNotificationService`, `WebInputLenguageService`, `ImageService`, `WebNotificationHost`
  7. `AddSingleton<IGizmoClient, TestClient>()` — registers current mock client
  8. `host.Services.InitializeClientServices()` — initializes `IUICompositionService` (loads assemblies) then calls `InitializeAsync` on all registered `IViewService` instances

**App Component:**
- Location: `Gizmo.Client.UI/App.razor` + `App.razor.cs`
- Triggers: Blazor renders `#app` div
- Responsibilities: Wires `JSRuntimeService`, `NavigationService`; feeds `IUICompositionService.AppAssembly` and `AdditionalAssemblies` into the Blazor `Router`; `ErrorBoundary` wraps `RouteView` with `CriticalErrorPage` fallback; default layout is `_Layout`

---

## Error Handling

**Strategy:** `ErrorBoundary` in `App.razor` catches unhandled render exceptions and shows `CriticalErrorPage`. Service-level errors are logged via injected `ILogger` in `ViewServiceBase`.

**Patterns:**
- `ViewServiceBase.InitializeAsync` wraps `OnInitializing` in try/catch, logs critical on failure
- ViewServices log errors but do not propagate exceptions to components
- Form states use DataAnnotations validation via `ValidatingViewStateBase`

---

## Cross-Cutting Concerns

**Logging:** `Microsoft.Extensions.Logging` injected into every `ViewServiceBase` subclass via constructor; minimum level set to `Trace` in `Program.cs`

**Validation:** DataAnnotations attributes on `ViewState` properties; `ValidatingViewStateBase` / `ValidatingViewStateServiceBase` pair handles validation trigger and error collection

**Authentication:** `IGizmoClient.IsUserLoggedIn` / `LoginStateChange` event drive auth state; `UserLoginStatusViewState` / `UserLoginStatusViewService` guard navigation; `UserAccessTokenHandler` and `UserApiClientDelegatingHandler` in `Gizmo.Client.UI.Services` handle token injection for web API clients

**Localization:** `ILocalizationService` (platform-specific: `WebLocalizationService` in browser, `WpfLocalizationService` in WPF); RESX resources in `Submodules/Gizmo.Client.UI.Resources`

**Platform detection:** `RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser"))` in `ServiceCollectionExtensions.cs` selects platform-specific service implementations

---

## DI Registration Summary

| Registration type | Mechanism |
|---|---|
| `*ViewService` classes | `[Register]` attribute + `AddViewServices(assembly)` reflection scan |
| `*ViewState` classes | `[Register]` attribute + `AddViewStates(assembly)` reflection scan |
| `IUICompositionService` | Manual in `AddClientUIServices()`, platform-branched |
| `IGizmoClient` | Manual in `Program.cs` — `TestClient` currently |
| Host-specific services | Manual in `Program.cs` |
| Web API HTTP clients | `AddSecureWebApiClients` / `AddUnsecureWebApiClients` builders |

---

*Architecture analysis: 2026-03-26*
