using System;
using System.Globalization;
using System.Threading.Tasks;

using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangePictureDialog : ShellComponentBase
    {
        // The square the picture is saved as. 512 is generous for a circle this shell
        // never draws larger than about 154 px, and stays well under 100 KB compressed.
        private const int MaxDimension = 512;
        private const double CompressionQuality = 0.82;

        // The upper bound of what the editor works on. The customer still frames a whole
        // picture, but a 12 MP phone photo does not get to sit in the WebView's memory at
        // its native size while they drag it around.
        private const int SourceMaxDimension = 1600;

        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<EmptyComponentResult> ResultCallback { get; set; }

        #endregion

        #region FIELDS

        private ElementReference _fileInputRef;
        private ElementReference _stageRef;
        private DotNetObjectReference<ChangePictureDialog> _selfRef;

        //The whole picture the editor works on. The cropped, compressed result is made
        //on save; there is nothing to keep in C# until then.
        private string _sourceDataUrl;
        private string _attachedSourceDataUrl;
        private int _zoomPercent = 100;

        private string _urlInput = string.Empty;
        private bool _isBusy;
        private string _errorMessage;

        #endregion

        #region OVERRIDES

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _selfRef = CreateDotNetObjectReference(this);
                await InvokeVoidAsync("setupAvatarPaste", _selfRef, nameof(OnImagePastedFromJs));
            }

            //The stage exists only once a picture has been chosen, so the editor is
            //attached here rather than in the load handler - and only for a picture it
            //is not already attached to.
            if (_sourceDataUrl != null && _sourceDataUrl != _attachedSourceDataUrl)
            {
                _attachedSourceDataUrl = _sourceDataUrl;
                _zoomPercent = 100;

                try
                {
                    await JsInvokeAsync<object>("avatarCropInit", _stageRef, _sourceDataUrl);
                }
                catch
                {
                    _errorMessage = ShellStringOverrides.Get(ShellStringOverrides.AVATAR_EDITOR_FAILED);
                    DispatchRender();
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public override void Dispose()
        {
            try
            {
                _ = InvokeVoidAsync("teardownAvatarPaste");
                _ = InvokeVoidAsync("avatarCropDispose");
            }
            catch
            {
                // The JS runtime may already be gone - there is nothing left to clean up.
            }

            _selfRef?.Dispose();

            base.Dispose();
        }

        #endregion

        #region EVENTS

        private async Task CloseDialog()
        {
            await InvokeVoidAsync("teardownAvatarPaste");
            await InvokeVoidAsync("avatarCropDispose");
            await DismissCallback.InvokeAsync();
        }

        private Task OnFileInputChanged(ChangeEventArgs _)
        {
            return LoadSourceAsync(() => JsInvokeAsync<AvatarSourceResult>(
                "avatarSourceFromInputElement", _fileInputRef, SourceMaxDimension));
        }

        [JSInvokable]
        public Task OnImagePastedFromJs(string dataUrl)
        {
            return LoadSourceAsync(() => JsInvokeAsync<AvatarSourceResult>(
                "avatarSourceFromDataUrl", dataUrl, SourceMaxDimension));
        }

        private Task OnLoadUrlClick()
        {
            if (string.IsNullOrWhiteSpace(_urlInput))
                return Task.CompletedTask;

            return LoadSourceAsync(() => JsInvokeAsync<AvatarSourceResult>(
                "avatarSourceFromUrl", _urlInput.Trim(), SourceMaxDimension));
        }

        private Task OnUrlInputKeyDown(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
                return OnLoadUrlClick();

            return Task.CompletedTask;
        }

        private async Task OnZoomInput(ChangeEventArgs args)
        {
            if (!int.TryParse(args.Value?.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var percent))
                return;

            _zoomPercent = percent;

            try
            {
                await JsInvokeAsync<double>("avatarCropSetZoom", percent / 100.0);
            }
            catch
            {
                // The editor went away between the drag and the callback.
            }
        }

        private async Task OnChooseDifferentClick()
        {
            await InvokeVoidAsync("avatarCropDispose");

            _sourceDataUrl = null;
            _attachedSourceDataUrl = null;
            _errorMessage = null;
        }

        private async Task OnSaveClick()
        {
            if (_sourceDataUrl is null)
                return;

            _isBusy = true;
            _errorMessage = null;
            DispatchRender();

            try
            {
                //What is uploaded is what the customer framed in the circle, not the
                //whole picture.
                var cropped = await JsInvokeAsync<CompressedImageResult>(
                    "avatarCropExport", MaxDimension, CompressionQuality);

                var bytes = ExtractBytesFromDataUrl(cropped.DataUrl);

                var saved = AvatarService.Current is not null
                    && await AvatarService.Current.UploadAsync(bytes, cropped.ContentType, default);

                if (saved)
                {
                    _isBusy = false;
                    await CloseDialog();
                    return;
                }

                //The service says what actually went wrong (not reachable at <url>,
                //refused with <status>). A bare "try later" left neither the customer
                //nor the operator able to tell a stopped service from bad credentials.
                _errorMessage = AvatarService.Current?.LastError
                    ?? ShellStringOverrides.Get(ShellStringOverrides.AVATAR_SAVE_FAILED);
            }
            catch
            {
                _errorMessage = ShellStringOverrides.Get(ShellStringOverrides.AVATAR_CROP_FAILED);
            }
            finally
            {
                _isBusy = false;
                DispatchRender();
            }
        }

        #endregion

        #region HELPERS

        private async Task LoadSourceAsync(Func<ValueTask<AvatarSourceResult>> load)
        {
            _errorMessage = null;
            _isBusy = true;
            DispatchRender();

            try
            {
                var result = await load();

                await InvokeVoidAsync("avatarCropDispose");

                _attachedSourceDataUrl = null;
                _sourceDataUrl = result.DataUrl;
            }
            catch
            {
                // A bad or unreachable link, a remote picture the page may not read, a
                // broken file, a format the browser does not know: all of them are the
                // same "that picture could not be used" to the customer.
                _errorMessage = ShellStringOverrides.Get(ShellStringOverrides.AVATAR_LOAD_FAILED);
            }
            finally
            {
                _isBusy = false;
                DispatchRender();
            }
        }

        private static byte[] ExtractBytesFromDataUrl(string dataUrl)
        {
            var comma = dataUrl.IndexOf(',');
            var base64 = comma >= 0 ? dataUrl[(comma + 1)..] : dataUrl;

            return Convert.FromBase64String(base64);
        }

        #endregion

        private sealed class CompressedImageResult
        {
            public string DataUrl { get; set; } = string.Empty;
            public string ContentType { get; set; } = string.Empty;
            public long ByteLength { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        private sealed class AvatarSourceResult
        {
            public string DataUrl { get; set; } = string.Empty;
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
