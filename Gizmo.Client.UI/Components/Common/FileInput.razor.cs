using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class FileInput : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public EventCallback<InputFileChangeEventArgs> OnChange { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-file-input")
                 .If("giz-file-input--full-width", () => IsFullWidth)
                 .Add(Class)
                 .AsString();

        #endregion

        private Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            return OnChange.InvokeAsync(e);
        }
    }
}
