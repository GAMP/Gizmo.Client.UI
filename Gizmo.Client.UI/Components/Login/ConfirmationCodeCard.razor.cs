using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq.Expressions;

namespace Gizmo.Client.UI.Components.Login
{
    public partial class ConfirmationCodeCard : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public EditContext EditContext { get; set; }

        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public string Subtitle { get; set; } = string.Empty;

        [Parameter]
        public string Placeholder { get; set; } = string.Empty;

        [Parameter]
        public string ConfirmationCode { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> CodeChanged { get; set; }

        [Parameter]
        public Expression<Func<string>> CodeExpression { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public bool? IsValid { get; set; }

        [Parameter]
        public bool HasError { get; set; }

        [Parameter]
        public string ErrorMessage { get; set; } = string.Empty;

        [Parameter]
        public EventCallback OnConfirm { get; set; }

        [Parameter]
        public EventCallback OnCloseError { get; set; }

        [Parameter]
        public RenderFragment HeaderLinks { get; set; }

        [Parameter]
        public RenderFragment FallbackContent { get; set; }
    }
}
