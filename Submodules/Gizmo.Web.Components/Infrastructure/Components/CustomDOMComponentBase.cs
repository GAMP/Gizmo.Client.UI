using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Base implementation for custom DOM components.
    /// </summary>
    public abstract class CustomDOMComponentBase : CustomComponentBase
    {
        #region CONSTRUCTOR
        protected CustomDOMComponentBase()
        {
        } 
        #endregion

        #region PRIVATE FIELDS

        private ElementReference _ref;

        #endregion

        #region PROPERTIES
        
        #region PUBLIC

        /// <summary>
        /// Gets or sets DOM element ref.
        /// </summary>
        public virtual ElementReference Ref
        {
            get { return _ref; }
            set
            {
                _ref = value;
                RefBack?.Set(value);
            }
        }

        /// <summary>
        /// Gets or sets DOM element id.
        /// </summary>
        [Parameter()]
        public string Id { get; set; } = ComponentIdGenerator.Generate();

        /// <summary>
        /// Gets or sets additional attributes that will be applied to created DOM element.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Gets or sets one or more classes for DOM element.
        /// </summary>
        [Parameter()]
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets inline style for DOM element.
        /// </summary>
        [Parameter()]
        public string Style { get; set; }

        #endregion

        #endregion
    }
}
