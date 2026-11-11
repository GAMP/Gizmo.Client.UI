using System.Globalization;
using System.Threading.Tasks;

using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class UserBan : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        UserBanViewState ViewState { get; set; }

        [Inject]
        UserViewService UserService { get; set; }

        protected string SignOutIn => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_STATE_BANNED_SIGN_OUT_IN,
            ViewState.Time.ToString(@"mm\:ss", CultureInfo.InvariantCulture));

        private Task SignOutAsync() => UserService.LogoutAsync();

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
