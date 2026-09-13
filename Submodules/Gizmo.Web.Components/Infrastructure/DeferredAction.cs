using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public class DeferredAction
    {
        public DeferredAction(Func<Task> action)
        {
            _action = action;
            _timer = new Timer(new TimerCallback(Fire));
        }

        private Func<Task> _action;
        private Timer _timer;

        public void Defer(TimeSpan delay)
        {
            // Fire action when time elapses (with no subsequent calls).
            _timer?.Change(delay, TimeSpan.FromMilliseconds(-1));
        }

        public void Cancel()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void Fire(object state)
        {
            _action.Invoke();
        }
    }
}
