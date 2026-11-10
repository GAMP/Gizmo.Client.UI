namespace Gizmo.Client.UI
{
    public sealed class ShellIntro
    {
        public bool Played { get; private set; }

        public void MarkPlayed() => Played = true;

        public void Reset() => Played = false;
    }
}
