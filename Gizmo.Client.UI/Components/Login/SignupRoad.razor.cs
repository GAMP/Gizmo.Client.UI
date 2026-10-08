using System;
using System.Collections.Generic;
using System.Linq;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public enum SignupStage
    {
        Rules,
        Verify,
        Nick,
        Password,
        Name,
        About,
        Contacts,
        Address,
        Card,
    }

    public static class SignupStages
    {
        public static bool HasName(IRegistrationSessionService session) =>
            session.RequiredUserInfo?.FirstName == true || session.RequiredUserInfo?.LastName == true;

        public static bool HasAbout(IRegistrationSessionService session) =>
            session.RequiredUserInfo?.BirthDate == true || session.RequiredUserInfo?.Sex == true;

        public static bool HasMobile(IRegistrationSessionService session) =>
            !session.HasConfirmedMobilePhone && session.RequiredUserInfo?.Mobile == true;

        public static bool HasEmail(IRegistrationSessionService session) =>
            session.Flow != RegistrationFlow.Email && session.RequiredUserInfo?.Email == true;

        public static bool HasContacts(IRegistrationSessionService session) =>
            HasMobile(session) || HasEmail(session) || session.RequiredUserInfo?.Phone == true;

        public static bool HasAddress(IRegistrationSessionService session) =>
            session.RequiredUserInfo?.Country == true ||
            session.RequiredUserInfo?.Address == true ||
            session.RequiredUserInfo?.City == true ||
            session.RequiredUserInfo?.PostCode == true;

        public static IReadOnlyList<SignupStage> Account(IRegistrationSessionService session)
        {
            var stages = new List<SignupStage> { SignupStage.Nick, SignupStage.Password };

            if (HasName(session))
                stages.Add(SignupStage.Name);

            if (HasAbout(session))
                stages.Add(SignupStage.About);

            if (HasContacts(session))
                stages.Add(SignupStage.Contacts);

            return stages;
        }

        public static IReadOnlyList<SignupStage> Road(IRegistrationSessionService session, bool hasRules, bool hasVerify)
        {
            var road = new List<SignupStage>();

            if (hasRules)
                road.Add(SignupStage.Rules);

            if (hasVerify)
                road.Add(SignupStage.Verify);

            road.AddRange(Account(session));

            if (HasAddress(session))
                road.Add(SignupStage.Address);

            road.Add(SignupStage.Card);

            return road;
        }
    }

    public sealed record SignupSlot(string Label, string Icon, string ClassName, string LineClass, bool IsLast);

    public partial class SignupRoad : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Parameter]
        public IReadOnlyList<SignupStage> Stages { get; set; } = Array.Empty<SignupStage>();

        [Parameter]
        public SignupStage Current { get; set; }

        protected IReadOnlyList<SignupSlot> Slots
        {
            get
            {
                var current = Math.Max(0, Stages.ToList().IndexOf(Current));

                return Stages
                    .Select((stage, index) =>
                    {
                        var done = index < current;
                        var now = index == current;

                        return new SignupSlot(
                            LabelOf(stage),
                            done ? "ph-bold ph-check" : "ph-bold " + IconOf(stage),
                            done ? "giz-signup-road__slot giz-signup-road__slot--done"
                                : now ? "giz-signup-road__slot giz-signup-road__slot--now"
                                : "giz-signup-road__slot",
                            done ? "giz-signup-road__line giz-signup-road__line--done" : "giz-signup-road__line",
                            index == Stages.Count - 1);
                    })
                    .ToList();
            }
        }

        private static string IconOf(SignupStage stage) => stage switch
        {
            SignupStage.Rules => "ph-seal-check",
            SignupStage.Verify => "ph-shield-check",
            SignupStage.Nick => "ph-user",
            SignupStage.Password => "ph-lock-key",
            SignupStage.Name => "ph-identification-card",
            SignupStage.About => "ph-cake",
            SignupStage.Contacts => "ph-phone",
            SignupStage.Address => "ph-map-pin",
            _ => "ph-flag-checkered",
        };

        private string LabelOf(SignupStage stage) => GrafitLocalization.GetString(stage switch
        {
            SignupStage.Rules => GrafitResourceKeys.SHELL_SIGNUP_ROAD_RULES,
            SignupStage.Verify => GrafitResourceKeys.SHELL_SIGNUP_STEP_VERIFY,
            SignupStage.Nick => GrafitResourceKeys.SHELL_SIGNUP_ROAD_NICK,
            SignupStage.Password => GrafitResourceKeys.SHELL_SIGNUP_ROAD_PASSWORD,
            SignupStage.Name => GrafitResourceKeys.SHELL_SIGNUP_ROAD_NAME,
            SignupStage.About => GrafitResourceKeys.SHELL_SIGNUP_STEP_ABOUT,
            SignupStage.Contacts => GrafitResourceKeys.SHELL_SIGNUP_ROAD_CONTACTS,
            SignupStage.Address => GrafitResourceKeys.SHELL_SIGNUP_ROAD_ADDRESS,
            _ => GrafitResourceKeys.SHELL_SIGNUP_STEP_DONE,
        });
    }
}
