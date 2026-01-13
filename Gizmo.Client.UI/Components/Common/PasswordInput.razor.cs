using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class PasswordInput : GizInputBase<string>, IGizInput
    {
        #region CONSTRUCTOR
        public PasswordInput()
        {
        }
        #endregion

        #region FIELDS

        private string _text;

        private string _previousValue;

        protected ElementReference _inputElement;

        private bool _hasValidateFunction;
        private bool _altPressed;

        private bool _hasFocus;

        #endregion

        #region PROPERTIES

        protected bool DisableDrop => (UpdateOnInput);

        #region IGizInput

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public string InternalLabel { get; set; }

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
        public bool IsPasswordVisible { get; set; }

        [Parameter]
        public EventCallback<bool> IsPasswordVisibleChanged { get; set; }

        [Parameter]
        public ValidationErrorStyles ValidationErrorStyle { get; set; } = ValidationErrorStyles.Label;

        public bool IsValid => _isValid;

        public string ValidationMessage => _validationMessage;

        #endregion

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public int MaxLength { get; set; }

        [Parameter]
        public bool UpdateOnInput { get; set; }

        [Parameter]
        public bool ShowRevealButton { get; set; }

        [Parameter]
        public bool CanFocusRevealButton { get; set; }

        [Parameter]
        public Func<char, Task<bool>> ValidateFunction { get; set; }

        #endregion

        #region EVENTS

        protected Task OnInputHandler(ChangeEventArgs args)
        {
            if (_hasValidateFunction)
                return Task.CompletedTask;

            if (UpdateOnInput)
            {
                var newText = args?.Value as string;

                string newValue = newText;

                if (!EqualityComparer<string>.Default.Equals(Value, newValue))
                {
                    return SetValueAsync(newValue);
                }
            }

            return Task.CompletedTask;
        }

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            if (!_hasValidateFunction)
                return;

            if (IsDisabled)
                return;

            if (args.Key == null)
                return;

            if (args.Key == "Tab")
            {
                if (args.ShiftKey)
                {
                    await InvokeVoidAsync("focusPrevious", _inputElement);
                }
                else
                {
                    await InvokeVoidAsync("focusNext", _inputElement);
                }
                return;
            }

            if (_altPressed)
            {
                if (!args.AltKey)
                {
                    _altPressed = false;
                }
            }
            else
            {
                if (args.Key == "Alt")
                {
                    _altPressed = true;
                }
            }

            if (_altPressed)
                return;

            var previousValue = Value;

            var inputSelectionRange = await JsInvokeAsync<InputSelectionRange>("getInputSelectionRange", _inputElement);

            int caretIndex = 0;

            string part1 = string.Empty;
            string part2 = string.Empty;

            switch (args.Key)
            {
                case "Home":
                    caretIndex = 0;
                    await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);

                    break;

                case "End":
                    if (!string.IsNullOrEmpty(previousValue))
                    {
                        caretIndex = previousValue.Length;
                        await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);
                    }

                    break;

                case "ArrowLeft":
                    caretIndex = inputSelectionRange.SelectionStart;

                    if (caretIndex > 0)
                    {
                        caretIndex -= 1;
                        await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);
                    }

                    break;

                case "ArrowRight":
                    caretIndex = inputSelectionRange.SelectionStart;

                    if (caretIndex < previousValue.Length)
                    {
                        caretIndex += 1;
                        await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);
                    }

                    break;

                case "Backspace":

                    caretIndex = inputSelectionRange.SelectionStart;

                    if (inputSelectionRange.SelectionStart != inputSelectionRange.SelectionEnd)
                    {
                        //If there is a selection.
                        if (inputSelectionRange.SelectionStart > 0)
                        {
                            //Get part before selection.
                            part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart);
                        }

                        if (inputSelectionRange.SelectionEnd < previousValue.Length)
                        {
                            //Get part after selection.
                            part2 = previousValue.Substring(inputSelectionRange.SelectionEnd, previousValue.Length - inputSelectionRange.SelectionEnd);
                        }

                        //The selection is removed.
                        string newValue = part1 + part2;
                        await SetValueAsync(newValue);
                    }
                    else
                    {
                        //If there is no selection.
                        if (inputSelectionRange.SelectionStart > 0)
                        {
                            //Get part before caret -1 character.
                            part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart - 1);
                        }

                        if (!string.IsNullOrEmpty(previousValue) && inputSelectionRange.SelectionStart < previousValue.Length)
                        {
                            //Get part after caret.
                            part2 = previousValue.Substring(inputSelectionRange.SelectionStart, previousValue.Length - inputSelectionRange.SelectionStart);
                        }

                        string newValue = part1 + part2;
                        await SetValueAsync(newValue);

                        caretIndex -= 1;
                    }

                    await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);

                    break;

                case "Delete":

                    caretIndex = inputSelectionRange.SelectionStart;

                    if (inputSelectionRange.SelectionStart != inputSelectionRange.SelectionEnd)
                    {
                        //If there is a selection.
                        if (inputSelectionRange.SelectionStart > 0)
                        {
                            //Get part before selection.
                            part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart);
                        }

                        if (inputSelectionRange.SelectionEnd < previousValue.Length)
                        {
                            //Get part after selection.
                            part2 = previousValue.Substring(inputSelectionRange.SelectionEnd, previousValue.Length - inputSelectionRange.SelectionEnd);
                        }

                        //The selection is removed.
                        string newValue = part1 + part2;
                        await SetValueAsync(newValue);
                    }
                    else
                    {
                        //If there is no selection.
                        if (inputSelectionRange.SelectionStart > 0)
                        {
                            //Get part before caret character.
                            part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart);
                        }

                        if (!string.IsNullOrEmpty(previousValue) && inputSelectionRange.SelectionStart + 1 < previousValue.Length)
                        {
                            //Get part after caret + 1.
                            part2 = previousValue.Substring(inputSelectionRange.SelectionStart + 1, previousValue.Length - (inputSelectionRange.SelectionStart + 1));
                        }

                        string newValue = part1 + part2;
                        await SetValueAsync(newValue);

                        //caretIndex -= 1;
                    }

                    await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);

                    break;

                default:

                    if (args.Key.Length == 1 && !char.IsControl(args.Key[0]))
                    {
                        caretIndex = inputSelectionRange.SelectionStart;

                        bool isValidCharacter = true;

                        if (ValidateFunction != null)
                        {
                            isValidCharacter = await ValidateFunction(args.Key[0]);
                        }

                        if (isValidCharacter)
                        {
                            if (inputSelectionRange.SelectionStart != inputSelectionRange.SelectionEnd)
                            {
                                //If there is a selection.
                                if (inputSelectionRange.SelectionStart > 0)
                                {
                                    //Get part before selection.
                                    part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart);
                                }

                                if (inputSelectionRange.SelectionEnd < previousValue.Length)
                                {
                                    //Get part after selection.
                                    part2 = previousValue.Substring(inputSelectionRange.SelectionEnd, previousValue.Length - inputSelectionRange.SelectionEnd);
                                }

                                //Replace selection with new character.
                                string newValue = part1 + args.Key + part2;
                                await SetValueAsync(newValue);
                            }
                            else
                            {
                                //If there is no selection.
                                if (inputSelectionRange.SelectionStart > 0)
                                {
                                    //Get part before caret.
                                    part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart);
                                }

                                if (!string.IsNullOrEmpty(previousValue) && inputSelectionRange.SelectionStart < previousValue.Length)
                                {
                                    //Get part after caret.
                                    part2 = previousValue.Substring(inputSelectionRange.SelectionStart, previousValue.Length - inputSelectionRange.SelectionStart);
                                }

                                //Insert new character between.
                                string newValue = part1 + args.Key + part2;
                                await SetValueAsync(newValue);
                            }

                            caretIndex += 1;
                            await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);
                        }
                    }

                    break;
            }
        }

        protected Task OnInputKeyUpHandler(KeyboardEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected Task OnInputKeyPressHandler(KeyboardEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected Task OnChangeHandler(ChangeEventArgs args)
        {
            if (!UpdateOnInput)
            {
                var newText = args?.Value as string;

                string newValue = newText;

                if (!EqualityComparer<string>.Default.Equals(Value, newValue))
                {
                    return SetValueAsync(newValue);
                }
            }

            return Task.CompletedTask;
        }

        protected async Task OnRootClickHandler(MouseEventArgs args)
        {
            await _inputElement.FocusAsync();

            await OnClick.InvokeAsync(args);
        }

        protected Task OnClickHandler(MouseEventArgs args)
        {
            //return OnClick.InvokeAsync(args);

            return Task.CompletedTask;
        }

        public Task OnClickButtonEyeHandler(MouseEventArgs args)
        {
            IsPasswordVisible = !IsPasswordVisible;
            return IsPasswordVisibleChanged.InvokeAsync(IsPasswordVisible);
        }

        public void OnFocusInHandler()
        {
            _hasFocus = true;
        }

        public void OnFocusOutHandler()
        {
            _hasFocus = false;
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(string value)
        {
            Value = value;
            UpdateText();
            await ValueChanged.InvokeAsync(Value);
            NotifyFieldChanged();
        }

        private void UpdateText()
        {
            var valueChanged = _previousValue != Value;
            if (valueChanged)
            {
                _previousValue = Value;

                _text = Value;
            }
        }

        protected void OnDropHandler()
        {
        }

        #endregion

        #region OVERRIDES

        protected override Task OnFirstAfterRenderAsync()
        {
            var attributes = new Dictionary<string, object>();

            if (MaxLength > 0)
                attributes["maxlength"] = MaxLength;

            Attributes = attributes;

            return base.OnFirstAfterRenderAsync();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            UpdateText();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _hasValidateFunction = ValidateFunction != null;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-password-input")
                 .If("giz-password-input--full-width", () => IsFullWidth)
                 .If("giz-password-input--internal-label", () => !string.IsNullOrWhiteSpace(InternalLabel))
                 .Add(Class)
                 .AsString();

        protected string InternalLabelClassName => new ClassMapper()
             .Add("giz-input-wrapper__internal-label")
             .If("giz-input-wrapper__internal-label--active", () => !string.IsNullOrWhiteSpace(InternalLabel) && (_text?.Length > 0 || _hasFocus))
             .AsString();

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
