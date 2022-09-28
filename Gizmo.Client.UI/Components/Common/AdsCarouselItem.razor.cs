using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarouselItem : CustomDOMComponentBase
    {
        private int _index;
        private int _slideDirection;
        private int _fade;

        [CascadingParameter]
        protected AdsCarousel Parent { get; set; }

        [Parameter]
        public AdvertisementViewState Advertisement { get; set; }

        internal void Clear()
        {
            _index = 0;
            _slideDirection = 0;
            _fade = 0;

            StateHasChanged();
        }

        public void SetIndex(int index, int slideDirection)
        {
            _index = index;
            _slideDirection = slideDirection;

            StateHasChanged();
        }

        public void FadeOut()
        {
            _fade = -1;

            StateHasChanged();
        }

        public void FadeIn(int index)
        {
            _index = index;
            _fade = 1;

            StateHasChanged();
        }

        #region OVERRIDE

        protected override void OnInitialized()
        {
            if (Parent != null)
            {
                Parent.Register(this);
            }
        }

        public override void Dispose()
        {
            try
            {
                if (Parent != null)
                {
                    Parent.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-ads-carousel-item")
                .If("current", () => _index == 1)
                .If("second", () => _index == 2)
                .If("third", () => _index == 3)
                .If("next", () => _index == 4)
                .If("slide-left-current", () => _index == 1 && _slideDirection == -1)
                .If("slide-left-second", () => _index == 2 && _slideDirection == -1)
                .If("slide-left-third", () => _index == 3 && _slideDirection == -1)
                .If("slide-left-next", () => _index == 4 && _slideDirection == -1)
                .If("slide-right-current", () => _index == 1 && _slideDirection == 1)
                .If("slide-right-second", () => _index == 2 && _slideDirection == 1)
                .If("slide-right-third", () => _index == 3 && _slideDirection == 1)
                .If("slide-right-next", () => _index == 4 && _slideDirection == 1)
                .If("fade-out-previously", () => _index > 0 && _fade == -1)
                .If("fade-in-current", () => _index > 0 && _fade == 1)
                .AsString();

        #endregion
    }
}