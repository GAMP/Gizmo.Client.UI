using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
	public partial class MenuActiveApplicationCard : CustomDOMComponentBase
	{
		protected bool _shouldRender;

		[Inject]
		public ActiveApplicationsService ActiveApplicationsService { get; set; }

		[Parameter]
		public ExecutableViewState Executable { get; set; }

		private Task OnClickMainButtonHandler()
		{
			switch (Executable.State)
			{
				case View.ExecutableState.None:
					return ActiveApplicationsService.RunExecutableAsyc(Executable.Id);

				default:
					return ActiveApplicationsService.TerminateExecutableAsyc(Executable.Id);
			}
		}

		#region OVERRIDES

		protected override void OnInitialized()
		{
			this.SubscribeChange(Executable); //TODO: A WE NEED TO UPDATE _shouldRender FROM SubscribeChange.
			base.OnInitialized();
		}

		public override void Dispose()
		{
			this.UnsubscribeChange(Executable);
			base.Dispose();
		}

		public override async Task SetParametersAsync(ParameterView parameters)
		{
			if (parameters.TryGetValue<ExecutableViewState>(nameof(Executable), out var newExecutable))
			{
				var executableChanged = !EqualityComparer<ExecutableViewState>.Default.Equals(Executable, newExecutable);
				if (executableChanged)
				{
					_shouldRender = true;
				}
			}

			await base.SetParametersAsync(parameters);
		}

		protected override bool ShouldRender()
		{
			return true; //TODO: A _shouldRender
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