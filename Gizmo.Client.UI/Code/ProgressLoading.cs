using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI
{
    public sealed class ProgressLoading
    {
        private Task _running = Task.CompletedTask;
        private bool _started;

        public void Ensure(Func<Task> load, bool failed)
        {
            if (_started && !failed)
                return;

            Start(load);
        }

        public void Refresh(Func<Task> load) => Start(load);

        private void Start(Func<Task> load)
        {
            if (!_running.IsCompleted)
                return;

            _started = true;
            _running = RunAsync(load);
        }

        private static async Task RunAsync(Func<Task> load)
        {
            await Task.Yield();
            await load();
        }
    }
}
