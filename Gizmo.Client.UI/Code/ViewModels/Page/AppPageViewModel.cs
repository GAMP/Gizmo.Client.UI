using Gizmo.Client.UI.Code.Component;
using Gizmo.Shared.ViewModels;
using Gizmo.Web.Components;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Code.ViewModels.Page
{
    /// <summary>
    /// App page view model.
    /// </summary>
    public class AppPageViewModel : PageViewModelBase
    {
        #region CONSTRUCTOR
        public AppPageViewModel()
        {
        }
        #endregion

        #region FILEDS
        private HashSet<MainMenuNavLinkViewModel> _navLinks;
        private bool isExpanded;
        private bool isLightTheme = true;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets main navigation link models.
        /// </summary>
        public HashSet<MainMenuNavLinkViewModel> NavLinks
        {
            get
            {
                if (_navLinks == null)
                    _navLinks = new HashSet<MainMenuNavLinkViewModel>();
                return _navLinks;
            }
        }

        public bool IsMenuExpanded
        {
            get { return isExpanded; }
            set
            {
                SetProperty(ref isExpanded, value);
            }
        }

        public bool IsLightTheme
        {
            get { return isLightTheme; }
            set { SetProperty(ref isLightTheme, value); }
        }

        #endregion

        #region OVERRIDES

        protected override Task OnInitializeAync(CancellationToken ct = default)
        {
            NavLinks.Clear();

            NavLinks.Add(new MainMenuNavLinkViewModel()
            {
                DefaultRoute = "./",
                IconClass = Icons.Sales,
                Title = "Home",
                //RequiredPolicy = null
            });
            NavLinks.Add(new MainMenuNavLinkViewModel()
            {
                DefaultRoute = "./games",
                IconClass = Icons.Monitor,
                Title = "Games",
                //RequiredPolicy = null
            });
            NavLinks.Add(new MainMenuNavLinkViewModel()
            {
                DefaultRoute = "./sales",
                IconClass = Icons.Sales,
                Title = "POS",
                // RequiredPolicy = null
            });
            NavLinks.Add(new MainMenuNavLinkViewModel()
            {
                DefaultRoute = "./apps",
                IconClass = Icons.Users,
                Title = "Apps",
                //RequiredPolicy = null
            });
            NavLinks.Add(new MainMenuNavLinkViewModel()
            {
                DefaultRoute = "./shop",
                IconClass = Icons.WaitingLines,
                Title = "Shop",
                // RequiredPolicy = null
            });

            return base.OnInitializeAync(ct);
        }

        #endregion
    }
}
