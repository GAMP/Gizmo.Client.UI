using Gizmo.Client.UI.Components;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Services
{
    public sealed class ClientDialogService : DialogServiceBase, IClientDialogService
    {
        public ClientDialogService(IServiceProvider serviceProvider, ILogger<ClientDialogService> logger) : base(serviceProvider, logger)
        {
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowCheckoutDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<PaymentMethodSelectorDialog>(new Dictionary<string, object>(), default, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowExecutableSelectorDialogAsync(int applicationId, CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ExecutableSelectorDialog>(new Dictionary<string, object>() { { "ApplicationId", applicationId } }, default, default, cancellationToken);
        }

        public Task<ShowDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<UserAgreementDialog, UserAgreementResult>(new Dictionary<string, object>(), default, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowTopUpDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<TopUpDialog>(new Dictionary<string, object>(), default, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowChangeEmailDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangeEmailDialog>(new Dictionary<string, object>(), default, default, default);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowChangeMobileDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangeMobileDialog>(new Dictionary<string, object>(), default, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowChangePasswordDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangePasswordDialog>(new Dictionary<string, object>(), default, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowChangePictureDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ChangePictureDialog>(new Dictionary<string, object>(), default, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowPaymentDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<PaymentDialog>(new Dictionary<string, object>(), default, default, cancellationToken);
        }
    }
}
