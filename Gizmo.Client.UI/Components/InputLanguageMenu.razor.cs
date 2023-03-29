﻿using System.Globalization;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class InputLanguageMenu : CustomDOMComponentBase
    {
        [Inject]
        public CultureInputViewStateService CultureService { get; set; }

        [Inject]
        public CultureInputViewState ViewState { get; set; }

        private Task ValueChangedHandler(CultureInfo culture)
        {
            return CultureService.SetCurrentInputCultureAsync(culture.TwoLetterISOLanguageName);
        }

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
