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
        private int _fade;
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
            _fade = 0;

            InvokeAsync(StateHasChanged);
        }

        public void FadeOut()
        {
            _fade = -1;

            InvokeAsync(StateHasChanged);
        }

        public void FadeIn(int index)
        {
            _index = index;
            _fade = 1;

            InvokeAsync(StateHasChanged);
        }

        private Task OnClickHandler()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return Task.CompletedTask;
            }

            if (!string.IsNullOrEmpty(_advertisementViewState.MediaUrl))
            {
                return ShowMediaDialogAsync();
            }

            return Task.CompletedTask;
        }

        private Task ShowMediaDialogAsync() =>
            AdvertisementsService.ShowMediaSync(_advertisementViewState);

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
                .If("giz-ads-media", () => _advertisementViewState.MediaUrlType != AdvertisementMediaUrlType.None && _advertisementViewState.MediaUrlType != AdvertisementMediaUrlType.Custom)
                .If("previous", () => _index == 1)
                .If("current", () => _index == 2)
                .If("next", () => _index == 3)
                .If("fade-out-previously", () => _index > 0 && _fade == -1)
                .If("fade-in-current", () => _index > 0 && _fade == 1)
                .AsString();

        #endregion
    }
}
