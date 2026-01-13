using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class Avatar : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public string Image { get; set; }

        [Parameter]
        public AvatarSizes Size { get; set; } = AvatarSizes.Medium;

        [Parameter]
        public AvatarVariants Variant { get; set; } = AvatarVariants.None;

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-avatar")
                 .Add($"giz-avatar--{Size.ToDescriptionString()}")
                 .If($"giz-avatar--rounded", () => Variant == AvatarVariants.Rounded)
                 .If($"giz-avatar--circle", () => Variant == AvatarVariants.Circle)
                 .AsString();
        
        #endregion

    }
}
