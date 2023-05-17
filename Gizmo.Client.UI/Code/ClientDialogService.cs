using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    public sealed class ClientDialogService : DialogServiceBase, IClientDialogService
    {
        public ClientDialogService(IServiceProvider serviceProvider, ILogger<ClientDialogService> logger) : base(serviceProvider, logger)
        {
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowCheckoutDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<CheckoutDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<ShowDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(UserAgreementDialogParameters userAgreementDialogParameters, CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<UserAgreementDialog, UserAgreementResult>(userAgreementDialogParameters.ToDictionary(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowChangeProfileDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangeProfileDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, default);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowChangeEmailDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangeEmailDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, default);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowChangeMobileDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangeMobileDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowChangePasswordDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangePasswordDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowChangePictureDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangePictureDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowMediaDialogAsync(MediaDialogParameters mediaDialogParameters, CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<MediaDialog>(mediaDialogParameters.ToDictionary(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
            }, default, cancellationToken);
        }

        public Task<ShowDialogResult<AlertDialogResult>> ShowAlertDialogAsync(string title, string message, AlertDialogButtons buttons = AlertDialogButtons.OK, AlertDialogIcons icon = AlertDialogIcons.None, CancellationToken cancellationToken = default)
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
    }
}
