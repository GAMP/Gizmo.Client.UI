using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public interface ISelect<TItemType>
    {
        void Register(ISelectItem<TItemType> selectItem, TItemType value);

        void UpdateItem(ISelectItem<TItemType> selectItem, TItemType value);

        void Unregister(ISelectItem<TItemType> selectItem, TItemType value);

        Task SetSelectedItem(ISelectItem<TItemType> selectItem);
    }
}
