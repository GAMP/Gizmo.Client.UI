using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangePictureDialog : CustomDOMComponentBase
    {
        public ChangePictureDialog()
        {
        }

        //private bool _shouldRender;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        public string Image { get; set; }

        private Task CloseDialog()
        {
            return DismissCallback.InvokeAsync();
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            //_shouldRender = false;
            int maxAllowedFiles = 1;
            long maxFileSize = 1024 * 1024 * 4;
            var upload = false;

            using var content = new MultipartFormDataContent();

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                try
                {
                    var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));

                    var fileType = e.File.ContentType;
                    var buffer = new byte[e.File.Size];

                    await e.File.OpenReadStream().ReadAsync(buffer);

                    Image = $"data:{fileType};base64,{Convert.ToBase64String(buffer)}";

                    //fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    //content.Add(fileContent, "\"files\"", file.Name);

                    upload = true;
                }
                catch
                {
                }
            }

            if (upload)
            {
                //var response = await Http.PostAsync("/Filesave", content);
            }
        }
    }
}
