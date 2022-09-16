using Gizmo.Client.UI.View.Services;
using Gizmo.UI;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route("/compo")]
    public partial class Compo : ComponentBase
    {
        public Compo()
        {
        }

        public decimal Deposits { get; set; }

        public bool Disabled { get; set; }

        public bool Outline { get; set; }

        public bool Shadow { get; set; }

        public bool Transparent { get; set; }

        public bool FullWidth { get; set; }

        public ButtonVariants Variant { get; set; }

        public ButtonColors Color { get; set; }
    }
}