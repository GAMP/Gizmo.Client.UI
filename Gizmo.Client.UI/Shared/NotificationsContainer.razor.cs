﻿using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class NotificationsContainer : CustomDOMComponentBase
    {
        public NotificationsContainer()
        {
        }

        [Inject]
        NotificationsService NotificationsService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(NotificationsService.ViewState);
            base.OnInitialized();
        }
    }
}