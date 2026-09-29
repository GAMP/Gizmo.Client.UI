using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public enum RegistrationStep
    {
        Verify,
        Account,
        About,
    }

    public partial class RegistrationStage : CustomDOMComponentBase
    {
        private static readonly RegistrationStep[] ProviderFlow = { RegistrationStep.Verify, RegistrationStep.Account, RegistrationStep.About };
        private static readonly RegistrationStep[] DirectFlow = { RegistrationStep.Account, RegistrationStep.About };

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        UserRegistrationConfigurationViewState ConfigurationViewState { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewState BasicFieldsViewState { get; set; }

        [Inject]
        LogoViewState LogoViewState { get; set; }

        [Parameter]
        public RegistrationStep Step { get; set; }

        private IReadOnlyList<RegistrationStep> Steps => ConfigurationViewState.IsDirectEnabled ? DirectFlow : ProviderFlow;

        private int CurrentIndex => Math.Max(0, Steps.ToList().IndexOf(Step));

        private bool HasClubLogo => !string.IsNullOrEmpty(LogoViewState.Logo);

        private bool HasNick => !string.IsNullOrWhiteSpace(BasicFieldsViewState.Username);

        private string NickText => HasNick
            ? BasicFieldsViewState.Username.Trim()
            : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_SIGNUP_NICK_PLACEHOLDER);

        private string NickClass => HasNick ? "giz-signup-card__nick" : "giz-signup-card__nick giz-signup-card__nick--empty";

        private string FullName => string.Join(" ", new[] { BasicFieldsViewState.FirstName, BasicFieldsViewState.LastName }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .Select(part => part.Trim()));

        private string SinceText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_SIGNUP_SINCE, DateTime.Now.ToString("d", CultureInfo.CurrentCulture));

        private string StepNumber(int index) => (index + 1).ToString(CultureInfo.CurrentCulture);

        private string StepLabel(int index) => GrafitLocalization.GetString(Steps[index] switch
        {
            RegistrationStep.Verify => GrafitResourceKeys.SHELL_SIGNUP_STEP_VERIFY,
            RegistrationStep.Account => GrafitResourceKeys.SHELL_SIGNUP_STEP_ACCOUNT,
            _ => GrafitResourceKeys.SHELL_SIGNUP_STEP_ABOUT,
        });

        private string StepClass(int index)
        {
            if (index < CurrentIndex)
                return "giz-signup-stage__step giz-signup-stage__step--done";

            return index == CurrentIndex ? "giz-signup-stage__step giz-signup-stage__step--current" : "giz-signup-stage__step";
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(BasicFieldsViewState);
            this.SubscribeChange(LogoViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(BasicFieldsViewState);
            this.UnsubscribeChange(LogoViewState);

            base.Dispose();
        }
    }
}
