using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderGlobalSearch : CustomDOMComponentBase
    {
        const int DEFAULT_DELAY = 500;

        public HeaderGlobalSearch()
        {
            _deferredAction = new DeferredAction(Search);
            _delayTimeSpan = new TimeSpan(0, 0, 0, 0, _delay);

            ProductResults = new List<SearchResultViewModel>();
            ApplicationResults = new List<SearchResultViewModel>();
        }

        #region FIELDS

        private DeferredAction _deferredAction;
        private int _delay = DEFAULT_DELAY;
        private TimeSpan _delayTimeSpan;
        private string _text;
        private bool _openDropDown;
        private bool _loading;
        private int _resultApplications;
        private int _resultProducts;

        #endregion

        #region PROPERTIES

        [Inject]
        ApplicationsPageService ApplicationsPageService { get; set; }

        [Inject]
        ShopPageService ShopService { get; set; }

        [Parameter]
        public int MinimumCharacters { get; set; } = 1;

        public List<SearchResultViewModel> ApplicationResults { get; set; }

        public List<SearchResultViewModel> ProductResults { get; set; }

        #endregion

        #region EVENTS

        protected Task OnFocusInHandler()
        {
            if (!string.IsNullOrEmpty(_text))
                _openDropDown = true;

            return Task.CompletedTask;
        }

        protected Task OnFocusOutHandler()
        {
            _openDropDown = false;

            return Task.CompletedTask;
        }

        protected Task OnInputHandler(ChangeEventArgs args)
        {
            var newValue = args?.Value as string;

            if (newValue != _text)
            {
                _text = newValue;

                if (!_openDropDown)
                {
                    _openDropDown = true;
                }

                if (MinimumCharacters == 0 || (MinimumCharacters > 0 && _text.Length >= MinimumCharacters))
                {
                    _deferredAction.Defer(_delayTimeSpan);
                }
            }

            return Task.CompletedTask;
        }

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        private void Clear()
        {
            ApplicationResults.Clear();
            ProductResults.Clear();
            _resultApplications = 0;
            _resultProducts = 0;
            _text = string.Empty;
            _loading = false;
            _openDropDown = false;
        }

        #endregion

        private async Task Search()
        {
            ApplicationResults.Clear();
            ProductResults.Clear();
            _resultApplications = 0;
            _resultProducts = 0;

            _loading = true;

            StateHasChanged();

            await Task.Delay(500);

            foreach (var app in ApplicationsPageService.ViewState.Applications.Where(a => a.Title.Contains(_text, StringComparison.InvariantCultureIgnoreCase)))
            {
                ApplicationResults.Add(new SearchResultViewModel() { Type = SearchResultTypes.Application, Id = app.Id, Name = app.Title, Image = app.Image });
                _resultApplications += 1;
            }

            foreach (var product in ShopService.ViewState.Products.Where(a => a.Name.Contains(_text, StringComparison.InvariantCultureIgnoreCase)))
            {
                ProductResults.Add(new SearchResultViewModel() { Type = SearchResultTypes.Product, Id = product.Id, Name = product.Name, Image = product.Image });
                _resultProducts += 1;
            }

            _loading = false;

            StateHasChanged();
        }

        #region CLASSMAPPERS

        protected string CloseButtonStyleValue => new StyleMapper()
                 .If($"visibility: hidden", () => string.IsNullOrEmpty(_text))
                 .AsString();

        #endregion

    }
}