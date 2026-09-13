using System.Linq;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Style mapper.
    /// </summary>
    public sealed class StyleMapper : BaseMapper
    {
        #region OVERRIDES
        
        public override string AsString()
        {
            var result = string.Join("; ", Items.Select(i => i()).Where(i => i != null));
            if (result.Length > 0)
                result += ";";
            return result;
        } 

        #endregion
    }
}
