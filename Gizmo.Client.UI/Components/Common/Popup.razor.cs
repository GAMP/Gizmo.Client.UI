using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class Popup : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Popup()
        {
        }
        #endregion

        #region FIELDS

        private bool _isOpen;

        private bool _customPosition;
        private double _top;
        private double _left;

        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;
                _ = IsOpenChanged.InvokeAsync(_isOpen);

                if (_isOpen && CanFocus)
                {
                    Task.Run(async () =>
                    {
                        await Focus();
                    });
                }
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public string MaximumHeight { get; set; }

        [Parameter]
        public bool IsModal { get; set; }

        [Parameter]
        public bool CloseOnClick { get; set; }

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

        [Parameter]
        public bool OffsetY { get; set; } = true;

        [Parameter]
        public bool CanFocus { get; set; }

        [Parameter]
        public bool HasDisabledCursor { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickInsidePopup { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnMouseDownOutsidePopup { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnMouseUpOutsidePopup { get; set; }

        #endregion

        #region METHODS

        private async Task Focus()
        {
            await InvokeVoidAsync("focusElement", Ref);
        }

        public void SetPosition(double top, double left)
        {
            _customPosition = true;
            _top = top;
            _left = left;
        }

        public void ResetPosition()
        {
            _customPosition = false;
            _top = 0;
            _left = 0;
        }

        public void Open()
        {
            IsOpen = true;

            StateHasChanged();
        }

        public void Close()
        {
            IsOpen = false;

            StateHasChanged();
        }

        #endregion

        #region EVENTS

        protected Task ClickInsidePopupHandler(MouseEventArgs args)
        {
            if (CloseOnClick)
                IsOpen = false;

            return OnClickInsidePopup.InvokeAsync(args);
        }

        protected void OnFocusOutHandler()
        {
            if (!IsModal && CanFocus)
                IsOpen = false;
        }

        protected Task OnMouseDownOutsidePopupHandler(MouseEventArgs args)
        {
            if (!IsModal)
                IsOpen = false;

            return OnMouseDownOutsidePopup.InvokeAsync(args);
        }

        protected Task OnMouseUpOutsidePopupHandler(MouseEventArgs args)
        {
            return OnMouseUpOutsidePopup.InvokeAsync(args);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-popup")
                 .Add($"giz-popup--{OpenDirection.ToDescriptionString()}")
                 .If("giz-popup--offset", () => OffsetY)
                 .If("giz-popup--open", () => IsOpen)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"max-height: {MaximumHeight}", () => !string.IsNullOrEmpty(MaximumHeight))
                 .If($"pointer-events: all", () => CanFocus)
                 .If($"top: {_top.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => _customPosition)
                 .If($"left: {_left.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => _customPosition)
                 .AsString();

        protected string PopupWrapperClassName => new ClassMapper()
                 .If("giz-popup-wrapper", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-popup-wrapper--visible", () => OpenDirection == PopupOpenDirections.Cursor && IsOpen)
                 .AsString();

        protected string PopupWrapperStyleValue => new StyleMapper()
                 .If($"pointer-events: none", () => CanFocus || HasDisabledCursor)
                 .AsString();

        #endregion

    }
}
