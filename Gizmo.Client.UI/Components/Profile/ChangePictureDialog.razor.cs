using Gizmo.Client.UI.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangePictureDialog : CustomDOMComponentBase
    {
        // Square output side, in pixels. 512 is generous for a circular
        // avatar rendered at any size this shell actually uses (the
        // biggest is the profile popup's 9.6rem/~154px circle) while still
        // landing well under 100KB per image after WebP/JPEG compression.
        private const int MaxDimension = 512;
        private const double CompressionQuality = 0.82;

        // Upper bound for the image handed to the editor. The customer still
        // crops from a full-frame picture, but a 12MP phone snapshot does not
        // get to sit in WebView memory at native size while they drag it around.
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

        //Full-frame source the editor works on. The cropped, compressed result is
        //produced only on save - there is nothing to keep in C# until then.
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

            //The stage element only exists once a source has been picked, so the
            //editor is attached here rather than in the load handler - and only for
            //a source it is not already attached to.
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
                    _errorMessage = "Не получилось открыть редактор.";
                    DispatchStateHasChanged();
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
                // JS runtime may already be torn down - nothing to clean up for.
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
                // Editor went away between the drag and the callback.
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
            DispatchStateHasChanged();

            try
            {
                //The bytes that get uploaded are produced here, from whatever the
                //customer framed in the circle - not from the whole picture.
                var cropped = await JsInvokeAsync<CompressedImageResult>(
                    "avatarCropExport", MaxDimension, CompressionQuality);

                var bytes = ExtractBytesFromDataUrl(cropped.DataUrl);

                var success = AvatarService.Current != null
                    && await AvatarService.Current.UploadAsync(bytes, cropped.ContentType, default);

                if (success)
                {
                    _isBusy = false;
                    await CloseDialog();
                    return;
                }

                // AvatarService fills LastError in with the actual reason
                // (proxy unreachable at <url>, rejected with <status>, ...).
                // A bare "try later" left nobody - customer or operator -
                // able to tell a stopped proxy from bad credentials.
                _errorMessage = AvatarService.Current?.LastError ?? "Не получилось сохранить. Попробуйте позже.";
            }
            catch
            {
                _errorMessage = "Не получилось обрезать картинку.";
            }
            finally
            {
                _isBusy = false;
                DispatchStateHasChanged();
            }
        }

        #endregion

        #region HELPERS

        private async Task LoadSourceAsync(Func<ValueTask<AvatarSourceResult>> load)
        {
            _errorMessage = null;
            _isBusy = true;
            DispatchStateHasChanged();

            try
            {
                var result = await load();

                await InvokeVoidAsync("avatarCropDispose");

                _attachedSourceDataUrl = null;
                _sourceDataUrl = result.DataUrl;
            }
            catch
            {
                // Covers: bad/unreachable URL, CORS-blocked remote image,
                // corrupt file, unsupported format - all of these are
                // equally "couldn't use that image" from the customer's
                // point of view.
                _errorMessage = "Не получилось загрузить картинку.";
            }
            finally
            {
                _isBusy = false;
                DispatchStateHasChanged();
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
