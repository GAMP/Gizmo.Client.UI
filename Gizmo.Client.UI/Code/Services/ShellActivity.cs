using System;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Whether the shell is worth spending work on right now.
    /// </summary>
    /// <remarks>
    /// The client host is a plain BlazorWebView and never clears the WebView's IsVisible
    /// when its window goes behind another application, so Chromium keeps painting at the
    /// monitor's refresh rate and <c>document.hidden</c> stays false. The browser side
    /// works out the real answer from focus and pushes it here through
    /// <c>ShellActivityWatcher</c>; every repeating timer in the shell stops while it is
    /// false. Static rather than a DI service: subscribers are layout components with no
    /// shared owner. They must unsubscribe on dispose - this event outlives them.
    /// </remarks>
    public static class ShellActivity
    {
        private static bool _isActive = true;

        /// <summary>
        /// True while the shell is in the foreground, or whenever the browser side has not
        /// reported in - an unreported state must not freeze a working shell.
        /// </summary>
        public static bool IsActive => _isActive;

        /// <summary>
        /// Raised whenever <see cref="IsActive"/> changes.
        /// </summary>
        public static event Action Changed;

        /// <summary>
        /// Records the current verdict. Called by <c>ShellActivityWatcher</c>.
        /// </summary>
        public static void SetActive(bool isActive)
        {
            if (_isActive == isActive)
                return;

            _isActive = isActive;

            Changed?.Invoke();
        }
    }
}
