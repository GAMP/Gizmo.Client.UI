using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangeProfileDialog : CustomDOMComponentBase
    {
        private bool _countriesLoaded;
        private CountryInfo _defaultCountry = null;
        private bool _isLoaded;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        CountryInformationService CountryInformationService { get; set; }

        [Inject]
        UserChangeProfileViewService UserChangeProfileViewStateService { get; set; }

        [Inject]
        UserChangeProfileViewState ViewState { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<EmptyComponentResult> ResultCallback { get; set; }

        public List<IconSelectCountry> Countries { get; set; } = new List<IconSelectCountry>();

        public void OnClickClearValueButtonHandler(MouseEventArgs args)
        {
            SetSelectedCountry(Countries.Where(a => a.PhonePrefix == "+").FirstOrDefault());
        }

        public IconSelectCountry GetSelectedCountry()
        {
            if (string.IsNullOrEmpty(ViewState.Country))
            {
                return null;
            }
            else
            {
                return Countries.Where(a => a.Text == ViewState.Country).FirstOrDefault();
            }
        }

        protected void SetSelectedCountry(IconSelectCountry value)
        {
            if (value == null)
            {
                UserChangeProfileViewStateService.SetCountry(null);
                UserChangeProfileViewStateService.SetPrefix(null);
            }
            else
            {
                UserChangeProfileViewStateService.SetCountry(value.Text);
                UserChangeProfileViewStateService.SetPrefix(value.PhonePrefix);
            }
        }

        private async Task CloseDialog()
        {
            await ResultCallback.InvokeAsync();
        }

        private void LoadUserCountry()
        {
            if (ViewState.IsInitialized == true && _countriesLoaded)
            {
                var userCountry = Countries.Where(a => a.Text == ViewState.Country).FirstOrDefault();
                if (userCountry != null)
                {
                    SetSelectedCountry(userCountry);
                }
                else
                {
                    IconSelectCountry defaultItem = null;

                    if (_defaultCountry != null && _defaultCountry.CallingCodeSuffixes.Count() > 0)
                    {
                        defaultItem = Countries.Where(a => a.PhonePrefix == _defaultCountry.CallingCodeRoot + _defaultCountry.CallingCodeSuffixes.First()).FirstOrDefault();
                    }

                    if (defaultItem == null)
                    {
                        var other = Countries.Where(a => a.Text == "Other").FirstOrDefault();
                        defaultItem = other;
                    }

                    SetSelectedCountry(defaultItem);
                }
                _isLoaded = true;
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            var countries = await CountryInformationService.GetCountryInfoAsync();

            foreach (var country in countries)
            {
                foreach (var suffix in country.CallingCodeSuffixes)
                {
                    Countries.Add(new IconSelectCountry()
                    {
                        Text = country.NativeName,
                        PhonePrefix = country.CallingCodeRoot + suffix,
                        Icon = country.FlagSvg
                    });
                }
            }

            var other = new IconSelectCountry()
            {
                Text = "Other",
                PhonePrefix = "+",
                Icon = "_content/Gizmo.Client.UI/img/no-flag-image.svg"
            };

            Countries.Add(other);

            foreach (var item in Countries)
            {
                item.Display = item.Text + " " + item.PhonePrefix;
            }

            //Render the list first.
            await InvokeAsync(StateHasChanged);

            //_defaultCountry = await CountryInformationService.GetCurrentCountryInfoAsync();

            LoadUserCountry();

            _countriesLoaded = true;

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_isLoaded)
            {
                LoadUserCountry();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
