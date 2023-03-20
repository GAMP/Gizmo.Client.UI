﻿using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class PasswordTooltip : CustomDOMComponentBase
    {
        private int _rulesPassed = 0;

        private bool _lengthRulePassed = false;
        private bool _lowerRulePassed = false;
        private bool _upperRulePassed = false;
        private bool _numberRulePassed = false;

        [Parameter]
        public string Password { get; set; }

        private void Calculate()
        {
            _rulesPassed = 0;

            _lengthRulePassed = false;
            _lowerRulePassed = false;
            _upperRulePassed = false;
            _numberRulePassed = false;

            if (string.IsNullOrEmpty(Password))
                return;

            if (Password.Length >= 8)
            {
                _rulesPassed += 1;
                _lengthRulePassed = true;
            }

            Regex lowerRulePassedRegex = new Regex("[A-Z]");
            Regex upperRuleRegex = new Regex("[a-z]");
            Regex numberRuleRegex = new Regex("[0-9]");

            if (lowerRulePassedRegex.Matches(Password).Count > 0)
            {
                _rulesPassed += 1;
                _lowerRulePassed = true;
            }

            if (upperRuleRegex.Matches(Password).Count > 0)
            {
                _rulesPassed += 1;
                _upperRulePassed = true;
            }

            if (numberRuleRegex.Matches(Password).Count > 0)
            {
                _rulesPassed += 1;
                _numberRulePassed = true;
            }
        }

        private string GetColor()
        {
            switch (_rulesPassed)
            {
                case 2:
                    return "#F2994A";

                case 3:
                    return "#F2C94C";

                case 4:
                    return "#219653"; //TODO: A ICON

                default:
                    return "#EB5757";
            }
        }

        private string GetPasswordDescription()
        {
            if (_rulesPassed == 4)
                return "Secure Password"; //TODO: A TRANSLATION

            if (!_lengthRulePassed)
                return "Password is too short"; //TODO: A TRANSLATION

            return "Password is too easy"; //TODO: A TRANSLATION
        }


        protected override async Task OnParametersSetAsync()
        {
            Calculate();

            await base.OnParametersSetAsync();
        }
    }
}
