/// ***********************************************************************
///
/// =================================
/// CLR版本    ：4.0.30319.42000
/// 命名空间    ：DrawEverything
/// 文件名称    ：DrawRadioBox.cs
/// =================================
/// 创 建 者    ：lican
/// 创建日期    ：2019/1/9 14:09:01 
/// 邮箱        ：nihaolican@qq.com
/// 功能描述    ：单选框
/// 使用说明    ：
/// =================================
/// 修改者    ：
/// 修改日期    ：
/// 修改内容    ：
/// =================================
///
/// ***********************************************************************

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DrawEverything
{
    public class DrawRadioBox : DrawRectangleBase
    {

        #region Private Fields


        private Image visiableImage;

        private bool ischecked;

        #endregion

        #region Properties

        public readonly int SpaceImageAndText;

        public Image CheckedImage { get; set; }

        public Image UnCheckedImage { get; set; }
        //选择状态
        public bool Checked
        {
            get { return ischecked; }
            set
            {
                ischecked = value;
                visiableImage = ischecked ? CheckedImage : UnCheckedImage;

            }
        }
        //文本
        public string Context { get; set; }
        //文字字体
        public Font TextFont { get; set; }

        //附加数据
        public string AdditionalData { get; set; }

        //勾选联动项
        public string TickLinkageItem { get; set; }

        //不勾选联动项
        public string UncheckedLinkageItem { get; set; }

        //提示信息
        public string TipMessage { get; set; }

        //组名称
        public string GroupName { get; set; } 

        #endregion

        #region Events

        #endregion

        #region Constructors 

        public DrawRadioBox()
        {
            SetRectangleF(0, 0, 1, 1);
            Initialize();
        }

        public DrawRadioBox(float x, float y, float width, float height, Font _font)
        {
            SpaceImageAndText = 2;
            Width = width;
            Height = height;
            SetRectangleF(x, y, width, height);
            TextFont = _font;
            Initialize();
        }

        #endregion

        #region Control Events

        #endregion

        #region Methods

        public static DrawRadioBox Create(float x, float y, float width, float height, Font _font, string context, Image checkimage, Image uncheckimage)
        {
            if (string.IsNullOrEmpty(context) || uncheckimage == null || checkimage == null)
                return null;
            try
            {
                var dobj = new DrawRadioBox(x, y, width, height, _font)
                {
                    Context = context,
                    TextFont = _font,
                    CheckedImage = checkimage,
                    UnCheckedImage = uncheckimage,
                    Checked = false,

                };
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
                RectangleFz = CalcSize(g, Context, TextFont, RectangleFz.X, RectangleFz.Y, new StringFormat());
            Brush brush = new SolidBrush(Stroke);
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;//使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                //绘制图像
                var h = visiableImage.Height >= Height ? 0 : (Height - visiableImage.Height) / 2;
                //var rectImage = new RectangleF(RectangleFz.X + 2, RectangleFz.Y + h-2, Math.Min(visiableImage.Width, Width), Math.Min(visiableImage.Height, Height));
                var rectImage = new RectangleF(RectangleFz.X + 2, RectangleFz.Y + h, visiableImage.Width, visiableImage.Height);
                g.DrawImage(visiableImage, rectImage);

                var rectText = new RectangleF(RectangleFz.X + visiableImage.Width + 2 + SpaceImageAndText, RectangleFz.Y + h, RectangleFz.Width, RectangleFz.Height);
                g.DrawString(Context, TextFont, brush, rectText);
            }
            catch (Exception ex)
            {
            }
        }

        public override void DrawTracker(Graphics g)
        {
            //base.DrawTracker(g);
        }

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



        #endregion
    }
}
