using System;
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
    }  
}
