using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarouselItem : CustomDOMComponentBase
    {
        private int _index;
        private int _fade;

        [CascadingParameter]
        protected AdsCarousel Parent { get; set; }

        [Parameter]
        public AdvertisementViewState Advertisement { get; set; }

        [Parameter]
        public bool Duplicate { get; set; }

        internal void Clear()
        {
            _index = 0;
            _fade = 0;

            InvokeAsync(StateHasChanged);
        }

        public void SetIndex(int index)
        {
            _index = index;

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
            Parent.SetCurrent(Advertisement); 
        }

        #region OVERRIDE

        protected override void OnInitialized()
        {
            if (Parent != null)
            {
                Parent.Register(this, Duplicate);
            }
        }

        public override void Dispose()
        {
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