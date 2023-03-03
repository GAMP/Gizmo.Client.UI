using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarouselItem : CustomDOMComponentBase
    {
        private int _index;
        private int _fade;
        private AdvertisementViewState _advertisementViewState;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [CascadingParameter]
        protected AdsCarousel Parent { get; set; }

        [Inject]
        AdvertisementsService AdvertisementsService { get; set; }

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

        private void OnClickHandler()
        {
            Parent.SetCurrent(_advertisementViewState); 
        }

        #region OVERRIDE

        protected override async Task OnInitializedAsync()
        {
            _advertisementViewState = await AdvertisementsService.GetAdvertisementViewStateAsync(AdvertisementId);

            this.SubscribeChange(_advertisementViewState);

            if (Parent != null)
            {
                Parent.Register(this, Duplicate);
            }
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_advertisementViewState);

            try
            {
                if (Parent != null)
                {
                    Parent.Unregister(this, Duplicate);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-ads-carousel-item")
                .If("previous", () => _index == 1)
                .If("current", () => _index == 2)
                .If("next", () => _index == 3)
                .If("fade-out-previously", () => _index > 0 && _fade == -1)
                .If("fade-in-current", () => _index > 0 && _fade == 1)
                .AsString();

        #endregion
    }
}
