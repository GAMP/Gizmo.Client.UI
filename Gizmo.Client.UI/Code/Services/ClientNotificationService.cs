using System;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Client notification service.
    /// </summary>
    public sealed class ClientNotificationService : NotificationsServiceBase , IClientNotificationService
    {
        public ClientNotificationService(IServiceProvider serviceProvider, ILogger<ClientNotificationService> logger) :base(serviceProvider,logger)
        {
        }
    }
}
