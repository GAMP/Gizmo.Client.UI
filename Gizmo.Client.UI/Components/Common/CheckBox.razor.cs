using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class CheckBox : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public CheckBox()
        {
        }
        #endregion

        #region FIELDS

        private bool _isChecked;
        private bool _isTriState;
        private bool _isIndeterminate;
        private bool _nextIndeterminate = false;

        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets if checkbox is checked.
        /// </summary>
        [Parameter()]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        [Parameter]
        public EventCallback<bool> IsCheckedChanged
        {
            get; set;
        }

        [Parameter()]
        public bool IsDisabled
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets if checkbox is tri state.
        /// </summary>
        [Parameter()]
        public bool IsTriState
        {
            get { return _isTriState; }
            set { _isTriState = value; }
        }

        [Parameter()]
        public bool IsReadOnly { get; set; }

        [Parameter()]
        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set
            {
                if (_isIndeterminate != value)
                {
                    _isIndeterminate = value;
                    StateHasChanged();

                    Task.Run(async () =>
                    {
                        await TrySetIndeterminateAsync(_isIndeterminate);
                    });
                }
            }
        }

        public bool PreventDefault
        {
            get
            {
                return IsReadOnly || _isIndeterminate;
            }
        }

        #endregion

        #region METHODS

        private async Task TrySetIndeterminateAsync(bool value)
        {
            await InvokeVoidAsync("setPropByElement", Ref, "indeterminate", value);
        }

        #endregion

        #region EVENTS

        protected override async Task OnFirstAfterRenderAsync()
        {
            if (IsIndeterminate)
            {
                await TrySetIndeterminateAsync(true);
            }
        }

        protected Task OnChangeCheckBoxHandler(ChangeEventArgs args)
        {
            var value = (bool)args.Value;

            if (IsChecked != value)
            {
                _nextIndeterminate = value;
                IsChecked = value;
                return IsCheckedChanged.InvokeAsync(IsChecked);
            }

            return Task.CompletedTask;
        }

        protected Task OnClickCheckBoxHandler(MouseEventArgs args)
        {
            if (IsIndeterminate)
            {
                IsIndeterminate = false;
            }
            else
            {
                if (IsTriState)
                {
                    if (_nextIndeterminate)
                    {
                        IsIndeterminate = true;
                    }
                }
            }

            return Task.CompletedTask;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-check-box")
                 .If("disabled", () => IsDisabled)
                 .AsString();

        #endregion

    }
}
