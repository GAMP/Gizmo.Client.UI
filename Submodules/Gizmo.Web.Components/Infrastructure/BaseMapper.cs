using System;
using System.Collections.Generic;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Base mapper class.
    /// </summary>
    public abstract class BaseMapper
    {
        #region FIELDS
        public List<Func<string>> _items;
        #endregion

        #region PROPERTIES

        #region PROTECTED
        
        /// <summary>
        /// Gets items.
        /// </summary>
        protected List<Func<string>> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<Func<string>>();
                return _items;
            }
        }  

        #endregion

        #endregion

        #region FUNCTIONS

        public void Add(Func<string> name)
        {
            Items.Add(name);
        }

        #endregion

        #region ABSTRACT FUNCTIONS

        /// <summary>
        /// Returns string representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public abstract string AsString();

        #endregion

        #region OVERRIDES
        
        public override string ToString() => AsString(); 

        #endregion
    }

    public static class BaseMapperExtensions
    {
        public static T Add<T>(this T m, string name) where T : BaseMapper
        {
            m.Add(() => name);
            return m;
        }

        public static T Get<T>(this T m, Func<string> funcName) where T : BaseMapper
        {
            m.Add(funcName);
            return m;
        }

        public static T GetIf<T>(this T m, Func<string> funcName, Func<bool> func) where T : BaseMapper
        {
            m.Add(() => func() ? funcName() : null);
            return m;
        }

        public static T If<T>(this T m, string name, Func<bool> func) where T : BaseMapper
        {
            m.Add(() => func() ? name : null);
            return m;
        }
    }
}
