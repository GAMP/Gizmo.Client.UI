using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderGlobalSearch : CustomDOMComponentBase, IAsyncDisposable
    {
        const int DEFAULT_DELAY = 500;

        public HeaderGlobalSearch()
        {
            _deferredAction = new DeferredAction(Search);
            _delayTimeSpan = new TimeSpan(0, 0, 0, 0, _delay);
        }

        #region FIELDS

        private DeferredAction _deferredAction;
        private int _delay = DEFAULT_DELAY;
        private TimeSpan _delayTimeSpan;

        private bool _hasFocus;

        private ElementReference _inputElement;

        #endregion

        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        GlobalSearchService SearchService { get; set; }

        [Parameter]
        public int MinimumCharacters { get; set; } = 0;

        #endregion

        #region EVENTS

        protected async Task OnFocusHandler()
        {
            await _inputElement.FocusAsync();
        }

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Enter":

                    await SearchService.ProcessEnterAsync();

                    break;

                case "ArrowUp":

                    break;

                case "ArrowDown":

                    break;

                default:

                    break;
            }
        }

        protected Task OnFocusInHandler()
        {
            _hasFocus = true;

            return Task.CompletedTask;
        }

        protected Task OnFocusOutHandler()
        {
            _hasFocus = false;

            return Task.CompletedTask;
        }

        protected async Task OnInputHandler(ChangeEventArgs args)
        {
            var newValue = args?.Value as string;

            if (newValue != SearchService.ViewState.SearchPattern)
            {
                await SearchService.UpdateSearchPatternAsync(newValue);
                await SearchService.OpenSearchAsync();

                if (SearchService.ViewState.SearchPattern.Length == 0 || SearchService.ViewState.SearchPattern.Length >= MinimumCharacters)
                {
                    _deferredAction.Defer(_delayTimeSpan);
                }
            }
        }

        private async Task Clear()
        {
            await SearchService.ClearResultsAsync();
            await SearchService.CloseSearchAsync();
        }

        #endregion

        private async Task Search()
        {
            await SearchService.SearchAsync();
        }

        #region CLASSMAPPERS

        protected override void OnInitialized()
        {
            this.SubscribeChange(SearchService.ViewState);
            base.OnInitialized();
        }

        protected string CloseButtonStyleValue => new StyleMapper()
                 .If($"visibility: hidden", () => string.IsNullOrEmpty(SearchService.ViewState.SearchPattern))
                 .AsString();

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("registerPopup", Ref);
                ClosePopupEventInterop = new ClosePopupEventInterop(JsRuntime);
                await ClosePopupEventInterop.SetupClosePopupEventCallback(args => ClosePopupHandler(args));
            }
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(SearchService.ViewState);

            ClosePopupEventInterop?.Dispose();

            base.Dispose();
        }

        private ClosePopupEventInterop ClosePopupEventInterop { get; set; }

        private async Task ClosePopupHandler(string args)
        {
            if (!_hasFocus)
            {
                if (args == Id)
                {
                    await SearchService.ClearResultsAsync();
                    await SearchService.CloseSearchAsync();
                }
            }
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterPopup", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}