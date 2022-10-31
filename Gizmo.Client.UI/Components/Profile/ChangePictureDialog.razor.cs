using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangePictureDialog : CustomDOMComponentBase
    {
        public ChangePictureDialog()
        {
        }

        private bool _isOpen;

        private bool shouldRender;

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;

                _ = IsOpenChanged.InvokeAsync(_isOpen);
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        private Task CloseDialog()
        {
            IsOpen = false;

            return Task.CompletedTask;
        }

        private Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            shouldRender = false;
            int maxAllowedFiles = 1;
            long maxFileSize = 1024 * 15;
            var upload = false;

            using var content = new MultipartFormDataContent();

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                try
                {
                    var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));

                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                    content.Add(fileContent, "\"files\"", file.Name);

                    upload = true;
                }
                catch (Exception ex)
                {
                }
            }

            if (upload)
            {
                //var response = await Http.PostAsync("/Filesave", content);
            }

            return Task.CompletedTask;
        }
    }
}