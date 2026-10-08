using System;

namespace Gizmo.Client.UI.Components
{
    public sealed class RegistrationCardContext
    {
        private string _pendingLoginName;

        public event EventHandler Changed;

        public PlayerCardData Draft { get; private set; }

        public PlayerCardData Welcome { get; private set; }

        public SignupStage? Stage { get; private set; }

        public void SetStage(SignupStage stage)
        {
            if (Stage == stage)
                return;

            Stage = stage;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SetDraft(PlayerCardData draft)
        {
            if (Draft == draft)
                return;

            Draft = draft;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Complete(PlayerCardData card)
        {
            Draft = null;
            Welcome = card;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void CloseWelcome()
        {
            _pendingLoginName = Welcome?.Nick;
            Welcome = null;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void DropWelcome()
        {
            if (Welcome is null)
                return;

            Welcome = null;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public string TakePendingLoginName()
        {
            var name = _pendingLoginName;
            _pendingLoginName = null;
            return name;
        }
    }
}
