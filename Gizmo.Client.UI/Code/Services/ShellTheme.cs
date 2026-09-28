using System;

namespace Gizmo.Client.UI.Services
{
    public static class ShellTheme
    {
        private static string _accent;

        public static string Accent => _accent;

        public static event Action Changed;

        public static void Set(string accent)
        {
            if (string.IsNullOrWhiteSpace(accent) || string.Equals(_accent, accent, StringComparison.Ordinal))
                return;

            _accent = accent;
            Changed?.Invoke();
        }
    }
}
