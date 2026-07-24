using System;

namespace Gizmo.Client.UI.Components.Login
{
    public readonly record struct ProviderOption(Guid PublicId, string Name, Guid ChannelGuid, bool IsPrimary);
}
