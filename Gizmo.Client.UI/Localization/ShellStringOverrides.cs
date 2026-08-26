using System;
using System.Collections.Generic;
using System.Globalization;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Localization
{
    /// <summary>
    /// Ships corrected interface strings inside the shell assembly.
    /// </summary>
    /// <remarks>
    /// The localization strings live in <c>Gizmo.Client.UI.Resources.dll</c>,
    /// which is part of the base client on each PC — not the skin. Editing the
    /// .resx therefore does not reach users through a skin swap. This class
    /// carries the fixed wording (currently ru + en) and is consulted before
    /// the resource manager, so the corrections travel with the skin's
    /// <c>Gizmo.Client.UI.dll</c>. Any key/culture not overridden falls through
    /// to the normal <see cref="ILocalizationService"/> lookup unchanged, so
    /// every other string and language is untouched.
    /// </remarks>
    public static class ShellStringOverrides
    {
        // key -> (two-letter culture -> text)
        private static readonly Dictionary<string, Dictionary<string, string>> _overrides =
            new()
            {
                ["GIZ_PASSWORD_RECOVERY_ENTER_EMAIL"] = new()
                {
                    ["ru"] = "Введите e-mail, привязанный к вашей учётной записи",
                    ["en"] = "Enter the e-mail linked to your account",
                },
                ["GIZ_PASSWORD_RECOVERY_ENTER_PHONE_NUMBER"] = new()
                {
                    ["ru"] = "Введите номер телефона, привязанный к вашей учётной записи",
                    ["en"] = "Enter the phone number linked to your account",
                },
                ["GIZ_PASSWORD_RECOVERY_PLEASE_ENTER_RECOVERY_CODE"] = new()
                {
                    ["ru"] = "Мы отправили код в SMS на {0}. Введите его ниже.",
                    ["en"] = "We sent a code by text to {0}. Enter it below.",
                },
                ["GIZ_PASSWORD_RECOVERY_SET_PASSWORD_MESSAGE"] = new()
                {
                    ["ru"] = "Почти готово — осталось придумать новый пароль.",
                    ["en"] = "Almost there — just pick a new password.",
                },
                ["GIZ_PASSWORD_RECOVERY_NO_VALID_MOBILE"] = new()
                {
                    ["ru"] = "К этому аккаунту не привязан номер телефона.",
                    ["en"] = "This account has no phone number linked to it.",
                },
                ["GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE"] = new()
                {
                    ["ru"] = "Не удалось сбросить пароль. Попробуйте ещё раз.",
                    ["en"] = "Couldn't reset the password. Please try again.",
                },
                ["GIZ_REGISTRATION_MOBILE_CONFIRMATION_MESSAGE"] = new()
                {
                    ["ru"] = "Проверьте код страны и введите номер телефона — мы отправим на него код подтверждения.",
                    ["en"] = "Check your country code and enter your phone number — we'll text you a confirmation code.",
                },
                // Заголовок экрана QR склеивался из «Войти через {0}» и имени
                // провайдера, которое клуб задаёт сам. У этого клуба оно —
                // «Получить код в Telegram», и вместе получалось «Войти через
                // Получить код в Telegram». Оставляем одно имя провайдера: оно
                // и так читается как действие, а для короткого имени вроде
                // «Telegram» это нормальный заголовок.
                ["GIZ_REGISTRATION_REDIRECT_TITLE"] = new()
                {
                    ["ru"] = "{0}",
                    ["en"] = "{0}",
                },
                // Подзаголовок «Без перенаправления, прямо в браузере» на этом
                // экране говорит не о том: рядом лежит QR-код и своя подсказка,
                // что с ним делать. Пустая строка — блок скрывается стилем.
                ["GIZ_REGISTRATION_REDIRECT_SUBTITLE"] = new()
                {
                    ["ru"] = "",
                    ["en"] = "",
                },
                ["GIZ_REGISTRATION_EMAIL_CONFIRMATION_MESSAGE"] = new()
                {
                    ["ru"] = "Введите e-mail — мы отправим на него код подтверждения.",
                    ["en"] = "Enter your e-mail — we'll send a confirmation code to it.",
                },
                ["GIZ_REGISTRATION_FAILED_MESSAGE"] = new()
                {
                    ["ru"] = "Что-то пошло не так. Обратитесь к администратору клуба.",
                    ["en"] = "Something went wrong. Please ask the club staff for help.",
                },
                ["GIZ_REGISTRATION_VE_MOBILE_PHONE_USED"] = new()
                {
                    ["ru"] = "Этот номер телефона уже занят.",
                    ["en"] = "That phone number is already taken.",
                },
                ["GIZ_REGISTRATION_VE_EMAIL_ADDRESS_USED"] = new()
                {
                    ["ru"] = "Этот e-mail уже занят.",
                    ["en"] = "That e-mail is already taken.",
                },

                // Login screen only (see Login.razor's field Label
                // attributes, which are the only call sites routed through
                // ShellStringOverrides for these two keys). The generic
                // GIZ_GEN_USERNAME/GIZ_GEN_PHONE_NUMBER resx text ("Имя
                // пользователя" / "Номер телефона") is correct everywhere
                // else it's used - it's only redundant here, immediately
                // under a segmented toggle that already says the exact same
                // word. Other call sites keep reading the resx directly and
                // are untouched by this override.
                ["GIZ_GEN_USERNAME"] = new()
                {
                    ["ru"] = "Логин или e-mail",
                    ["en"] = "Username or e-mail",
                },
                ["GIZ_GEN_PHONE_NUMBER"] = new()
                {
                    ["ru"] = "Введите номер",
                    ["en"] = "Enter your number",
                },
            };

        /// <summary>
        /// Returns the overridden string for the current UI culture, or the
        /// normal localized string when there is no override.
        /// </summary>
        /// <summary>
        /// То же, но для строк с подстановкой ({0}, {1}…).
        /// </summary>
        /// <remarks>
        /// Форматирование делается по инвариантной культуре, как и в самом
        /// LocalizationService: подставляются имена и числа, а не даты.
        /// </remarks>
        public static string Get(ILocalizationService localizationService, string key, params object[] args)
        {
            var format = Get(localizationService, key);

            if (args is null || args.Length == 0 || string.IsNullOrEmpty(format))
                return format;

            try
            {
                return string.Format(CultureInfo.CurrentCulture, format, args);
            }
            catch (FormatException)
            {
                //Строка без плейсхолдеров или с чужой разметкой - отдаём как есть,
                //это лучше, чем пустой экран.
                return format;
            }
        }

        public static string Get(ILocalizationService localizationService, string key)
        {
            if (_overrides.TryGetValue(key, out var byCulture))
            {
                var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                if (byCulture.TryGetValue(lang, out var text))
                    return text;
            }

            return localizationService.GetString(key);
        }
    }
}
