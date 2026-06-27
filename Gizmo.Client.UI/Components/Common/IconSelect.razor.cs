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
    public partial class IconSelect<TValue> : GizInputBase<TValue>, IGizInput where TValue : IconSelectItem
    {
        const int DEFAULT_DELAY = 500;

        #region CONSTRUCTOR
        public IconSelect()
        {
            _deferredAction = new DeferredAction(Search);
            _delayTimeSpan = new TimeSpan(0, 0, 0, 0, _delay);
        }
        #endregion

        #region FIELDS

        private string _text = string.Empty;
        private List<TValue> _filteredItems = new List<TValue>();

        private TValue _selectedItem;
        private List _popupContent;
        private ElementReference _inputElement;
        private bool _isOpen;
        private double _popupX;
        private double _popupY;
        private double _popupWidth;

        private bool _hasParsingErrors;
        private string _parsingErrors;
        private ValidationMessageStore _validationMessageStore;

        private bool _clickHandled = false;

        private DeferredAction _deferredAction;
        private int _delay = DEFAULT_DELAY;
        private TimeSpan _delayTimeSpan;

        private bool _customInput;

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
        public IEnumerable<TValue> ItemSource { get; set; }

        [Parameter]
        public TValue SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
            }
        }

        [Parameter]
        public EventCallback<TValue> SelectedItemChanged { get; set; }

        [Parameter]
        public string MaximumHeight { get; set; }

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

        [Parameter]
        public string PopupClass { get; set; }

        [Parameter]
        public int MinimumCharacters { get; set; } = 1;

        /// <summary>
        /// Gets or sets if virtualization is enabled.
        /// </summary>
        [Parameter]
        public bool IsVirtualized { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public bool CanClearValue { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickClearValueButton { get; set; }

        [Parameter]
        public FieldIdentifier FieldIdentifier
        {
            get
            {
                return _fieldIdentifier;
            }
            set
            {
                _fieldIdentifier = value;
            }
        }

        #endregion

        #region METHODS

        protected void SetSelectedItem(TValue value)
        {
            var selectionChanged = _selectedItem != value;
            SelectedItem = value;
            if (selectionChanged)
                _ = SelectedItemChanged.InvokeAsync(_selectedItem);

            if (SelectedItem != null)
                _text = SelectedItem.Text;
            else
                _text = string.Empty;

            _customInput = false;

            _hasParsingErrors = false;
            _parsingErrors = string.Empty;

            _isOpen = false;

            //Validate();
            NotifyFieldChanged();
        }

        #endregion

        #region EVENTS

        private void IsOpenChangedHandler(bool value)
        {
            if (_isOpen && !value)
            {
                TrySetSelectedItem();
            }

            _isOpen = value;
        }

        protected async Task OnClickInput()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            if (IsDisabled || IsLoading)
                return;

            if (!_isOpen)
                await Open();
            else
                _isOpen = false;
        }

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            if (IsDisabled)
                return;

            if (args.Key == null)
                return;

            if (args.Key == "Escape")
            {
                _isOpen = false;

                TrySetSelectedItem();

                return;
            }

            if (args.Key == "Tab")
            {
                _isOpen = false;

                TrySetSelectedItem();

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

                    if (activeItemIndex == -1) //If no item was selected.
                    {
                        activeItemIndex = 0; //Select the first item.
                    }
                    else
                    {
                        if (activeItemIndex >= 0 && activeItemIndex < _filteredItems.Count)
                        {
                            SetSelectedItem(_filteredItems[activeItemIndex]);

                            //Close the popup.
                            _isOpen = false;
                        }

                        return;
                    }

                    break;

                case "ArrowDown":

                    if (activeItemIndex == -1 || activeItemIndex == listSize - 1) //If no item was selected or the last item was selected.
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

                    if (activeItemIndex == -1 || activeItemIndex == 0) //If no item was selected or the first item was selected.
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

                    _customInput = true;

                    return;
            }

            //Update the selected item in the list.
            await _popupContent.SetActiveItemIndex(activeItemIndex);
        }

        public Task OnInputHandler(ChangeEventArgs args)
        {
            _text = (string)args.Value;

            if (MinimumCharacters > 0 && _text.Length >= MinimumCharacters)
            {
                _deferredAction.Defer(_delayTimeSpan);
            }

            //StateHasChanged();
            return Task.CompletedTask;
        }

        public Task OnClickClearValueButtonHandler(MouseEventArgs args)
        {
            _clickHandled = true;
            return OnClickClearValueButton.InvokeAsync(args);
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnParametersSet()
        {
            if (EditContext != _lastEditContext && EditContext != null)
            {
                _validationMessageStore = new ValidationMessageStore(EditContext);
            }

            if (ItemSource != null)
                _filteredItems = ItemSource.ToList();

            if (SelectedItem != null)
                _text = SelectedItem.Text;
            else
                _text = string.Empty;

            base.OnParametersSet();
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

        private void TrySetSelectedItem()
        {
            if (_customInput)
            {
                var selectedItem = ItemSource.Where(a => string.Compare(a.Text, _text, false) == 0).FirstOrDefault();
                if (selectedItem != null)
                {
                    SetSelectedItem(selectedItem);
                }
                else
                {
                    var selectionChanged = _selectedItem != null;
                    SelectedItem = null;
                    if (selectionChanged)
                        _ = SelectedItemChanged.InvokeAsync(_selectedItem);

                    _hasParsingErrors = true;
                    _parsingErrors = "The field is invalid."; //TODO: A TRANSLATE

                    //Validate();
                    NotifyFieldChanged();
                }
            }
        }

        private async Task Open()
        {
            if (OpenDirection == PopupOpenDirections.Cursor)
            {
                var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
                var popupContentSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _popupContent.Ref);

                var inputSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _inputElement);

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

        private Task Search()
        {
            if (string.IsNullOrEmpty(_text))
            {
                if (ItemSource != null)
                    _filteredItems = ItemSource.ToList();
            }
            else
            {
                if (ItemSource != null)
                    _filteredItems = ItemSource.Where(a => a.Text.Contains(_text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            return InvokeAsync(StateHasChanged);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-select")
                 .If("giz-select--full-width", () => IsFullWidth)
                 .If("giz-select--valid", () => IsValid)
                 .If("giz-select--invalid", () => !IsValid)
                 .If("disabled", () => IsDisabled || IsLoading)
                 .If("giz-icon-select--virtualized", () => IsVirtualized)
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
