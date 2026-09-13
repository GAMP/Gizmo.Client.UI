using System.Linq;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// CSS Class mapper.
    /// </summary>
    public sealed class ClassMapper : BaseMapper
    {
        #region OVERRIDES
        public override string AsString()
        {
            return string.Join(" ", Items.Select(i => i()).Where(i => i != null));
        } 
        #endregion
    }
}
