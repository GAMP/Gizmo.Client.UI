using System.Runtime.CompilerServices;
using Gizmo.Client.UI.View.States;

namespace Gizmo.Client.UI
{
    // Reads members that clients before the payment events lack; call only when
    // ClientFeatures.PaymentEvents is true. Not inlined, so a caller never binds to them.
    public static class PaymentIntentStatus
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool IsPaid(UserOnlineDepositViewState state) => state.IsPaid;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool IsFailed(UserOnlineDepositViewState state) => state.IsPaymentFailed;
    }
}
