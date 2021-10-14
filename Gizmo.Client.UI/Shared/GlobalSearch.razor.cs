﻿using Gizmo.Client.UI.Enumerations;
using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class GlobalSearch : CustomDOMComponentBase
    {
        const int DEFAULT_DELAY = 500;

        public GlobalSearch()
        {
            _deferredAction = new DeferredAction(Search);
            _delayTimeSpan = new TimeSpan(0, 0, 0, 0, _delay);

            Random random = new Random();

            Applications = new List<ApplicationViewModel>();
            Applications.Add(new ApplicationViewModel() { Id = 1, Name = "Grand Theft Auto IV", Image = "Gta-5.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 2, Name = "Cyberpunk 2077", Image = "Cyber Punks.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 3, Name = "Fortnite", Image = "Fortnite.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 4, Name = "Minecraft", Image = "Minecraft.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 5, Name = "League of Legends", Image = "League.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 6, Name = "Steam", Image = "Steam.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 7, Name = "Epic", Image = "Epic-games.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 8, Name = "Valorant", Image = "Valorant.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 9, Name = "Apex Legends", Image = "Apex.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 10, Name = "Windows Apps", Image = "Window-apps.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });

            Products = new List<ProductViewModel>();
            Products.Add(new ProductViewModel() { Id = 1, Name = "Mars Bar", Image = "Cola.png" });
            Products.Add(new ProductViewModel() { Id = 2, Name = "Snickers Bar", Image = "Cola.png" });
            Products.Add(new ProductViewModel() { Id = 3, Name = "Pizza (Small)", Image = "Cola.png" });
            Products.Add(new ProductViewModel() { Id = 4, Name = "Pizza and Cola", Image = "Cola.png" });
            Products.Add(new ProductViewModel() { Id = 5, Name = "Coca Cola (Can)", Image = "Cola.png" });
            Products.Add(new ProductViewModel() { Id = 6, Name = "Six Hours (6)", Image = "Cola.png" });
            Products.Add(new ProductViewModel() { Id = 7, Name = "Six Hours (6 Weekends)", Image = "Cola.png" });

            Results = new List<SearchResultViewModel>();
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

        [Parameter]
        public int MinimumCharacters { get; set; } = 1;

        public List<ApplicationViewModel> Applications { get; set; }

        public List<ProductViewModel> Products { get; set; }

        public List<SearchResultViewModel> Results { get; set; }

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
            Results.Clear();
            _resultApplications = 0;
            _resultProducts = 0;
            _text = string.Empty;
            _loading = false;
            _openDropDown = false;
        }

        #endregion

        private async Task Search()
        {
            Results.Clear();
            _resultApplications = 0;
            _resultProducts = 0;

            _loading = true;

            StateHasChanged();

            await Task.Delay(500);

            foreach (var app in Applications.Where(a => a.Name.Contains(_text, StringComparison.InvariantCultureIgnoreCase)))
            {
                Results.Add(new SearchResultViewModel() { Type = SearchResultTypes.Application, Id = app.Id, Name = app.Name, Image = app.Image });
                _resultApplications += 1;
            }

            foreach (var product in Products.Where(a => a.Name.Contains(_text, StringComparison.InvariantCultureIgnoreCase)))
            {
                Results.Add(new SearchResultViewModel() { Type = SearchResultTypes.Product, Id = product.Id, Name = product.Name, Image = product.Image });
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