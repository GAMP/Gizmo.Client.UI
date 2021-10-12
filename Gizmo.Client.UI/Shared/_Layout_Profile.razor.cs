using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System;

namespace Gizmo.Client.UI.Shared
{
    public partial class _Layout_Profile : LayoutComponentBase
    {
        public _Layout_Profile()
        {
            User = new UserViewModel();
            User.Username = "Infidel 06";
            User.RegistrationDate = new DateTime(2020, 3, 4);
            User.Balance = 10.76m;
            User.Time = new TimeSpan(6, 36, 59);
            User.Points = 416;
            User.Picture = "_content/Gizmo.Client.UI/img/Cyber Punks.png";
            //User.Picture = "_content/Gizmo.Client.UI/img/Avatar-1.png";
        }

        public UserViewModel User { get; set; }
    }
}