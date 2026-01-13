namespace Gizmo.Client.UI.Components
{
    public interface IGizInput
    {
        public string Label { get; set; }

        public string Placeholder { get; set; }

        public string LeftIcon { get; set; }

        public string RightIcon { get; set; }

        public Icons? LeftSVGIcon { get; set; }

        public Icons? RightSVGIcon { get; set; }

        public InputSizes Size { get; set; }

        public bool HasOutline { get; set; }

        public bool HasShadow { get; set; }

        public bool IsTransparent { get; set; }

        public bool IsFullWidth { get; set; }

        public string Width { get; set; }

        public ValidationErrorStyles ValidationErrorStyle { get; set; }

        public bool IsValid { get; }

        public string ValidationMessage { get; }
    }
}