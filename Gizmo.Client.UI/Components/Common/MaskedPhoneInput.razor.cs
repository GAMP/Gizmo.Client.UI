using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class MaskedPhoneInput<TValue> : MaskedNumericInputBase<TValue>
    {
        #region CONSTRUCTOR
        public MaskedPhoneInput()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public string Prefix { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-masked-phone-input")
                 .Add("giz-text-input")
                 .If("giz-text-input--full-width", () => IsFullWidth)
                 .Add(Class)
                 .AsString();

        #endregion

    }
}
