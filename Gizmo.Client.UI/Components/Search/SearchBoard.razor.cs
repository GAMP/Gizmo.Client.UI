using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class SearchBoard : CustomDOMComponentBase
    {
        private const int APPS_SHOWN = 6;
        private const int PRODUCTS_SHOWN = 10;
        private const int VENDOR_PAGE = 10;

        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        AppExeViewStateLookupService AppExeViewStateLookupService { get; set; }

        [Inject]
        GlobalSearchViewService GlobalSearchService { get; set; }

        [Parameter]
        public IReadOnlyList<GlobalSearchResultViewState> Executables { get; set; } = Array.Empty<GlobalSearchResultViewState>();

        [Parameter]
        public IReadOnlyList<GlobalSearchResultViewState> Products { get; set; } = Array.Empty<GlobalSearchResultViewState>();

        [Parameter]
        public string Pattern { get; set; }

        protected sealed record SearchApp(int ApplicationId, IReadOnlyList<int> ExecutableIds);

        private IReadOnlyList<SearchApp> _apps = Array.Empty<SearchApp>();

        private string _appsKey;

        protected SearchApp BestApp => _apps.FirstOrDefault();

        protected int? BestProductId => BestApp is null && Products.Count > 0 ? Products[0].Id : null;

        protected IReadOnlyList<SearchApp> RestApps => _apps.Skip(1).Take(APPS_SHOWN).ToList();

        protected IReadOnlyList<int> RestProducts => Products
            .Skip(BestApp is null ? 1 : 0)
            .Take(PRODUCTS_SHOWN)
            .Select(a => a.Id)
            .ToList();

        protected bool HasRest => RestApps.Count > 0 || RestProducts.Count > 0;

        protected string BoardClass => HasRest ? "giz-search-board" : "giz-search-board giz-search-board--single";

        protected bool MoreExecutables => Executables.Count > VENDOR_PAGE;

        protected bool MoreProducts => Products.Count > VENDOR_PAGE;

        private Task ShowAllExecutables() => GlobalSearchService.ViewAllResultsAsync(SearchResultTypes.Executables);

        private Task ShowAllProducts() => GlobalSearchService.ViewAllResultsAsync(SearchResultTypes.Products);

        protected override async Task OnParametersSetAsync()
        {
            var key = string.Join(",", Executables.Select(a => a.Id));

            if (key != _appsKey)
            {
                _appsKey = key;

                var apps = await GroupByAppAsync(Executables);

                if (key == _appsKey)
                    _apps = apps;
            }

            await base.OnParametersSetAsync();
        }

        private async Task<IReadOnlyList<SearchApp>> GroupByAppAsync(IReadOnlyList<GlobalSearchResultViewState> executables)
        {
            var order = new List<int>();
            var groups = new Dictionary<int, List<int>>();

            foreach (var result in executables)
            {
                var exe = await AppExeViewStateLookupService.GetStateAsync(result.Id);

                if (exe is null)
                    continue;

                if (!groups.TryGetValue(exe.ApplicationId, out var ids))
                {
                    ids = new List<int>();
                    groups[exe.ApplicationId] = ids;
                    order.Add(exe.ApplicationId);
                }

                ids.Add(result.Id);
            }

            return order.Select(a => new SearchApp(a, groups[a])).ToList();
        }
    }
}
