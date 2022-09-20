using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderGlobalSearch : CustomDOMComponentBase
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
        private string _text;
        private bool _openDropDown;

        private bool _hasFocus;

        #endregion

        #region PROPERTIES

        [Inject]
        SearchService SearchService { get; set; }

        [Parameter]
        public int MinimumCharacters { get; set; } = 1;

        #endregion

        #region EVENTS

        protected Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Enter":
                    break;

                default:
                    break;
            }

            return Task.CompletedTask;
        }

        protected Task OnFocusInHandler()
        {
            if (!string.IsNullOrEmpty(_text))
                _openDropDown = true;

            _hasFocus = true;

            return Task.CompletedTask;
        }

        protected Task OnFocusOutHandler()
        {
            _hasFocus = false;

            return Task.CompletedTask;
        }

        protected Task OnInputHandler(ChangeEventArgs args)
        {
            var newValue = args?.Value as string;

            if (newValue != _text)
            {
                _text = newValue;

                if (!_openDropDown)
                {
                    _openDropDown = true;
                }

                if (MinimumCharacters == 0 || (MinimumCharacters > 0 && _text.Length >= MinimumCharacters))
                {
                    _deferredAction.Defer(_delayTimeSpan);
                }
            }

            return Task.CompletedTask;
        }

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        private async Task Clear()
        {
            await SearchService.ClearResultsAsync();

            _text = string.Empty;
            _openDropDown = false;
        }

        #endregion

        private async Task Search()
        {
            await SearchService.ClearResultsAsync();
            await SearchService.SearchAsync(_text);
        }

        #region CLASSMAPPERS

        protected override void OnInitialized()
        {
            this.SubscribeChange(SearchService.ViewState);
            base.OnInitialized();
        }

        protected string CloseButtonStyleValue => new StyleMapper()
                 .If($"visibility: hidden", () => string.IsNullOrEmpty(_text))
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
            ClosePopupEventInterop?.Dispose();
            _ = JsRuntime.InvokeVoidAsync("unregisterPopup", Ref);

            base.Dispose();
        }

        private ClosePopupEventInterop ClosePopupEventInterop { get; set; }

        private Task ClosePopupHandler(string args)
        {
            if (!_hasFocus)
            {
                if (args == Id)
                    _openDropDown = false;

                StateHasChanged();
            }

            return Task.CompletedTask;
        }
    }
}