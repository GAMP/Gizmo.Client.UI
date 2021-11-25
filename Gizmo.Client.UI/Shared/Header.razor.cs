﻿using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using System;

namespace Gizmo.Client.UI.Shared
{
    public partial class Header : CustomDOMComponentBase
    {
        public Header()
        {
            User = new UserViewModel();
            User.Username = "Infidel 06";
            User.RegistrationDate = new DateTime(2020, 3, 4);
            User.Balance = 10.76m;
            User.CurrentTimeProduct = "Six Hours (6) for 10$ Pack";
            User.Time = new TimeSpan(6, 36, 59);
            User.Points = 416;
            User.Picture = "_content/Gizmo.Client.UI/img/Cyber Punks.png";
            //User.Picture = "_content/Gizmo.Client.UI/img/Avatar-1.png";
        }

        private bool _isOpen;

        public UserViewModel User { get; set; }

        public void ToggleActiveApps()
        {
            _isOpen = !_isOpen;
        }
    }
}