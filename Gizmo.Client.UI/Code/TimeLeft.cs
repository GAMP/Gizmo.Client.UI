using System;

namespace Gizmo.Client.UI
{
    public static class TimeLeft
    {
        public static TimeSpan NotNegative(TimeSpan? time) => time is TimeSpan value && value > TimeSpan.Zero ? value : TimeSpan.Zero;
    }
}
