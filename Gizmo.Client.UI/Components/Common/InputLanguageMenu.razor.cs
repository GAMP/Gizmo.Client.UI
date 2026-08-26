using System;
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
    public partial class InputLanguageMenu : CustomDOMComponentBase
    {
        [Inject]
        public InputLanguageViewService LanguageService { get; set; }

        [Inject]
        public InputLanguageViewState ViewState { get; set; }

        // The keyboard-layout tile in the login screen's bottom-right corner
        // used to be completely independent of the interface language picked
        // in the bottom-left one: switching the shell to Russian left the
        // corner still saying ENG, with no way to tell it was even related.
        // Following the interface language is what people expect from these
        // two sitting on the same screen.
        [Inject]
        public ClientLocalizationViewState LocalizationViewState { get; set; }

        private DotNetObjectReference<InputLanguageMenu> _selfRef;

        private Task ValueChangedHandler(CultureInfo culture)
        {
            return LanguageService.SetCurrentInputLanguageAsync(culture.TwoLetterISOLanguageName);
        }

        //Raised from the localization service, not the UI thread. Not async void: a dispatcher
        //fault during host teardown would be rethrown on the thread pool and exit the client
        //(see CustomComponentBase.DispatchWorkflow).
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

        /// <summary>
        /// Called from JS whenever the OS keyboard layout changes (Alt+Shift
        /// and friends), with the character the physical A key now produces.
        /// </summary>
        /// <remarks>
        /// The desktop host's IInputLanguageService declares a LanguageChange
        /// event that it never raises, and its CurrentInputLanguage getter
        /// throws NotImplementedException - so nothing on the C# side ever
        /// learns about an Alt+Shift switch and this tile sat frozen on
        /// whatever was last picked through the UI. That service lives in the
        /// host EXE, which a skin cannot replace, so the layout is detected
        /// browser-side instead (see setupInputLayoutWatch in internal.js).
        /// </remarks>
        [JSInvokable]
        public async Task OnKeyboardLayoutDetected(string sampleChar)
        {
            var language = LanguageFromSampleChar(sampleChar);

            if (language is not null)
                await ApplyDetectedLanguageAsync(language);

            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Maps the character produced by the physical A key to a language,
        /// by script. Covers the scripts this shell actually ships layouts
        /// for; anything else stays unrecognised rather than guessing.
        /// </summary>
        private static string LanguageFromSampleChar(string sampleChar)
        {
            if (string.IsNullOrEmpty(sampleChar))
                return null;

            var c = sampleChar[0];

            if (c >= 'Ѐ' && c <= 'ӿ')
                return "ru";

            if (c >= 'Ͱ' && c <= 'Ͽ')
                return "el";

            if (c < 'ɐ')
                return "en";

            return null;
        }

        private Task ApplyDetectedLanguageAsync(string twoLetterIsoName)
        {
            if (string.Equals(ViewState.CurrentInputLanguage?.TwoLetterISOLanguageName, twoLetterIsoName, StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;

            // Only follow when that layout is actually installed on this
            // station - otherwise leave the customer's own choice alone.
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
                // JS runtime may already be gone - nothing left to clean up.
            }

            _selfRef?.Dispose();

            base.Dispose();
        }
    }
}
