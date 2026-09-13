using System.Linq;
using System.Reflection;

namespace Gizmo.Client.UI
{
    /// <summary>
    /// The shell's own version, as written in the project file (<c>GrafitVersion</c>)
    /// and stamped into the assembly's metadata by the build.
    /// </summary>
    /// <remarks>
    /// Read from the assembly rather than kept as a constant so the number lives in one
    /// place - the csproj - together with the Gizmo release it is built for.
    /// </remarks>
    public static class ShellVersion
    {
        /// <summary>The shell version, e.g. "1.0.7"; empty if the build did not stamp it.</summary>
        public static string Grafit { get; } = Read("GrafitVersion");

        private static string Read(string key) =>
            typeof(ShellVersion).Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(a => a.Key == key)?.Value ?? string.Empty;
    }
}
