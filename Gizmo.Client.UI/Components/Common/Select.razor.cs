using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class Select<TValue> : GizInputBase<TValue>, ISelect<TValue>, IGizInput
    {
        #region CONSTRUCTOR
        public Select()
        {
        }
        #endregion

        #region FIELDS

        private Dictionary<TValue, ISelectItem<TValue>> _items = new Dictionary<TValue, ISelectItem<TValue>>();
        private ISelectItem<TValue> _selectedItem;
        private List _popupContent;
        private ElementReference _inputWrapperElement;
        private ElementReference _inputElement;
        private bool _isOpen;
        private double _popupX;
        private double _popupY;
        private double _popupWidth;

        private bool _hasParsingErrors;
        private string _parsingErrors;
        private ValidationMessageStore _validationMessageStore;

        private bool _clickHandled = false;

        private TValue _previousValue;

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

        public bool IsValid => !_hasParsingErrors && _isValid;

        public string ValidationMessage => _hasParsingErrors ? _parsingErrors : _validationMessage;

        #endregion

        [Parameter]
        public Icons? HandleSVGIcon { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public string MaximumHeight { get; set; }

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

        [Parameter]
        public bool OffsetY { get; set; }

        [Parameter]
        public bool CanClearValue { get; set; }

        [Parameter]
        public string PopupClass { get; set; }

        #endregion

        #region METHODS

        internal async Task SetSelectedValue(TValue value)
        {
            if (!EqualityComparer<TValue>.Default.Equals(Value, value))
            {
                Value = value;
                await ValueChanged.InvokeAsync(Value);
                NotifyFieldChanged();
            }
        }

        #endregion

        #region EVENTS

        protected async Task OnClickInput()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            if (!IsDisabled)
            {
                if (!_isOpen)
                    await Open();
                else
                    _isOpen = false;
            }
        }

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

            if (!_isOpen)
                await Open();

            //If list has items.
            //Get the index of the selected item.

            int activeItemIndex = _popupContent.GetActiveItemIndex();
            int listSize = _popupContent.GetListSize();

            switch (args.Key)
            {
                case "Enter":

                    if (activeItemIndex == -1) //If not item was selected.
                    {
                        activeItemIndex = 0; //Select the first item.
                    }
                    else
                    {
                        //Set the value of the Select based on the selected item.
                        var selectItem = _items.Where(a => a.Value.ListItem == _popupContent.ActiveItem).Select(a => a.Value).FirstOrDefault();
                        await SetSelectedItem(selectItem);
                        await _popupContent.SetActiveItemIndex(activeItemIndex);

                        //Close the popup.
                        _isOpen = false;

                        return;
                    }

                    break;

                case "ArrowDown":

                    if (activeItemIndex == -1 || activeItemIndex == listSize - 1) //If not item was selected or the last item was selected.
                    {
                        //Select the first item.
                        activeItemIndex = 0;
                    }
                    else
                    {
                        //Select the next item.
                        activeItemIndex += 1;
                    }

                    break;
                case "ArrowUp":

                    if (activeItemIndex == -1 || activeItemIndex == 0) //If not item was selected or the first item was selected.
                    {
                        //Select the last item.
                        activeItemIndex = listSize - 1;
                    }
                    else
                    {
                        //Select the previous item.
                        activeItemIndex -= 1;
                    }

                    break;

                default:
                    return;
            }

            //Update the selected item in the list.
            await _popupContent.SetActiveItemIndex(activeItemIndex);
        }

        public Task OnClickButtonClearValueHandler(MouseEventArgs args)
        {
            _clickHandled = true;
            return SetSelectedValue(default(TValue));
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //The _items list will be initialized after the first render.
                if (Value != null)
                {
                    if (_items.ContainsKey(Value))
                    {
                        await SelectItem(false, string.Empty, _items[Value]);
                    }
                    else
                    {
                        await SelectItem(true, "The field is required!", null); //TODO: A TRANSLATE
                    }
                }
                else
                {
                    await SelectItem(false, string.Empty, null);
                }
            }

            await base.OnAfterRenderAsync(firstRender);
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
            if (!EqualityComparer<TValue>.Default.Equals(_previousValue, Value))
            {
                _previousValue = Value;
                if (Value != null)
                {
                    if (_items.ContainsKey(Value))
                    {
                        await SelectItem(false, string.Empty, _items[Value]);
                    }
                    else
                    {
                        await SelectItem(true, "The field is required!", null); //TODO: A TRANSLATE
                    }
                }
                else
                {
                    await SelectItem(false, string.Empty, null);
                }
            }

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

        #region METHODS

        public void Register(ISelectItem<TValue> selectItem, TValue value)
        {
            if (value == null)
                return;

            _items[value] = selectItem;
        }

        public void UpdateItem(ISelectItem<TValue> selectItem, TValue value)
        {
            if (value == null)
                return;

            var actualItem = _items.Where(a => a.Value == selectItem).FirstOrDefault();
            if (!actualItem.Equals(default(KeyValuePair<TValue, ISelectItem<TValue>>)) && actualItem.Key != null)
            {
                _items.Remove(actualItem.Key);
            }

            _items[value] = selectItem;
        }

        public void Unregister(ISelectItem<TValue> selectItem, TValue value)
        {
            if (value == null)
                return;

            var actualItem = _items.Where(a => a.Value == selectItem).FirstOrDefault();
            if (!actualItem.Equals(default(KeyValuePair<TValue, ISelectItem<TValue>>)))
            {
                _items.Remove(actualItem.Key);
            }
        }

        public async Task SelectItem(bool hasParsingErrors, string parsingErrors, ISelectItem<TValue> selectItem)
        {
            bool refresh = false;

            if (_hasParsingErrors != hasParsingErrors)
            {
                _hasParsingErrors = hasParsingErrors;
                _parsingErrors = parsingErrors;

                refresh = true;
            }

            if (_selectedItem != selectItem)
            {
                _selectedItem = selectItem;

                await _popupContent.SetSelectedItem(_selectedItem?.ListItem);

                refresh = true;
            }

            if (refresh)
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        public async Task SetSelectedItem(ISelectItem<TValue> selectItem)
        {
            bool requiresRefresh = _isOpen;

            _isOpen = false;

            if (_selectedItem == selectItem)
            {
                if (requiresRefresh)
                    await InvokeAsync(StateHasChanged);

                return;
            }

            _selectedItem = selectItem;

            _hasParsingErrors = false;
            _parsingErrors = String.Empty;

            await InvokeAsync(StateHasChanged);

            if (selectItem != null)
                await SetSelectedValue(selectItem.Value);
            else
                await SetSelectedValue(default(TValue));
        }

        private async Task Open()
        {
            if (OpenDirection == PopupOpenDirections.Cursor)
            {
                var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
                var popupContentSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _popupContent.Ref);

                var inputSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _inputWrapperElement);

                _popupX = inputSize.Left;
                _popupWidth = inputSize.Width;

                if (inputSize.Bottom + popupContentSize.Height > windowSize.Height)
                {
                    _popupY = windowSize.Height - popupContentSize.Height;
                }
                else
                {
                    _popupY = inputSize.Bottom;
                }
            }

            int activeItemIndex = _popupContent.GetSelectedItemIndex();
            await _popupContent.SetActiveItemIndex(activeItemIndex);

            _isOpen = true;
        }

        private bool IsNullable()
        {
            if (Nullable.GetUnderlyingType(typeof(TValue)) != null)
                return true;

            return false;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-select")
                 .If("giz-select--full-width", () => IsFullWidth)
                 .If("giz-select--valid", () => IsValid)
                 .If("giz-select--invalid", () => !IsValid)
                 .If("open", () => _isOpen)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-select__dropdown")
                 .If("giz-select__dropdown--cursor", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-select__dropdown--full-width", () => OpenDirection != PopupOpenDirections.Cursor)
                 .If(PopupClass, () => !string.IsNullOrEmpty(PopupClass))
                 .AsString();

        protected string PopupStyleValue => new StyleMapper()
                 .If($"top: {_popupY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"left: {_popupX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"width: {_popupWidth.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        #endregion

    }
}
