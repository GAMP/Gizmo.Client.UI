using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.Components.Base
{
    /// <summary>
    /// Base class for page components with view model.
    /// </summary>
    /// <typeparam name="TModelType">View model type.</typeparam>
    public abstract class PageComponentWithModelBase<TModelType> : ComponentWithModelBase<TModelType> where TModelType : IPageViewModel
    {
        #region CONSTRUCTOR
        public PageComponentWithModelBase()
        {
        }
        #endregion
    }
}
