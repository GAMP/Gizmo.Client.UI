using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Gizmo.Client.UI.Components
{
    public class GizInputBase<TValue> : CustomDOMComponentBase
    {
        #region FIELDS

        //private Guid _guid = Guid.NewGuid();
        private Expression<Func<TValue>> _lastValueExpression;
        protected FieldIdentifier _fieldIdentifier;
        protected EditContext _lastEditContext;
        protected bool _isValid = true;
        protected string _validationMessage;

        #endregion

        /// <summary>
        /// Get EditContext of parent EditForm.
        /// </summary>
        [CascadingParameter]
        protected EditContext EditContext { get; set; } = default!;

        #region PROPERTIES

        /// <summary>
        /// Binding value expression.
        /// </summary>
        [Parameter]
        public Expression<Func<TValue>> ValueExpression { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        /// <summary>
        /// Gets or sets if input is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets if input is read only.
        /// </summary>
        [Parameter]
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets if input is hidden.
        /// </summary>
        [Parameter]
        public bool IsHidden { get; set; }

        #endregion

        #region OVERRIDES

        protected override void OnParametersSet()
        {
            //If the ValueExpression has changed, then update filed identifier.
            if (ValueExpression != null && ValueExpression != _lastValueExpression)
            {
                _fieldIdentifier = FieldIdentifier.Create(ValueExpression);
                _lastValueExpression = ValueExpression;
            }

            //If the EditContext has changed, then update validation handlers.
            if (EditContext != _lastEditContext)
            {
                RemoveValidationHandlers();

                if (EditContext != null)
                {
                    EditContext.OnValidationRequested += OnValidationRequested;
                    EditContext.OnValidationStateChanged += OnValidationStateChanged;
                    _lastEditContext = EditContext;
                }
            }

            base.OnParametersSet();
        }

        #endregion

        #region EVENTS

        private void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
        {
            Validate();
        }

        private void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            if (EditContext != null && !_fieldIdentifier.Equals(default(FieldIdentifier)))
            {
                var errors = EditContext.GetValidationMessages(_fieldIdentifier).ToList();
                if (errors.Count > 0)
                {
                    _isValid = false;
                    _validationMessage = String.Join(" ", errors);
                }
                else
                {
                    _isValid = true;
                    _validationMessage = null;
                }
                InvokeAsync(StateHasChanged);
            }
        }

        #endregion

        #region METHODS

        private void RemoveValidationHandlers()
        {
            if (_lastEditContext != null)
            {
                _lastEditContext.OnValidationRequested -= OnValidationRequested;
                _lastEditContext.OnValidationStateChanged -= OnValidationStateChanged;

                ClearValidationMessageStore();
            }
        }

        protected void NotifyFieldChanged()
        {
            if (EditContext != null && !_fieldIdentifier.Equals(default(FieldIdentifier)))
            {
                EditContext.NotifyFieldChanged(_fieldIdentifier);
            }
        }

        #endregion

        public override void Dispose()
        {
            RemoveValidationHandlers();

            base.Dispose();
        }

        public virtual void Validate()
        {

        }

        protected virtual void ClearValidationMessageStore()
        {

        }
    }
}
