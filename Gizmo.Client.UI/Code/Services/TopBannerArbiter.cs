using System;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Decides which of the top bar's attention banners is allowed on screen.
    /// </summary>
    /// <remarks>
    /// Both banners live in the same flex row between the icon pill and the profile
    /// pill, and both grow from width:0 - two of them open at once would push the
    /// whole cluster off the bar. They are also not equal in weight: the avatar nudge
    /// is an advertisement that can wait, a deployment is the answer to "why has my
    /// game not started yet" and is time critical, so deployment always wins and the
    /// nudge steps aside for as long as it runs.
    ///
    /// A static holder rather than a DI service on purpose: both banners are plain
    /// layout components with no shared owner to hang a scoped service off, and the
    /// state is a single bool with process lifetime.
    /// </remarks>
    public static class TopBannerArbiter
    {
        private static bool _deploymentVisible;

        /// <summary>
        /// Raised whenever the answer to <see cref="DeploymentVisible"/> changes.
        /// </summary>
        /// <remarks>
        /// Subscribers are layout components; they must unsubscribe on dispose, since
        /// this event outlives every one of them.
        /// </remarks>
        public static event Action Changed;

        /// <summary>
        /// True while the deployment banner occupies the slot.
        /// </summary>
        public static bool DeploymentVisible => _deploymentVisible;

        public static void SetDeploymentVisible(bool visible)
        {
            if (_deploymentVisible == visible)
                return;

            _deploymentVisible = visible;
            Changed?.Invoke();
        }
    }
}
