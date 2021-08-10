using Gizmo.Client.UI.Code.ViewModels.Page;
using Gizmo.Client.UI.Components.Base;


namespace Gizmo.Client.UI
{
    public partial class App //: ComponentWithModelBase<AppPageViewModel>
    {
        #region CONSTRUTCOR
        public App() : base()
        {
        }
        #endregion

        #region PROPERTIES

        ///// <summary>
        ///// Gets navigation manager.
        ///// </summary>
        //[Inject()]
        //private NavigationManager NavigationManager { get; set; }



        #endregion

        #region OVERRIDES

        //protected override async Task OnInitializedAsync()
        //{
        //    //detach state change handler
        //   // AuthService.SessionStateChanged -= OnSessionStateChanged;

        //    //attach state change handler
        //   // AuthService.SessionStateChanged += OnSessionStateChanged;

        //    //continue initialization
        //    await base.OnInitializedAsync();

        //    //validate current authentication state
        //    //await AuthService.DetermineStateAsync();
        //}

        #endregion

        #region EVENT HANDLERS

        //private void OnSessionStateChanged(object sender, UserSessionChangeArgs args)
        //{
        //    if (args.State == UserSessionState.Authenticated)
        //    {
        //        NavigationManager.NavigateTo("./");
        //    }
        //    else
        //    {
        //        NavigationManager.NavigateTo("./login");
        //    }
        //}

        #endregion

        #region IDisposable

        //protected override void Dispose()
        //{
        //    AuthService.SessionStateChanged -= OnSessionStateChanged;
        //    GC.SuppressFinalize(this);
        //    base.Dispose();
        //}

        #endregion
    }
}
