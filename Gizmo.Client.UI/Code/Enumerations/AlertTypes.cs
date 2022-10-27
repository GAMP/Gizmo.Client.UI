using System.ComponentModel;

namespace Gizmo.Client.UI
{
    public enum AlertTypes
    {
        [Description("danger")]
        Danger,

        [Description("success")]
        Success,

        [Description("warning")]
        Warning,

        [Description("info")]
        Info,

        [Description("accent")]
        Accent
    }
}
