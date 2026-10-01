using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Shared
{
    public partial class TopBarNews : ShellComponentBase
    {
        private const int NEWS_CEILING = 30;
        private const int FIRST_OPEN_DELAY_MS = 1700;

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        IOptions<ClientHomeOptions> ClientHomeOptions { get; set; }

        [Inject]
        AdvertisementsViewState AdvertisementsViewState { get; set; }

        private bool _open;
        private bool _unseen = true;
        private bool _announced;
        private int _index;

        private List<AdvertisementViewState> _news = new();

        private List<AdvertisementViewState> News => _news;

        private void ReadNews() => _news = AdvertisementsViewState.Advertisements
            .Take(NEWS_CEILING)
            .ToList();

        private int Count => News.Count;

        private bool IsShown => ClientHomeOptions.Value.Disabled && Count > 0;

        private AdvertisementViewState Current => Count > 0 ? News[_index % Count] : null;

        private IEnumerable<AdvertisementViewState> Others => News.Where(item => item.Id != Current?.Id);

        private string PositionText => $"{(_index % Math.Max(1, Count)) + 1} / {Count}";

        private string Label => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_NEWS_TITLE);

        private string TooltipText => _open ? null : Label;

        private string ButtonClass
        {
            get
            {
                var classes = "giz-header__user-menu-item giz-news-button";

                if (_open)
                    classes += " giz-news-button--open";

                if (_unseen)
                    classes += " giz-news-button--unseen";

                return classes;
            }
        }

        private void Toggle()
        {
            _open = !_open;
            _unseen = false;
        }

        private void Close() => _open = false;

        private void Previous() => _index = (_index - 1 + Count) % Count;

        private void Next() => _index = (_index + 1) % Count;

        private void Select(int id)
        {
            var position = News.FindIndex(item => item.Id == id);

            if (position >= 0)
                _index = position;
        }

        private void AnnounceOnce()
        {
            if (_announced || !IsShown)
                return;

            _announced = true;

            DispatchWorkflow(async () =>
            {
                await Task.Delay(FIRST_OPEN_DELAY_MS);

                if (!IsShown)
                    return;

                _open = true;
                _unseen = false;
                StateHasChanged();
            });
        }

        private void OnNewsChanged(object sender, EventArgs e)
        {
            ReadNews();
            AnnounceOnce();
        }

        protected override void OnInitialized()
        {
            ReadNews();
            AdvertisementsViewState.OnChange += OnNewsChanged;
            this.SubscribeChange(AdvertisementsViewState);

            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
                AnnounceOnce();

            base.OnAfterRender(firstRender);
        }

        public override void Dispose()
        {
            AdvertisementsViewState.OnChange -= OnNewsChanged;
            this.UnsubscribeChange(AdvertisementsViewState);

            base.Dispose();
        }
    }
}
