using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizDock : CustomDOMComponentBase
    {
        #region FIELDS

        private List<GizDockItem> _items = new List<GizDockItem>();

        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        #endregion

        #region METHODS

        internal void Register(GizDockItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(GizDockItem item)
        {
            _items.Remove(item);
        }

        //internal void Activate(GizDockItem item)
        //{
        //    var index = _items.IndexOf(item);
        //    if (index > 0)
        //    {
        //        _items[index - 1].HalfScale();
        //    }
        //    if (index < _items.Count - 1)
        //    {
        //        _items[index + 1].HalfScale();
        //    }
        //    item.Scale();
        //}

        //internal void Deactivate(GizDockItem item)
        //{
        //    var index = _items.IndexOf(item);
        //    if (index > 0)
        //    {
        //        _items[index - 1].Reset(true);
        //    }
        //    if (index < _items.Count - 1)
        //    {
        //        _items[index + 1].Reset(true);
        //    }
        //    item.Reset();
        //}

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
