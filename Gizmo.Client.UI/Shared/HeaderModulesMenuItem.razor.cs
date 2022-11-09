using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using System;

namespace Gizmo.Client.UI
{
    /// <summary>
    /// Page module display item template.
    /// </summary>
    public partial class HeaderModulesMenuItem : ComponentBase
    {
        #region FIELDS
        private UIPageModuleMetadata _metaData; 
        #endregion  

        #region PROPERTIES

        [Inject()]
        private ILocalizationService LocalizationService { get;set; }
        
        /// <summary>
        /// Gets or sets template component type.
        /// </summary>
        [Parameter()]
        public Type TemplateType { get; set; }

        /// <summary>
        /// Gets or sets module metadata.
        /// </summary>
        [Parameter()]
        public UIPageModuleMetadata MetaData
        {
            get { return _metaData; }
            set { _metaData = value; }
        }

        #endregion
    }
}
