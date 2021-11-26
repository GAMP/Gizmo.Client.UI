using Gizmo.Client.UI.Code;
using Gizmo.Client.UI.Components;
using Gizmo.Shared.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_SHOP)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_SHOP_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_SHOP_DESCRIPTION"), ModuleDisplayOrder(5)]
    [Route("/validation")]
    public partial class ValidationForm : ComponentBase
    {
        public ValidationForm()
        {
            _culture = new CultureInfo("en-US", false);
        }

        CultureInfo _culture;

        [Inject]
        private ILocalizationService _localizationService { get; set; }

        public ValidationModel Model { get; set; } = new ValidationModel();

        public ValidationModel Sample1Model { get; set; } = new ValidationModel();

        public ValidationModel Sample2Model { get; set; } = new ValidationModel();
      
        private CustomValidation? _customValidation;

        private void HandleValidSubmit()
        {
            _customValidation?.ClearErrors();

            var errors = new Dictionary<string, List<string>>();

            var maximumBirthDate = new DateTime(2003, 01, 01);

            if (Sample1Model.BirthDate > maximumBirthDate)
            {
                string fieldName = "";
                string maxDateError = "The date must be lesser than {0}";

                try
                {
                    fieldName = _localizationService.GetString("FIELD_BIRTH_DATE");
                    maxDateError = _localizationService.GetString("VALIDATION_ERROR_MAX_DATE");
                }
                catch (Exception ex)
                {
                    //GetString should not pass _DEFAULT_ARGS to get the translations as is. We need it as is for the ParsingErrorMessage to work.
                }

                errors.Add(nameof(ValidationModel.BirthDate),
                    new()
                    {
                        string.Format(maxDateError, maximumBirthDate)
                    });
            }

            if (errors.Any())
            {
                _customValidation?.DisplayErrors(errors);
            }
            else
            {
                // Process the valid form
            }
        }
    }

    public class ValidationModel
    {
        [StringLength(2, ErrorMessageResourceName = "MODULE_PAGE_APPS_TITLE",
            ErrorMessageResourceType = typeof(Resources.Properties.Resources))]
        [Required(ErrorMessageResourceName = "VALIDATION_ERROR_REQUIRED_FIELD",
            ErrorMessageResourceType = typeof(Resources.Properties.Resources))]
        public string Email { get; set; }

        public DateTime BirthDate { get; set; } = DateTime.Now;

        [Required(ErrorMessageResourceName = "VALIDATION_ERROR_REQUIRED_FIELD",
            ErrorMessageResourceType = typeof(Resources.Properties.Resources))]
        public decimal Amount { get; set; } = 10000.42m;
    }
}