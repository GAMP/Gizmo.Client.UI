namespace Gizmo.Client.UI
{
    public struct BoundingClientRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }

        public bool Overlaps(BoundingClientRect rect)
        {
            if (this.Left > rect.Right || rect.Left > this.Right)
            {
                return false;
            }

            if (this.Top > rect.Bottom || rect.Top > this.Bottom)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";
        }
    }
}
