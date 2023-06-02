using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Client notification service.
    /// </summary>
    public sealed class ClientNotificationService : NotificationsServiceBase , IClientNotificationService
    {
        public ClientNotificationService(INotificationsHost notificationsHost,
            IServiceProvider serviceProvider, ILogger<ClientNotificationService> logger) :base(notificationsHost, serviceProvider,logger)
        {
        }

        public Task<AddNotificationResult<EmptyComponentResult>>  ShowAlertNotification(int alertTypes,
            string title, 
            string message,
            CancellationToken cancellationToken =default)
        {
            return ShowNotificationAsync<GizNotification>(new Dictionary<string, object>() 
            {
                {"Icon",alertTypes },
                {"Title",title}, 
                {"Message",message},

            }, default, default, cancellationToken);
        }
    }  
}
