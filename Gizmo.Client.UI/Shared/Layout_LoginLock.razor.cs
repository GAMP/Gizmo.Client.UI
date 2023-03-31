﻿using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class Layout_LoginLock : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject()]
        HostUserLockViewStateService HostUserLockService { get; set; }

        [Inject]
        HostUserLockViewState ViewState { get; set; }

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
