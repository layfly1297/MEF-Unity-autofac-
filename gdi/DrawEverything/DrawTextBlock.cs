using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace DrawEverything
{
    //绘制文本标签
    public class DrawTextBlock : DrawRectangleBase
    {
        #region Fields

        public static Font LastFontText = new Font("Microsoft Sans Serif", 12);
        public static string LastInputText = "";
        public static StringFormat LastStringFormat = new StringFormat();
        public StringFormat TextAnchor;
        private const string Tag = "textBlock";

        #endregion Fields

        #region Constructors

        public DrawTextBlock()
        {
            Font = new Font("Microsoft Sans Serif", 9 * Zoom);
            Text = "";
            SetRectangleF(0, 0, 1, 1);
            Initialize();
            TextAnchor = new StringFormat();
        }

        public DrawTextBlock(float x, float y)
        {
            Font = new Font(LastFontText.FontFamily, LastFontText.Size * Zoom);
            Text = LastInputText;
            //文字锚
            TextAnchor = new StringFormat(DrawTextBlock.LastStringFormat);
            RectangleFz = new RectangleF(x * Zoom, y * Zoom, 0, 0);
            Initialize();
        }

        #endregion Constructors

        #region Properties

        public Font Font { get; set; }

        public string Text
        {
            get; set;
        }

        public float Y
        {
            get
            {
                return RectangleFz.Y;
            }
            set
            {
                RectangleFz = new RectangleF(RectangleFz.X, value, RectangleFz.Width, RectangleFz.Height);
            }
        }

        #endregion Properties

        #region Methods

        public static RectangleF CalcSize(Graphics g, string txt, Font fnt, float x, float y, StringFormat fmt)
        {
            SizeF rectNeed = g.MeasureString(txt, fnt);
            var rect = new RectangleF(x, y, rectNeed.Width, rectNeed.Height);
            if (fmt.Alignment == StringAlignment.Center)
                rect.X -= rect.Width / 2;
            else if (fmt.Alignment == StringAlignment.Far)
                rect.X -= rect.Width;
            return rect;
        }

        public static DrawTextBlock Create(DataStructureTextBlock svg)
        {
            if (string.IsNullOrEmpty(svg.Text))
                return null;
            try
            {
                var dobj = new DrawTextBlock(svg.X, svg.Y)
                { Text = svg.Text };
                dobj.SetStyleFromSvg(svg);
                return dobj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public override void Draw(Graphics g)
        {
            if (RectangleFz.Width == 0 || RectangleFz.Height == 0)
                RectangleFz = CalcSize(g, Text, Font, RectangleFz.X, RectangleFz.Y, TextAnchor);
            Brush brush = new SolidBrush(Stroke);
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;//使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawString(Text, Font, brush, RectangleFz, TextAnchor);
            }
            catch (Exception ex)
            {
            }
        }


        public override void Resize(SizeF newscale, SizeF oldscale)
        {
            base.Resize(newscale, oldscale);
            float newfw = RecalcFloat(Font.Size, newscale.Width, oldscale.Width);
            Font = new Font(Font.FontFamily.Name, newfw, Font.Style);
        }

        [CLSCompliant(false)]
        public bool SetStyleFromSvg(DataStructureTextBlock svg)
        {
            try
            {

                Text = svg.Text;
                //font
                Stroke = svg.BackColor;

                Font = svg.TextBlockFont;//new Font(family, size, (FontStyle)fs);
                //				y -= font.Size; 
                RectangleFz = new RectangleF(svg.X, svg.Y, svg
                    .Width, svg.Height);
                TextAnchor.Alignment = svg.Alignment;
                switch (TextAnchor.Alignment)
                {
                    case StringAlignment.Far:
                        TextAnchor.Alignment = StringAlignment.Far;
                        RectangleFz = new RectangleF(svg.X - svg.Width, svg.Y, svg.Width, svg.Height);
                        break;
                    case StringAlignment.Center:
                        TextAnchor.Alignment = StringAlignment.Center;
                        RectangleFz = new RectangleF(svg.X - svg.Width / 2, svg.Y, svg.Width, svg.Height);
                        break;
                    default:
                        TextAnchor.Alignment = StringAlignment.Near;
                        RectangleFz = new RectangleF(svg.X, svg.Y, svg.Width, svg.Height);
                        break;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion Methods
    }
}