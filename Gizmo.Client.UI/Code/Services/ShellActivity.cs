using System;

namespace Gizmo.Client.UI.Services
{
    public static class ShellActivity
    {
        private static bool _isActive = true;

        public static bool IsActive => _isActive;

        public static event Action Changed;

        public static void SetActive(bool isActive)
        {
            if (_isActive == isActive)
                return;

            _isActive = isActive;

            Changed?.Invoke();
        }
    }
}
