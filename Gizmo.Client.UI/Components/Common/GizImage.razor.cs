﻿using Gizmo.Client.UI.Services;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizImage : ComponentBase , IDisposable
    {
        #region CONSTRUCTOR
        public GizImage()
        { }
        #endregion

        #region PROPERTIES

        private ElementReference ElementReference
        {
            get;
            set;
        }

        [Inject()]
        private ImageService ImageService
        {
            get;
            init;
        }

        [Inject()]
        private IJSRuntime JS
        {
            get; init;
        }

        /// <summary>
        /// Gets or sets image type.
        /// </summary>
        [Parameter()]
        public ImageType ImageType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets image id.
        /// </summary>
        [Parameter()]
        public int ImageId
        {
            get; set;
        }

        #endregion

        #region FIELDS
        readonly CancellationTokenSource _cancellationTokenSource = new(); 
        #endregion

        #region OVERRIDES

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                try
                {
                    using (Stream imageStream = await ImageService.ImageStreamGetAsync(ImageType, ImageId, _cancellationTokenSource.Token))
                    using (var streamReference = new DotNetStreamReference(imageStream))
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
