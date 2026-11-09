using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class AlertDialog : CustomDOMComponentBase
    {
        protected sealed record DialogAction(AlertDialogResultButton Result, string Text, bool IsPrimary);

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback<AlertDialogResult> ResultCallback { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Message { get; set; }

        [Parameter]
        public AlertDialogButtons Buttons { get; set; }

        [Parameter]
        public AlertTypes Icon { get; set; }

        protected string DialogClass => Icon switch
        {
            AlertTypes.Danger => "giz-tile-dialog giz-tile-dialog--danger",
            AlertTypes.Warning => "giz-tile-dialog giz-tile-dialog--warning",
            AlertTypes.Success => "giz-tile-dialog giz-tile-dialog--success",
            _ => "giz-tile-dialog"
        };

        protected string IconClass => Icon switch
        {
            AlertTypes.Danger => "ph-bold ph-warning-octagon",
            AlertTypes.Warning => "ph-bold ph-warning",
            AlertTypes.Success => "ph-bold ph-check-circle",
            AlertTypes.Info => "ph-bold ph-info",
            _ => IsQuestion ? "ph-bold ph-question" : "ph-bold ph-info"
        };

        protected bool HasMessage => !string.IsNullOrWhiteSpace(Message);

        protected string ShownTitle => Reworded?.Title ?? Title;

        protected string ShownMessage => Reworded?.Message ?? Message;

        private (string Title, string Message)? Reworded
        {
            get
            {
                if (Message == Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_LOGOUT_VERIFICATION_MESSAGE)))
                    return (GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DIALOG_SIGN_OUT_TITLE), GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DIALOG_SIGN_OUT_TEXT));

                if (Message == Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_SHOP_VERIFY_CLEAR_CART)))
                    return (GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DIALOG_CLEAR_CART_TITLE), GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DIALOG_CLEAR_CART_TEXT));

                return null;
            }
        }

        protected string CloseLabel => Text(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_CLOSE));

        protected IReadOnlyList<DialogAction> Actions => Buttons switch
        {
            AlertDialogButtons.OK => new[] { Primary(AlertDialogResultButton.OK, nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_OK)) },
            AlertDialogButtons.OKCancel => new[]
            {
                Secondary(AlertDialogResultButton.Cancel, nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_CANCEL)),
                Primary(AlertDialogResultButton.OK, nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_OK))
            },
            AlertDialogButtons.YesNo => new[]
            {
                Secondary(AlertDialogResultButton.No, nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_NO)),
                Primary(AlertDialogResultButton.Yes, nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_YES))
            },
            AlertDialogButtons.YesNoCancel => new[]
            {
                Secondary(AlertDialogResultButton.Cancel, nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_CANCEL)),
                Secondary(AlertDialogResultButton.No, nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_NO)),
                Primary(AlertDialogResultButton.Yes, nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_YES))
            },
            _ => Array.Empty<DialogAction>()
        };

        protected static string ActionClass(DialogAction action) => action.IsPrimary
            ? "giz-tile-dialog__button giz-tile-dialog__button--primary"
            : "giz-tile-dialog__button";

        private bool IsQuestion => Buttons is AlertDialogButtons.YesNo or AlertDialogButtons.YesNoCancel or AlertDialogButtons.OKCancel;

        private DialogAction Primary(AlertDialogResultButton result, string key) => new(result, Text(key), true);

        private DialogAction Secondary(AlertDialogResultButton result, string key) => new(result, Text(key), false);

        private string Text(string key) => GrafitLocalization.GetString(key);

        private string Vendor(string key) => LocalizationService.GetString(key);

        private async Task CloseDialog(AlertDialogResultButton alertDialogResultButton)
        {
            await ResultCallback.InvokeAsync(new AlertDialogResult() { Button = alertDialogResultButton });
        }
    }
}
