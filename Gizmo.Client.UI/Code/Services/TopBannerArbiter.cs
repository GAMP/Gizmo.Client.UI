using System;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Decides which of the top bar's pills is allowed on screen.
    /// </summary>
    /// <remarks>
    /// They all live in the same flex row between the icons and the profile circle, and
    /// they all grow from width:0 - two of them open at once would push the cluster off
    /// the bar. They are not equal in weight either: a deployment is the answer to "why
    /// has my game not started yet" and the level hint is offered once per sign-in, while
    /// the invitation to add a picture is an advertisement that can wait. So the first
    /// two take the slot and the invitation steps aside for as long as they hold it.
    ///
    /// A static holder rather than a service: the pills are plain layout components with
    /// no shared owner to hang a scoped service off, and the state is two booleans with
    /// the lifetime of the process.
    /// </remarks>
    public static class TopBannerArbiter
    {
        private static bool _deploymentVisible;
        private static bool _hintVisible;

        /// <summary>
        /// Raised whenever the answer to <see cref="SlotTaken"/> changes.
        /// </summary>
        /// <remarks>
        /// Subscribers are layout components; they must unsubscribe on dispose, since
        /// this event outlives every one of them.
        /// </remarks>
        public static event Action Changed;

        /// <summary>True while something more important than an advertisement is there.</summary>
        public static bool SlotTaken => _deploymentVisible || _hintVisible;

        /// <summary>The deployment pill has it.</summary>
        public static void SetDeploymentVisible(bool visible) => Set(ref _deploymentVisible, visible);

        /// <summary>The sign-in hint about the level has it.</summary>
        public static void SetHintVisible(bool visible) => Set(ref _hintVisible, visible);

        private static void Set(ref bool field, bool visible)
        {
            if (field == visible)
                return;

            var before = SlotTaken;
            field = visible;

            if (SlotTaken != before)
                Changed?.Invoke();
        }
    }
}
