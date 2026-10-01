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
        Finish,
    }

    public partial class RegistrationStage : CustomDOMComponentBase
    {
        public sealed record OrbitIcon(string IconClass, double X, double Y, double Size, double Opacity, double Tilt);

        private static readonly IReadOnlyList<OrbitIcon> Orbit = BuildOrbit();

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        UserRegistrationConfigurationViewState ConfigurationViewState { get; set; }

        [Parameter]
        public RegistrationStep Step { get; set; }

        [Parameter]
        public bool HasAboutStep { get; set; }

        [Parameter]
        public PlayerCardData CardData { get; set; }

        private IReadOnlyList<OrbitIcon> OrbitIcons => Orbit;

        private IReadOnlyList<RegistrationStep> Steps
        {
            get
            {
                var steps = new List<RegistrationStep>();

                if (!ConfigurationViewState.IsDirectEnabled)
                    steps.Add(RegistrationStep.Verify);

                steps.Add(RegistrationStep.Account);

                if (HasAboutStep || Step == RegistrationStep.About)
                    steps.Add(RegistrationStep.About);

                steps.Add(RegistrationStep.Finish);

                return steps;
            }
        }

        private int CurrentIndex => Math.Max(0, Steps.ToList().IndexOf(Step));

        private bool IsFinish(int index) => Steps[index] == RegistrationStep.Finish;

        private string StepNumber(int index) => (index + 1).ToString(CultureInfo.CurrentCulture);

        private string StepLabel(int index) => GrafitLocalization.GetString(Steps[index] switch
        {
            RegistrationStep.Verify => GrafitResourceKeys.SHELL_SIGNUP_STEP_VERIFY,
            RegistrationStep.Account => GrafitResourceKeys.SHELL_SIGNUP_STEP_ACCOUNT,
            RegistrationStep.About => GrafitResourceKeys.SHELL_SIGNUP_STEP_ABOUT,
            _ => GrafitResourceKeys.SHELL_SIGNUP_STEP_DONE,
        });

        private string StepClass(int index)
        {
            if (index < CurrentIndex)
                return "giz-signup-stage__step giz-signup-stage__step--done";

            return index == CurrentIndex ? "giz-signup-stage__step giz-signup-stage__step--current" : "giz-signup-stage__step";
        }

        private string LinkClass(int index) => index < CurrentIndex
            ? "giz-signup-stage__link giz-signup-stage__link--done"
            : "giz-signup-stage__link";

        private static IReadOnlyList<OrbitIcon> BuildOrbit()
        {
            var items = new (string Icon, double Angle, double Reach, double Size, double Opacity)[]
            {
                ("ph-game-controller",    14, 1.00, 3.0, .55),
                ("ph-headset",            52, 1.12, 2.2, .30),
                ("ph-desktop-tower",      98, 1.04, 2.6, .45),
                ("ph-keyboard",          140, 1.14, 2.0, .25),
                ("ph-crosshair-simple",  184, 1.00, 2.8, .50),
                ("ph-mouse",             218, 1.12, 1.8, .22),
                ("ph-joystick",          256, 1.04, 2.4, .40),
                ("ph-lightning",         290, 1.12, 2.2, .30),
                ("ph-cpu",               322, 1.02, 2.8, .45),
                ("ph-wifi-high",         348, 1.14, 2.0, .25),
            };

            const double radiusX = 33.0;
            const double radiusY = 21.0;

            return items
                .Select(item =>
                {
                    var radians = item.Angle * Math.PI / 180.0;
                    return new OrbitIcon(
                        item.Icon,
                        Math.Round(Math.Cos(radians) * radiusX * item.Reach, 2),
                        Math.Round(Math.Sin(radians) * radiusY * item.Reach, 2),
                        item.Size,
                        item.Opacity,
                        Math.Round(Math.Sin(radians) * 15.0, 1));
                })
                .ToList();
        }

        private static string OrbitStyle(OrbitIcon icon) => string.Create(CultureInfo.InvariantCulture,
            $"--giz-orbit-x: {icon.X}rem; --giz-orbit-y: {icon.Y}rem; --giz-orbit-size: {icon.Size}rem; --giz-orbit-opacity: {icon.Opacity}; --giz-orbit-tilt: {icon.Tilt}deg");
    }
}
