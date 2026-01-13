using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ButtonGroup : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public ButtonGroup()
        {
        }
        #endregion

        #region FIELDS

        private HashSet<Button> _items = new HashSet<Button>();
        private Button _selectedItem;
        private ICollection<Button> _selectedItems = new HashSet<Button>();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Whether the button group is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Whether is mandatory to have an item selected.
        /// </summary>
        [Parameter]
        public bool IsMandatory { get; set; }

        /// <summary>
        /// The selection mode of the button group.
        /// </summary>
        [Parameter]
        public SelectionMode SelectionMode { get; set; } = SelectionMode.Single;

        /// <summary>
        /// The selected button in the button group.
        /// </summary>
        [Parameter]
        public Button SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem == value)
                    return;

                SelectItem(value, true);
            }
        }

        [Parameter]
        public EventCallback<Button> SelectedItemChanged { get; set; }

        [Parameter]
        public EventCallback<ICollection<Button>> SelectedItemsChanged { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public ICollection<Button> SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; }
        }

        #endregion

        #region METHODS

        internal void SelectItem(Button item, bool selected)
        {
            bool wasSelected = SelectedItems?.Contains(item) == true;

            if (wasSelected == selected)
                return;

            //In single selection mode
            if (SelectionMode == SelectionMode.Single)
            {
                //If is mandatory
                if (IsMandatory)
                {
                    //Ignore null and same button clicks.
                    if (_selectedItem == item || item == null)
                        return;

                    //If button is different then set it as selected.
                    _selectedItem = item;

                    SelectedItems?.Clear();
                    SelectedItems?.Add(_selectedItem);
                }
                else //If is not mandatory
                {
                    //Clear selected items list.
                    SelectedItems?.Clear();

                    //If same button the set null as selected.
                    if (_selectedItem == item)
                    {
                        _selectedItem = null;
                    }
                    else //If button is different then set it as selected.
                    {
                        _selectedItem = item;
                        SelectedItems?.Add(_selectedItem);
                    }
                }

                _ = SelectedItemChanged.InvokeAsync(_selectedItem);
                _ = SelectedItemsChanged.InvokeAsync(_selectedItems);

                //Update button states.
                foreach (var button in _items.ToArray())
                {
                    button.SetSelected(_selectedItem == button);
                }
            }
            else //In extended selection mode
            {
                var firstSelected = _items.Where(a => a != item && a.GetSelected()).FirstOrDefault();

                //If is mandatory
                if (IsMandatory)
                {
                    //Ignore null.
                    if (item == null)
                        return;

                    if (wasSelected)
                    {
                        //If the item is the only one selected then ignore.
                        if (firstSelected == null)
                            return;

                        _selectedItem = firstSelected;

                        SelectedItems?.Remove(item);

                        //Update button state.
                        item.SetSelected(false);
                    }
                    else
                    {
                        _selectedItem = item;

                        SelectedItems?.Add(_selectedItem);

                        //Update button state.
                        item.SetSelected(true);
                    }
                }
                else //If is not mandatory
                {
                    if (wasSelected)
                    {
                        //If same button the set the first available as selected.
                        if (_selectedItem == item)
                        {
                            _selectedItem = firstSelected;
                        }
                    }
                    else
                    {
                        _selectedItem = item;
                    }

                    //Set button state.
                    item.SetSelected(selected);
                }

                _ = SelectedItemChanged.InvokeAsync(_selectedItem);
                _ = SelectedItemsChanged.InvokeAsync(_selectedItems);
            }
        }

        internal void Register(Button item)
        {
            _items.Add(item);
        }

        internal void Unregister(Button item)
        {
            _items.Remove(item);
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            //If is mandatory and there is no item selected, select the first available item if any.
            if (IsMandatory && SelectedItem == null)
            {
                var firstItem = _items.FirstOrDefault();

                if (firstItem != null)
                {
                    SelectItem(firstItem, true);
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-button-group")
                 .If("disabled", () => IsDisabled)
                 .AsString();

        #endregion

    }
}
