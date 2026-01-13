using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public interface ISelectItem<TItemType>
    {
        ListItem ListItem { get; set; }

        TItemType Value { get; set; }

        string Text { get; set; }

        RenderFragment ChildContent { get; set; }
    }
}
