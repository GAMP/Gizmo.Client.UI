using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderUserMenuItem : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }
        
        [Inject]
        public UserMenuViewState UserMenuViewState { get; set; }
        
        [Inject]
        private IServiceProvider ServiceProvider { get; set; }
        
        [Inject]
        private ILogger<HeaderUserMenuItem> Logger { get; set; }
        
        [Inject]
        private IClientDialogService DialogService { get; set; }
        
        [Parameter]
        public UIUserMenuModuleMetadata MetaData { get; set; }

        private async Task HandleClickAsync()
        {
            var dialogType = MetaData?.DialogItemType;
            if (dialogType == null || DialogService == null)
                return;

            var genericDefinitions = DialogService.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == "ShowCustomDialogAsync" && m.IsGenericMethodDefinition)
                .ToArray();

            if (genericDefinitions.Length == 0)
                throw new InvalidOperationException("No generic overload of ShowCustomDialogAsync<TDialog>() was found on the service type");

            var oneArgsDefinition = genericDefinitions.FirstOrDefault(m => m.GetParameters().Length == 1);
            if (oneArgsDefinition == null)
                throw new InvalidOperationException("No suitable overload of ShowCustomDialogAsync<TDialog>() was found for the call");

            object arg = CancellationToken.None;

            var closed = oneArgsDefinition.MakeGenericMethod(dialogType);
            var taskObject = closed.Invoke(DialogService, new[] { arg });

            if (taskObject is not Task task)
                return;

            await task;

            var taskType = taskObject!.GetType();
            var resultProp = taskType.GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
            var dialogRef = resultProp?.GetValue(taskObject);
            if (dialogRef == null)
                return;

            var dialogRefType = dialogRef.GetType();
            var resultField = dialogRefType.GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
            var addComponentResult = resultField?.GetValue(dialogRef);

            if (addComponentResult?.ToString() == nameof(AddComponentResultCode.Opened))
            {
                var waitMethod = dialogRefType.GetMethod("WaitForResultAsync", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (waitMethod != null)
                {
                    var waitTask = (Task)waitMethod.Invoke(dialogRef, Array.Empty<object>());
                    if (waitTask != null)
                    {
                        await waitTask;
                    }
                }
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserMenuViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserMenuViewState);

            base.Dispose();
        }
    }
}
