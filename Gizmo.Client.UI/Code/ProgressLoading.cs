namespace Gizmo.Client.UI
{
    public sealed class ProgressLoading
    {
        private bool _started;

        public bool TryStart()
        {
            if (_started)
                return false;

            _started = true;
            return true;
        }

        public void MarkStarted() => _started = true;
    }
}
