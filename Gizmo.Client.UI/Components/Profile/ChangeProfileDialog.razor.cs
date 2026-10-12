using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangeProfileDialog : CustomDOMComponentBase
    {
        private string _selectedPhoneCountryName;
        private bool _isClosing;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        IPhoneValidationService PhoneValidationService { get; set; }

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

        private Task OnPhoneCountryChangedAsync(PhoneCountrySelection selection)
        {
            _selectedPhoneCountryName = selection?.CountryName;

            if (selection == null)
            {
                UserChangeProfileViewStateService.SetPhoneRegionCode(null);
                UserChangeProfileViewStateService.SetMobilePhone(null);
            }
            else
            {
                UserChangeProfileViewStateService.SetPhoneRegionCode(selection.RegionCode);
                UserChangeProfileViewStateService.SetMobilePhone(selection.CallingCodeDigits);
            }

            return Task.CompletedTask;
        }

        private Task OnPhoneValueChangedAsync(string value)
        {
            UserChangeProfileViewStateService.SetMobilePhone(value);
            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            if (ViewState.ShowMobilePhone && !string.IsNullOrEmpty(ViewState.PhoneRegionCode))
            {
                var countries = await PhoneValidationService.GetCountriesAsync();
                _selectedPhoneCountryName = countries.FirstOrDefault(country => country.RegionCode == ViewState.PhoneRegionCode)?.CountryName;
            }

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (ViewState.IsComplete && !_isClosing)
            {
                _isClosing = true;
                await ResultCallback.InvokeAsync();
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
