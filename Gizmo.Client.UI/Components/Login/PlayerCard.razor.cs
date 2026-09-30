using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public sealed record PlayerCardData(
        string Nick,
        string FirstName,
        string LastName,
        DateTime? BirthDate,
        string Phone,
        string Email,
        string City)
    {
        public static PlayerCardData From(
            UserRegistrationBasicFieldsViewState basic,
            UserRegistrationAdditionalFieldsViewState additional,
            IRegistrationSessionService session)
        {
            var phone = CountDigits(basic.MobilePhone) >= PlayerCard.PHONE_DIGITS_SHOWN + 1
                ? basic.MobilePhone
                : session.Flow == RegistrationFlow.Sms ? session.ActualContact : null;

            var email = !string.IsNullOrWhiteSpace(basic.Email) && basic.Email.Contains('@')
                ? basic.Email
                : session.Flow == RegistrationFlow.Email ? session.ActualContact : null;

            return new PlayerCardData(
                basic.Username?.Trim() ?? string.Empty,
                basic.FirstName?.Trim() ?? string.Empty,
                basic.LastName?.Trim() ?? string.Empty,
                basic.BirthDate,
                phone ?? string.Empty,
                email?.Trim() ?? string.Empty,
                additional.City?.Trim() ?? string.Empty);
        }

        internal static int CountDigits(string value) => value?.Count(char.IsDigit) ?? 0;
    }

    public partial class PlayerCard : CustomDOMComponentBase
    {
        internal const int PHONE_DIGITS_SHOWN = 4;

        private const string BARCODE_SAMPLE = "player";

        public sealed record CardFact(string Icon, string Text);

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        LogoViewState LogoViewState { get; set; }

        [Parameter]
        public PlayerCardData Data { get; set; }

        private bool HasClubLogo => !string.IsNullOrEmpty(LogoViewState.Logo);

        private string Nick => Data?.Nick ?? string.Empty;

        private bool HasNick => Nick.Length > 0;

        private IReadOnlyList<string> NickGlyphs => Glyphs(Nick);

        private string NickPlaceholder => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_SIGNUP_NICK_PLACEHOLDER);

        private string FullName => string.Join(" ", new[] { Data?.FirstName, Data?.LastName }
            .Where(part => !string.IsNullOrWhiteSpace(part)));

        private bool HasName => FullName.Length > 0;

        private IReadOnlyList<string> NameGlyphs => Glyphs(FullName);

        private string NamePlaceholder => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_SIGNUP_NAME_PLACEHOLDER);

        private string SinceText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_SIGNUP_SINCE,
            DateTime.Now.ToString("d", CultureInfo.CurrentCulture));

        private IReadOnlyList<CardFact> Facts
        {
            get
            {
                var facts = new List<CardFact>();

                if (Data?.BirthDate is DateTime birthDate)
                    facts.Add(new CardFact("ph-bold ph-cake", birthDate.ToString("M", CultureInfo.CurrentCulture)));

                if (PhoneTail is { Length: > 0 } phone)
                    facts.Add(new CardFact("ph-bold ph-device-mobile", phone));

                if (MaskedEmail is { Length: > 0 } email)
                    facts.Add(new CardFact("ph-bold ph-at", email));

                if (!string.IsNullOrWhiteSpace(Data?.City))
                    facts.Add(new CardFact("ph-bold ph-map-pin", Data.City));

                return facts;
            }
        }

        private string PhoneTail
        {
            get
            {
                var digits = new string((Data?.Phone ?? string.Empty).Where(char.IsDigit).ToArray());

                if (digits.Length <= PHONE_DIGITS_SHOWN)
                    return null;

                var tail = digits[^PHONE_DIGITS_SHOWN..];
                return $"••• {tail[..2]}-{tail[2..]}";
            }
        }

        private string MaskedEmail
        {
            get
            {
                var email = Data?.Email;
                var at = email?.IndexOf('@') ?? -1;

                if (at < 1 || at == email.Length - 1)
                    return null;

                return email[0] + "•••" + email[at..];
            }
        }

        private string BarcodeClass => HasNick
            ? "giz-player-card__code"
            : "giz-player-card__code giz-player-card__code--empty";

        private IReadOnlyList<string> Bars => Barcode(HasNick ? Nick : BARCODE_SAMPLE);

        internal static IReadOnlyList<string> Barcode(string source)
        {
            var bars = new List<string> { Bar(1, 1), Bar(1, 2) };

            foreach (var code in (source ?? string.Empty).Select(c => (int)c))
            {
                bars.Add(Bar(1 + code % 3, 1 + (code >> 2) % 2));
                bars.Add(Bar(1 + (code >> 3) % 2, 1 + (code >> 1) % 3));
            }

            bars.Add(Bar(1, 1));
            bars.Add(Bar(2, 0));

            return bars;
        }

        private static string Bar(int width, int gap) =>
            string.Create(CultureInfo.InvariantCulture, $"--giz-bar-w: {width}; --giz-bar-gap: {gap}");

        private static IReadOnlyList<string> Glyphs(string text)
        {
            var glyphs = new List<string>();
            var elements = StringInfo.GetTextElementEnumerator(text ?? string.Empty);

            while (elements.MoveNext())
                glyphs.Add(elements.GetTextElement());

            return glyphs;
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(LogoViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(LogoViewState);

            base.Dispose();
        }
    }
}
