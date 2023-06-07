#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Client notification service.
    /// </summary>
    public sealed class ClientNotificationService : NotificationsServiceBase , IClientNotificationService
    {
        public ClientNotificationService(IOptionsMonitor<NotificationsOptions> options,
            IServiceProvider serviceProvider,
            ILogger<ClientNotificationService> logger) :base(options,serviceProvider,logger)
        {
        }

        public Task<AddNotificationResult<EmptyComponentResult>>  ShowAlertNotification(AlertTypes alertTypes,
            string title, 
            string message,
            NotificationDisplayOptions? displayOptions ,
            NotificationAddOptions? addOptions,
            CancellationToken cancellationToken =default)
        {
            return ShowNotificationAsync<GizNotification>(new Dictionary<string, object>() 
            {
                {"Icon",alertTypes },
                {"Title",title}, 
                {"Message",message},

            }, displayOptions, addOptions, cancellationToken);
        }
    }  
}
