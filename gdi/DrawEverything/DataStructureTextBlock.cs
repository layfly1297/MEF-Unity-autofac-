namespace DrawEverything
{
    using System.Drawing;
    public class DataStructureTextBlock
    {
        public Font TextBlockFont { get; set; }

        public StringAlignment Alignment { get; set; }

        public Color BackColor { get; set; }

        public string Text { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public string Id { get; set; }
    }
}