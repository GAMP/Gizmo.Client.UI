using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class QuantityPicker : CustomDOMComponentBase
    {
        [Parameter]
        public int Quantity { get; set; }

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public int Minimum { get; set; } = 0;

        [Parameter]
        public EventCallback<MouseEventArgs> OnAddQuantityButtonClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnRemoveQuantityButtonClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public Task RemoveQuantity(MouseEventArgs args)
        {
            return OnRemoveQuantityButtonClick.InvokeAsync(args);
        }

        public Task AddQuantity(MouseEventArgs args)
        {
            return OnAddQuantityButtonClick.InvokeAsync(args);
        }

        public Task OnClickHandler(MouseEventArgs args)
        {
            return OnClick.InvokeAsync(args);
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-shop-quantity-picker")
                 .Add($"giz-shop-quantity-picker--{Size.ToDescriptionString()}")
                 .AsString();

        #endregion

    }
}
