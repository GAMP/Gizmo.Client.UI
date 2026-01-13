using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class DataGridRow<TItemType> : CustomDOMComponentBase
    {
        #region FIELDS

        private DataGrid<TItemType> _parent;
        private IEnumerable<DataGridColumn<TItemType>> _columns;
        private TItemType _item;
        private bool _isSelected = false;
        private bool _isOpen = false;
        private bool _isEditMode = false;

        private bool _shouldRender = false;

        #endregion

        #region PROPERTIES

        [CascadingParameter(Name = "Parent")]
        protected DataGrid<TItemType> Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent == value)
                    return;

                _parent = value;

                this.Refresh();
            }
        }

        [Parameter]
        public IEnumerable<DataGridColumn<TItemType>> Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                if (_columns == value)
                    return;

                _columns = value;

                this.Refresh();
            }
        }

        [Parameter]
        public bool DetailTemplateCustomColumns { get; set; }

        [Parameter]
        public TItemType Item
        {
            get
            {
                return _item;
            }
            set
            {
                if (EqualityComparer<TItemType>.Default.Equals(_item, value))
                    return;

                _item = value;

                this.Refresh();

                if (Parent != null)
                {
                    Parent.UpdateItem(this, _item);
                    _ = Parent.OnRowBound.InvokeAsync(new DataGridRowBound<TItemType>() { Row = this });
                }
            }
        }

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

                this.Refresh();
            }
        }

        [Parameter]
        public bool IsSelectable { get; set; }

        [Parameter]
        public bool IsDropdown { get; set; }

        public string CustomStyle { get; set; }

        public bool HideDetails { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
             .If("selected", () => _isSelected)
             .If("giz-data-grid-row--dropdown", () => IsDropdown && !HideDetails)
             .If("giz-data-grid-row--open", () => IsOpen).AsString();

        #endregion

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            if (Parent != null)
            {
                await Parent.Register(this, Item);
                await Parent.OnRowBound.InvokeAsync(new DataGridRowBound<TItemType>() { Row = this });
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            try
            {
                if (Parent != null)
                {
                    Parent.Unregister(this, Item);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region EVENTS

        protected async Task OnClickEvent(MouseEventArgs args, TItemType item)
        {
            //TODO: A ADD DELAY FOR DOUBLE CLICK?
            if (IsDropdown && !HideDetails)
            {
                IsOpen = !IsOpen;
            }
            else
            {
                if (IsSelectable && Parent.SelectOnClick)
                {
                    await Parent?.SelectRow(this, !_isSelected);
                }
            }
        }

        protected async Task OnDoubleClickEvent(MouseEventArgs args, TItemType item)
        {
            await Parent?.DoubleClickRow(this);
        }

        protected async Task IsCheckedChangedHandler(bool value)
        {
            await Parent?.SelectRow(this, value);
        }

        protected async Task ContextMenuHandler(MouseEventArgs args)
        {
            if (Parent != null)
            {
                if (IsSelectable && Parent.SelectOnClick)
                {
                    await Parent?.SelectRow(this, !_isSelected);
                }

                if (args.Button == 2)
                {
                    await Parent.RightClickItem(Item);

                    //if (Parent.ContextMenu != null)
                    //{
                    //    await Parent.OpenContextMenu(args.ClientX, args.ClientY);
                    //}
                }
            }
        }

        #endregion

        #region METHODS

        public void Refresh()
        {
            _shouldRender = true;

            StateHasChanged();
        }

        internal void SetSelected(bool value)
        {
            if (!IsSelectable)
                return;

            if (_isSelected == value)
                return;

            _isSelected = value;
            _shouldRender = true;

            StateHasChanged();
        }

        internal void SetEditMode(bool value)
        {
            if (_isEditMode == value)
                return;

            _isEditMode = value;
            //_shouldRender = true;

            //StateHasChanged();
        }

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override bool ShouldRender()
        {
            return Parent?.RerenderOnStateChange == true || _shouldRender;
        }
    }
}
