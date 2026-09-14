using System;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Client;
using Gizmo.UI;
using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    /// <summary>
    /// An image served by the host's image service, with the loading / empty / error
    /// placeholders the markup supplies.
    /// </summary>
    /// <remarks>
    /// The host answers an image request only while it is connected and a user is signed
    /// in: it asks the server for the image's hash before it will use its own cache. A
    /// request made during a dropped connection therefore fails or hangs, and every image
    /// on screen that happened to be re-requested at that moment (a changed view state
    /// re-keys its image) stayed a placeholder for the rest of the session. So: a load that
    /// has not answered in <see cref="LoadTimeout"/> shows the error placeholder instead of
    /// a shimmer, and any image in an error state is requested again when the client
    /// reconnects or a user signs in.
    /// </remarks>
    public partial class GizImage : CustomDOMComponentBase
    {
        /// <summary>
        /// How long a load may stay unanswered before the error placeholder replaces the
        /// loading one. The request itself is left running; a late answer still lands.
        /// </summary>
        private static readonly TimeSpan LoadTimeout = TimeSpan.FromSeconds(15);

        #region PROPERTIES

        [Inject]
        private IImageService ImageService { get; init; }

        [Inject]
        private IGizmoClient GizmoClient { get; init; }

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
        /// 3 - Success
        /// </summary>
        private int _imageResultStatusCode;
        private ImageType _previousImageType;
        private int? _previousImageId;
        private string _imageSource;
        readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _loaded;

        //Serial number of the latest load: an answer from an earlier one (a retry started
        //while it was still running) must not overwrite a newer result.
        private int _loadSerial;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            if (GizmoClient != null)
            {
                GizmoClient.ConnectionStateChange += OnConnectionStateChange;
                GizmoClient.LoginStateChange += OnLoginStateChange;
            }

            base.OnInitialized();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var imageTypeChanged = _previousImageType != ImageType;
            var imageIdChanged = _previousImageId != ImageId;

            if (imageTypeChanged || imageIdChanged || !_loaded)
            {
                _loaded = true;

                await LoadAsync();
            }
        }

        public override void Dispose()
        {
            if (GizmoClient != null)
            {
                GizmoClient.ConnectionStateChange -= OnConnectionStateChange;
                GizmoClient.LoginStateChange -= OnLoginStateChange;
            }

            _cancellationTokenSource.Cancel();

            base.Dispose();
        }

        #endregion

        #region EVENTS

        //Both arrive from the client's own threads; the retry touches component state, so
        //it goes through the renderer (never async void - see CustomComponentBase).
        private void OnConnectionStateChange(object sender, ConnectionStateEventArgs e)
        {
            if (e.IsConnected)
                RetryIfFailed();
        }

        private void OnLoginStateChange(object sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoggedIn)
                RetryIfFailed();
        }

        private void RetryIfFailed()
        {
            //Only a failed load is worth repeating: a picture that arrived stays, and a
            //load still in flight will answer on its own now that the connection is back.
            if (_imageResultStatusCode != 2 || !ImageId.HasValue)
                return;

            DispatchWorkflow(LoadAsync);
        }

        #endregion

        #region HELPERS

        /// <summary>
        /// Re-renders unless the component is already gone: a load answering after
        /// disposal (the page was left) must not throw into the renderer.
        /// </summary>
        private async Task RenderAsync()
        {
            if (IsDisposed)
                return;

            try
            {
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException or OperationCanceledException)
            {
            }
        }

        private async Task LoadAsync()
        {
            if (!ImageId.HasValue)
            {
                _imageResultStatusCode = 1;

                await RenderAsync();

                return;
            }

            var serial = ++_loadSerial;

            _imageResultStatusCode = 0;

            _previousImageType = ImageType;
            _previousImageId = ImageId.Value;

            try
            {
                var load = ImageService.ImageSourceGetAsync(ImageType, ImageId.Value, _cancellationTokenSource.Token).AsTask();

                //A request the host cannot answer (connection dropped between the ask and
                //the reply) would leave a shimmer on screen for good. After the timeout
                //the error placeholder takes over and a reconnect asks again; should the
                //original request answer after all, its picture is still shown.
                var first = await Task.WhenAny(load, Task.Delay(LoadTimeout, _cancellationTokenSource.Token));
                if (first != load && serial == _loadSerial)
                {
                    _imageResultStatusCode = 2;
                    await RenderAsync();
                }

                var source = await load;

                if (serial != _loadSerial)
                    return;

                _imageSource = source;
                _imageResultStatusCode = source == null ? 2 : string.IsNullOrEmpty(source) ? 1 : 3;

                await RenderAsync();
            }
            catch (OperationCanceledException)
            {
                //we have cancelled loading, this only happens on dispose so no extra action is needed
                //in order to render any component change
                if (serial != _loadSerial)
                    return;

                _imageResultStatusCode = 2;
                await RenderAsync();
            }
            catch (Exception)
            {
                if (serial != _loadSerial)
                    return;

                _imageResultStatusCode = 2;
                await RenderAsync();
            }
        }

        #endregion

        #region CLASSMAPPERS

        protected string ImageClassName => new ClassMapper()
            .Add($"giz-image--{ImageFitType.ToDescriptionString()}")
            .AsString();

        #endregion
    }
}
