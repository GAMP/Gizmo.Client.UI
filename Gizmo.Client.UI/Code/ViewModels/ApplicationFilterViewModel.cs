using Gizmo.Shared.ViewModels;
using System.Collections.Generic;

namespace Gizmo.Client.UI.ViewModels
{
    public class ApplicationFilterViewModel
    {
        private List<ApplicationFilterOptionViewModel> _options;

        public int Id { get; set; }

        public string Name { get; set; }

        public List<ApplicationFilterOptionViewModel> Options
        {
            get
            {
                if (_options == null)
                    _options = new List<ApplicationFilterOptionViewModel>();

                return _options;
            }
            set
            {
                _options = value;
            }
        }
    }
}