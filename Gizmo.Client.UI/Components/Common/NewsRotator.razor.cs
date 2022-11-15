using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class NewsRotator : CustomDOMComponentBase
    {
        #region FIELDS

        private System.Threading.Timer _timer;

        private List<NewsRotatorItem> _items = new List<NewsRotatorItem>();

        private int _selectedIndex;

        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int Interval { get; set; }

        #endregion

        internal void Register(NewsRotatorItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(NewsRotatorItem item)
        {
            _items.Remove(item);
        }

        private void SlideNext(object stateInfo)
        {
            int newIndex = _selectedIndex + 1;
            if (newIndex > _items.Count - 1)
                newIndex = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].Clear();
            }

            _items[_selectedIndex].SetCurrent();
            _items[newIndex].SetNext();

            _selectedIndex = newIndex;

            //InvokeAsync(StateHasChanged);
        }

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var intervalChanged = parameters.TryGetValue<int>(nameof(Interval), out var newInterval);
            if (intervalChanged)
            {
                if (Interval > 0)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }
                    _timer = new System.Threading.Timer(SlideNext, new System.Threading.AutoResetEvent(false), Interval, Interval);
                }
                else
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }
                }
            }
        }

        public override void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            base.Dispose();
        }

        #endregion
    }
}
