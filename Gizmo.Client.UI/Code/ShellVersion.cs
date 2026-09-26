using System.Linq;
using System.Reflection;

namespace Gizmo.Client.UI
{
    public static class ShellVersion
    {
        public static string Grafit { get; } = Read("GrafitVersion");

        private static string Read(string key) =>
            typeof(ShellVersion).Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(a => a.Key == key)?.Value ?? string.Empty;
    }
}
