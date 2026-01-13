using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class DataGridDetailTemplate : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public DataGridDetailTemplate()
        {
        }
        #endregion

        [Parameter]
        public int Columns { get; set; }

        [Parameter]
        public bool DetailTemplateCustomColumns { get; set; }

        [Parameter()]
        public RenderFragment ChildContent { get; set; }

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
