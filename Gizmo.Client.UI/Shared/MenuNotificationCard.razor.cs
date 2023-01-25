using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuNotificationCard : CustomDOMComponentBase
	{
		protected bool _shouldRender;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NotificationsService NotificationsService { get; set; }

        [Parameter]
        public NotificationViewState Notification { get; set; }

		#region OVERRIDES

		protected override void OnInitialized()
		{
			this.SubscribeChange(Notification); //TODO: A WE NEED TO UPDATE _shouldRender FROM SubscribeChange.
			base.OnInitialized();
		}

		public override void Dispose()
		{
			this.UnsubscribeChange(Notification);
			base.Dispose();
		}

		public override async Task SetParametersAsync(ParameterView parameters)
		{
			if (parameters.TryGetValue<NotificationViewState>(nameof(Notification), out var newNotification))
			{
				var notificationChanged = !EqualityComparer<NotificationViewState>.Default.Equals(Notification, newNotification);
				if (notificationChanged)
				{
					_shouldRender = true;
				}
			}

			await base.SetParametersAsync(parameters);
		}

		protected override bool ShouldRender()
		{
			return _shouldRender;
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (!firstRender)
			{
				_shouldRender = false;
				await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
			}
			else
			{
				await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
			}

			await base.OnAfterRenderAsync(firstRender);
		}

		#endregion
	}
}