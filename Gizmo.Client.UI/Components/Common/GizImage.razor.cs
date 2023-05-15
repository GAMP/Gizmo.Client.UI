using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.UI;
using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
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

        /// <summary>
        /// This placeholder is shown when image is loading.
        /// If not set, default loading placeholder is shown.
        /// </summary>
        [Parameter]
        public RenderFragment LoadingPlaceholder { get; set; }

        /// <summary>
        /// This placeholder is shown when image is not found or ImageId is null.
        /// If not set, default empty result placeholder is shown.
        /// </summary>
        [Parameter]
        public RenderFragment EmptyResultPlaceholder { get; set; }

        /// <summary>
        /// This placeholder is shown when was an error while loading image.
        /// If not set, default error placeholder is shown.
        /// </summary>
        [Parameter]
        public RenderFragment ErrorPlaceholder { get; set; }

        [Parameter]
        public ImageFitType ImageFitType { get; set; } = ImageFitType.Fill;

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

        #region CLASSMAPPERS

        protected string ImageClassName => new ClassMapper()
                 .Add($"giz-image--{ImageFitType.ToDescriptionString()}")
                 .AsString();

        #endregion

    }
}
