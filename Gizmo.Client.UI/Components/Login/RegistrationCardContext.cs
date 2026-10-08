using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public sealed class RegistrationCardContext
    {
        private string _pendingLoginName;
        private object _backOwner;

        public event EventHandler Changed;

        public PlayerCardData Draft { get; private set; }

        public PlayerCardData Welcome { get; private set; }

        public SignupStage? Stage { get; private set; }

        public Func<Task> Back { get; private set; }

        public void SetBack(object owner, Func<Task> back)
        {
            _backOwner = owner;
            Back = back;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void ClearBack(object owner)
        {
            if (!ReferenceEquals(_backOwner, owner))
                return;

            _backOwner = null;
            Back = null;
            Changed?.Invoke(this, EventArgs.Empty);
        }

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
