using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ComboButton : ButtonBase, ISelect<int>
    {
        #region CONSTRUCTOR
        public ComboButton()
        {
        }
        #endregion

        #region FIELDS

        private Dictionary<int, ISelectItem<int>> _items = new Dictionary<int, ISelectItem<int>>();
        private ISelectItem<int> _selectedItem;
        private List _popupContent;

        private bool _isOpen;
        private double _popupX;
        private double _popupY;
        private double _popupWidth;

        #endregion

        #region PROPERTIES

        #region PUBLIC

        [Parameter]
        public EventCallback OnClickMainButton { get; set; }

        //[Parameter]
        //public EventCallback OnClickDropDownButton { get; set; }

        [Parameter]
        public ButtonVariants Variant { get; set; } = ButtonVariants.Fill;

        [Parameter]
        public bool IsFullWidth { get; set; }

        /// <summary>
        /// Gets or sets label.
        /// </summary>
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public string MaximumHeight { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment DropDownItems { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public bool StopPropagation { get; set; } = true;

        [Parameter]
        public decimal Progress { get; set; }

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

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
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        public ListDirections Direction { get; set; } = ListDirections.Right;

        #endregion

        #endregion

        #region EVENTS

        protected async Task OnClickDropDownButtonHandler()
        {
            //await OnClickDropDownButton.InvokeAsync();

            if (!IsDisabled)
            {
                if (!IsOpen)
                    await Open();
                else
                    IsOpen = false;
            }
        }

        protected Task OnClickMainButtonHandler(MouseEventArgs args)
        {
            return OnClickMainButton.InvokeAsync();
        }

        #endregion

        private async Task Open()
        {
            if (OpenDirection == PopupOpenDirections.Cursor)
            {
                var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
                var popupContentSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _popupContent.Ref);

                var inputSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);

                if (inputSize.Left + popupContentSize.Width > windowSize.Width)
                {
                    //Open direction right to left.
                    _popupX = inputSize.Right - popupContentSize.Width;
                    Direction = ListDirections.Left;
                }
                else
                {
                    _popupX = inputSize.Left;
                    Direction = ListDirections.Right;
                }

                _popupWidth = inputSize.Width;

                if (inputSize.Bottom + popupContentSize.Height > windowSize.Height)
                {
                    //Open direction bottom to top.
                    _popupY = windowSize.Height - popupContentSize.Height;
                }
                else
                {
                    _popupY = inputSize.Bottom;
                }
            }

            int activeItemIndex = _popupContent.GetSelectedItemIndex();
            await _popupContent.SetActiveItemIndex(activeItemIndex);

            IsOpen = true;
        }

		#region OVERRIDES

		//protected override async Task OnAfterRenderAsync(bool firstRender)
		//{
		//	if (!firstRender)
		//	{
		//		await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
		//	}

		//	await base.OnAfterRenderAsync(firstRender);
		//}

		#endregion

		#region CLASSMAPPERS

		protected string ClassName => new ClassMapper()
                 .Add("giz-combo-button")
                 .If("giz-combo-button--full-width", () => IsFullWidth)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-combo-button__dropdown")
                 .If("giz-combo-button__dropdown--cursor", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-combo-button__dropdown--full-width", () => OpenDirection != PopupOpenDirections.Cursor)
                 .AsString();

        protected string PopupStyleValue => new StyleMapper()
                 .If($"top: {_popupY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"left: {_popupX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"min-width: {_popupWidth.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        #endregion

        public void Register(ISelectItem<int> selectItem, int value)
        {
            _items[value] = selectItem;
        }

        public void UpdateItem(ISelectItem<int> selectItem, int value)
        {
            //var actualItem = _items.Where(a => a.Value == selectItem).FirstOrDefault();
            //if (!actualItem.Equals(default(KeyValuePair<int, SelectItem<int>>)) && actualItem.Key != null)
            //{
            //    _items.Remove(actualItem.Key);
            //}

            //_items[value] = selectItem;
        }

        public void Unregister(ISelectItem<int> selectItem, int value)
        {
            //var actualItem = _items.Where(a => a.Value == selectItem).FirstOrDefault();
            //if (!actualItem.Equals(default(KeyValuePair<int, SelectItem<int>>)))
            //{
            //    _items.Remove(actualItem.Key);
            //}
        }

        public Task SetSelectedItem(ISelectItem<int> selectItem)
        {
            //bool requiresRefresh = _isOpen;

            //_isOpen = false;

            //if (_selectedItem == selectItem)
            //{
            //    if (requiresRefresh)
            //        StateHasChanged();

            //    return Task.CompletedTask;
            //}

            //_selectedItem = selectItem;

            //_hasParsingErrors = false;
            //_parsingErrors = String.Empty;

            //StateHasChanged();

            //if (selectItem != null)
            //    return SetSelectedValue(selectItem.Value);
            //else
            //    return SetSelectedValue(default(TValue));

            return Task.CompletedTask;
        }
    }
}
