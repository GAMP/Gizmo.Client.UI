using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class Icon : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Icon()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Source { get; set; }

        [Parameter]
        public Icons? SVGIcon { get; set; }

        [Parameter]
        public IconSizes Size { get; set; } = IconSizes.Medium;

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public IconBackgroundStyles BackgroundStyle { get; set; } = IconBackgroundStyles.None;

        [Parameter]
        public string BackgroundColor { get; set; }

        [Parameter]
        public bool IsVisible { get; set; } = true;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        #endregion

        #region EVENTS

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return OnClick.InvokeAsync(args);
        }

        #endregion

        #region CLASSMAPPERS

        protected string FAClassName => new ClassMapper()
                 .If("fa-xs", () => Size == IconSizes.Small)
                 .If("fa-1x", () => Size == IconSizes.Medium)
                 .If("fa-2x", () => Size == IconSizes.Large)
                 .If("fa-3x", () => Size == IconSizes.ExtraLarge)
                 .If("fa-stack", () => BackgroundStyle != IconBackgroundStyles.None)
                 .AsString();

        protected string FAIconClassName => new ClassMapper()
                 .If("fa-stack-1x", () => BackgroundStyle != IconBackgroundStyles.None)
                 .AsString();

        protected string ClassName => new ClassMapper()
                 .Add($"giz-icon")
                 .Add($"giz-icon--{Size.ToDescriptionString()}")
                 .If("giz-icon--circle", () => BackgroundStyle == IconBackgroundStyles.Circle)
                 .If("giz-icon--background", () => BackgroundStyle != IconBackgroundStyles.None)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"color: { Color }", () => !string.IsNullOrEmpty(Color))
                 .If($"background-color: { BackgroundColor }", () => BackgroundStyle != IconBackgroundStyles.None)
                 .If($"visibility: hidden", () => !IsVisible)
                 .AsString();

        #endregion

    }
}
