using System;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// HTML element id generator.
    /// </summary>
    /// <remarks>
    /// The class is used for generating unique id for HTML dom elements.
    /// </remarks>
    public static class ComponentIdGenerator
    {
        #region PRIVATE CONSTANTS
        private const string ID_PREFIX = "component_id_";
        #endregion

        #region FUNCTIONS
        
        /// <summary>
        /// Generates new html element id with default prefix.
        /// </summary>
        /// <returns>New HTML element id.</returns>
        public static string Generate()
        {
            return Generate(ID_PREFIX);
        }

        /// <summary>
        /// Generates new html element id.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string Generate(string prefix)
        {
            return prefix + Guid.NewGuid();
        } 

        #endregion
    }
}
