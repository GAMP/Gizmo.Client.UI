using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class DataGridCell<TItemType> : CustomDOMComponentBase
    {
        [Parameter]
        public DataGridColumn<TItemType> Column { get; set; }

        [Parameter]
        public TItemType Item { get; set; }

        [Parameter]
        public bool IsEditMode { get; set; }

        protected string StyleValue => new StyleMapper()
                 .If($"text-align: {Column.TextAlignment.ToDescriptionString()};", () => Column.TextAlignment != TextAlignments.Left)
                 .AsString();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
