using Gizmo.Client.UI.View.States;

namespace Gizmo.Client.UI
{
    public static class ClientFeatures
    {
        public static bool Ladder { get; } = Has("Gizmo.Client.UI.View.States.UserLadderSummaryViewState");

        public static bool PaymentEvents { get; } =
            typeof(UserOnlineDepositViewState).GetProperty(nameof(UserOnlineDepositViewState.IsPaid)) is not null;

        private static bool Has(string typeName) => typeof(UserViewState).Assembly.GetType(typeName, throwOnError: false) is not null;
    }
}
