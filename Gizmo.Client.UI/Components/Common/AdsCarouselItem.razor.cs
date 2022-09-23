using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarouselItem : CustomDOMComponentBase
    {
        private int _index;

        [CascadingParameter]
        protected AdsCarousel Parent { get; set; }

        [Parameter]
        public AdvertisementViewState Advertisement { get; set; }

        public void SetIndex(int index)
        {
            _index = index;

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
                .If("first", () => _index == 1)
                .If("second", () => _index == 2)
                .If("third", () => _index == 3)
                .AsString();

        #endregion
    }
}
