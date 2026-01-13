using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProgressBar
    {
        #region CONSTRUCTOR
        public ProgressBar()
        {
        }
        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES

        [Parameter]
        public decimal Value { get; set; }

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public bool IsIndeterminate { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-progress-bar")
                 .If("giz-progress-bar--indeterminate", () => IsIndeterminate)
                 .AsString();

		protected string BackgroundStyleValue => new StyleMapper()
				 .If($"left: 0.1rem", () => Value > 0)
				 .If($"width: calc(100% - 0.1rem)", () => Value > 0)
				 .AsString();

		protected string BarStyleValue => new StyleMapper()
                 .If($"width: {Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}%", () => !IsIndeterminate)
                 .If($"background-color: {Color}", () => !string.IsNullOrEmpty(Color))
                 .AsString();

        #endregion

    }
}
