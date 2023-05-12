using System;
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
        private AdvertisementViewState _advertisementViewState;


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
        public bool Duplicate { get; set; }

        internal void Clear()
        {
            _index = 0;
            _direction = 0;

            InvokeAsync(StateHasChanged);
        }

        public void Hide()
        {
            _index = 0;
            _direction = 0;

            InvokeAsync(StateHasChanged);
        }

        public void ShowInPosition(int index, int direction)
        {
            _index = index;
            _direction = direction;

            InvokeAsync(StateHasChanged);
        }

        private Task OnClickHandler()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return Task.CompletedTask;
            }

            if (_index != 2)
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

            this.SubscribeChange(_advertisementViewState);

            Parent?.Register(this, Duplicate);
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_advertisementViewState);

            Parent?.Unregister(this, Duplicate);

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-ads-carousel-item")
                .If("giz-ads-carousel-item--media", () => _advertisementViewState.MediaUrlType != AdvertisementMediaUrlType.None)
                .If("previous", () => _index == 1)
                .If("current", () => _index == 2)
                .If("next", () => _index == 3)
                .If("bring-to-front-right", () => _index == 2 && _direction == 1)
                .If("send-to-back-left", () => _index == 1 && _direction == 1)
                .If("bring-to-front", () => _index == 3 && _direction == 1)

                .If("bring-to-front-left", () => _index == 2 && _direction == -1)
                .If("send-to-back-right", () => _index == 3 && _direction == -1)
                //.If("send-to-back", () => _index == 1 && _direction == -1)
                .AsString();

        #endregion
    }
}
