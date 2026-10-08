namespace Gizmo.Client.UI.Components
{
    public sealed record RecoveryRequest(bool IsPhone, string Value, string Country, string RegionCode);

    public sealed class RecoveryHandoff
    {
        public RecoveryRequest Pending { get; private set; }

        public void Offer(RecoveryRequest request) => Pending = request;

        public RecoveryRequest Take()
        {
            var request = Pending;
            Pending = null;
            return request;
        }
    }
}
