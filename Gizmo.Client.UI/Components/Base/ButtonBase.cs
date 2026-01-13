using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Windows.Input;

namespace Gizmo.Client.UI.Components
{
    public class ButtonBase : CustomDOMComponentBase
    {
        protected bool _selected;

        #region PROPERTIES

        /// <summary>
        /// Gets or sets html button type.
        /// </summary>
        [Parameter]
        public string Type { get; set; } = "button";

        /// <summary>
        /// Gets or sets html element name.
        /// </summary>
        [Parameter]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets button size.
        /// </summary>
        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        /// <summary>
        /// Gets or sets button color.
        /// </summary>
        [Parameter]
        public ButtonColors Color { get; set; } = ButtonColors.Primary;

        /// <summary>
        /// Gets or sets if button is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets if button has shadow.
        /// </summary>
        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public ICommand Command { get; set; }

        [Parameter]
        public object CommandParameter { get; set; }

        #endregion

        public virtual bool GetSelected()
        {
            return _selected;
        }

        public virtual void SetSelected(bool selected)
        {
            _selected = selected;
        }
    }
}
