using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.Client.UI.Components;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    public sealed class ClientDialogService : DialogServiceBase, IClientDialogService
    {
        public ClientDialogService(IOptionsMonitor<DialogOptions> options,
            IServiceProvider serviceProvider,
            ILogger<ClientDialogService> logger) : base(options, serviceProvider, logger)
        {
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowCheckoutDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<CheckoutDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(UserAgreementDialogParameters userAgreementDialogParameters, bool allowContinueWithoutAccept = false, CancellationToken cancellationToken = default)
        {
            var parameters = userAgreementDialogParameters.ToDictionary();
            parameters["AllowContinueWithoutAccept"] = allowContinueWithoutAccept;

            return ShowDialogAsync<UserAgreementDialog, UserAgreementResult>(parameters, new DialogDisplayOptions()
            {
                Closable = false,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowChangeProfileDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangeProfileDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, default);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowChangePasswordDialogAsync(bool closable, CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangePasswordDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = closable,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowChangePictureDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangePictureDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowMediaDialogAsync(MediaDialogParameters mediaDialogParameters, CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<MediaDialog>(mediaDialogParameters.ToDictionary(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<AlertDialogResult>> ShowAlertDialogAsync(string title, string message, AlertDialogButtons buttons = AlertDialogButtons.OK, AlertTypes icon = AlertTypes.None, CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<AlertDialog, AlertDialogResult>(new Dictionary<string, object>()
            {
                { "Title", title },
                { "Message", message },
                { "Buttons", buttons },
                { "Icon", icon }
            }, new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowUserOnlineDepositsDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<UserOnlineDepositsDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowConfirmReservationDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ConfirmReservationDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<RegistrationAgreementsResult>> ShowRegistrationAgreementsDialogAsync(
            IReadOnlyList<RegistrationAgreement> agreements,
            CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<RegistrationAgreementsDialog, RegistrationAgreementsResult>(
                new Dictionary<string, object>
                {
                    { nameof(RegistrationAgreementsDialog.Agreements), agreements }
                },
                new DialogDisplayOptions { Closable = true, CloseOnClick = false },
                default, cancellationToken);
        }
    }
}
