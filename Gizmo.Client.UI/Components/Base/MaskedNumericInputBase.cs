using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public class MaskedNumericInputBase<TValue> : MaskedInputBase<TValue>, IGizInput
    {
        #region FIELDS

        private int _chars = 0;
        private string _previousMask = string.Empty;
        private TValue _previousValue;
        private bool _previousAllowMoreDigits;

        private StringConverter<TValue> _converter = new StringConverter<TValue>();

        private bool _hasParsingErrors;
        private string _parsingErrors;
        private ValidationMessageStore _validationMessageStore;

        private bool _altPressed;

        #endregion

        #region PROPERTIES

        #region IGizInput

        public override bool IsValid => !_hasParsingErrors && !_converter.HasGetError && _isValid;

        public override string ValidationMessage => _hasParsingErrors ? _parsingErrors : _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        [Parameter]
        public string Mask { get; set; }

        [Parameter]
        public bool AllowMoreDigits { get; set; }

        [Parameter]
        public int ExtraDigits { get; set; } = 7;

        #endregion

        #region EVENTS

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
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

            //var inputSelectionRange = await JsInvokeAsync<InputSelectionRange>("getInputSelectionRange");

            var currentValue = _converter.SetValue(Value);

            switch (args.Key)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":

                    if (!string.IsNullOrEmpty(currentValue) && ((currentValue.Length >= _chars && !AllowMoreDigits) || currentValue.Length >= _chars + ExtraDigits && AllowMoreDigits))
                        return;

                    //Append previous value with pressed key.
                    currentValue += args.Key;

                    //Update value, display text and raise events.
                    await SetValueAsync(_converter.GetValue(currentValue));

                    break;

                case "Backspace":

                    if (string.IsNullOrEmpty(currentValue) || currentValue.Length == 0)
                        return;

                    //Remove the last character from value.
                    currentValue = currentValue.Substring(0, currentValue.Length - 1);

                    //Update value, display text and raise events.
                    await SetValueAsync(_converter.GetValue(currentValue));

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

        #endregion

        #region METHODS

        protected async Task SetValueAsync(TValue value)
        {
            //Update value.
            Value = value;

            //Update display text.
            await UpdateText();

            //Raise events.
            await ValueChanged.InvokeAsync(Value);
            NotifyFieldChanged();
        }

        private async Task UpdateText()
        {
            try
            {
                var maskChanged = _previousMask != Mask;
                if (maskChanged)
                {
                    _shouldRender = true;
                    _previousMask = Mask;

                    //If Mask changed recalculate the number of digits.
                    _chars = Mask.Where(a => a == '#').Count();
                    _mask_left = Mask;
                }

                var allowMoreDigitsChanged = _previousAllowMoreDigits != AllowMoreDigits;
                if (allowMoreDigitsChanged)
                {
                    _shouldRender = true;
                    _previousAllowMoreDigits = AllowMoreDigits;

                    if (!AllowMoreDigits)
                    {
                        var currentValue = _converter.SetValue(Value);

                        //If AllowMoreDigits changed to false and the number of digits in current value is greater than the allowed number of digits,
                        //we have to trim the value.
                        if (!string.IsNullOrEmpty(currentValue) && currentValue.Length > _chars)
                        {
                            //Update the value.
                            Value = _converter.GetValue(currentValue.Substring(0, _chars));

                            //Raise events.
                            await ValueChanged.InvokeAsync(Value);
                            NotifyFieldChanged();
                        }
                    }
                }

                var valueChanged = !EqualityComparer<TValue>.Default.Equals(_previousValue, Value);
                if (valueChanged)
                {
                    _shouldRender = true;
                    _previousValue = Value;
                }

                if (maskChanged || allowMoreDigitsChanged || valueChanged)
                {
                    _text = string.Empty;

                    var currentValue = _converter.SetValue(Value);

                    if (!string.IsNullOrEmpty(currentValue) && currentValue.Length > 0)
                    {
                        int additionalCharacters = 0; //Number of characters other than the '#' digit placeholder.

                        for (int i = 0; i < Mask.Length; i++)
                        {
                            if (i > currentValue.Length - 1 + additionalCharacters)
                                break;

                            if (Mask[i] == '#')
                            {
                                _text += currentValue[i - additionalCharacters];
                            }
                            else
                            {
                                _text += Mask[i];
                                additionalCharacters += 1;
                            }
                        }

                        if (AllowMoreDigits && currentValue.Length > Mask.Length - additionalCharacters)
                        {
                            _text += currentValue.Substring(Mask.Length - additionalCharacters);
                            _mask_left = string.Empty;
                        }
                        else
                        {
                            _mask_left = Mask.Substring(currentValue.Length + additionalCharacters);
                        }
                    }
                    else
                    {
                        _text = string.Empty;
                        _mask_left = Mask;
                    }

                    //Validate new value.
                    if (!string.IsNullOrEmpty(currentValue) && !currentValue.All(char.IsDigit))
                    {
                        _hasParsingErrors = true;
                        _parsingErrors = "The field should be a number."; //TODO: A TRANSLATE
                    }
                    else
                    {
                        _hasParsingErrors = false;
                        _parsingErrors = string.Empty;
                    }
                }
            }
            catch
            {

            }
        }

        protected void OnDropHandler()
        {
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            if (Culture != null)
            {
                _converter.Culture = Culture;
            }

            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (EditContext != _lastEditContext && EditContext != null)
            {
                _validationMessageStore = new ValidationMessageStore(EditContext);
            }

            base.OnParametersSet();
        }

        protected override async Task OnParametersSetAsync()
        {
            await UpdateText();

            await base.OnParametersSetAsync();
        }

        public override void Validate()
        {
            if (_validationMessageStore != null)
            {
                _validationMessageStore.Clear();

                if (_hasParsingErrors)
                {
                    _validationMessageStore.Add(_fieldIdentifier, _parsingErrors);
                }
            }
        }

        #endregion
    }
}
