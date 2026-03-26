# Coding Conventions

**Analysis Date:** 2026-03-26

## Naming Patterns

### Files

- Razor components: `PascalCase.razor` + `PascalCase.razor.cs` (code-behind pair)
- Layouts: `_Layout.razor` / `_Layout.razor.cs` (underscore prefix for layouts)
- Login sub-layouts: `Layout_LoginRotator.razor` (compound prefix, no underscore on child)
- Pages: `PascalCase.razor` / `PascalCase.razor.cs` in `Pages/<Section>/`
- Shared shell components: `PascalCase.razor` / `PascalCase.razor.cs` in `Shared/`
- C# non-component code: `PascalCase.cs` in `Code/`, `Code/Services/`, `Code/ViewModels/`, `Code/Enumerations/`
- SCSS partials: `_kebab-case.scss` with underscore prefix

### C# Classes

- Components, pages, view services, view states: `PascalCase`
- Private fields: `_camelCase` (enforced by `.editorconfig`)
- Constants: `PascalCase` (enforced by `.editorconfig`)
- Static fields: `PascalCase`
- Parameters, local variables: `camelCase`
- Namespaces follow project/folder hierarchy: `Gizmo.Client.UI.Components`, `Gizmo.Client.UI.Pages`, `Gizmo.Client.UI.Shared`

### View Services and View States

- View services must end with `ViewService`: `AppsPageViewService`, `UserBalanceViewService`
- View states must end with `ViewState`: `AppsPageViewState`, `UserBalanceViewState`
- This suffix is required for automatic DI registration via `AddClientViewServices()` / `AddClientViewStates()`
- Deviating from these suffixes silently breaks registration

### Localization Keys

- All localization string keys use prefix `GIZ_` in `SCREAMING_SNAKE_CASE`
- Examples: `GIZ_MODULE_PAGE_APPS_TITLE`, `GIZ_GRACE_PERIOD_MESSAGE`, `GIZ_GEN_ADS`
- Accessed via `LocalizationService.GetString("KEY")` injected as `ILocalizationService`

### CSS Classes

- All project-specific classes use `giz-` prefix
- Structure follows BEM-inspired conventions (not strict BEM):
  - Block: `giz-block`
  - Element: `giz-block__element`
  - Modifier: `giz-block--modifier`
- Examples from `ApplicationCard.razor`: `giz-app-card`, `giz-app-card__content`, `giz-app-card__content__image`, `giz-app-card--hovered`
- Examples from `_Layout.razor`: `giz-background`, `giz-container`, `giz-app__header`, `giz-app__body`
- Non-project CSS classes (like `global-search-no-results`) appear in limited areas without `giz-` prefix

## Code Organization

### Region Blocks

C# files consistently use `#region` / `#endregion` blocks for organization:
- `#region PROPERTIES` — component parameters and injected services
- `#region FIELDS` — private fields in view services
- `#region METHODS` — public and private methods
- `#region CONSTRUCTOR` — constructor in view services
- `#region OVERRIDES` — overridden lifecycle methods
- `#region PARAMETERS` — nested inside PROPERTIES for Blazor `[Parameter]` attributes

### Import Organization

C# `using` directives are sorted with `System.*` first (enforced by `.editorconfig`):
```csharp
using System.Threading.Tasks;

using Gizmo.Client.Options;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;

using Microsoft.AspNetCore.Components;
```

Razor files use `@namespace` at the top to set the component namespace explicitly:
```razor
@namespace Gizmo.Client.UI.Pages
@inherits CustomDOMComponentBase
```

## Component and Page Pattern

### Standard Razor + Code-Behind Pattern

Every component or page consists of two files:

`Component.razor`:
```razor
@namespace Gizmo.Client.UI.Components
@inherits CustomDOMComponentBase

<div class="giz-block">
    <!-- markup -->
</div>
```

`Component.razor.cs`:
```csharp
namespace Gizmo.Client.UI.Components
{
    public partial class Component : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Inject]
        SomeViewState ViewState { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public SomeViewState Item { get; set; }

        #endregion

        #region METHODS

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            base.Dispose();
        }

        #endregion
    }
}
```

### Base Classes

- Components and pages inherit `CustomDOMComponentBase` (from `Gizmo.Web.Components`)
- Layouts inherit `LayoutComponentBase`
- Base component classes go in `Components/Base/`: `ButtonBase.cs`, `GizInputBase.cs`, `MaskedInputBase.cs`
- Interface definitions go in `Components/Interfaces/`: `IGizInput.cs`

### Page Module Attributes

Pages in the main nav modules use additional class-level attributes:
```csharp
[ModuleGuid(KnownModules.MODULE_APPS)]
[ModuleDisplayOrder(1)]
[PageUIModule(TitleLocalizationKey = "GIZ_MODULE_PAGE_APPS_TITLE", DescriptionLocalizationKey = "GIZ_MODULE_PAGE_APPS_DESCRIPTION")]
[DefaultRoute(ClientRoutes.ApplicationsRoute), Route(ClientRoutes.ApplicationsRoute)]
public partial class AppsIndex : CustomDOMComponentBase
```

### ViewService Pattern

View services:
- Are `sealed` classes
- Inherit `ViewStateServiceBase<TViewState>`
- Carry `[Register()]` attribute for automatic DI registration
- Carry `[Route(...)]` attribute to link to navigation lifecycle
- Receive dependencies via constructor injection
- Store private dependencies as `readonly` fields with `_camelCase` naming
- Expose public `Task`-returning methods called by components
- Mutate `ViewState` properties directly (properties have `internal set`)
- Call `DebounceViewStateChanged()` to trigger re-render after state changes

### ViewState Pattern

