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
            return ShowDialogAsync<CheckoutDialog>(new Dictionary<string, object>(), default, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowExecutableSelectorDialogAsync(int applicationId, CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<ExecutableSelectorDialog>(new Dictionary<string, object>() { { "ApplicationId", applicationId } }, new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
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

        public Task<ShowDialogResult<EmptyDialogResult>> ShowPaymentDialogAsync(PaymentDialogParameters paymentDialogParameters, CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<PaymentDialog>(paymentDialogParameters.ToDictionary(), default, default, cancellationToken);
        }

        public Task<ShowDialogResult<EmptyDialogResult>> ShowAdvertisementDialogAsync(AdvertisementViewState state, CancellationToken cancellationToken = default)
        {
            var stateParams = new Dictionary<string, object>()
            {
                {nameof(AdvertisementViewState), state}
            };
            return ShowDialogAsync<AdsCarouselItemDialog>(stateParams, new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
            }, default, cancellationToken);
        }
    }
}
