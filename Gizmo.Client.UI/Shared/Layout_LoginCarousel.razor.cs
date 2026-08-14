using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class Layout_LoginCarousel : CustomDOMComponentBase
    {
        private const int SlideCount = 4;
        private int _currentIndex = 1;

        //TODO stub
        private string SlideText => "Amet minim mollit non deserunt ullamco est sit aliqua dolor do amet sint. " +
                                    "Velit officia consequat duis enim velit mollit. Exercitation veniam consequat";

        private void GoToSlide(int index)
        {
            _currentIndex = index;
            StateHasChanged();
        }
    }
}