View states:
- Are `sealed` classes
- Inherit `ViewStateBase`
- Carry `[Register()]` attribute
- All properties have `internal set` — only the corresponding service may mutate them
- Initialize collection properties to `Enumerable.Empty<T>()` as default

### ViewState Subscription in Components

Components that react to state changes subscribe in `OnInitialized` and unsubscribe in `Dispose`:
```csharp
protected override void OnInitialized()
{
    this.SubscribeChange(ViewState);
    base.OnInitialized();
}

public override void Dispose()
{
    this.UnsubscribeChange(ViewState);
    base.Dispose();
}
```

Components needing async cleanup additionally implement `IAsyncDisposable`:
```csharp
public async ValueTask DisposeAsync()
{
    await InvokeVoidAsync("unregisterAppsSticky", Ref).ConfigureAwait(false);
    Dispose();
}
```

## Routing

- Route constants live in `Submodules/Gizmo.Client.Shared/Code/ClientRoutes.cs`
- Routes are applied to pages via `[Route(ClientRoutes.SomeRoute)]`
- Main module pages also carry `[DefaultRoute(ClientRoutes.SomeRoute)]`
- Login shell pages use `_Layout_Login` layout
- Main app pages use `_Layout` layout

## Async and Event Handling

- Event handlers return `Task` when possible
- Synchronous handlers return `Task.CompletedTask`
- `CancellationToken` parameters are propagated through service chains
- JSInterop calls use `InvokeVoidAsync(...)` (inherited from `CustomDOMComponentBase`)

## SCSS Conventions

### Layer Order (enforced by import sequence in `src/scss/themes/client/main.scss`)

1. `_variables.scss` — color tokens, z-index values, shadow values
2. `_typography.scss` — font definitions and typographic mixins
3. `_global.scss` — base theme body/container look
4. `gizmo.web.components/_*.scss` — shared primitive overrides (one file per component)
5. `components/_*.scss` — client-specific component and page styles

### Where to Write Styles

- Page-level styles: `src/scss/themes/client/components/_page-<section>.scss`
- Shell/layout styles: `src/scss/themes/client/components/_layout.scss` or `_layout-login.scss`
- Header styles: `src/scss/themes/client/components/_header.scss`
- Shared primitive overrides: `src/scss/themes/client/gizmo.web.components/_<component>.scss`
- External/white-label overrides: `src/scss/external.scss`
- Token changes: `src/scss/themes/client/_variables.scss`

### SCSS Nesting Style

BEM-like nesting using `&` for elements and modifiers:
```scss
.giz-header {
    &__modules-menu {           // element: giz-header__modules-menu
        &-item {                // element: giz-header__modules-menu-item
            &.active {          // modifier via state class
            }
        }
    }
}
```

Variables use `$name-theme-client` suffix for theme-scoped tokens: `$primary-color-theme-client`, `$typo-primary-theme-client`.

Z-index values are defined as named SCSS variables: `$dialog-index`, `$header-dropdown-index`, `$lock-overlay-index`.

### What Not to Edit

- `wwwroot/` — this is generated webpack output
- `Submodules/Gizmo.Web.Components/src/scss/` — shared library, not this project's source

## JavaScript Conventions

### Three-File Architecture

All JS source lives in `Gizmo.Client.UI/src/js/`:

| File | Class | Purpose |
|---|---|---|
| `internal.js` | `window.InternalFunctions` | Browser behavior, DOM events, fullscreen, keyboard interop |
| `external.js` | `window.ExternalFunctions` | White-label extensions, third-party integrations |
| `api.js` | `window.ClientAPI` | JS bridge for Blazor-to-JS calls via `DotNetObjectReference` |

### Function Naming

- Class names: `PascalCase` (e.g. `InternalFunctions`, `ExternalFunctions`)
- Method names: `PascalCase` for public static methods matching Blazor `[JSInvokable]` naming
- Private helpers: `camelCase` (e.g. `subscribe`, `fullScreenChangeHandler`)
- Nested feature classes are static inner classes: `InternalFunctions.FullScreen`

### Calling JS from Blazor

Via `CustomDOMComponentBase.InvokeVoidAsync(...)`:
```csharp
await InvokeVoidAsync("registerAdsAutoCollapse");
await InvokeVoidAsync("unregisterAppsSticky", Ref).ConfigureAwait(false);
```

### Calling Blazor from JS

Via `ClientAPI` class holding a `dotnetObjectReference`:
```javascript
window.ClientAPI = class ClientAPI {
    static dotnetObjectReference;
    static SetDotnetObjectReference(value) { this.dotnetObjectReference = value; }
    static async SetUsernameAsync(username) {
        await this.dotnetObjectReference.invokeMethodAsync("SetUsernameAsync", username);
    }
};
```

### External Extension Pattern

Custom integrations go in `ExternalFunctions` with static methods and optional nested classes:
```javascript
window.ExternalFunctions = class ExternalFunctions {
    static Advertisement = class Advertisement {
        static async OnLoad(parameters) { /* ... */ }
    };
};
```

## Error Handling in View Services

Services wrap IGizmoClient calls in try/catch and log via `Logger.LogError`:
```csharp
catch (Exception ex)
{
    Logger.LogError(ex, "Failed to filter applications.");
}
```

Services do not re-throw to components — failures are swallowed after logging, leaving the view state in its last valid state.

## Comments

- XML doc comments (`///`) on public properties and methods in view services and base classes
- `//TODO:` comments in code for known unfinished items (e.g., untranslated error strings, rendering bugs)
- Section comments in SCSS mark file purpose: `//=============== Page Apps ================//`
- Inline region comments in C#: `//TODO: te only reason we need...`

---

*Convention analysis: 2026-03-26*
