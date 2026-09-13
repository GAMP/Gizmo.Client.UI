using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public class ForwardRef<T>
    {
        #region FIELDS
        private T _current;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets current reference.
        /// </summary>
        public T Current
        {
            get { return _current; }
        }

        #endregion

        #region FUNCTIONS

        /// <summary>
        /// Sets current reference value.
        /// </summary>
        /// <param name="value">Reference value.</param>
        public void Set(T value)
        {
            _current = value;
        } 
        
        #endregion
    }

    public class ForwardRef :ForwardRef<ElementReference>
    {
    }
}
