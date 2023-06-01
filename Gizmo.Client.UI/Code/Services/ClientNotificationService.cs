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
        public ClientNotificationService(INotificationsHost notificationsHostWindow, IServiceProvider serviceProvider, ILogger<ClientNotificationService> logger) :base(serviceProvider,logger)
        {
            _notificationsHostWindow = notificationsHostWindow;
            _ =_notificationsHostWindow.ShowAsync();
        }       

        private readonly INotificationsHost _notificationsHostWindow;
    }

  
}
