using System.Globalization;

namespace Gizmo.Client.UI
{
    public class Converter<T, U>
    {
        public bool InvariantDecimalPoint { get; set; }

        public CultureInfo Culture { get; set; } = CultureInfo.CurrentUICulture;

        public string Format { get; set; } = null;

        public bool HasGetError { get; set; }

        public string GetErrorMessage { get; set; }

        public bool HasSetError { get; set; }

        public string SetErrorMessage { get; set; }

        protected void ClearGetError()
        {
            HasGetError = false;
            GetErrorMessage = string.Empty;
        }

        protected void ClearSetError()
        {
            HasSetError = false;
            SetErrorMessage = string.Empty;
        }

        protected void UpdateGetError(string message)
        {
            HasGetError = true;
            GetErrorMessage = message;
        }

        protected void UpdateSetError(string message)
        {
            HasSetError = true;
            SetErrorMessage = message;
        }
    }
}
