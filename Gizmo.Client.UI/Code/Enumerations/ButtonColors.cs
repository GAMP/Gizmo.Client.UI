using System.ComponentModel;

namespace Gizmo.Client.UI
{
    /// <summary>
    /// Button sizes.
    /// </summary>
    public enum ButtonColors
    {
        [Description("primary")]
        Primary = 0,

        [Description("secondary")]
        Secondary = 1,

        [Description("warning")]
        Warning = 2,

        [Description("danger")]
        Danger = 3,

        [Description("success")]
        Success = 4,

        [Description("info")]
        Info = 5,

        [Description("accent")]
        Accent = 6
    }
}
