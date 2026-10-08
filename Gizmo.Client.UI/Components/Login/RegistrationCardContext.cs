using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.Services;

namespace Gizmo.Client.UI.Components
{
    public sealed class RegistrationCardContext
    {
        private const int VERIFICATION_SECONDS = 300;

        private string _pendingLoginName;
        private object _backOwner;
        private string _token;
        private DateTime _tokenIssuedUtc;
        private bool _resume;

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

        public void NoteToken(string token)
        {
            if (string.IsNullOrEmpty(token) || token == _token)
                return;

            _token = token;
            _tokenIssuedUtc = DateTime.UtcNow;
        }

        public bool IsVerificationExpired(IRegistrationSessionService session)
        {
            if (string.IsNullOrEmpty(session.Token) || session.Token != _token)
                return false;

            var seconds = session.ExpiresInSeconds > 0 ? session.ExpiresInSeconds : VERIFICATION_SECONDS;
            return DateTime.UtcNow - _tokenIssuedUtc >= TimeSpan.FromSeconds(seconds);
        }

        public static string VerifyAgainRoute(RegistrationFlow flow) => flow switch
        {
            RegistrationFlow.Sms => ClientRoutes.RegistrationPhoneRoute,
            RegistrationFlow.Email => ClientRoutes.RegistrationEmailRoute,
            RegistrationFlow.Redirect => ClientRoutes.RegistrationRedirectRoute,
            _ => ClientRoutes.RegistrationProvidersRoute,
        };

        public void RequestResume() => _resume = true;

        public bool TakeResume()
        {
            var resume = _resume;
            _resume = false;
            return resume;
        }

        public string TakePendingLoginName()
        {
            var name = _pendingLoginName;
            _pendingLoginName = null;
            return name;
        }
    }
}
