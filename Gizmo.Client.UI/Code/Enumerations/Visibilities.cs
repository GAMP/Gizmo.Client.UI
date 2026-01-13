using System.ComponentModel;

namespace Gizmo.Client.UI
{
    /// <summary>
    /// Visibilities.
    /// </summary>
    public enum Visibilities
    {
        Default,

        [Description("visible")]
        Visible,

        [Description("hidden")]
        Hidden,

        [Description("collapse")]
        Collapse
    }
}
