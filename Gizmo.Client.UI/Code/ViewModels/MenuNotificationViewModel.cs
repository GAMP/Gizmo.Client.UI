﻿using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class MenuNotificationViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Time { get; set; }
    }
}