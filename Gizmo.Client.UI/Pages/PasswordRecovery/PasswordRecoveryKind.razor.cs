using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Net.Mail;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryKindRoute)]
    public partial class PasswordRecoveryKind : CustomDOMComponentBase
    {
        [CascadingParameter] RecoveryHandoff Recovery { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        PasswordRecoveryKindViewService PasswordRecoveryKindViewService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        protected bool IsHandingOff { get; private set; }

        protected override void OnInitialized()
        {
            if (Recovery?.Pending is { } request)
            {
                IsHandingOff = true;
                PasswordRecoveryKindViewService.SelectKind(KindOf(request));
            }

            base.OnInitialized();
        }

        private static bool IsEmail(string value) =>
            !string.IsNullOrWhiteSpace(value) &&
            MailAddress.TryCreate(value.Trim(), out var address) &&
            address.Address == value.Trim() &&
            address.Host.Contains('.');

        private static PasswordRecoveryIdentifierKind KindOf(RecoveryRequest request)
        {
            if (request.IsPhone)
                return PasswordRecoveryIdentifierKind.MobilePhone;

            return IsEmail(request.Value)
                ? PasswordRecoveryIdentifierKind.Email
                : PasswordRecoveryIdentifierKind.Username;
        }
    }
}
