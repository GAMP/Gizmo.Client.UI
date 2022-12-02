using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizDockItem : CustomDOMComponentBase
    {
        private bool _scaled;
        private bool _halfScaled;

        private bool _isOpen;

        [CascadingParameter]
        protected GizDock Parent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public void OnMouseOverHandler(MouseEventArgs args)
        {
            if (Parent != null)
            {
                Parent.Activate(this);
            }
        }

        public void OnMouseOutHandler(MouseEventArgs args)
        {
            if (Parent != null)
            {
                Parent.Deactivate(this);
            }
        }

        internal void Scale()
        {
            _scaled = true;

            _isOpen = true;
        }

        internal void HalfScale()
        {
            _halfScaled = true;

            StateHasChanged();
        }

        internal void Reset(bool changeState = false)
        {
            _isOpen = false;
            _scaled = false;
            _halfScaled = false;

            if (changeState)
                StateHasChanged();
        }

        #region OVERRIDE

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnInitialized()
        {
            if (Parent != null)
            {
                Parent.Register(this);
            }
        }

        public override void Dispose()
        {
            try
            {
                if (Parent != null)
                {
                    Parent.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-dock-item")
                .If("scaled", () => _scaled)
                .If("half-scaled", () => _halfScaled)
                .AsString();

        #endregion
    }
}