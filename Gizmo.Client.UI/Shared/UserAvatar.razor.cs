using System;
using System.Threading.Tasks;

using Gizmo.Client.UI.Services;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class UserAvatar : ShellComponentBase
    {
        [Inject]
        IClientDialogService DialogService { get; set; }

        /// <summary>The picture the account carries, if the client knows one.</summary>
        [Parameter]
        public string Picture { get; set; }

        //`Class` comes from CustomDOMComponentBase; declaring it again here made
        //Blazor refuse the component outright ("more than one parameter matching 'class'").

        /// <summary>
        /// Whether this avatar is the signed-in customer's own and big enough to offer
        /// the editor. Does nothing unless the club runs the picture service.
        /// </summary>
        [Parameter]
        public bool Editable { get; set; }

        /// <summary>
        /// What to draw: the account's own picture, or the one the club's service holds.
        /// </summary>
        protected string Shown => string.IsNullOrEmpty(Picture) ? AvatarService.Current?.Picture : Picture;

        protected override void OnInitialized()
        {
            if (AvatarService.Current is not null)
                AvatarService.Current.Changed += OnAvatarChanged;

            base.OnInitialized();
        }

        public override void Dispose()
        {
            if (AvatarService.Current is not null)
                AvatarService.Current.Changed -= OnAvatarChanged;

            base.Dispose();
        }

        //The picture arrives on the service's own thread.
        private void OnAvatarChanged() => DispatchRender();

        private Task OpenEditor() => DialogService.ShowChangePictureDialogAsync();
    }
}
