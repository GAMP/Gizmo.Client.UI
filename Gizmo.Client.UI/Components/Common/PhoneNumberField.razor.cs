using Gizmo.Client.Options;
using Gizmo.Client.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public sealed record PhoneCountrySelection(string CountryName, string? RegionCode, string CallingCodeDigits);

    public partial class PhoneNumberField : CustomDOMComponentBase
    {
        private const string DEFAULT_NATIONAL_MASK = "###-###-####";

        private bool _isLoaded;
        private readonly Dictionary<string, string> _regionCodes = new();
        private readonly Dictionary<string, string?> _masks = new();
        private readonly Dictionary<string, int> _maxLengths = new();
        private List<IconSelectCountry> _countries = new();

        [Inject]
        IPhoneValidationService PhoneValidationService { get; set; }

        [Inject]
        IOptionsMonitor<ClientRegionalOptions> RegionalOptions { get; set; }

        [Parameter]
        public string? SelectedCountryName { get; set; }

        [Parameter]
        public string? PhoneValue { get; set; }

        /// <summary>
        /// Binding expression for the phone value, forwarded to the inner MaskedPhoneInput so its
        /// EditContext FieldIdentifier resolves to the page's phone field (e.g. ViewState.MobilePhone /
        /// ViewState.LoginName). Required for the phone input to display its own inline validation.
        /// </summary>
        [Parameter]
        public Expression<Func<string?>>? PhoneValueExpression { get; set; }

        [Parameter]
        public FieldIdentifier CountryFieldIdentifier { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public string CountryLabel { get; set; } = string.Empty;

        [Parameter]
        public string PhoneLabel { get; set; } = string.Empty;

        [Parameter]
        public string SearchPlaceholder { get; set; } = string.Empty;

        [Parameter]
        public InputSizes Size { get; set; } = InputSizes.Medium;

        [Parameter]
        public bool IsFullWidth { get; set; }

        /// <summary>
        /// When true (default), renders each input inside its own giz-registration-form-input /
        /// giz-form-input wrapper div, matching the Registration and PasswordRecovery page layout.
        /// Set to false for Login where both inputs share an outer giz-form-input div.
        /// </summary>
        [Parameter]
        public bool RenderWrappers { get; set; } = true;

        /// <summary>
        /// When true (default), shows the X clear button on the country select when a country is selected.
        /// Set to false for BasicFields where no clear button is needed.
        /// </summary>
        [Parameter]
        public bool CanClearCountry { get; set; } = true;

        [Parameter]
        public EventCallback<PhoneCountrySelection?> CountryChanged { get; set; }

        [Parameter]
        public EventCallback<string?> PhoneValueChanged { get; set; }

        private IconSelectCountry? GetSelectedCountry()
        {
            if (string.IsNullOrEmpty(SelectedCountryName))
                return null;
            return _countries.FirstOrDefault(c => c.Text == SelectedCountryName);
        }

        /// <summary>
        /// Builds the mask for the value layout actually stored by the phone input: calling-code digits
        /// followed by the national significant number. The server mask is generated from the national
        /// example (RU: "8 (912) 345-67-89" -> "# (###) ###-##-##"), so it can carry a trunk-prefix
        /// placeholder that is not part of the stored value, and it never covers the calling code.
        /// Both are corrected here, otherwise the '#' count caps input below the real number length
        /// (e.g. Greece: mask 10 digits, needed 2 + 10).
        /// </summary>
        private string GetMask()
        {
            var selected = GetSelectedCountry();

            var nationalMask = DEFAULT_NATIONAL_MASK;
            if (selected != null)
            {
                _masks.TryGetValue(selected.Text, out var serverMask);
                _maxLengths.TryGetValue(selected.Text, out var maxLength);
                nationalMask = ToNationalSignificantMask(serverMask, maxLength) ?? DEFAULT_NATIONAL_MASK;
            }

            var lockedLength = GetLockedPrefixLength();
            if (lockedLength == 0)
                return nationalMask;

            return new string('#', lockedLength) + " " + nationalMask;
        }

        /// <summary>
        /// Reduces the server mask to the national significant number, i.e. to exactly <paramref name="maxLength"/>
        /// digit placeholders, by dropping the leading trunk-prefix placeholders and the separators after them.
        /// Returns null when neither a mask nor a length is known.
        /// </summary>
        private static string? ToNationalSignificantMask(string? mask, int maxLength)
        {
            if (string.IsNullOrEmpty(mask))
                return maxLength > 0 ? new string('#', maxLength) : null;

            if (maxLength <= 0)
                return mask;

            var placeholders = mask.Count(c => c == '#');

            if (placeholders == maxLength)
                return mask;

            //The mask cannot hold the whole national number, so grouping is dropped in favour of capacity.
            if (placeholders < maxLength)
                return new string('#', maxLength);

            var extra = placeholders - maxLength;
            var index = 0;
            while (index < mask.Length && extra > 0)
            {
                if (mask[index] == '#')
                    extra -= 1;
                index += 1;
            }

            return mask.Substring(index).TrimStart(' ', '-', '.', '/');
        }

        private int GetLockedPrefixLength()
        {
            var selected = GetSelectedCountry();
            if (selected == null || string.IsNullOrEmpty(selected.PhonePrefix))
                return 0;
            return selected.PhonePrefix.Count(char.IsDigit);
        }

        private async Task OnCountrySelectedAsync(IconSelectCountry? value)
        {
            if (value == null)
            {
                await CountryChanged.InvokeAsync(null);
            }
            else
            {
                _regionCodes.TryGetValue(value.Text, out var regionCode);
                await CountryChanged.InvokeAsync(
                    new PhoneCountrySelection(value.Text, regionCode, ToCallingCodeDigits(value.PhonePrefix)));
            }
        }

        private Task OnClearCountryClickedAsync(MouseEventArgs args)
            => OnCountrySelectedAsync(null);

        // Local, offline flag asset by ISO 3166-1 alpha-2 region code (bundled via flag-icons in Phase 1).
        // The IconSelect/IconSelectListItem markup falls back to no-flag-image.svg via onerror when missing.
        private static string GetFlagPath(string? regionCode)
            => string.IsNullOrEmpty(regionCode)
                ? "_content/Gizmo.Client.UI/img/no-flag-image.svg"
                : $"_content/Gizmo.Client.UI/img/flags/{regionCode.ToLowerInvariant()}.svg";

        private static string ToCallingCodeDigits(string? callingCode)
        {
            var digits = callingCode ?? string.Empty;
            if (digits.StartsWith("+"))
                digits = digits[1..];
            return digits;
        }

        protected override async Task OnInitializedAsync()
        {
            var countries = await PhoneValidationService.GetCountriesAsync();

            foreach (var country in countries.OrderBy(c => c.CountryName))
            {
                _regionCodes[country.CountryName] = country.RegionCode;
                _masks[country.CountryName] = country.InputMask;
                _maxLengths[country.CountryName] = country.MaxLength;
                _countries.Add(new IconSelectCountry
                {
                    Text = country.CountryName,
                    PhonePrefix = country.CallingCode,
                    Icon = GetFlagPath(country.RegionCode)
                });
            }

            foreach (var item in _countries)
                item.Display = item.Text + " " + item.PhonePrefix;

            _isLoaded = true;
            await InvokeAsync(StateHasChanged);

            // Pre-select the server-configured country (from regional options) when nothing is selected yet.
            // Never overwrites a user-typed or state-restored selection. Wiring is identical to a manual
            // selection, so the page also gets the calling code seeded into the phone input.
            if (string.IsNullOrEmpty(SelectedCountryName))
            {
                var defaultRegionCode = RegionalOptions.CurrentValue?.CountryCode;
                if (!string.IsNullOrEmpty(defaultRegionCode))
                {
                    var match = countries.FirstOrDefault(c =>
                        string.Equals(c.RegionCode, defaultRegionCode, StringComparison.OrdinalIgnoreCase));

                    if (match != null)
                        await CountryChanged.InvokeAsync(
                            new PhoneCountrySelection(match.CountryName, match.RegionCode, ToCallingCodeDigits(match.CallingCode)));
                }
            }

            await base.OnInitializedAsync();
        }
    }
}
