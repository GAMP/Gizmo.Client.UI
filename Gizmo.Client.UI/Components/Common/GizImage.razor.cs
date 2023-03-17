using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class GizImage : ComponentBase, IDisposable
    {
        #region PROPERTIES

        private ElementReference ElementReference { get; set; }

        [Inject]
        private IImageService ImageService { get; init; }

        [Inject]
        private IJSRuntime JS { get; init; }

        /// <summary>
        /// Gets or sets image type.
        /// </summary>
        [Parameter]
        public ImageType ImageType { get; set; }

        /// <summary>
        /// Gets or sets image id.
        /// </summary>
        [Parameter]
        public int? ImageId { get; set; }

        [Parameter]
        public RenderFragment LoadingPlaceholder { get; set; }
        [Parameter]
        public RenderFragment EmptyResultPlaceholder { get; set; }
        [Parameter]
        public RenderFragment ErrorPlaceholder { get; set; }

        #endregion

        #region FIELDS
        /// <summary>
        /// 0 - Loading
        /// 1 - EmptyResult
        /// 2 - Error
        /// </summary>
        private byte _imageResultStatusCode;
        private ImageType _previousImageType;
        private int _previousImageId;
        readonly CancellationTokenSource _cancellationTokenSource = new();

        #endregion

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            if (!ImageId.HasValue)
            {
                _imageResultStatusCode = 1;

                await InvokeAsync(StateHasChanged);

                return;
            }

            var imageTypeChanged = _previousImageType != ImageType;
            var imageIdChanged = _previousImageId != ImageId.Value;

            if (imageTypeChanged || imageIdChanged)
            {
                _imageResultStatusCode = 0;

                _previousImageType = ImageType;
                _previousImageId = ImageId.Value;

                try
                {
                    using var imageStream = await ImageService.ImageStreamGetAsync(ImageType, ImageId.Value, _cancellationTokenSource.Token);

                    if (imageStream is null || imageStream == Stream.Null)
                    {
                        _imageResultStatusCode = 1;

                        await InvokeAsync(StateHasChanged);

                        return;
                    }

                    using var streamReference = new DotNetStreamReference(imageStream);

                    await JS.InvokeVoidAsync("ClientFunctions.SetImageSourceAsync", ElementReference, streamReference);
                }
                catch (Exception)
                {
                    _imageResultStatusCode = 2;
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
        #endregion
    }
}
