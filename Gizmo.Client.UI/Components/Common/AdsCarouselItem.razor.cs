using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarouselItem : CustomDOMComponentBase
    {
        private bool _clickHandled = false;

        private int _index;
        private int _direction;
        private bool _switchSide;
        private AdvertisementViewState _advertisementViewState;
        private Dictionary<string, object> _contentParameters;

        [Inject]
        public AdvertisementViewState AdvertisementViewState
        {
            get { return _advertisementViewState; }
            private set { _advertisementViewState = value; }
        }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        AdvertisementsViewService AdvertisementsService { get; set; }

        [Inject]
        CommandProviderService CommandProviderService { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [CascadingParameter]
        protected AdsCarousel Parent { get; set; }

        [Parameter]
        public int AdvertisementId { get; set; }

        [Parameter]
        public bool SimpleMode { get; set; }

        internal void Clear()
        {
            _index = 0;
            _direction = 0;
            _switchSide = false;

            InvokeAsync(StateHasChanged);
        }

        public void Hide()
        {
            _index = 0;
            _direction = 0;
            _switchSide = false;

            InvokeAsync(StateHasChanged);
        }

        public void ShowInPosition(int index, int direction, bool switchSide)
        {
            _index = index;
            _direction = direction;
            _switchSide = switchSide;

            InvokeAsync(StateHasChanged);
        }

        private Task OnClickHandler()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return Task.CompletedTask;
            }

            if (!SimpleMode && _index != 2)
            {
                Parent.SetCurrent(AdvertisementId);
            }
            else
            {
                if (!string.IsNullOrEmpty(_advertisementViewState.MediaUrl))
                {
                    return ShowMediaDialogAsync();
                }
            }

            return Task.CompletedTask;
        }

        private Task ShowMediaDialogAsync()
        {
            _clickHandled = true;

            return AdvertisementsService.ShowMediaSync(_advertisementViewState);
        }

        private async Task ViewDetailsAsync()
        {
            _clickHandled = true;

            if (_advertisementViewState.Url is not null)
                await JSRuntime.InvokeAsync<object>("open", CancellationToken.None, _advertisementViewState.Url);
        }

        private Task ExecuteCommandAsync()
        {
            _clickHandled = true;

            if (_advertisementViewState.Command is not null)
                return CommandProviderService.ExecuteCommandAsync(_advertisementViewState.Command);

            return Task.CompletedTask;
        }

        private Icons? GetCommandIcon()
        {
            if (_advertisementViewState.Command != null)
            {
                switch (_advertisementViewState.Command.Type)
                {
                    case ViewServiceCommandType.Add:
                        return Icons.ShoppingCart_Client;
                }
            }
            return Icons.Open_Client;
        }

        private string GetCommandName()
        {
            if (_advertisementViewState.Command != null)
            {
                switch (_advertisementViewState.Command.Type)
                {
                    case ViewServiceCommandType.Add:
                        return LocalizationService.GetString("GIZ_GEN_ADD_TO_CART");

                    case ViewServiceCommandType.Launch:
                        return LocalizationService.GetString("GIZ_GEN_LAUNCH");
                }
            }

            return LocalizationService.GetString("GIZ_GEN_VIEW_DETAILS");
        }

        #region OVERRIDE

        protected override async Task OnInitializedAsync()
        {
            _advertisementViewState = await AdvertisementsService.GetAdvertisementViewStateAsync(AdvertisementId);

            if (_advertisementViewState != null)
            {
                this.SubscribeChange(_advertisementViewState);

                if (_advertisementViewState.IsCustomTemplate)
                {
                    _contentParameters = new Dictionary<string, object>()
                    {
                        {nameof(AdvertisementViewState.Title), _advertisementViewState.Title }
                    };
                }
            }

            Parent?.Register(this);
        }

        public override void Dispose()
        {
            if (_advertisementViewState != null)
            {
                this.UnsubscribeChange(_advertisementViewState);
            }

            Parent?.Unregister(this);

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-ads-carousel-item")
                .If("giz-ads-carousel-item--media", () => _advertisementViewState.MediaUrlType != AdvertisementMediaUrlType.None)
                //Left item to fade out.
                .If("previous-out", () => _index == 4 && _direction == 1 && !_switchSide)
                //Left item.
                .If("previous", () => _index == 1)
                //Center item.
                .If("current", () => _index == 2)
                //Right item.
                .If("next", () => _index == 3)
                //Right item to fade out.
                .If("next-out", () => _index == 4 && _direction == -1 && !_switchSide)
                //From center to left.
                .If("send-to-back-left", () => _index == 1 && _direction == 1)
                //From center to right.
                .If("send-to-back-right", () => _index == 3 && _direction == -1)
                //From right to center.
                .If("bring-to-front-right", () => _index == 2 && _direction == 1)
                //From left to center.
                .If("bring-to-front-left", () => _index == 2 && _direction == -1)
                //.If("bring-to-front", () => _index == 3 && _direction == 1)
                //Fade out side item.
                .If("send-to-back", () => _index == 4 && !_switchSide)
                //From left to right.
                .If("send-to-right", () => _index == 3 && _direction == 1 && _switchSide)
                //From right to left.
                .If("send-to-left", () => _index == 1 && _direction == -1 && _switchSide)

                .AsString();

        #endregion
    }
}
