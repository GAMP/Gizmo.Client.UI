using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI
{
    public sealed class ShellAssembly
    {
        private int _holds;
        private TaskCompletionSource _released = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public void Hold()
        {
            if (_holds++ == 0 && _released.Task.IsCompleted)
                _released = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public void Release()
        {
            if (_holds == 0)
                return;

            if (--_holds == 0)
                _released.TrySetResult();
        }

        public Task WhenReleasedAsync() => _holds == 0 ? Task.CompletedTask : _released.Task;
    }
}
