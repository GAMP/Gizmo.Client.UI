using System;
using System.Collections.Generic;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components.Login
{
    public partial class VerificationProviderSelector : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public IReadOnlyList<ProviderOption> PriorityProviders { get; set; } = Array.Empty<ProviderOption>();

        [Parameter]
        public IReadOnlyList<ProviderOption> AltProviders { get; set; } = Array.Empty<ProviderOption>();

        [Parameter]
        public bool HasError { get; set; }

        [Parameter]
        public string ErrorMessage { get; set; } = string.Empty;

        [Parameter]
        public bool ShowAll { get; set; }

        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public string Subtitle { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<int> OnSelect { get; set; }

        private static string GetProviderCssClass(bool isPrimary) =>
            isPrimary ? "giz-registration-provider-btn--primary" : string.Empty;
    }
}
