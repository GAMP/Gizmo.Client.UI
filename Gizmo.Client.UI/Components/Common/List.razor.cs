using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class List : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public List()
        {
        }
        #endregion

        #region FIELDS

        private List<ListItem> _items = new List<ListItem>();
        private HashSet<List> _childLists = new HashSet<List>();
        private ListItem _selectedItem;
        private ListItem _activeItem;
        private ListDirections _direction = ListDirections.Right;

        #endregion

        #region PROPERTIES

        [CascadingParameter]
        protected List ParentList { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool CanClick { get; set; }

        [Parameter]
        public bool CanSelect { get; set; }

        [Parameter]
        public bool PreserveIconSpace { get; set; }

        [Parameter]
        public ListItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                _ = SetSelectedItem(value);
            }
        }

        [Parameter]
        public EventCallback<ListItem> SelectedItemChanged { get; set; }

        [Parameter]
        public EventCallback<ListItem> OnClickItem { get; set; }

        [Parameter]
        public ListDirections Direction
        {
            get
            {
                return _direction;

            }
            set
            {
                _direction = value;

                foreach (var item in _childLists)
                {
                    item.Direction = _direction;
                }
            }
        }

        [Parameter]
        public string MaximumHeight { get; set; }

        [Parameter]
        public RenderFragment ListHeader { get; set; }

        [Parameter]
        public string BodyClass { get; set; }

        [Parameter]
        public bool ExpandBottomToTop { get; set; }

        public ListItem ActiveItem
        {
            get
            {
                return _activeItem;
            }
            set
            {
                if (_activeItem == value)
                    return;

                _activeItem = value;
            }
        }

        #endregion

        #region METHODS

        internal void Collapse()
        {
            foreach (var item in _items)
            {
                item.IsExpanded = false;
            }

            foreach (var item in _childLists)
            {
                item.Collapse();
            }
        }

        public async Task SetSelectedItem(ListItem item)
        {
            if (IsDisabled)
                return;

            if (!CanSelect)
                return;

            if (_selectedItem == item)
                return;

            _selectedItem = item;
            await SelectedItemChanged.InvokeAsync(item);

            foreach (var listItem in _items.ToArray())
                listItem.SetSelected(item == listItem);

            foreach (var childList in _childLists.ToArray())
                await childList.SetSelectedItem(item);

            if (ParentList != null)
                await ParentList.SetSelectedItem(item);
        }

        internal async Task SetActiveItem(ListItem item)
        {
            if (IsDisabled)
                return;

            if (!CanSelect)
                return;

            if (_activeItem == item)
                return;

            _activeItem = item;

            foreach (var listItem in _items.ToArray())
                listItem.SetActive(item == listItem);

            foreach (var childList in _childLists.ToArray())
                await childList.SetActiveItem(item);

            if (ParentList != null)
                await ParentList.SetActiveItem(item);
        }

        internal async Task SetClickedItem(ListItem item)
        {
            if (ParentList != null)
                await ParentList.SetClickedItem(item);

            await OnClickItem.InvokeAsync(item);

            await SetSelectedItem(item);
        }

        internal void Register(ListItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(ListItem item)
        {
            _items.Remove(item);
        }

        internal void Register(List child)
        {
            _childLists.Add(child);
        }

        internal void Unregister(List child)
        {
            _childLists.Remove(child);
        }

        public int GetSelectedItemIndex()
        {
            if (_selectedItem != null)
                return _items.IndexOf(_selectedItem);
            else
                return -1;
        }

        internal async Task SetSelectedItemIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                var item = _items[index];
                await SetSelectedItem(item);
                await InvokeVoidAsync("scrollListItemIntoView", item.Ref);
            }
        }

        public int GetActiveItemIndex()
        {
            if (_activeItem != null)
                return _items.IndexOf(_activeItem);
            else
                return -1;
        }

        public async Task SetActiveItemIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                var item = _items[index];
                await SetActiveItem(item);
                await InvokeVoidAsync("scrollListItemIntoView", item.Ref);
            }
        }

        public int GetListSize()
        {
            return _items.Count;
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            if (ParentList != null)
            {
                ParentList.Register(this);
                IsDisabled = ParentList.IsDisabled;
                Direction = ParentList.Direction;
            }
        }

        public override void Dispose()
        {
            ParentList?.Unregister(this);

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-list")
                 .Add($"giz-list--{Direction.ToDescriptionString()}")
                 .If("giz-list--top", () => ExpandBottomToTop)
                 .If("giz-list--clickable", () => CanClick)
                 .If("giz-list--selectable", () => CanSelect)
                 .If("giz-list--with-header", () => ListHeader != null)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"max-height: {MaximumHeight}", () => !string.IsNullOrEmpty(MaximumHeight))
                 .AsString();

        #endregion
    }
}
