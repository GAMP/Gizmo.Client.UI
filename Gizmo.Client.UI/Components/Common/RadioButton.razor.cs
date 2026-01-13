using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class RadioButton : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public RadioButton()
        {
        }
        #endregion

        #region FIELDS

        private bool _isChecked;

        #endregion

        #region PROPERTIES

        [Parameter]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets if element is checked.
        /// </summary>
        [Parameter]
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (_isChecked == value)
                    return;

                _isChecked = value;
                _ = IsCheckedChanged.InvokeAsync(_isChecked);
            }
        }

        [Parameter]
        public EventCallback<bool> IsCheckedChanged { get; set; }

        /// <summary>
        /// Gets or sets if element is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets radio button name.
        /// </summary>
        [Parameter]
        public string GroupName { get; set; }

        #endregion

        #region EVENTS

        protected void OnChangeRadioButtonHandler(ChangeEventArgs args)
        {
            IsChecked = ((string)args.Value).Equals("on");
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-radio-button")
                 .If("disabled", () => IsDisabled)
                 .AsString();

        #endregion

    }
}
