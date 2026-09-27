using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class InputLanguageMenu : ShellComponentBase
    {
        [Inject]
        public InputLanguageViewService LanguageService { get; set; }

        [Inject]
        public InputLanguageViewState ViewState { get; set; }

        [Inject]
        public ClientLocalizationViewState LocalizationViewState { get; set; }

        private DotNetObjectReference<InputLanguageMenu> _selfRef;

        private Task ValueChangedHandler(CultureInfo culture)
        {
            return LanguageService.SetCurrentInputLanguageAsync(culture.TwoLetterISOLanguageName);
        }

        private void OnClientCultureChanged(object sender, EventArgs e)
        {
            DispatchWorkflow(async () =>
            {
                var uiLanguage = LocalizationViewState.CurrentCulture?.TwoLetterISOLanguageName;

                if (!string.IsNullOrEmpty(uiLanguage))
                    await ApplyDetectedLanguageAsync(uiLanguage);

                StateHasChanged();
            });
        }

        [JSInvokable]
        public async Task OnKeyboardLayoutDetected(string sample)
        {
            if (ScriptOfSample(sample) is { } script)
                await FollowLayoutAsync(script, ExactLanguageOfSample(sample));

            await InvokeAsync(StateHasChanged);
        }

        private enum Script { Latin, Cyrillic, Greek }

        private static readonly HashSet<string> CYRILLIC_LANGUAGES = new(StringComparer.OrdinalIgnoreCase)
        {
            "ru", "uk", "be", "bg", "mk", "sr", "kk", "ky", "mn", "tg", "tt", "ba",
        };

        private static Script? ScriptOfSample(string sample)
        {
            if (string.IsNullOrEmpty(sample))
                return null;

            var c = sample[0];

            if (c >= 'Ѐ' && c <= 'ӿ')
                return Script.Cyrillic;

            if (c >= 'Ͱ' && c <= 'Ͽ')
                return Script.Greek;

            return c < 'ɐ' ? Script.Latin : null;
        }

        private static string ExactLanguageOfSample(string sample)
        {
            if (sample is null || sample.Length < 3 || ScriptOfSample(sample) != Script.Cyrillic)
                return null;

            return (sample[1], sample[2]) switch
            {
                ('ы', 'щ') => "ru",
                ('і', 'ў') => "be",
                ('і', 'щ') => "uk",
                _ => null,
            };
        }

        private static Script ScriptOf(string language)
        {
            if (string.Equals(language, "el", StringComparison.OrdinalIgnoreCase))
                return Script.Greek;

            return language is not null && CYRILLIC_LANGUAGES.Contains(language) ? Script.Cyrillic : Script.Latin;
        }

        private Task FollowLayoutAsync(Script script, string exactLanguage)
        {
            var current = ViewState.CurrentInputLanguage?.TwoLetterISOLanguageName;

            if (exactLanguage is not null)
                return ApplyDetectedLanguageAsync(exactLanguage);

            if (current is not null && ScriptOf(current) == script)
                return Task.CompletedTask;

            var uiLanguage = LocalizationViewState.CurrentCulture?.TwoLetterISOLanguageName;

            var match = ViewState.AvailableInputLanguages
                .Select(a => a.TwoLetterISOLanguageName)
                .Where(a => ScriptOf(a) == script)
                .OrderBy(a => string.Equals(a, uiLanguage, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .FirstOrDefault();

            return match is null ? Task.CompletedTask : ApplyDetectedLanguageAsync(match);
        }

        private Task ApplyDetectedLanguageAsync(string twoLetterIsoName)
        {
            if (string.Equals(ViewState.CurrentInputLanguage?.TwoLetterISOLanguageName, twoLetterIsoName, StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;

            if (!ViewState.AvailableInputLanguages.Any(a => string.Equals(a.TwoLetterISOLanguageName, twoLetterIsoName, StringComparison.OrdinalIgnoreCase)))
                return Task.CompletedTask;

            return LanguageService.SetCurrentInputLanguageAsync(twoLetterIsoName);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            LocalizationViewState.OnChange += OnClientCultureChanged;

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _selfRef = CreateDotNetObjectReference(this);
                await InvokeVoidAsync("setupInputLayoutWatch", _selfRef, nameof(OnKeyboardLayoutDetected), 800);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            LocalizationViewState.OnChange -= OnClientCultureChanged;

            try
            {
                _ = InvokeVoidAsync("teardownInputLayoutWatch");
            }
            catch
            {
            }

            _selfRef?.Dispose();

            base.Dispose();
        }
    }
}
