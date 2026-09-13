using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Forward ref context.
    /// </summary>
    /// <typeparam name="TRef">Type parameter.</typeparam>
    public partial class ForwardRefContext<TRef> : ComponentBase
    {
        #region FIELDS
        private readonly ForwardRef<TRef> _context = new ForwardRef<TRef>();
        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// Gets or sets child content.
        /// </summary>
        [Parameter]
        public RenderFragment<ForwardRef<TRef>> ChildContent { get; set; } 

        #endregion
    }
}
