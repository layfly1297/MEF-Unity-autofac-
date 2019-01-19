using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace DrawEverything
{
    /// <summary>
    /// 绘制矩形
    /// </summary>
    public class DrawRectangleBase : DrawBase
    {

        #region Fields

        /// <summary>
        /// 常量 tag 标签
        /// </summary>
        private const string Tag = "rect";

        /// <summary>
        /// 矩形对象内部
        /// </summary>
        private RectangleF rectangle;

        #endregion Fields

        #region Constructors
        /// <summary>
        /// 构造默认赋值
        /// </summary>
        public DrawRectangleBase()
        {
            SetRectangleF(0, 0, 1, 1);
            Initialize();
        }
        /// <summary>
        /// 构造 赋值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public DrawRectangleBase(float x, float y, float width, float height)
        {
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
            Initialize();
        }

        #endregion Constructors


        #region Properties

        /// <summary>
        ///获取跟踪点数 （手柄）
        /// </summary>
        public override int HandleCount => 8;

        /// <summary>
        /// 矩形属性
        /// </summary>
        public Rectangle Rect
        {
            get
            {
                //避免锁定 创建新的对象返回
                var rect = new Rectangle
                {
                    X = (int)(rectangle.X / Zoom),
                    Y = (int)(rectangle.Y / Zoom),
                    Width = (int)(rectangle.Width / Zoom),
                    Height = (int)(rectangle.Height / Zoom)
                };
                return rect;
            }
            set
            {
                rectangle = value;
            }
        }

        /// <summary>
        /// 背景图片
        /// </summary>
        public Image FillBackGroundImage { get; set; }


        /// <summary>
        /// 背景文本描述
        /// </summary>
        public string BackGroundDescriptText { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        protected float Height
        {
            get
            {
                return rectangle.Height;
            }
            set
            {
                rectangle.Height = value;
            }
        }

        /// <summary>
        /// 浮点矩形
        /// </summary>
        protected RectangleF RectangleFz
        {
            get
            {
                return rectangle;
            }
            set
            {
                rectangle = value;
            }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        protected float Width
        {
            get
            {
                return rectangle.Width;
            }
            set
            {
                rectangle.Width = value;
            }
        }



        #endregion Properties

        #region Methods

        #region 正规划矩形

        /// <summary>
        /// 求正规化矩形 x 取小做位置点x ,y取小做位置点y ,取差距为大小
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static RectangleF GetNormalizedRectangle(float x1, float y1, float x2, float y2)
        {
            if (x2 < x1)
            {
                float tmp = x2;
                x2 = x1;
                x1 = tmp;
            }

            if (y2 < y1)
            {
                float tmp = y2;
                y2 = y1;
                y1 = tmp;
            }

            return new RectangleF(x1, y1, x2 - x1, y2 - y1);
        }

        public static RectangleF GetNormalizedRectangle(PointF p1, PointF p2)
        {
            return GetNormalizedRectangle(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static RectangleF GetNormalizedRectangle(RectangleF r)
        {
            return GetNormalizedRectangle(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
        }

        /// <summary>
        /// Normalize rectangle 正常矩形
        /// </summary>
        public override void Normalize()
        {
            rectangle = GetNormalizedRectangle(rectangle);
        }

        #endregion

        #region 重载
        /// <summary>
        ///画矩形
        /// </summary>
        /// <param name="g"></param>
        public override void Draw(Graphics g)
        {
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;//使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                RectangleF r = GetNormalizedRectangle(RectangleFz);//正规化矩形
                if (Fill != Color.Empty) //判断是否填充颜色
                {
                    Brush brush = new SolidBrush(Fill);
                    g.FillRectangle(brush, r);
                }
                Pen pen = new Pen(Stroke, StrokeWidth); //画矩形无填充
                g.DrawRectangle(pen, r.X, r.Y, r.Width, r.Height);
                if (!string.IsNullOrEmpty(BackGroundDescriptText))
                {
                    g.DrawString(BackGroundDescriptText, SystemFonts.DefaultFont, SystemBrushes.ActiveCaptionText,r);
                }
                if (FillBackGroundImage != null)
                {
                    //rectangle.Width = FillBackGroundImage.Width;
                    //rectangle.Height = FillBackGroundImage.Height; 
                    g.DrawImage(FillBackGroundImage, rectangle);
                }
                pen.Dispose();
            }
            catch (Exception ex)
            {
                Error("DrawRectangle Draw" + ex.ToString());
            }
        }


        /// <summary>
        /// 根据跟踪点获取位置点坐标
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override PointF GetHandle(int handleNumber)
        {
            var xCenter = rectangle.X + rectangle.Width / 2;
            var yCenter = rectangle.Y + rectangle.Height / 2;
            var x = rectangle.X;
            float y = rectangle.Y; 
            switch (handleNumber)
            {
                case 1:
                    x = rectangle.X;
                    y = rectangle.Y;
                    break;
                case 2:
                    x = xCenter;
                    y = rectangle.Y;
                    break;
                case 3:
                    x = rectangle.Right;
                    y = rectangle.Y;
                    break;
                case 4:
                    x = rectangle.Right;
                    y = yCenter;
                    break;
                case 5:
                    x = rectangle.Right;
                    y = rectangle.Bottom;
                    break;
                case 6:
                    x = xCenter;
                    y = rectangle.Bottom;
                    break;
                case 7:
                    x = rectangle.X;
                    y = rectangle.Bottom;
                    break;
                case 8:
                    x = rectangle.X;
                    y = yCenter;
                    break;
            }
            return new PointF(x, y);
        }

        /// <summary>
        /// 根据跟踪点获取鼠标光标
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber)
            {
                case 1:
                    return Cursors.SizeNWSE;
                case 2:
                    return Cursors.SizeNS;
                case 3:
                    return Cursors.SizeNESW;
                case 4:
                    return Cursors.SizeWE;
                case 5:
                    return Cursors.SizeNWSE;
                case 6:
                    return Cursors.SizeNS;
                case 7:
                    return Cursors.SizeNESW;
                case 8:
                    return Cursors.SizeWE;
                default:
                    return Cursors.Default;
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// 判断是否选中 根据位置点判断是否选择跟踪点
        /// 返回值：-1-没有命中/0-命中任何地方/&gt;1-句柄编号
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override int HitTest(PointF point)
        {
            if (Selected)
            {
                for (int i = 1; i <= HandleCount; i++)
                {
                    if (GetHandleRectangle(i).Contains(point))
                        return i;
                }
            }

            if (PointInObject(point))
                return 0;

            return -1;
        }

        /// <inheritdoc />
        /// <summary>
        /// 判断是否相交
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public override bool IntersectsWith(RectangleF rect)
        {
            try
            {
                return RectangleFz.IntersectsWith(rect);
            }
            catch (Exception ex)
            {
                Error("DrawRectangle Intersect" + ex.ToString());
                return false;
            }
        }

        /// <inheritdoc />
        ///  <summary>
        /// 移动对象
        ///  </summary>
        ///  <param name="deltaX"></param>
        ///  <param name="deltaY"></param>
        public override void Move(float deltaX, float deltaY)
        {
            rectangle.X += deltaX;
            rectangle.Y += deltaY;
        }

        /// <summary>
        ///  调整大小时修改矩形大小
        /// </summary>
        /// <param name="point"></param>
        /// <param name="handleNumber"></param>
        public override void MoveHandleTo(PointF point, int handleNumber)
        {
            float left = RectangleFz.Left;
            float top = RectangleFz.Top;
            float right = RectangleFz.Right;
            float bottom = RectangleFz.Bottom;
            switch (handleNumber)
            {
                case 1:
                    left = point.X;
                    top = point.Y;
                    break;
                case 2:
                    top = point.Y;
                    break;
                case 3:
                    right = point.X;
                    top = point.Y;
                    break;
                case 4:
                    right = point.X;
                    break;
                case 5:
                    right = point.X;
                    bottom = point.Y;
                    break;
                case 6:
                    bottom = point.Y;
                    break;
                case 7:
                    left = point.X;
                    bottom = point.Y;
                    break;
                case 8:
                    left = point.X;
                    break;
            }
            SetRectangleF(left, top, right - left, bottom - top);
        }


        /// <summary>
        /// 调整大小事件
        /// </summary>
        /// <param name="newscale"></param>
        /// <param name="oldscale"></param>
        public override void Resize(SizeF newscale, SizeF oldscale)
        {
            PointF p = RecalcPoint(RectangleFz.Location, newscale, oldscale);
            var ps = new PointF(RectangleFz.Width, RectangleFz.Height);
            ps = RecalcPoint(ps, newscale, oldscale);
            RectangleFz = new RectangleF(p.X, p.Y, ps.X, ps.Y);
            RecalcStrokeWidth(newscale, oldscale);
        }

        /// <summary>
        /// 判断点是否在对象上
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override bool PointInObject(PointF point)
        {
            return rectangle.Contains(point);
        }

        /// <summary>
        /// 设置矩形尺寸
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected void SetRectangleF(float x, float y, float width, float height)
        {
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
        }

        #endregion Methods

        #region

        /// <summary>
        /// 创建矩形对象
        /// </summary> 
        /// <param name="rectangle"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DrawRectangleBase Create(Rectangle rectangle, string name)
        {
            try
            {
                var dobj =
                    new DrawRectangleBase(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) { Name = name };

                return dobj;
            }
            catch (Exception)
            {
                // ("DrawRectangle CreateRectangle" + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 跟踪转移信息
        /// </summary>
        public override void Dump()
        {
            base.Dump();

            Trace.WriteLine("rectangle.X = " + rectangle.X.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Y = " + rectangle.Y.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Width = " + rectangle.Width.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Height = " + rectangle.Height.ToString(CultureInfo.InvariantCulture));
        }
        #endregion
        #endregion
    }

}