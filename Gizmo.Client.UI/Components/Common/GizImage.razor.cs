using System;
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
        public int ImageId { get; set; }

        [Parameter]
        public RenderFragment Placeholder { get; set; }

        #endregion

        #region FIELDS

        private ImageType _previousImageType;
        private int _previousImageId;
        readonly CancellationTokenSource _cancellationTokenSource = new();

        #endregion

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var imageTypeChanged = _previousImageType != ImageType;
            var imageIdChanged = _previousImageId != ImageId;

            if (imageTypeChanged || imageIdChanged)
            {
                _previousImageType = ImageType;
                _previousImageId = ImageId;

                try
                {
                    using var imageStream = await ImageService.ImageStreamGetAsync(ImageType, ImageId, _cancellationTokenSource.Token);

                    using var streamReference = new DotNetStreamReference(imageStream);

                    await JS.InvokeVoidAsync("ClientFunctions.SetImageSourceAsync", ElementReference, streamReference);
                }
                catch (OperationCanceledException)
                {
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
