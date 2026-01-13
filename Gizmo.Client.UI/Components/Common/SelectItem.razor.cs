using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class SelectItem<TValue> : CustomDOMComponentBase, ISelectItem<TValue>
    {
        #region CONSTRUCTOR
        public SelectItem()
        {
        }
        #endregion

        #region FIELDS

        private TValue _value;

        #endregion

        #region PROPERTIES

        public ListItem ListItem { get; set; }

        [CascadingParameter]
        protected ISelect<TValue> Parent { get; set; }

        [Parameter]
        public TValue Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (EqualityComparer<TValue>.Default.Equals(_value, value))
                    return;

                _value = value;

                if (Parent != null)
                {
                    Parent.UpdateItem(this, _value);
                }
            }
        }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        #endregion

        #region EVENTS

        protected Task OnClickListItemHandler(MouseEventArgs args)
        {
            if (Parent != null)
            {
                return Parent.SetSelectedItem(this);
            }

            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            if (Parent != null)
            {
                Parent.Register(this, Value);
            }
        }

        public override void Dispose()
        {
            try
            {
                if (Parent != null)
                {
                    Parent.Unregister(this, Value);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

    }
}
