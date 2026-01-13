using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Components
{
    public partial class TextInput<TValue> : GizInputBase<TValue>, IGizInput
    {
        #region CONSTRUCTOR
        public TextInput()
        {
        }
        #endregion

        #region FIELDS

        private StringConverter<TValue> _converter = new StringConverter<TValue>();
        private string _text;

        private TValue _previousValue;

        protected ElementReference _inputElement;

        private bool _hasValidateFunction;
        private bool _altPressed;

        private bool _hasFocus;

        #endregion

        [Inject()]
        private ILogger<IGizInput> Logger
        {
            get;
            set;
        }

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
        public IconSizes LeftIconSize { get; set; } = IconSizes.Medium;

        [Parameter]
        public string RightIcon { get; set; }

        [Parameter]
        public IconSizes RightIconSize { get; set; } = IconSizes.Medium;

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

        public bool IsValid => _isValid && !_converter.HasGetError;

        public string ValidationMessage => _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        [Parameter]
        public RenderFragment LeftContent { get; set; }

        [Parameter]
        public RenderFragment RightContent { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public string Type { get; set; } = "text";

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public int Min { get; set; }

        [Parameter]
        public int Max { get; set; }

        [Parameter]
        public int MaxLength { get; set; }

        [Parameter]
        public bool UpdateOnInput { get; set; }

        [Parameter]
        public bool IsMultiLine { get; set; }

        [Parameter]
        public bool InvariantDecimalPoint { get; set; }

        [Parameter]
        public CultureInfo Culture { get; set; }

        [Parameter]
        public Func<char, Task<bool>> ValidateFunction { get; set; }

        [Parameter]
        public string InputClass { get; set; }

        [Parameter]
        public bool CanClearValue { get; set; }

        [Parameter]
        public bool EnableAutoComplete { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnInputKeyDown { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnInputKeyUp { get; set; }

        #endregion

        #region EVENTS

        public Task OnClickButtonClearValueHandler(MouseEventArgs args)
        {
            return SetValueAsync(default(TValue));
        }

        protected Task OnInputHandler(ChangeEventArgs args)
        {
            if (_hasValidateFunction)
                return Task.CompletedTask;

            if (UpdateOnInput)
            {
                var newText = args?.Value as string;

                TValue newValue = _converter.GetValue(newText);

                if (!EqualityComparer<TValue>.Default.Equals(Value, newValue))
                {
                    return SetValueAsync(newValue);
                }
            }

            return Task.CompletedTask;
        }

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            await OnInputKeyDown.InvokeAsync(args);

            if (!_hasValidateFunction)
                return;

            if (IsDisabled)
                return;

            try
            {
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

                var previousValue = _converter.SetValue(Value);

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
                            TValue newValue = _converter.GetValue(part1 + part2);
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

                            TValue newValue = _converter.GetValue(part1 + part2);
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
                            TValue newValue = _converter.GetValue(part1 + part2);
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

                            TValue newValue = _converter.GetValue(part1 + part2);
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
                                    TValue newValue = _converter.GetValue(part1 + args.Key + part2);
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
                                    TValue newValue = _converter.GetValue(part1 + args.Key + part2);
                                    await SetValueAsync(newValue);
                                }

                                caretIndex += 1;
                                await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);
                            }
                        }

                        break;
                }
            }
            catch (NullReferenceException nullRef)
            {
                Logger.LogCritical(nullRef, "Null reference in input handler.");
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error in input handler.");
            }
        }

        protected Task OnInputKeyUpHandler(KeyboardEventArgs args)
        {
            return OnInputKeyUp.InvokeAsync(args);
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

                TValue newValue = _converter.GetValue(newText);

                if (!EqualityComparer<TValue>.Default.Equals(Value, newValue))
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

        private bool IsNullable()
        {
            if (typeof(TValue) == typeof(string))
                return true;

            if (Nullable.GetUnderlyingType(typeof(TValue)) != null)
                return true;

            return false;
        }

        private bool IsNull()
        {
            if (Value == null)
                return true;

            if (typeof(TValue) == typeof(string))
                return string.IsNullOrEmpty((string)(object)Value);

            return false;
        }

        protected async Task SetValueAsync(TValue value)
        {
            Value = value;
            UpdateText();
            await ValueChanged.InvokeAsync(Value);
            NotifyFieldChanged();
        }

        private void UpdateText()
        {
            var valueChanged = !EqualityComparer<TValue>.Default.Equals(_previousValue, Value);
            if (valueChanged)
            {
                _previousValue = Value;

                _text = _converter.SetValue(Value);
            }
        }

        protected void OnDropHandler()
        {
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            _converter.InvariantDecimalPoint = InvariantDecimalPoint;

            if (Culture != null)
            {
                _converter.Culture = Culture;
            }

            base.OnInitialized();
        }

        protected override Task OnFirstAfterRenderAsync()
        {
            var attributes = new Dictionary<string, object>();

            if (Min > 0)
                attributes["min"] = Min;

            if (Max > 0)
                attributes["max"] = Max;

            if (MaxLength > 0)
                attributes["maxlength"] = MaxLength;

            if (!EnableAutoComplete)
            {
                attributes["aria-autocomplete"] = "none";
                attributes["autocomplete"] = "off";
                attributes["autofill"] = "off";
            }

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
                 .Add("giz-text-input")
                 .If("giz-text-input--full-width", () => IsFullWidth)
                 .If("giz-text-input--internal-label", () => !string.IsNullOrWhiteSpace(InternalLabel))
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
