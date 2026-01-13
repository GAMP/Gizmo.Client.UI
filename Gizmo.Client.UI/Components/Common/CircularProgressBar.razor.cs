using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class CircularProgressBar
    {
        #region CONSTRUCTOR
        public CircularProgressBar()
        {
        }
        #endregion

        #region FIELDS

        private decimal _value;
        private decimal _left;
        private decimal _right;
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

                if (_value > 50)
                {
                    _left = 180;
                    _right = (_value - 50) / 50 * 180;
                }
                else
                {
                    _left = _value / 50 * 180;
                    _right = 0;
                }
            }
        }

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public CircularProgressBarSizes Size { get; set; } = CircularProgressBarSizes.Medium;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-circular-progress-bar")
                 .Add($"giz-circular-progress-bar--{Size.ToDescriptionString()}")
                 .AsString();

        #endregion

    }
}
