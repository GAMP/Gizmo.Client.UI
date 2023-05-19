using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route("/compo")]
    public partial class Compo : ComponentBase
    {
        public Compo()
        {
            Countries = new List<IconSelectCountry>();

            Countries.Add(new IconSelectCountry()
            {
                Text = "Georgia",
                PhonePrefix = "+995",
                Icon = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 16 16\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><g clip-path=\"url(#clip0_2031_59669)\"><path d=\"M8 16C12.4183 16 16 12.4183 16 8C16 3.58172 12.4183 0 8 0C3.58172 0 0 3.58172 0 8C0 12.4183 3.58172 16 8 16Z\" fill=\"#F0F0F0\"/><path d=\"M15.9323 6.95653H9.04353H9.0435V0.0677188C8.70191 0.02325 8.35366 0 8 0C7.64634 0 7.29809 0.02325 6.95653 0.0677188V6.95647V6.9565H0.0677188C0.02325 7.29809 0 7.64634 0 8C0 8.35372 0.02325 8.70191 0.0677188 9.04347H6.95647H6.9565V15.9323C7.29809 15.9768 7.64634 16 8 16C8.35366 16 8.70191 15.9768 9.04347 15.9323V9.04353V9.0435H15.9323C15.9768 8.70191 16 8.35372 16 8C16 7.64634 15.9768 7.29809 15.9323 6.95653V6.95653Z\" fill=\"#D80027\"/><path d=\"M4.86956 3.82609V2.78259H3.82609V3.82609H2.78259V4.86956H3.82609V5.91303H4.86956V4.86956H5.91303V3.82609H4.86956Z\" fill=\"#D80027\"/><path d=\"M12.1739 3.82609V2.78259H11.1304V3.82609H10.087V4.86956H11.1304V5.91303H12.1739V4.86956H13.2174V3.82609H12.1739Z\" fill=\"#D80027\"/><path d=\"M4.86956 11.1304V10.0869H3.82609V11.1304H2.78259V12.1739H3.82609V13.2174H4.86956V12.1739H5.91303V11.1304H4.86956Z\" fill=\"#D80027\"/><path d=\"M12.1739 11.1304V10.0869H11.1304V11.1304H10.087V12.1739H11.1304V13.2174H12.1739V12.1739H13.2174V11.1304H12.1739Z\" fill=\"#D80027\"/></g><defs><clipPath id=\"clip0_2031_59669\"><rect width=\"16\" height=\"16\" fill=\"white\"/></clipPath></defs></svg>"
            });
            Countries.Add(new IconSelectCountry()
            {
                Text = "Germany",
                PhonePrefix = "+49",
                Icon = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 16 16\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><g clip-path=\"url(#clip0_2031_59676)\"><path d=\"M0.497597 10.7825C1.62794 13.8289 4.56028 15.9999 8 15.9999C11.4397 15.9999 14.3721 13.8289 15.5024 10.7825L8 10.0869L0.497597 10.7825Z\" fill=\"#FFDA44\"/><path d=\"M8 0C4.56028 0 1.62794 2.171 0.497597 5.21741L8 5.91303L15.5024 5.21737C14.3721 2.171 11.4397 0 8 0Z\" fill=\"black\"/><path d=\"M0.497594 5.21741C0.176031 6.08406 0 7.02144 0 8C0 8.97856 0.176031 9.91594 0.497594 10.7826H15.5024C15.824 9.91594 16 8.97856 16 8C16 7.02144 15.824 6.08406 15.5024 5.21741H0.497594Z\" fill=\"#D80027\"/></g><defs><clipPath id=\"clip0_2031_59676\"><rect width=\"16\" height=\"16\" fill=\"white\"/></clipPath></defs></svg>"
            });
            Countries.Add(new IconSelectCountry()
            {
                Text = "Greece",
                PhonePrefix = "+30",
                Icon = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 16 16\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><g clip-path=\"url(#clip0_2031_59683)\"><path d=\"M8 16C12.4183 16 16 12.4183 16 8C16 3.58172 12.4183 0 8 0C3.58172 0 0 3.58172 0 8C0 12.4183 3.58172 16 8 16Z\" fill=\"#F0F0F0\"/><path d=\"M8 5.91302H15.7244C15.5238 5.16883 15.2189 4.46755 14.8258 3.82605H8V5.91302Z\" fill=\"#338AF3\"/><path d=\"M3.02009 14.2609H12.9799C13.71 13.6794 14.3361 12.9731 14.8258 12.174H1.17419C1.66391 12.9731 2.29003 13.6794 3.02009 14.2609Z\" fill=\"#338AF3\"/><path d=\"M2.78259 1.93555C2.15366 2.47714 1.60994 3.11492 1.17419 3.82605H2.78259V1.93555Z\" fill=\"#338AF3\"/><path d=\"M8 8.00005C8 7.18518 8 6.50321 8 5.91309H4.86956V8.00005H2.78259V5.91309H0.275594C0.09625 6.57852 0 7.27796 0 8.00005C0 8.72215 0.09625 9.42159 0.275594 10.087H15.7244C15.9038 9.42159 16 8.72215 16 8.00005H8Z\" fill=\"#338AF3\"/><path d=\"M8 0C6.88909 0 5.83112 0.226656 4.86956 0.635875V3.82609H8C8 3.09759 8 2.478 8 1.73913H12.9799C11.6133 0.65075 9.88278 0 8 0Z\" fill=\"#338AF3\"/></g><defs><clipPath id=\"clip0_2031_59683\"><rect width=\"16\" height=\"16\" fill=\"white\"/></clipPath></defs></svg>"
            });
            Countries.Add(new IconSelectCountry()
            {
                Text = "Italy",
                PhonePrefix = "+39",
                Icon = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 16 16\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><g clip-path=\"url(#clip0_2031_59688)\"><path d=\"M8 16C12.4183 16 16 12.4183 16 8C16 3.58172 12.4183 0 8 0C3.58172 0 0 3.58172 0 8C0 12.4183 3.58172 16 8 16Z\" fill=\"#F0F0F0\"/><path d=\"M16 7.99996C16 4.56025 13.829 1.6279 10.7826 0.497559V15.5024C13.829 14.372 16 11.4397 16 7.99996Z\" fill=\"#D80027\"/><path d=\"M0 7.99996C0 11.4397 2.171 14.372 5.21741 15.5024V0.497559C2.171 1.6279 0 4.56025 0 7.99996Z\" fill=\"#6DA544\"/></g><defs><clipPath id=\"clip0_2031_59688\"><rect width=\"16\" height=\"16\" fill=\"white\"/></clipPath></defs></svg>"
            });
            Countries.Add(new IconSelectCountry()
            {
                Text = "Uzbekistan",
                PhonePrefix = "+998",
                Icon = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 16 16\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><g clip-path=\"url(#clip0_2031_59693)\"><path d=\"M8 16C12.4183 16 16 12.4183 16 8C16 3.58172 12.4183 0 8 0C3.58172 0 0 3.58172 0 8C0 12.4183 3.58172 16 8 16Z\" fill=\"white\"/><path d=\"M16 7.99996C16 4.56025 13.829 1.6279 10.7826 0.497559V15.5024C13.829 14.372 16 11.4397 16 7.99996Z\" fill=\"white\"/><path d=\"M0 7.99996C0 11.4397 2.171 14.372 5.21741 15.5024V0.497559C2.171 1.6279 0 4.56025 0 7.99996Z\" fill=\"white\"/></g><defs><clipPath id=\"clip0_2031_59693\"><rect width=\"16\" height=\"16\" fill=\"white\"/></clipPath></defs></svg>"
            });
            Countries.Add(new IconSelectCountry()
            {
                Text = "Other",
                PhonePrefix = "+",
                Icon = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 16 16\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><g clip-path=\"url(#clip0_2031_59693)\"><path d=\"M8 16C12.4183 16 16 12.4183 16 8C16 3.58172 12.4183 0 8 0C3.58172 0 0 3.58172 0 8C0 12.4183 3.58172 16 8 16Z\" fill=\"white\"/><path d=\"M16 7.99996C16 4.56025 13.829 1.6279 10.7826 0.497559V15.5024C13.829 14.372 16 11.4397 16 7.99996Z\" fill=\"white\"/><path d=\"M0 7.99996C0 11.4397 2.171 14.372 5.21741 15.5024V0.497559C2.171 1.6279 0 4.56025 0 7.99996Z\" fill=\"white\"/></g><defs><clipPath id=\"clip0_2031_59693\"><rect width=\"16\" height=\"16\" fill=\"white\"/></clipPath></defs></svg>"
            });
        }

        private bool _isopen;

        public void OnTerminateClick()
        {
            _isopen = false;
        }
        public void OnTerminateClick2()
        {
        }

        public List<IconSelectCountry> Countries { get; set; }

        public IconSelectCountry SelectedCountry { get; set; }

        public string Mask { get; set; } = "###.###-###!###";

        public string Phone { get; set; }

        public DateTime? BirthDate { get; set; }

        public decimal Deposits { get; set; }

        public bool Disabled { get; set; }

        public bool Outline { get; set; }

        public bool Shadow { get; set; }

        public bool Transparent { get; set; }

        public bool FullWidth { get; set; }

        public string Test { get; set; } = "1";

        public async Task test1(bool value)
        {
            //if (value)
            //    Test = "1";

            await Task.Delay(1000);

            await InvokeAsync(StateHasChanged);

            StateHasChanged();
        }

        public void test2(bool value)
        {
            if (value)
                Test = "2";
        }

        public Gizmo.Web.Components.ButtonVariants Variant { get; set; }

        public Gizmo.Web.Components.ButtonColors Color { get; set; }
    }
}
