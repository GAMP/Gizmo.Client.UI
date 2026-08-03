using System;

namespace Gizmo.Client.UI.Components.Login
{
    public readonly record struct ProviderOption(int MethodId, string Name, Guid ChannelGuid, bool IsPrimary);
}
