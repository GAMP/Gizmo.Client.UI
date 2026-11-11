using System.Collections.Generic;
using System.Linq;

using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public sealed record PasswordRule(string Text, bool Passed);

    public partial class PasswordTooltip : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Parameter]
        public PasswordTooltipViewState PasswordTooltipViewState { get; set; }

        private PasswordTooltipViewState State => PasswordTooltipViewState;

        private bool IsStrong => State.TotalRules > 0 && State.PassedRules >= State.TotalRules;

        protected string ClassName => IsStrong
            ? "giz-password-tooltip giz-password-tooltip--strong"
            : State.PassedRules == 0 ? "giz-password-tooltip giz-password-tooltip--weak" : "giz-password-tooltip giz-password-tooltip--fair";

        protected string Title => IsStrong
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PASSWORD_STRONG)
            : string.IsNullOrWhiteSpace(State.ErrorMessage) ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PASSWORD_WEAK) : State.ErrorMessage;

        protected string CountText => $"{State.PassedRules}/{State.TotalRules}";

        protected IEnumerable<bool> Segments => Enumerable.Range(0, State.TotalRules).Select(i => i < State.PassedRules);

        protected IReadOnlyList<PasswordRule> Rules
        {
            get
            {
                var rules = new List<PasswordRule>();
                var min = State.MinimumLengthRule ?? 0;
                var max = State.MaximumLengthRule ?? 0;

                if (min > 0 && max > 0)
                    rules.Add(new(GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RULE_RANGE_LENGTH), min, max), State.LengthRulePassed));
                else if (min > 0)
                    rules.Add(new(GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RULE_MIN_LENGTH), min), State.LengthRulePassed));
                else if (max > 0)
                    rules.Add(new(GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RULE_MAX_LENGTH), max), State.LengthRulePassed));

                if (State.HasLowerCaseCharactersRule)
                    rules.Add(new(GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RULE_LOWER_CASE)), State.LowerCaseCharactersRulePassed));

                if (State.HasUpperCaseCharactersRule)
                    rules.Add(new(GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RULE_UPPER_CASE)), State.UpperCaseCharactersRulePassed));

                if (State.HasNumbersRule)
                    rules.Add(new(GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RULE_NUMBER)), State.NumbersRulePassed));

                if (State.HasSpecialCharactersRule)
                    rules.Add(new(GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PASSWORD_RULE_SPECIAL), State.SpecialCharactersRulePassed));

                return rules;
            }
        }

        protected static string SegmentClass(bool passed) => passed
            ? "giz-password-tooltip__segment giz-password-tooltip__segment--on"
            : "giz-password-tooltip__segment";

        protected static string RuleClass(PasswordRule rule) => rule.Passed
            ? "giz-password-tooltip__rule giz-password-tooltip__rule--passed"
            : "giz-password-tooltip__rule";

        protected static string RuleIcon(PasswordRule rule) => rule.Passed ? "ph-bold ph-check-circle" : "ph ph-circle";

        protected override void OnInitialized()
        {
            this.SubscribeChange(PasswordTooltipViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(PasswordTooltipViewState);

            base.Dispose();
        }
    }
}
