using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
	public partial class ExecutableCard : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

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
			this.SubscribeChange(Executable);
			base.OnInitialized();
		}

		public override void Dispose()
		{
			this.UnsubscribeChange(Executable);
			base.Dispose();
		}

		//protected override async Task OnAfterRenderAsync(bool firstRender)
		//{
		//	if (!firstRender)
		//	{
		//		await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
		//	}
		//	else
		//	{
		//		await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
		//	}

		//	await base.OnAfterRenderAsync(firstRender);
		//}

		#endregion
	}
}