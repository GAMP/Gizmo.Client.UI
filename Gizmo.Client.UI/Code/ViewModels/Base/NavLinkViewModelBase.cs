using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Code.ViewModels.Base
{
    /// <summary>
    /// Navigation link view model base.
    /// </summary>
    public abstract class NavLinkViewModelBase 
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets menu item title.
        /// </summary>
        public string Title
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets menu item tooltip.
        /// </summary>
        public string ToolTip
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets menu item class.
        /// </summary>
        public Icons IconClass
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets navlink class.
        /// </summary>
        public string Class
        {
            get; set;
        } = "nav-link d-flex align-center justify-start relative cursor-pointer";

        /// <summary>
        /// Gets or sets default route.
        /// </summary>
        public string DefaultRoute
        {
            get; set;
        }

        /// <summary>
        /// Gets navlink match.
        /// </summary>
        public NavLinkMatch Match
        {
            get; set;
        } = NavLinkMatch.All;

        #endregion
    }
}
