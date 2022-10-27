using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;

namespace Gizmo.Client.UI.Components
{
    public partial class CarouselItem : CustomDOMComponentBase
    {
        private bool _isCurrent;
        private int _slideDirection;

        [CascadingParameter]
        protected Carousel Parent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        internal void Clear()
        {
            _isCurrent = false;
            _slideDirection = 0;
        }

        internal void SlideLeft(bool current, bool changeState = false)
        {
            _isCurrent = current;
            _slideDirection = -1;

            if (changeState)
                InvokeAsync(StateHasChanged);
        }

        internal void SlideRight(bool current)
        {
            _isCurrent = current;
            _slideDirection = 1;
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
                .Add("giz-carousel-item")
                .If("giz-carousel-item--active", () => _slideDirection != 0)
                .If("giz-carousel-item--current", () => !_isCurrent && _slideDirection != 0)
                .If("slide-left-current", () => _isCurrent && _slideDirection == -1)
                .If("slide-left-next", () => !_isCurrent && _slideDirection == -1)
                .If("slide-right-current", () => _isCurrent && _slideDirection == 1)
                .If("slide-right-next", () => !_isCurrent && _slideDirection == 1)
                .AsString();

        #endregion

    }
}