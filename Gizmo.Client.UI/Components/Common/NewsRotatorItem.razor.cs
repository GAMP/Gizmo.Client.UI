#nullable enable

using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class NewsRotatorItem : CustomDOMComponentBase
    {
        private bool _isFadingOut;
        private bool _isFadingIn;

        private string? _image = null;
        private string? _title = null;
        private string? _summary = null;
        private string? _url = null;
        private string? _channelImage = null;

        [Inject]
        IJSRuntime? JSRuntime { get; set; }

        [Parameter]
        public string? Image { get; set; }

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public string? Summary { get; set; }

        [Parameter]
        public string? Url { get; set; }

        [Parameter]
        public string? ChannelImage { get; set; }

        public async Task OnClickHandle()
        {
            if (!string.IsNullOrEmpty(_url))
                await JSRuntime.InvokeVoidAsync("open", _url);
        }

        #region OVERRIDE

        protected override async Task OnParametersSetAsync()
        {
            var imageChanged = _image != Image;
            var titleChanged = _title != Title;
            var summaryChanged = _summary != Summary;
            var urlChanged = _url != Url;
            var channelImageChanged = _channelImage != ChannelImage;

            if (imageChanged || titleChanged || summaryChanged || urlChanged || channelImageChanged)
            {
                if (!string.IsNullOrEmpty(_image) || !string.IsNullOrEmpty(_title) || !string.IsNullOrEmpty(_summary) || !string.IsNullOrEmpty(_url) || !string.IsNullOrEmpty(_channelImage)) //Do not fade out on initialization.
                {
                    _isFadingOut = true;
                    _isFadingIn = false;

                    await InvokeAsync(StateHasChanged);

                    await Task.Delay(1000); //Fade out time.
                }

                _image = Image;
                _title = Title;
                _summary = Summary;
                _url = Url;
                _channelImage = ChannelImage;

                _isFadingOut = false;
                _isFadingIn = true;

                await InvokeAsync(StateHasChanged);
            }

            await base.OnParametersSetAsync();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-news-rotator-item")
                .If("giz-news-rotator-item--current fade-out", () => _isFadingOut)
                .If("giz-news-rotator-item--next fade-in", () => _isFadingIn)
                .AsString();

        #endregion
    }
}
