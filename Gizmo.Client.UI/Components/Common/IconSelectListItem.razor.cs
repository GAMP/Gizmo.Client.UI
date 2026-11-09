using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class IconSelectListItem : CustomDOMComponentBase
    {
        protected bool _shouldRender;

        #region PROPERTIES

        [Parameter]
        public IconSelectItem Item { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected string Name => Item is IconSelectCountry country && !string.IsNullOrEmpty(country.Text) ? country.Text : Item.Display;

        protected string Code => (Item as IconSelectCountry)?.PhonePrefix;

        #endregion

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<IconSelectItem>(nameof(Item), out var newItem))
            {
                var valueChanged = !EqualityComparer<IconSelectItem>.Default.Equals(Item, newItem);
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            await base.SetParametersAsync(parameters);
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
            }
            else
            {
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion
    }
}
