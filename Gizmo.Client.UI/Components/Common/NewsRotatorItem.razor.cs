using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;

namespace Gizmo.Client.UI.Components
{
    public partial class NewsRotatorItem : CustomDOMComponentBase
    {
        private bool _isCurrent;
        private bool _isNext;

        [CascadingParameter]
        protected NewsRotator Parent { get; set; }

        internal void Clear()
        {
            _isCurrent = false;
            _isNext = false;

            InvokeAsync(StateHasChanged);
        }

        internal void SetCurrent()
        {
            _isCurrent = true;

            InvokeAsync(StateHasChanged);
        }

        internal void SetNext()
        {
            _isNext = true;

            InvokeAsync(StateHasChanged);
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
                .Add("giz-news-rotator-item")
                .If("giz-news-rotator-item--current fade-out", () => _isCurrent)
                .If("giz-news-rotator-item--next fade-in", () => _isNext)
                .AsString();

        #endregion
    }
}