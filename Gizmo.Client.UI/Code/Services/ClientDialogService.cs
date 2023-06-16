using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.Client.UI.Components;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
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

        public Task<AddDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(UserAgreementDialogParameters userAgreementDialogParameters, CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<UserAgreementDialog, UserAgreementResult>(userAgreementDialogParameters.ToDictionary(), new DialogDisplayOptions()
            {
                Closable = true,
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

        public Task<AddDialogResult<EmptyComponentResult>> ShowChangeEmailDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangeEmailDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, default);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowChangeMobileDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangeMobileDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowChangePasswordDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangePasswordDialog>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
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
    }
}
