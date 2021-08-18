using Gizmo.Shared.Shared.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Client localization service.
    /// </summary>
    public class UILocalizationService : LocalizationServiceBase
    {
        #region CONSTRUCTOR
        public UILocalizationService(ILogger<UILocalizationService> logger, IStringLocalizer<Resources> localizer) : base(logger, localizer)
        {
        } 
        #endregion
    }
}
