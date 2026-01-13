using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public class MaskedInputBase<TValue> : GizInputBase<TValue>, IGizInput
    {
        #region FIELDS

        protected bool _shouldRender;

        protected string _mask_left = string.Empty;

        protected ElementReference _inputElement;
        protected string _text;

        #endregion

        #region PROPERTIES

        #region IGizInput

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

        [Parameter]
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public Icons? RightSVGIcon { get; set; }

        [Parameter]
        public InputSizes Size { get; set; } = InputSizes.Medium;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsTransparent { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public ValidationErrorStyles ValidationErrorStyle { get; set; } = ValidationErrorStyles.Label;

        public virtual bool IsValid => _isValid;

        public virtual string ValidationMessage => _validationMessage;

        #endregion

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public CultureInfo Culture { get; set; }
        
        #endregion

        #region EVENTS

        protected Task OnPasteHandler(ClipboardEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected async Task OnMouseDownHandler(MouseEventArgs args)
        {
            await _inputElement.FocusAsync();
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion
    }
}
