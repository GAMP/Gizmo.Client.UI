using System.Collections.Generic;
using System.Linq;

using Gizmo.UI.View.States;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public abstract class CustomVirtualizedDOMComponentBase<T> : CustomDOMComponentBase where T : class, IViewState
    {
        private const int DefaultColumnsCount = 6;

        protected string _gridColumnsStyle;

        [Parameter]
        public int ColumnsCount { get; set; }

        [Parameter]
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        protected override void OnParametersSet()
        {
            var resolved = ColumnsCount < 2 || ColumnsCount > 10 ? DefaultColumnsCount : ColumnsCount;

            _gridColumnsStyle = $"grid-template-columns: repeat({resolved}, 1fr)";

            base.OnParametersSet();
        }
    }
}
