using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class Badge : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Badge()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool IsInline { get; set; }

        [Parameter]
        public BadgeSize Size { get; set; } = BadgeSize.Normal;

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public string BackgroundColor { get; set; } //TODO: A = "#5a67f2";

        [Parameter]
        public bool IsVisible { get; set; } = true;

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-badge")
                 .If("giz-badge--corner", () => !IsInline)
                 .If("giz-badge--small", () => Size == BadgeSize.Small)
                 .AsString();

        protected string BadgeStyleValue => new StyleMapper()
                 .If($"color: {Color}", () => !string.IsNullOrEmpty(Color))
                 .If($"background-color: {BackgroundColor}", () => !string.IsNullOrEmpty(BackgroundColor))
                 .If($"border-color: {BackgroundColor}", () => !string.IsNullOrEmpty(BackgroundColor))
                 .If($"visibility: hidden", () => !IsVisible)
                 .AsString();

        #endregion

    }
}
