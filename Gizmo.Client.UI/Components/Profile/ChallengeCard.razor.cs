using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ChallengeCard : ProfileCardBase
    {
        [Parameter] public UserChallengeViewState Item { get; set; } = null!;
    }
}
