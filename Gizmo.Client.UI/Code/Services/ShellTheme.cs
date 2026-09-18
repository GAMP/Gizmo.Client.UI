using System;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// The accent palette the shell is running in, shared between the two Blazor roots.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The palette is chosen on the page: the <c>data-accent</c> attribute on
    /// <c>&lt;html&gt;</c>, written by <c>grafitTheme</c> in internal.js from the skin's
    /// index.html or from a club's stylesheet (<c>--gg-palette</c>). The notifications
    /// window is a separate WebView with its own document and no club stylesheet, so it
    /// cannot find the word by itself; the main window publishes what it resolved here
    /// and the notifications host applies it. Both roots share this process, which is
    /// what makes a static holder the right place - the same pattern as
    /// <see cref="ShellActivity"/>.
    /// </para>
    /// <para>
    /// Static rather than a service: registering another service in the skin's
    /// composition is one more thing that can fail to load.
    /// </para>
    /// </remarks>
    public static class ShellTheme
    {
        private static string _accent;

        /// <summary>
        /// The palette the main window resolved, or null before it has.
        /// </summary>
        public static string Accent => _accent;

        /// <summary>
        /// Raised on the thread that resolved the palette. Subscribers that render must
        /// marshal to their own dispatcher.
        /// </summary>
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
