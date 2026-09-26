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
        public async Task OnKeyboardLayoutDetected(string sampleChar)
        {
            var language = LanguageFromSampleChar(sampleChar);

            if (language is not null)
                await ApplyDetectedLanguageAsync(language);

            await InvokeAsync(StateHasChanged);
        }

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
