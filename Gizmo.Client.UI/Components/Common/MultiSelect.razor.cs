using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class MultiSelect<TValue> : GizInputBase<List<TValue>>, ISelect<TValue>, IGizInput
    {
        #region CONSTRUCTOR
        public MultiSelect()
        {
        }
        #endregion

        #region FIELDS

        private Dictionary<TValue, ISelectItem<TValue>> _items = new Dictionary<TValue, ISelectItem<TValue>>();
        private List<ISelectItem<TValue>> _selectedItems = new List<ISelectItem<TValue>>();
        private ElementReference _popupContent;
        private ElementReference _inputElement;
        private bool _isOpen;
        private double _popupX;
        private double _popupY;
        private double _popupWidth;

        private bool _clickHandled = false;

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

        public bool IsValid => _isValid;

        public string ValidationMessage => _validationMessage;

        #endregion

        [Parameter]
        public Icons? HandleSVGIcon { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public List<TValue> Value { get; set; }

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

        internal async Task SetSelectedValue(List<TValue> value)
        {
            Value = value;
            await ValueChanged.InvokeAsync(Value);
            NotifyFieldChanged();
        }

        public string GetSelectedItemsText()
        {
            if (Value == null || Value.Count == 0)
            {
                return Placeholder;
            }
            else
            {
                List<string> result = new List<string>();

                foreach (var item in _items)
                {
                    if (Value.Contains(item.Key))
                    {
                        result.Add(item.Value.Text);
                    }
                }

                return string.Join(", ", result);
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

        public async Task OnClickButtonClearValueHandler(MouseEventArgs args)
        {
            _clickHandled = true;

            Clear();

            await SetSelectedValue(_selectedItems.Select(a => a.Value).ToList());
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Clear();

            if (Value != null)
            {
                foreach (var item in Value)
                {
                    if (_items.ContainsKey(item))
                    {
                        await SelectItem(_items[item]);
                    }
                }
            }

            if (firstRender)
            {
                StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<List<TValue>>(nameof(Value), out var newValue))
            {
                var valueChanged = !EqualityComparer<List<TValue>>.Default.Equals(Value, newValue);
                if (valueChanged)
                {
                    Clear();

                    if (newValue != null)
                    {
                        foreach (var item in newValue)
                        {
                            if (_items.ContainsKey(item))
                            {
                                await SelectItem(_items[item]);
                            }
                        }
                    }
                }
            }

            await base.SetParametersAsync(parameters);
        }

        #endregion

        #region METHODS

        private void Clear()
        {
            foreach (var item in _selectedItems)
            {
                if (item is MultiSelectItem<TValue> multiSelectItem)
                    multiSelectItem.SetSelected(false);
            }

            _selectedItems.Clear();
        }

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

        public Task SelectItem(ISelectItem<TValue> selectItem)
        {
            if (selectItem != null)
            {
                if (!_selectedItems.Contains(selectItem))
                {
                    _selectedItems.Add(selectItem);

                    if (selectItem is MultiSelectItem<TValue> multiSelectItem)
                        multiSelectItem.SetSelected(true);
                }
            }

            return Task.CompletedTask;
        }

        public async Task SetSelectedItem(ISelectItem<TValue> selectItem)
        {
            bool selected = false;

            if (!_selectedItems.Contains(selectItem))
            {
                _selectedItems.Add(selectItem);
                selected = true;
            }
            else
            {
                _selectedItems.Remove(selectItem);
            }

            if (selectItem is MultiSelectItem<TValue> multiSelectItem)
                multiSelectItem.SetSelected(selected);

            await SetSelectedValue(_selectedItems.Select(a => a.Value).ToList());

            StateHasChanged();
        }

        private async Task Open()
        {
            if (OpenDirection == PopupOpenDirections.Cursor)
            {
                var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
                var popupContentSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _popupContent);

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

            _isOpen = true;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-multi-select")
                 .If("giz-multi-select--full-width", () => IsFullWidth)
                 .If("open", () => _isOpen)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-multi-select__dropdown")
                 .If("giz-multi-select__dropdown--cursor", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-multi-select__dropdown--full-width", () => OpenDirection != PopupOpenDirections.Cursor)
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
