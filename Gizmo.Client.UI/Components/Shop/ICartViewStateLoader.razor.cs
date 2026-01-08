using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ICartViewStateLoader : CustomDOMComponentBase
    {
        #region Parameters

        [Parameter]
        public ICartViewState? ViewState { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public string? MinimumWidth { get; set; }

        [Parameter]
        public string? MinimumHeight { get; set; }

        #endregion

        #region ClassMappers

        protected string ClassName => new ClassMapper()
                 .Add(Class)
                 .Add("giz-wm-icart-loader")
                 .If("loading", () => ViewState == null || ViewState.IsStateUpdateRequired || ViewState.IsStateUpdating)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .Add(Style)
                 .If($"min-width: {MinimumWidth}", () => !string.IsNullOrEmpty(MinimumWidth))
                 .If($"min-height: {MinimumHeight}", () => !string.IsNullOrEmpty(MinimumHeight))
                 .AsString();

        #endregion
    }
}
