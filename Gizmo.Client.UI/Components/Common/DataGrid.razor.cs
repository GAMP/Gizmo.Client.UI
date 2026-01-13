using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    /// <summary>
    /// Data grid component.
    /// </summary>
    public partial class DataGrid<TItemType> : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public DataGrid()
        {
        }
        #endregion

        #region FIELDS

        private EditContext _editContext;
        private bool _validationFailed = false;

        private readonly HashSet<DataGridColumn<TItemType>> _columns = new(Enumerable.Empty<DataGridColumn<TItemType>>());
        private ICollection<TItemType> _selectedItems = new HashSet<TItemType>();
        internal Dictionary<TItemType, DataGridRow<TItemType>> _rows = new Dictionary<TItemType, DataGridRow<TItemType>>();

        private bool _hasSelectedItems;
        private bool _hasSelectedAllItems;
        private int _providerTotalItems = 0;

        private ICollection<TItemType> _itemSource;
        private TItemType _selectedItem;
        private RenderFragment _childContent;

        private BoundingClientRect _dataGridSize;

        private bool _deselect = false;

        #region CONTEXT MENU

        private double _clientX;
        private double _clientY;
        //private Menu _contextMenu;
        private ElementReference _table;

        #endregion

        #region SORTING

        private DataGridColumn<TItemType> _sortColumn;
        private SortDirections _sortDirection;
        private TItemType _activeItem;

        #endregion

        #region PERFORMANCE

        private bool _shouldRender;
        private ICollection<TItemType> _previousItemSource;
        private TItemType _previousSelectedItem;

        #endregion

        #region CRUD

        private bool _newRow = false;
        private TItemType _editedRow = default(TItemType);

        #endregion

        #endregion

        #region PROPERTIES

        [Parameter]
        public bool RerenderOnStateChange { get; set; }

        [Parameter]
        public DataGridVariants Variant { get; set; } = DataGridVariants.Default;

        /// <summary>
        /// Gets or sets item source.
        /// </summary>
        [Parameter]
        public ICollection<TItemType> ItemSource
        {
            get
            {
                return _itemSource;
            }
            set
            {
                if (_itemSource == value)
                    return;

                _itemSource = value;
            }
        }

        /// <summary>
        /// Gets or sets selected item.
        /// </summary>
        [Parameter]
        public TItemType SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (EqualityComparer<TItemType>.Default.Equals(_selectedItem, value))
                    return;

                _selectedItem = value;

                if (SelectionMode == SelectionMode.Single && SelectedItems?.Contains(_selectedItem) == false)
                {
                    SelectedItems?.Clear();
                    SelectedItems?.Add(_selectedItem);
                }
            }
        }

        [Parameter]
        public RenderFragment ChildContent
        {
            get
            {
                return _childContent;
            }
            set
            {
                if (_childContent == value)
                    return;

                _childContent = value;

                this.Refresh();
            }
        }

        [Parameter]
        public bool HasStickyHeader { get; set; }

        [Parameter]
        public bool IsSelectable { get; set; }

        [Parameter]
        public bool ShowCheckBoxes { get; set; }

        [Parameter]
        public bool SelectOnClick { get; set; }

        /// <summary>
        /// Gets or sets if virtualization enabled.
        /// </summary>
        [Parameter]
        public bool IsVirtualized { get; set; }

        /// <summary>
        /// Gets or sets item size.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter]
        public int ItemSize { get; set; }

        /// <summary>
        /// Gets or sets overscan count.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter]
        public int OverscanCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets items provider delegate.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter]
        public ItemsProviderDelegate<TItemType> ItemsProvider { get; set; }

        /// <summary>
        /// Gets or sets placeholder template.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter]
        public RenderFragment PlaceHolderTemplate { get; set; }

        /// <summary>
        /// Gets columns collection.
        /// </summary>
        public IEnumerable<DataGridColumn<TItemType>> Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// The item under mouse on right click.
        /// </summary>
        [Parameter]
        public TItemType ActiveItem
        {
            get
            {
                return _activeItem;
            }
            set
            {
                if (EqualityComparer<TItemType>.Default.Equals(_activeItem, value))
                    return;

                _activeItem = value;

                _ = ActiveItemChanged.InvokeAsync(_activeItem);
            }
        }

        [Parameter]
        public string RowClass { get; set; }

        /// <summary>
        /// Gets or sets selected items.
        /// </summary>
        [Parameter]
        public ICollection<TItemType> SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; }
        }

        /// <summary>
        /// Gets or sets selection mode.
        /// </summary>
        [Parameter]
        public SelectionMode SelectionMode { get; set; }

        [Parameter]
        public RenderFragment<TItemType> DetailTemplate { get; set; }

        [Parameter]
        public bool DetailTemplateCustomColumns { get; set; }

        [Parameter]
        public EventCallback<TItemType> SelectedItemChanged { get; set; }

        [Parameter]
        public EventCallback<ICollection<TItemType>> SelectedItemsChanged { get; set; }

        [Parameter]
        public EventCallback<TItemType> ActiveItemChanged { get; set; }

        [Parameter]
        public EventCallback<TItemType> OnDoubleClickItem { get; set; }

        //[Parameter]
        //public RenderFragment ContextMenu { get; set; }

        //[Parameter]
        //public bool ShowColumnSelector { get; set; }

        [Parameter]
        public string TableClass { get; set; }

        [Parameter]
        public bool AllowCreate { get; set; }

        [Parameter]
        public bool AllowUpdate { get; set; }

        [Parameter]
        public bool AllowDelete { get; set; }

        [Parameter]
        public string InitialSortField { get; set; }

        [Parameter]
        public SortDirections InitialSortDirection { get; set; }

        [Parameter]
        public EventCallback<DataGridBeginOperation> OnBeginOperation { get; set; }

        [Parameter]
        public EventCallback<DataGridOperation> OnCompleteOperation { get; set; }

        [Parameter]
        public EventCallback<DataGridRowBound<TItemType>> OnRowBound { get; set; }

        [Parameter]
        public EventCallback<TItemType> OnRightClick { get; set; }

        private WindowResizeEventInterop WindowResizeEventInterop { get; set; }

        private WindowMouseDownEventInterop WindowMouseDownEventInterop { get; set; }

        #endregion

        #region OVERRIDES

        protected override async Task OnFirstAfterRenderAsync()
        {
            if (IsVirtualized && ItemsProvider != null)
            {
                var itemsProviderResult = await ItemsProvider.Invoke(new ItemsProviderRequest());
                _providerTotalItems = itemsProviderResult.TotalItemCount;
            }

            if (!string.IsNullOrEmpty(InitialSortField))
            {
                var initialSortColumn = _columns.Where(a => a.Field == InitialSortField).FirstOrDefault();

                if (initialSortColumn != null)
                    SortByColumn(initialSortColumn, InitialSortDirection);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }
            else
            {
                //Global events required to exit edit mode if user clicks outside the datagrid.
                WindowResizeEventInterop = new WindowResizeEventInterop(JsRuntime);
                await WindowResizeEventInterop.SetupWindowResizeEventCallback(args => WindowResizeHandler(args));

                WindowMouseDownEventInterop = new WindowMouseDownEventInterop(JsRuntime);
                await WindowMouseDownEventInterop.SetupWindowMouseDownEventCallback(args => WindowMouseDownHandler(args));
            }

            if (SelectionMode == SelectionMode.Single)
            {
                if (_selectedItem != null && _rows.ContainsKey(_selectedItem))
                {
                    var selectedRow = _rows[_selectedItem];
                    selectedRow.SetSelected(true);

                    foreach (var row in _rows.Where(a => a.Value != selectedRow))
                    {
                        row.Value.SetSelected(false);
                    }
                }
                else
                {
                    //Deselect all rows.
                    foreach (var row in _rows)
                    {
                        row.Value.SetSelected(false);
                    }
                }
            }

            await RefreshControlSize();

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
            bool newItemSource = !EqualityComparer<ICollection<TItemType>>.Default.Equals(_previousItemSource, _itemSource);
            bool newSelectedItem = !EqualityComparer<TItemType>.Default.Equals(_previousSelectedItem, _selectedItem);

            _previousItemSource = _itemSource;
            _previousSelectedItem = _selectedItem;

            //In case of new item source make a full refresh.
            if (newItemSource)
            {
                VerifySelected();

                this.Refresh();
            }
            else
            {
                if (newSelectedItem)
                {
                    if (VerifySelected())
                        this.Refresh();
                }
            }

            await base.OnParametersSetAsync();
        }

        protected override bool ShouldRender()
        {
            return RerenderOnStateChange || _shouldRender;
        }

        public override void Dispose()
        {
            WindowResizeEventInterop?.Dispose();
            WindowMouseDownEventInterop?.Dispose();

            base.Dispose();
        }

        #endregion

        #region EVENTS

        protected async Task IsCheckedChangedHandler(bool value)
        {
            if (_hasSelectedAllItems)
            {
                SelectedItems?.Clear();

                //Set all items selected property to false.
                foreach (var row in _rows.ToArray())
                {
                    row.Value.SetSelected(false);
                }

                _hasSelectedItems = false;
                _hasSelectedAllItems = false;
            }
            else
            {
                //Set all items selected property to true.
                foreach (var row in _rows.ToArray())
                {
                    SelectedItems.Add(row.Key);
                    row.Value.SetSelected(true);
                }

                _hasSelectedItems = true;
                _hasSelectedAllItems = true;
            }

            await SelectedItemChanged.InvokeAsync(SelectedItems.FirstOrDefault());
            await SelectedItemsChanged.InvokeAsync();
        }

        internal void OnHeaderRowMouseEvent(MouseEventArgs args, DataGridColumn<TItemType> column)
        {
            SortByColumn(column, null);
        }

        private async Task WindowResizeHandler(EventArgs args)
        {
            await RefreshControlSize();
        }

        private async Task WindowMouseDownHandler(MouseEventArgs args)
        {
            if (_editedRow != null)
            {
                //If user clicks outside the datagrid then exit edit mode.
                if (args.ClientY < _dataGridSize.Top || args.ClientY > _dataGridSize.Bottom ||
                    args.ClientX < _dataGridSize.Left || args.ClientX > _dataGridSize.Right)
                {
                    await ExitEditMode();

                    _shouldRender = true;
                    StateHasChanged();
                }
            }
        }

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            if (args.Key == null || args.Key == "Tab")
                return;

            if (_editedRow != null && args.Key == "Enter")
            {
                var lastEditedRow = _editedRow;
                await ExitEditMode();

                if (!_validationFailed)
                {
                    this.Refresh(lastEditedRow);

                    _shouldRender = true;
                    StateHasChanged();
                }
            }
        }

        protected Task ContextMenuHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region METHODS

        #region CRUD

        public async Task CreateRow()
        {
            TItemType newRow = (TItemType)Activator.CreateInstance(typeof(TItemType));
            await CreateRow(newRow);
        }

        public async Task CreateRow(TItemType item)
        {
            if (!AllowCreate)
                return;

            //If ItemSource is null then ignore it.
            if (ItemSource == null)
                return;

            //If there is already a row in edit mode then ignore.
            if (_editedRow != null)
                return;

            //If item is null then ignore it.
            if (EqualityComparer<TItemType>.Default.Equals(item, default(TItemType)))
                return;

            //Store the new row so we can set it in edit mode when it's added to the DataGrid.
            _newRow = true;
            _editedRow = item;

            var dataGridBeginOperation = new DataGridBeginOperation() { OperationType = DataGridOperationTypes.EditRow, Data = _editedRow };
            await OnBeginOperation.InvokeAsync(dataGridBeginOperation);

            if (dataGridBeginOperation.Cancel)
                return;

            _editContext = new EditContext(_editedRow);
            _editContext.EnableDataAnnotationsValidation();

            if (IsVirtualized)
            {
                //TODO: Virtualization
            }
            else
            {
                ItemSource.Add(_editedRow);
            }

            //We need to refresh the DataGrid to see the new row.
            this.Refresh();
        }

        public async Task UpdateRow(TItemType item)
        {
            if (!AllowUpdate)
                return;

            //If there is already a row in edit mode then ignore.
            if (_editedRow != null)
                return;

            //If item is null then ignore it.
            if (EqualityComparer<TItemType>.Default.Equals(item, default(TItemType)))
                return;

            _newRow = false;
            _editedRow = item;

            var dataGridBeginOperation = new DataGridBeginOperation() { OperationType = DataGridOperationTypes.EditRow, Data = _editedRow };
            await OnBeginOperation.InvokeAsync(dataGridBeginOperation);

            if (dataGridBeginOperation.Cancel)
                return;

            if (_rows.ContainsKey(_editedRow))
            {
                _editContext = new EditContext(_editedRow);
                _editContext.EnableDataAnnotationsValidation();

                _rows[_editedRow].SetEditMode(true);

                //Required to refresh the EditContext on the row.
                this.Refresh();
            }
        }

        #endregion

        private void SortByColumn(DataGridColumn<TItemType> column, SortDirections? sortDirection)
        {
            if (column.CanSort)
            {
                foreach (var item in Columns)
                {
                    if (item != column)
                    {
                        item.IsSorted = false;
                    }
                }

                if (_sortColumn != column)
                {
                    _sortColumn = column;
                    
                    if (sortDirection.HasValue)
                    {
                        _sortDirection = sortDirection.Value;
                    }
                    else
                    {
                        _sortDirection = SortDirections.Ascending;
                    }

                    column.SortDirection = _sortDirection;

                    column.IsSorted = true;
                }
                else
                {
                    if (sortDirection.HasValue)
                    {
                        _sortDirection = sortDirection.Value;
                    }
                    else
                    {
                        if (_sortDirection == SortDirections.Ascending)
                        {
                            _sortDirection = SortDirections.Descending;
                        }
                        else
                        {
                            _sortDirection = SortDirections.Ascending;
                        }
                    }

                    column.SortDirection = _sortDirection;

                    column.IsSorted = true;
                }

                this.Refresh();
            }
        }

        private async Task ExitEditMode()
        {
            //If the selected row is not the edited row.
            if (_editedRow != null)
            {
                //Validate before exit edit mode.
                if (_editContext != null && !_editContext.Validate())
                {
                    _validationFailed = true;
                    return;
                }

                _validationFailed = false;

                var lastEditedRow = _editedRow;

                if (_rows.ContainsKey(_editedRow))
                {
                    _rows[_editedRow].SetEditMode(false);
                }

                _newRow = false;
                _editedRow = default(TItemType);

                await OnCompleteOperation.InvokeAsync(new DataGridOperation() { OperationType = DataGridOperationTypes.EditRow, Data = lastEditedRow });
            }
        }

        public void Refresh()
        {
            _shouldRender = true;

            foreach (var row in _rows)
            {
                row.Value.Refresh();
            }

            StateHasChanged();
        }

        public void Refresh(TItemType item)
        {
            if (_rows.ContainsKey(item))
            {
                _rows[item].Refresh();
            }
        }

        internal Task RightClickItem(TItemType item)
        {
            ActiveItem = item;

            return OnRightClick.InvokeAsync(ActiveItem);
        }

        //internal async Task OpenContextMenu(double clientX, double clientY)
        //{
        //    var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
        //    var contextMenuSize = await _contextMenu.GetListBoundingClientRect();

        //    //var gridPosition = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);
        //    //_clientX = clientX - gridPosition.Left;
        //    //if (gridPosition.Left + _clientX + contextMenuSize.Width > windowSize.Width)
        //    //{
        //    //    _clientX = windowSize.Width - gridPosition.Left - contextMenuSize.Width - 40;
        //    //}

        //    //_clientY = clientY - gridPosition.Top;

        //    if (clientX > windowSize.Width / 2)
        //    {
        //        //Open direction right to left.
        //        _clientX = clientX - contextMenuSize.Width;
        //        _contextMenu.SetDirection(ListDirections.Left);
        //    }
        //    else
        //    {
        //        _clientX = clientX;
        //        _contextMenu.SetDirection(ListDirections.Right);
        //    }

        //    if (clientY > windowSize.Height / 2)
        //    {
        //        //Open direction bottom to top.
        //        _clientY = clientY - contextMenuSize.Height;
        //        _contextMenu.ExpandBottomToTop = true;
        //    }
        //    else
        //    {
        //        _clientY = clientY;
        //        _contextMenu.ExpandBottomToTop = false;
        //    }

        //    _contextMenu.Open(_clientX, _clientY);
        //}

        internal void AddColumn(DataGridColumn<TItemType> column)
        {
            _columns.Add(column);
        }

        internal void RemoveColumn(DataGridColumn<TItemType> column)
        {
            _columns.Remove(column);
        }

        internal async Task Register(DataGridRow<TItemType> row, TItemType item)
        {
            if (item == null)
                return;

            //InvokeVoidAsync("writeLine", $"Add row {item}");

            _rows[item] = row;

            if (SelectedItems.Contains(item))
                _rows[item].SetSelected(true);

            if (EqualityComparer<TItemType>.Default.Equals(item, _editedRow))
            {
                var dataGridBeginOperation = new DataGridBeginOperation() { OperationType = DataGridOperationTypes.EditRow, Data = _editedRow };
                await OnBeginOperation.InvokeAsync(dataGridBeginOperation);

                if (!dataGridBeginOperation.Cancel)
                {
                    _rows[item].SetEditMode(true);
                }
            }
        }

        internal void UpdateItem(DataGridRow<TItemType> row, TItemType item)
        {
            if (item == null)
                return;

            //InvokeVoidAsync("writeLine", $"Update row {item}");

            var actualRow = _rows.Where(a => a.Value == row).FirstOrDefault();
            if (!actualRow.Equals(default(KeyValuePair<DataGridRow<TItemType>, TItemType>)) && actualRow.Key != null)
            {
                //InvokeVoidAsync("writeLine", $"Remove previous row {actualRow.Key}");
                _rows.Remove(actualRow.Key);
            }

            _rows[item] = row;

            if (SelectedItems.Contains(item))
            {
                //InvokeVoidAsync("writeLine", $"Selected row {item}");
                _rows[item].SetSelected(true);
            }
            else
            {
                //InvokeVoidAsync("writeLine", $"Deselected row {item}");
                _rows[item].SetSelected(false);
            }
        }

        internal void Unregister(DataGridRow<TItemType> row, TItemType item)
        {
            if (item == null)
                return;

            //InvokeVoidAsync("writeLine", $"Remove row {item}");
            //_rows.Remove(item);

            var actualRow = _rows.Where(a => a.Value == row).FirstOrDefault();
            if (!actualRow.Equals(default(KeyValuePair<DataGridRow<TItemType>, TItemType>)))
            {
                //InvokeVoidAsync("writeLine", $"Remove row {actualRow.Key}");
                _rows.Remove(actualRow.Key);
            }
        }

        internal async Task DoubleClickRow(DataGridRow<TItemType> item)
        {
            TItemType dataItem = item.Item;

            await OnDoubleClickItem.InvokeAsync(dataItem);
        }

        internal bool VerifySelected()
        {
            bool selectionUpdated = false;

            if (SelectedItems != null)
            {
                var previouslySelectedItems = SelectedItems.ToList();

                foreach (var item in previouslySelectedItems)
                {
                    if (_itemSource.Where(a => EqualityComparer<TItemType>.Default.Equals(a, item)).Count() == 0)
                    {
                        SelectedItems.Remove(item);
                        selectionUpdated = true;
                    }
                }
            }

            if (SelectedItems == null || SelectedItems.Count == 0)
            {
                _selectedItem = default;
            }

            return selectionUpdated;
        }

        internal async Task SelectRow(DataGridRow<TItemType> item, bool selected)
        {
            //Called once data row item is clicked, right clicked or row checkbox clicked.

            TItemType dataItem = item.Item;

            bool wasSelected = SelectedItems?.Contains(dataItem) == true;

            if (wasSelected == selected)
                return;

            if (SelectionMode == SelectionMode.Single)
            {
                //In single selection mode.

                //If current row is already selected.
                if (wasSelected)
                {
                    _selectedItem = dataItem;

                    if (AllowUpdate)
                    {
                        _newRow = false;
                        _editedRow = _selectedItem;

                        if (_rows.ContainsKey(_editedRow))
                        {
                            var dataGridBeginOperation = new DataGridBeginOperation() { OperationType = DataGridOperationTypes.EditRow, Data = _editedRow };
                            await OnBeginOperation.InvokeAsync(dataGridBeginOperation);

                            if (!dataGridBeginOperation.Cancel)
                            {
                                _editContext = new EditContext(_editedRow);
                                _editContext.EnableDataAnnotationsValidation();

                                _rows[_editedRow].SetEditMode(true);

                                //Required to refresh the EditContext on the row.
                                this.Refresh();
                            }
                        }
                    }
                    else
                    {
                        if (_deselect)
                        {
                            //Clear selected items list and set selected property to false.
                            SelectedItems?.Clear();
                            item.SetSelected(false);

                            _selectedItem = default;
                        }
                    }
                }
                else
                {
                    await ExitEditMode();

                    if (_validationFailed)
                        return;

                    _selectedItem = dataItem;

                    //Clear selected items list, add only this item in the list and set selected property to true.
                    SelectedItems?.Clear();
                    SelectedItems?.Add(dataItem);
                    item.SetSelected(true);

                    //Set all other items selected property to false.
                    foreach (var row in _rows.Where(a => a.Value != item).ToArray())
                    {
                        row.Value.SetSelected(false);
                    }
                }
            }
            else if (SelectionMode == SelectionMode.Extended)
            {
                //No matter of selection the clicked item is always the selected one
                _selectedItem = dataItem;

                //In extended selection mode.

                //If current row is already selected.
                if (wasSelected)
                {
                    //Remove current row from selected items list and set selected property to false.
                    SelectedItems.Remove(dataItem);
                    item.SetSelected(false);
                }
                else
                {
                    //Add current row in selected items list and set selected property to true.
                    SelectedItems?.Add(dataItem);
                    item.SetSelected(true);
                }

                if (SelectedItems == null || SelectedItems.Count == 0)
                {
                    _selectedItem = default;
                }
            }

            UpdateHeaderCheckbox();

            await SelectedItemChanged.InvokeAsync(_selectedItem);
            await SelectedItemsChanged.InvokeAsync();
        }

        private void UpdateHeaderCheckbox()
        {
            var previousHasSelectedItems = _hasSelectedItems;
            var previousHasSelectedAllItems = _hasSelectedAllItems;

            if (SelectionMode == SelectionMode.Single)
            {
                _hasSelectedItems = _selectedItem != null;
                _hasSelectedAllItems = _selectedItem != null && _itemSource.Count == 1;
            }
            else
            {
                if (SelectedItems?.Count > 0)
                {
                    _hasSelectedItems = true;
                    if (ItemSource != null)
                    {
                        if (SelectedItems?.Count == ItemSource.Count)
                        {
                            _hasSelectedAllItems = true;
                        }
                        else
                        {
                            _hasSelectedAllItems = false;
                        }
                    }
                    else
                    {
                        if (SelectedItems?.Count == _providerTotalItems)
                        {
                            _hasSelectedAllItems = true;
                        }
                        else
                        {
                            _hasSelectedAllItems = false;
                        }
                    }
                }
                else
                {
                    _hasSelectedItems = false;
                }
            }

            if (previousHasSelectedItems != _hasSelectedItems ||
                previousHasSelectedAllItems != _hasSelectedAllItems)
                this.Refresh();
        }

        private async Task RefreshControlSize()
        {
            _dataGridSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .If("giz-data-grid-wrapper", () => Variant == DataGridVariants.Default)
                 .If("giz-data-grid--sticky-header", () => HasStickyHeader)
                 .AsString();

        protected string TableClassName => new ClassMapper()
                 .If("giz-data-grid", () => Variant == DataGridVariants.Default)
                 .AsString();

        #endregion

        private IEnumerable<TItemType> GetSortedData()
        {
            if (_sortColumn == null)
                return ItemSource;

            if (_sortColumn.SortFunction != null)
            {
                return _sortColumn.SortFunction.Invoke(_sortDirection);
            }
            else
            {
                Comparer comparer = new Comparer(_sortColumn.Field, _sortDirection);
                List<TItemType> tmp = ItemSource.ToList();
                tmp.Sort(comparer);
                return tmp;
            }
        }

        public class Comparer : IComparer<TItemType>
        {
            private string _sortColumn;
            private SortDirections _sortDirection;

            public Comparer(string sortColumn, SortDirections sortDirections)
            {
                _sortColumn = sortColumn;
                _sortDirection = sortDirections;
            }

            public int Compare(TItemType? x, TItemType? y)
            {
                var property = typeof(TItemType).GetProperty(_sortColumn);

                var xValue = property.GetValue(x);
                var yValue = property.GetValue(y);

                int result = 0;

                if (property.PropertyType == typeof(decimal))
                {
                    result = decimal.Compare((decimal)xValue, (decimal)yValue);
                }

                if (property.PropertyType == typeof(string))
                {
                    result = string.Compare((string)xValue, (string)yValue);
                }

                if (property.PropertyType == typeof(DateTime))
                {
                    result = DateTime.Compare((DateTime)xValue, (DateTime)yValue);
                }

                if (property.PropertyType == typeof(TimeSpan))
                {
                    result = TimeSpan.Compare((TimeSpan)xValue, (TimeSpan)yValue);
                }

                if (property.PropertyType == typeof(short))
                {
                    if ((short)xValue > (short)yValue)
                        result = 1;

                    if ((short)xValue < (short)yValue)
                        result = -1;
                }

                if (property.PropertyType == typeof(int))
                {
                    if ((int)xValue > (int)yValue)
                        result = 1;

                    if ((int)xValue < (int)yValue)
                        result = -1;
                }

                if (property.PropertyType == typeof(long))
                {
                    if ((long)xValue > (long)yValue)
                        result = 1;

                    if ((long)xValue < (long)yValue)
                        result = -1;
                }

                if (_sortDirection == SortDirections.Descending)
                    result *= -1;

                return result;
            }
        }
    }
}
