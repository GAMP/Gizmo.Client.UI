using Gizmo.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components.Base
{
    /// <summary>
    /// Base class for components with view model.
    /// </summary>
    /// <typeparam name="TModelType">View model type.</typeparam>
    public abstract class ComponentWithModelBase<TModelType> : ComponentBase where TModelType : IComponentViewModel
    {
        #region CONSTRUCTOR
        public ComponentWithModelBase()
        {
        }
        #endregion

        #region FIELDS
        TModelType model;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Default component logger.
        /// </summary>
        /// <remarks>
        /// The logger is used to produce debuggin output per each component type.
        /// </remarks>
        [Inject()]
        protected ILogger<ComponentWithModelBase<TModelType>> Logger
        {
            get;
            set;
        }

        #endregion

        #region PROPERTIES
        /// <summary>
        /// Injected view model instance.
        /// </summary>
        [Inject()]
        public virtual TModelType ViewModel
        {
            get { return model; }
            protected set
            {
                model = value;
            }
        }
        #endregion

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            //detach event handlers
            ViewModel.PropertyChanged -= OnViewModelPropetryChanged;

            //attach event handlers
            ViewModel.PropertyChanged += OnViewModelPropetryChanged;

            //initialize viwe model
            await ViewModel.InitializeAync();

            //initialize component
            await base.OnInitializedAsync();
        }

        #endregion        

        #region EVENT HANDLERS
        /// <summary>
        /// This methid will be called on a model property change.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">PropertyChanged eventArgs.</param>
        protected virtual void OnViewModelPropetryChanged(object sender, PropertyChangedEventArgs e)
        {
        }
        #endregion

        #region IDisposable
        protected virtual void Dispose()
        {
            //detach any event handlers
            ViewModel.PropertyChanged -= OnViewModelPropetryChanged;
        }
        #endregion
    }
}
