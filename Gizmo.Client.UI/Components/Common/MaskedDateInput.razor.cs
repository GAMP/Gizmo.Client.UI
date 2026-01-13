using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class MaskedDateInput<TValue> : MaskedInputBase<TValue>
    {
        #region CONSTRUCTOR
        public MaskedDateInput()
        {
            //Set default culture and format;
            _culture = CultureInfo.CurrentCulture;
            _format = _culture.DateTimeFormat.ShortDatePattern;
            _converter = new DateConverter<TValue>();
            _converter.Culture = _culture;
            _converter.Format = _format;

            _tempConverter = new DateConverter<string>();
            _tempConverter.Culture = _culture;
            _tempConverter.Format = _format;

            _separator = _culture.DateTimeFormat.DateSeparator[0];

            var expandedFormat = _format;

            if (!expandedFormat.Contains("dd"))
            {
                expandedFormat = expandedFormat.Replace("d", "dd");
            }

            if (!expandedFormat.Contains("MM"))
            {
                expandedFormat = expandedFormat.Replace("M", "MM");
            }

            _mask = expandedFormat;
            _chars = _mask.Count();
            _mask_left = _mask;
        }
        #endregion

        #region FIELDS

        private TValue _previousValue;

        private CultureInfo _culture;
        private string _format;
        private DateConverter<TValue> _converter;
        private DateConverter<string> _tempConverter;
        private char _separator;

        private string _previousMask = string.Empty;

        private string _mask;
        private int _chars = 0;

        private bool _hasParsingErrors;
        private string _parsingErrors;
        private ValidationMessageStore _validationMessageStore;

        #endregion

        #region PROPERTIES

        public override bool IsValid => !_hasParsingErrors && _isValid && !_converter.HasGetError;

        public override string ValidationMessage => _hasParsingErrors ? _parsingErrors : _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

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

            if (args.Key == _separator.ToString())
            {
                var lastSeparator = _text.LastIndexOf(_separator) + 1;
                var nextSeparator = _mask.IndexOf(_separator, lastSeparator);

                if (nextSeparator != -1)
                {
                    if (_text.Length != nextSeparator)
                    {
                        var length = nextSeparator - lastSeparator;
                        var blockValue = _text.Substring(lastSeparator);
                        blockValue = blockValue.PadLeft(length, '0');

                        var previousBlocks = string.Empty;
                        if (lastSeparator > 0)
                        {
                            previousBlocks = _text.Substring(0, lastSeparator - 1);
                        }

                        if (previousBlocks.Length > 0)
                        {
                            blockValue = _separator + blockValue;
                        }

                        _text = previousBlocks + blockValue;

                        if (_text.Length == _chars)
                        {
                            await ValidateInputAndSetValue();
                        }
                        else
                        {
                            if (_mask[_text.Length] == _separator)
                            {
                                _text += _separator;
                            }
                        }
                    }
                }
            }
            else
            {
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

                        if (!string.IsNullOrEmpty(_text) && _text.Length >= _chars)
                            return;

                        _text += args.Key;

                        if (_text.Length == _chars)
                        {
                            await ValidateInputAndSetValue();
                        }
                        else
                        {
                            if (_mask[_text.Length] == _separator)
                            {
                                _text += _separator;
                            }
                        }

                        break;

                    case "Backspace":

                        if (string.IsNullOrEmpty(_text) || _text.Length == 0)
                            return;

                        _text = _text.Substring(0, _text.Length - 1);

                        if (_mask[_text.Length] == _separator)
                        {
                            _text = _text.Substring(0, _text.Length - 1);
                        }

                        if (_text.Length == 0)
                        {
                            if (IsNullable())
                            {
                                await ValidateInputAndSetValue();
                            }
                        }

                        break;
                }
            }

            if (!string.IsNullOrEmpty(_text) && _text.Length > 0)
            {
                _mask_left = _mask.Substring(_text.Length);
            }
            else
            {
                _mask_left = _mask;
            }
        }

        protected async Task OnFocusOutHandler(FocusEventArgs args)
        {
            DateTime? temp = _tempConverter.SetValue(_text);
            if (!_tempConverter.HasSetError)
            {
                //If there are no parsing errors update value and text.

                _hasParsingErrors = false;
                _parsingErrors = String.Empty;

                var tmpValue = _converter.GetValue(temp);

                if (!EqualityComparer<TValue>.Default.Equals(tmpValue, Value))
                {
                    //Update value.
                    Value = tmpValue;

                    //Update display text.
                    await UpdateText();

                    //Refresh parsing validation message.
                    Validate();

                    //Raise events.
                    await ValueChanged.InvokeAsync(Value);
                    NotifyFieldChanged();
                }
            }
            else
            {
                //If there are parsing errors update text from previous value.

                _hasParsingErrors = false;
                _parsingErrors = String.Empty;

                //Update display text.
                await UpdateText(true);

                //Refresh parsing validation message.
                Validate();
            }
        }

        #endregion

        #region METHODS

        private bool IsNullable()
        {
            if (Nullable.GetUnderlyingType(typeof(TValue)) != null)
                return true;

            return false;
        }

        private async Task ValidateInputAndSetValue()
        {
            DateTime? temp = _tempConverter.SetValue(_text);
            if (!_tempConverter.HasSetError)
            {
                _hasParsingErrors = false;
                _parsingErrors = String.Empty;

                //Update value.
                Value = _converter.GetValue(temp);

                //Update display text.
                await UpdateText();

                //Refresh parsing validation message.
                Validate();

                //Raise events.
                await ValueChanged.InvokeAsync(Value);

                NotifyFieldChanged();
            }
            else
            {
                _hasParsingErrors = true;
                _parsingErrors = "The field should be a date."; //TODO: A TRANSLATE

                //Refresh parsing validation message.
                Validate();
            }
        }

        private Task UpdateText(bool forceUpdate = false)
        {
            try
            {
                if (!EqualityComparer<TValue>.Default.Equals(_previousValue, Value) || forceUpdate)
                {
                    _shouldRender = true;
                    _previousValue = Value;

                    _text = string.Empty;

                    var currentValue = _converter.SetValue(Value);

                    if (currentValue.HasValue)
                    {
                        _text = currentValue.Value.ToString(_mask);
                        _mask_left = string.Empty;
                    }
                    else
                    {
                        _text = string.Empty;
                        _mask_left = _mask;
                    }
                }
            }
            catch
            {

            }

            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDES

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
            if (_validationMessageStore != null && !_fieldIdentifier.Equals(default(FieldIdentifier)))
            {
                _validationMessageStore.Clear();

                if (_hasParsingErrors)
                {
                    _validationMessageStore.Add(_fieldIdentifier, _parsingErrors);
                }
            }
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-masked-date-input")
                 .Add("giz-text-input")
                 .If("giz-text-input--full-width", () => IsFullWidth)
                 .Add(Class)
                 .AsString();

        #endregion

    }
}
