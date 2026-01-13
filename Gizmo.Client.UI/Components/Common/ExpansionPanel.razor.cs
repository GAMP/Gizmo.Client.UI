using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExpansionPanel : CustomDOMComponentBase, IAsyncDisposable
    {
        #region CONSTRUCTOR
        public ExpansionPanel()
        {
        }
        #endregion

        protected bool _shouldRender;

        #region PROPERTIES

        [Parameter]
        public RenderFragment ExpansionPanelHeader { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsCollapsed { get; set; }

        [Parameter]
        public EventCallback<bool> IsCollapsedChanged { get; set; }

        #endregion

        //protected async Task OnKeyDownHandler(KeyboardEventArgs args)
        //{
        //    if (args.Key == null)
        //        return;

        //    if (args.Key == "Enter")
        //    {
        //        await InvokeVoidAsync("expansionPanelToggle", Ref);
        //    }
        //}

        protected async Task OnClickHeader()
        {
            await InvokeVoidAsync("expansionPanelToggle", Ref);
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-expansion-panel")
                 .AsString();

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("registerExpansionPanel", Ref);
                ExpansionPanelEventInterop = new ExpansionPanelEventInterop(JsRuntime);
                await ExpansionPanelEventInterop.SetupExpansionPanelEventCallback(args => ExpansionPanelHandler(args));
            }
            else
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private ExpansionPanelEventInterop ExpansionPanelEventInterop { get; set; }

        private async Task ExpansionPanelHandler(ExpansionPanelEventArgs args)
        {
            if (args.Id == Id)
            {
                if (IsCollapsed != args.IsCollapsed)
                {
                    IsCollapsed = args.IsCollapsed;
                    await IsCollapsedChanged.InvokeAsync(IsCollapsed);

                    _shouldRender = true;
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        public override void Dispose()
        {
            ExpansionPanelEventInterop?.Dispose();

            base.Dispose();
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterExpansionPanel", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}
