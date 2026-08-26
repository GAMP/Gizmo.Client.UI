using Gizmo.Client.UI.Services;
using Gizmo.Web.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenuUserAvatar : CustomDOMComponentBase
    {
        protected string Picture => AvatarService.Current?.Picture;

        protected override void OnInitialized()
        {
            if (AvatarService.Current != null)
                AvatarService.Current.Changed += OnAvatarChanged;

            base.OnInitialized();
        }

        private void OnAvatarChanged()
        {
            DispatchStateHasChanged();
        }

        public override void Dispose()
        {
            if (AvatarService.Current != null)
                AvatarService.Current.Changed -= OnAvatarChanged;

            base.Dispose();
        }
    }
}
