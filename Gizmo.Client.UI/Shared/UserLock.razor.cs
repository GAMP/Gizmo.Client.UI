using System;
using System.Globalization;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Shared
{
    public partial class UserLock : CustomDOMComponentBase
    {
        private const int PIN_LENGTH = 4;

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        UserLockViewService UserLockService { get; set; }

        [Inject]
        UserLockViewState ViewState { get; set; }

        [Inject]
        UserViewState UserViewState { get; set; }

        [Inject]
        UserBalanceViewState UserBalanceViewState { get; set; }

        private ElementReference _root;
        private bool _wasShown;

        private bool IsShown => ViewState.IsLocking || ViewState.IsLocked;

        private bool IsErrorShown => ViewState.IsLocked && !string.IsNullOrEmpty(ViewState.Error) && ViewState.InputPassword.Length == PIN_LENGTH;

        private bool IsPinIncomplete => ViewState.InputPassword.Length < PIN_LENGTH;

        private string LockModifier => ViewState.IsLocking
            ? "giz-lock--set"
            : IsErrorShown ? "giz-lock--error" : null;

        private string HeadTitle => ViewState.IsLocking
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_LOCK_SET_TITLE)
            : UserViewState.Username ?? string.Empty;

        private string HeadSubtitle => GrafitLocalization.GetString(ViewState.IsLocking
            ? GrafitResourceKeys.SHELL_LOCK_SET_HINT
            : GrafitResourceKeys.SHELL_LOCK_LOCKED);

        private bool HasTimeLeft => UserBalanceViewState.Time.HasValue;

        private TimeSpan TimeLeft => UserBalanceViewState.Time is TimeSpan time && time > TimeSpan.Zero ? time : TimeSpan.Zero;

        private int HoursLeft => (int)TimeLeft.TotalHours;

        private string MinutesLeft => HoursLeft > 0
            ? TimeLeft.Minutes.ToString("00", CultureInfo.InvariantCulture)
            : TimeLeft.Minutes.ToString(CultureInfo.InvariantCulture);

        private string EnoughUntilText => DateTime.Now.Add(TimeLeft).ToString("t", CultureInfo.CurrentCulture);

        private string DotClass(int index)
        {
            var length = ViewState.InputPassword.Length;

            if (index < length)
                return "giz-lock__dot giz-lock__dot--on";

            return index == length ? "giz-lock__dot giz-lock__dot--next" : "giz-lock__dot";
        }

        private async Task PressDigitAsync(int digit)
        {
            if (ViewState.InputPassword.Length >= PIN_LENGTH)
                UserLockService.SetInputPassword(string.Empty);

            await UserLockService.PutDigitAsync(digit);

            if (ViewState.IsLocked && ViewState.InputPassword.Length == PIN_LENGTH)
                await UserLockService.UnlockAsync();
        }

        private Task OnKeyDown(KeyboardEventArgs args)
        {
            if (args.Key.Length == 1 && char.IsDigit(args.Key[0]))
                return PressDigitAsync(args.Key[0] - '0');

            return args.Key switch
            {
                "Backspace" => UserLockService.DeleteDigitAsync(),
                "Enter" when !IsPinIncomplete => ViewState.IsLocking ? UserLockService.SetPasswordAsync() : UserLockService.UnlockAsync(),
                "Escape" when ViewState.IsLocking => UserLockService.CancelLockAsync(),
                _ => Task.CompletedTask,
            };
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserBalanceViewState);

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsShown && !_wasShown)
                await _root.FocusAsync();

            _wasShown = IsShown;

            await base.OnAfterRenderAsync(firstRender);
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(UserBalanceViewState);

            base.Dispose();
        }
    }
}
