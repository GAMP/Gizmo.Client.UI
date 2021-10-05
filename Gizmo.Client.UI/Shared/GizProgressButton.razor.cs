using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class GizProgressButton : CustomDOMComponentBase
    {
        #region FIELDS

        private decimal _value;

        #endregion

        #region PROPERTIES

        [Parameter]
        public decimal Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        
        [Parameter]
        public bool Loading { get; set; }

        #endregion

    }
}
