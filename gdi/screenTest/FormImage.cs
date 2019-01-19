using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawEverything;

namespace screenTest
{


    //（附公式：px = pt* DPI / 72)   默认DPI 96   像素和榜转换公式
    public partial class FormImage : Form
    {
        #region 变量

        /// <summary>
        /// 选择模式
        /// </summary>
        private enum SelectionMode
        {
            None, //没选择
            NetSelection,   // group selection is active 组选择激活 网格捕获选择
            Move,           // object(s) are moves //对象移动
            Size            // object is resized //对象大小
        }

        private Point pointTemp = new Point(0, 0);
        /// <summary>
        /// 选择模式
        /// </summary>
        private SelectionMode selectionMode;
        /// <summary>
        ///  默认DPI 96   像素和榜转换公式
        /// </summary>
        private const float DefaultBL = (3f / 4f);

        /// <summary>
        /// 鼠标按下起始位置
        /// </summary>
        private PointF startPoint;

        /// <summary>
        /// 当前位置点 指 发生变化记录的点
        /// </summary>
        private PointF lastPoint;

        //图像缓冲区上下文
        private BufferedGraphicsContext bufferedGraphicsContext;

        /// <summary>
        /// 图像缓冲对象
        /// </summary>
        private BufferedGraphics bufferedGraphics;

        //默认矩形大小
        private RectangleF rectangle;

        //默认矩形大小
        private RectangleF rectangleCopy;

        private RectangleF netSelectedRectangleF;
        //绘边颜色
        private Color stroke;

        /// <summary>
        /// 是否选择矩形
        /// </summary>
        private bool Selected;

        /// <summary>
        /// 手柄数  跟踪点数
        /// </summary>
        public virtual int HandleCount
        {
            get
            {
                return 8;
            }
        }

        /// <summary>
        ///绘边宽度
        /// </summary> 
        private float strokeWidth;

        private Image image;

        private int handleNumber = 0;
        #endregion 
        //public event SingleSelectModelBingHandle<SingelSelectDataModle> DataBind;
        public FormImage()
        {
            InitializeComponent();

            //窗体中控件的事件晚期绑定
            for (int i = 0; i < this.Controls.Count; i++)
            {
                this.Controls[i].MouseDown += new MouseEventHandler(MyMouseDown);
                this.Controls[i].MouseLeave += new EventHandler(MyMouseLeave);
                this.Controls[i].MouseMove += new MouseEventHandler(MyMouseMove);
            }
            Initialize();
            //DataBind?.Invoke(new SingelSelectDataModle());
        }



        float bl = 0;
        void Initialize()
        {
            //image = Image.FromFile(@"ioc.png");
            image = Properties.Resources.ioc;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);


            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true); //默认启动双缓冲
            //SetStyle(ControlStyles.DoubleBuffer, true);
            //SetStyle(ControlStyles.UserPaint, true); 
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.

            rectangle = new Rectangle(10, 10, 100, 100); //默认矩形 100，100

            netSelectedRectangleF = new RectangleF(0, 0, 0, 0);

            bufferedGraphicsContext = BufferedGraphicsManager.Current;
            bufferedGraphicsContext.MaximumBuffer = new Size(this.Width, this.Height);
            bufferedGraphics = bufferedGraphicsContext.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width + 1, this.Height + 1));
            //bufferedGraphics = bufferedGraphicsContext.Allocate(this.CreateGraphics(), Rectangle.Round(new RectangleF(rectangle.X, rectangle.Y, rectangle.Width + 10, rectangle.Height + 10)));
            //转换毫米
            rectangle = new RectangleF(PxConvertMillimetre(10f), PxConvertMillimetre(10f), PxConvertMillimetre(100f), PxConvertMillimetre(100f)); //默认矩形 100，100
            rectangleCopy = rectangle;
            stroke = Color.LightSeaGreen;
            strokeWidth = 0.1f;
            Drawe();
            Selected = true;//默认添加上去的就是选择的 
        }

        #region 移动

        #region  像素和 榜 互转  （附公式：px = pt* DPI / 72)   默认DPI 96   像素和榜转换公式
        //px = pt* DPI / 72   1px=0.75pt
        /// <summary>
        /// 像素转换 毫米  （象素数 / DPI = 英寸数  英寸数* 25.4 = 毫米数）
        /// </summary>
        /// <param name="oldPx">像素</param>
        /// <param name="newMm">毫米输出</param>
        public static float PxConvertMillimetre(float oldPx)
        {
            return oldPx / 96f * 25.4f;
        }



        /// <summary>
        /// 像素转榜     Pixel convert Point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="dpi"></param>
        /// <returns></returns>
        PointF PxToPt(PointF point, int dpi = 96)
        {
            PointF pointF = new PointF();
            pointF.X = 72f * point.X / dpi;
            pointF.Y = 72f * point.Y / dpi;
            return pointF;
        }


        /// <summary>
        /// 榜转像素  Point convert pixel
        /// </summary>
        /// <param name="point"></param>
        /// <param name="dpi"></param>
        /// <returns></returns>
        PointF PtToPx(PointF point, int dpi = 96)
        {

            PointF pointF = new PointF();
            pointF.X = point.X * dpi / 72f;
            pointF.Y = point.Y * dpi / 72f;
            return pointF;
        }

        #endregion


        private void FormImage_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint.X = PxConvertMillimetre(e.Location.X);
            lastPoint.Y = PxConvertMillimetre(e.Location.Y);
            startPoint = lastPoint;
            //创建光标
            //caret.caretImplementation.Create(2, SystemFonts.DefaultFont.Height);
            //caret.caretImplementation.SetPosition(e.X, e.Y);
            //caret.caretImplementation.Show();
            //var sdd = InputLanguage.CurrentInputLanguage;
            //var sdz = InputLanguage.InstalledInputLanguages;
            //pointTemp = e.Location;
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            selectionMode = SelectionMode.None;
            //赋值当前点 
            //var point = PxConvertMillimetre(pointTemp);
            //point = PxToPt(point);

            //判断选择添加跟踪点 没有选择不添加
            handleNumber = HitTest(lastPoint);
            if (handleNumber != -1)
            {
                DrawTracker(bufferedGraphics.Graphics);
                switch (handleNumber)
                {
                    case 0:
                        selectionMode = SelectionMode.Move;
                        break;
                    default:
                        this.Cursor = GetHandleCursor(handleNumber);
                        selectionMode = SelectionMode.Size;
                        break;
                }
            }
            else
            {
                selectionMode = SelectionMode.NetSelection;
                Drawe();
            }

            //lastPoint = e.Location;
            //startPoint = e.Location;
        }

        private void FormImage_MouseMove(object sender, MouseEventArgs e)
        {
            PointF point = e.Location;
            //转换毫米
            point.X = PxConvertMillimetre(point.X);
            point.Y = PxConvertMillimetre(point.Y);
            var handleTemp = HitTest(point);
            // 未按鼠标按钮时设置光标
            if (e.Button == MouseButtons.None)
            {
                switch (handleTemp)
                {
                    case -1:
                        this.Cursor = Cursors.Default;
                        break;
                    case 0:
                        this.Cursor = Cursors.SizeAll;//四方向
                        break;
                    default:
                        this.Cursor = GetHandleCursor(handleTemp);
                        break;
                }
            }
            //不是左键时返回
            if (e.Button != MouseButtons.Left)
                return;

            //按下左键时 找出以前和现在的位置之间的差异 
            float dx = point.X - lastPoint.X;
            float dy = point.Y - lastPoint.Y;

            //lastPoint.X = e.X;
            //lastPoint.Y = e.Y;
            lastPoint.X = PxConvertMillimetre(e.Location.X);
            lastPoint.Y = PxConvertMillimetre(e.Location.Y);

            // resize
            if (selectionMode == SelectionMode.Size)
            {
                MoveHandleTo(point, handleNumber);

                ////移动时绘画
                //Drawe();
                //DrawTracker(bufferedGraphics.Graphics);
            }
            // move
            if (selectionMode == SelectionMode.Move)
            {

                this.Cursor = Cursors.SizeAll;
                Moveing(dx, dy);
                //移动时绘画
                Drawe();
                DrawTracker(bufferedGraphics.Graphics);
            }

            if (selectionMode == SelectionMode.NetSelection)
            {
                netSelectedRectangleF = GetNormalizedRectangle(startPoint, lastPoint);
                Drawe();
                DrawNetSelection(bufferedGraphics.Graphics);
            }
            this.Invalidate();

        }

        private void FormImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectionMode == SelectionMode.NetSelection)
            {
                selectionMode = SelectionMode.None;
                Drawe();
            }
            //this.Invalidate(); ;
        }

        ///  <summary>
        /// 调整大小时修改矩形大小
        ///  </summary>
        ///  <param name="point"></param> 
        /// <param name="handleNumbers"></param>
        public void MoveHandleTo(PointF point, int handleNumbers)
        {
            float left = rectangle.Left;
            float top = rectangle.Top;
            float right = rectangle.Right;
            float bottom = rectangle.Bottom;
            switch (handleNumbers)
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
            // w =右边减去左边的  h 等于底部减去上部的
            SetRectangleF(left, top, right - left, bottom - top);

            //移动时绘画
            Drawe();
            DrawTracker(bufferedGraphics.Graphics);
        }

        protected void SetRectangleF(float x, float y, float width, float height)
        {
            //设置最小值
            if (width < 20)
            {
                width = 20;
            }
            if (height < 20)
            {
                height = 20;
            }

            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
        }

        /// <summary>
        ///移动对象
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        private void Moveing(float deltaX, float deltaY)
        {
            rectangle.X += deltaX;
            rectangle.Y += deltaY;
        }

        /// <summary>
        ///画矩形
        /// </summary>
        /// <param name="g"></param>
        void Drawe()
        {
            try
            {
                //清除背景
                bufferedGraphics.Graphics.FillRectangle(Brushes.White, 0, 0, this.Width, this.Height);
                //默认像素点
                //bufferedGraphics.Graphics.PageUnit = GraphicsUnit.Point; //榜
                bufferedGraphics.Graphics.PageUnit = GraphicsUnit.Millimeter; //毫米
                bufferedGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;//使绘图质量最高，即消除锯齿
                bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                bufferedGraphics.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                RectangleF r = GetNormalizedRectangle(rectangle);//正规化矩形  
                Pen pen = new Pen(stroke, strokeWidth);
                if (image != null)
                {
                    #region 白色透明
                    //Color customColor = Color.FromArgb(100, Color.White);
                    //SolidBrush shadowBrush = new SolidBrush(customColor);
                    //bufferedGraphics.Graphics.FillRectangle(shadowBrush,r); 
                    //Color customColor = Color.FromArgb(50, Color.Transparent);
                    Brush b = new SolidBrush(Color.FromArgb(30, Color.Green));
                    bufferedGraphics.Graphics.FillRectangle(b, r);
                    #endregion

                    //var g = Graphics.FromImage(image);
                    //g.PageUnit = GraphicsUnit.Millimeter; //毫米
                    //g.SmoothingMode = SmoothingMode.AntiAlias;//使绘图质量最高，即消除锯齿
                    //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //g.CompositingQuality = CompositingQuality.HighQuality;
                    ////image.Save("1m.jpg");
                    ////g.DrawImage(image, r.X, r.Y, r.Width, r.Height);
                    ////g.FillRectangle(Brushes.White, 0, 0, this.Width, this.Height);
                    ////g.DrawImage(image, 0, 0, new RectangleF(r.X, r.Y, r.Width, r.Height), GraphicsUnit.Millimeter);
                    //var vont = new Font(this.Font.FontFamily,
                    //     9, GraphicsUnit.Millimeter);
                    ////g.DrawString(@"你好我是测试ADN12321asd", SystemFonts.DefaultFont,SystemBrushes.ControlText,0,0);
                    ////g.DrawString(@"你好我是测试ADN12321asd", vont, SystemBrushes.ControlText, new RectangleF(r.X, r.Y, r.Width, r.Height));
                    ////g.DrawString(@"你好我是测试ADN12321asd", vont, SystemBrushes.ControlText,0,0);

                    //g.Dispose();
                    //image.Save("2.jpg");
                    bufferedGraphics.Graphics.DrawImage(image, r.X, r.Y, r.Width, r.Height);
                    //bufferedGraphics.Graphics.DrawPath(Pens.Gray, CreateRoundedRectangle(Rectangle.Round(r), 24, false));
                    //image.Save("3.jpg");
                }
                else
                {
                    bufferedGraphics.Graphics.DrawRectangle(pen, r.X, r.Y, r.Width, r.Height);
                }


                pen.Dispose();
            }
            catch (Exception ex)
            {
            }
        }


        /*
                 * 为了鼓励学习研究精神,该函数,仅能用于本示例
                 * 如果使用到其他项目,可能会存在错误
                 * 如果您确实需要正确代码,请学习位图相关信息
                */
        /// <summary>
        /// 取得一个图片中非透明色部分的区域。
        /// </summary>
        /// <param name="Picture">取其区域的图片。</param>
        /// <param name="TransparentColor">透明色。</param>
        /// <returns>图片中非透明色部分的区域</returns>
        public unsafe static Region ImageToRegionPx(Image Picture, Color TransparentColor)
        {
            if (Picture == null) return null;
            Region rgn = new Region();
            rgn.MakeEmpty();

            Bitmap bitmap = null;
            if (Picture.GetType() != typeof(Bitmap))
                bitmap = new Bitmap(Picture);
            else
                bitmap = (Bitmap)Picture;

            int width = bitmap.Width;
            int height = bitmap.Height;
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte* p = (byte*)bmData.Scan0;
            int offset = bmData.Stride - width * 3;

            int p0, p1, p2;         // 记录透明色
            p0 = TransparentColor.R;
            p1 = TransparentColor.G;
            p2 = TransparentColor.B;

            Rectangle curRect = new Rectangle();
            curRect.Height = 1;

            int start = -1;
            // 行座标 ( Y col ) 
            for (int Y = 0; Y < height; Y++)
            {
                // 列座标 ( X row ) 
                for (int X = 0; X < width; X++)
                {
                    if (start == -1 && (p[0] != p0 || p[1] != p1 || p[2] != p2))     //如果 之前的点没有不透明 且 不透明 
                    {
                        start = X;                            //记录这个点
                        curRect.X = X;
                        curRect.Y = Y;
                    }
                    else if (start > -1 && (p[0] == p0 && p[1] == p1 && p[2] == p2))      //如果 之前的点是不透明 且 透明
                    {
                        curRect.Width = X - curRect.X;
                        rgn.Union(curRect);
                        start = -1;
                    }

                    if (X == width - 1 && start > -1)        //如果 之前的点是不透明 且 是最后一个点
                    {
                        curRect.Width = X - curRect.X;
                        rgn.Union(curRect);
                        start = -1;
                    }
                    p += 3;//下一个内存地址
                }
                p += offset;
            }
            bitmap.UnlockBits(bmData);
            bitmap.Dispose();
            return rgn;
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle b, int r, bool fill = false)
        {
            var path = new GraphicsPath();
            var r2 = (int)r / 2;
            var fix = fill ? 1 : 0;

            b.Location = new Point(b.X - 1, b.Y - 1);
            if (!fill)
                b.Size = new Size(b.Width - 1, b.Height - 1);

            path.AddArc(b.Left, b.Top, r, r, 180, 90);
            path.AddLine(b.Left + r2, b.Top, b.Right - r2 - fix, b.Top);

            path.AddArc(b.Right - r - fix, b.Top, r, r, 270, 90);
            path.AddLine(b.Right, b.Top + r2, b.Right, b.Bottom - r2);

            path.AddArc(b.Right - r - fix, b.Bottom - r - fix, r, r, 0, 90);
            path.AddLine(b.Right - r2, b.Bottom, b.Left + r2, b.Bottom);

            path.AddArc(b.Left, b.Bottom - r - fix, r, r, 90, 90);
            path.AddLine(b.Left, b.Bottom - r2, b.Left, b.Top + r2);

            return path;
        }

        /// <summary>
        /// 选择对象绘画跟踪点
        /// </summary>
        /// <param name="g"></param>
        public virtual void DrawTracker(Graphics g)
        {
            if (!Selected)
                return;

            var brush = new SolidBrush(Color.LightSeaGreen);
            g.SmoothingMode = SmoothingMode.AntiAlias;//使绘图质量最高，即消除锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PageUnit = GraphicsUnit.Millimeter;

            g.DrawRectangle(new Pen(Color.LightSeaGreen, 0.1f), Rectangle.Round(rectangle));
            for (int i = 1; i <= HandleCount; i++)
            {
                try
                {

                    g.FillEllipse(brush, GetHandleRectangle(i));
                }
                catch
                { }
            }
            brush.Dispose();
        }

        /// <summary>
        /// 画鼠标点击拖拽矩形虚线
        /// </summary>
        /// <param name="g"></param>
        public void DrawNetSelection(Graphics g)
        {
            g.PageUnit = GraphicsUnit.Millimeter;
            var r = Rectangle.Round(netSelectedRectangleF);
            ControlPaint.DrawFocusRectangle(g, r, Color.LightSeaGreen, Color.Transparent);
        }

        /// <summary>
        /// 判断是否选中 根据位置点判断是否选择跟踪点
        /// 返回值：-1-没有命中/0-命中任何地方/>1-句柄编号
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int HitTest(PointF point)
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


        /// <summary>
        /// 判断点是否在对象上
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected bool PointInObject(PointF point)
        {
            var rect = rectangle;
            if (rectangle.Width < 0 || rectangle.Height < 0)
            {
                rect = new RectangleF(rect.X, rect.Y, Math.Abs(rect.Width), Math.Abs(rect.Height));
                return rect.Contains(point);
            }
            return rectangle.Contains(point);
        }


        /// <summary>
        /// Get handle rectangle by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public virtual RectangleF GetHandleRectangle(int handleNumber)
        {
            var point = GetHandle(handleNumber);

            return new RectangleF(point.X - 1f, point.Y - 1f, 2f, 2f); //设置跟踪点矩形大小
        }


        /// <summary>
        /// 根据跟踪点获取位置点坐标
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public PointF GetHandle(int handleNumber)
        {
            float x, xCenter, yCenter;

            xCenter = rectangle.X + rectangle.Width / 2;
            yCenter = rectangle.Y + rectangle.Height / 2;
            x = rectangle.X;
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
        public Cursor GetHandleCursor(int handleNumber)
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

        private void FormImage_Resize(object sender, EventArgs e)
        {
            OnResize(sender, e);
        }

        #endregion

        private void OnResize(object sender, EventArgs e)
        {
            //  设置缓冲图形上下文的最大大小。
            bufferedGraphicsContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            //如果有先释放 赋值空对象
            if (bufferedGraphics != null)
            {
                bufferedGraphics.Dispose();
                bufferedGraphics = null;
            }
            //再创建新的缓冲区
            bufferedGraphics = bufferedGraphicsContext.Allocate(this.CreateGraphics(),
                new Rectangle(0, 0, this.Width, this.Height));

            // 使背景被清除并重新绘制  
            Drawe();
            //刷新
            this.Refresh();
        }
        #endregion

        #region key
        private void FormImage_Paint(object sender, PaintEventArgs e)
        {
            bufferedGraphics.Render(e.Graphics);
            //ClearGrafx();

        }

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_CHAR = 0x0102;
        private bool isDownControl = false;
        //protected override void WndProc(ref Message m)
        //{
        //    switch (m.Msg)
        //    {
        //        //case WM_IME_SETCONTEXT:
        //        //    if (m.WParam.ToInt32() == 1)
        //        //    {
        //        //        //ImmAssociateContext(this.Handle, m_hImc);
        //        //    }
        //        //    break;

        //        case WM_KEYDOWN: //键盘按下
        //            WMKeydown(ref m);
        //            break;
        //        case WM_KEYUP: //键盘抬起
        //            WMKeyup(ref m);
        //            break;
        //        case WM_CHAR: //文字输入
        //            WMChar(ref m);
        //            break;
        //        default:
        //            base.WndProc(ref m);
        //            break;
        //    }
        //}
        #region 键盘事件相关处理方法
        /// <summary>
        /// 判断当前按键是否是功能键
        /// </summary>
        /// <param name="wParam"></param>
        /// <returns></returns>
        private bool KeyDownIsControlKeys(int wParam)
        {
            if (wParam == (int)Keys.F1 ||
                wParam == (int)Keys.F2 ||
                wParam == (int)Keys.F3 ||
                wParam == (int)Keys.F4 ||
                wParam == (int)Keys.F5 ||
                wParam == (int)Keys.F6 ||
                wParam == (int)Keys.F7 ||
                wParam == (int)Keys.F8 ||
                wParam == (int)Keys.F9 ||
                wParam == (int)Keys.F10 ||
                wParam == (int)Keys.F11 ||
                wParam == (int)Keys.F12 ||
                wParam == (int)Keys.Up ||
                wParam == (int)Keys.Down ||
                wParam == (int)Keys.Left ||
                wParam == (int)Keys.Right ||
                wParam == (int)Keys.Insert ||
                wParam == (int)Keys.Delete ||
                wParam == (int)Keys.Home ||
                wParam == (int)Keys.End ||
                wParam == (int)Keys.PageUp ||
                wParam == (int)Keys.PageDown ||
                wParam == (int)Keys.ShiftKey ||
                wParam == (int)Keys.ControlKey ||
                //|| keyUpwParam == (int)Keys.Alt //此方法捕获不到此Alt键盘事件
                wParam == (int)Keys.Tab)
            {
                //Console.WriteLine("功能键:{0},{1}", (Keys)wParam, wParam);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 判断当前输入是否功能键
        /// </summary>
        /// <param name="wParam"></param>
        /// <returns></returns>
        private bool WMCharIsControlKeys(int wParam)
        {
            if (wParam == (int)Keys.Back
             || wParam == (int)Keys.Escape
             || wParam == (int)Keys.Enter)
            {
                // Console.WriteLine("功能键:{0},{1}", (Keys)wParam, wParam);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 键盘按下事件处理
        /// </summary>
        /// <param name="m"></param>
        private void WMKeydown(ref Message m)
        {

            int keydownwParam = (int)m.WParam;
            if (keydownwParam == (int)Keys.ControlKey)
            {
                isDownControl = true;
            }
            if (KeyDownIsControlKeys(keydownwParam))
            {

                Console.WriteLine("功能键:{0},{1}", (Keys)keydownwParam, keydownwParam);
                //var tuple2 = new Tuple<int, List<Keys>>(0, new List<Keys> { (Keys)keydownwParam });
                //_documentControl.OnKeyInput(tuple2);
            }
        }
        /// <summary>
        /// 键盘抬起事件处理
        /// </summary>
        /// <param name="m"></param>
        private void WMKeyup(ref Message m)
        {
            int keyUpwParam = (int)m.WParam;
            if (keyUpwParam == (int)Keys.ControlKey)
            {
                isDownControl = false;
            }
        }
        /// <summary>
        /// 文字输入事件处理
        /// </summary>
        /// <param name="m"></param>
        private void WMChar(ref Message m)
        {
            if (isDownControl)
            {
                //control按下时为组合件,不作为文字输入
                return;
            }
            int wParam = (int)m.WParam;
            if (WMCharIsControlKeys(wParam))
            {
                Console.WriteLine("功能键:{0},{1}", (Keys)wParam, wParam);
                //var tuple = new Tuple<int, List<Keys>>(0, new List<Keys> { (Keys)wParam });
                //_documentControl.OnKeyInput(tuple);
                return;
            }
            string txt = char.ConvertFromUtf32(wParam).ToString();
        }


        #endregion //键盘事件相关处理方法 

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Image _image = ImageFromBytes(ReadPngMemImage(@"ioc.png"));
            _image.Save("kl.png");
        }


        /// <summary>
        /// Get Image object from byte array
        /// </summary>
        /// <param name="arrb"></param>
        /// <returns></returns>
        public static Image ImageFromBytes(byte[] arrb)
        {
            if (arrb == null)
                return null;
            try
            {
                // Perform the conversion
                var ms = new MemoryStream();
                const int offset = 0;
                ms.Write(arrb, offset, arrb.Length - offset);
                Image im = new Bitmap(ms);
                ms.Close();
                return im;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Load image from file to byte array
        /// </summary>
        /// <param name="flnm">File name</param>
        /// <returns>byte array</returns>
        public static byte[] ReadPngMemImage(string flnm)
        {
            try
            {
                FileStream fs = new FileStream(flnm, FileMode.Open, FileAccess.Read);
                MemoryStream ms = new MemoryStream();
                Bitmap bm = new Bitmap(fs);
                //bm.MakeTransparent(Color.FromArgb(0, Color.White));

                bm.Save(ms, ImageFormat.Png);
                BinaryReader br = new BinaryReader(ms);
                ms.Position = 0;
                byte[] arrpic = br.ReadBytes((int)ms.Length);
                br.Close();
                fs.Close();
                ms.Close();
                return arrpic;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading file " + ex, "");

                return null;
            }
        }


        #region 控件处理拖拽 改变大小
        /// <summary>
        ///  有关鼠标样式的相关枚举
        /// </summary>
        private enum EnumMousePointPosition
        {
            MouseSizeNone = 0, //无
            MouseSizeRight = 1, //拉伸右边框
            MouseSizeLeft = 2,  //拉伸左边框
            MouseSizeBottom = 3, //拉伸下边框
            MouseSizeTop = 4, //拉伸上边框
            MouseSizeTopLeft = 5,//拉伸左上角
            MouseSizeTopRight = 6,//拉伸右上角
            MouseSizeBottomLeft = 7,//拉伸左下角
            MouseSizeBottomRight = 8,//拉伸右下角
            MouseDrag = 9,//鼠标拖动
            IBeam = 10 //获取 I 型光标，该光标用于显示单击鼠标时文本光标出现的位置。
        }
        const int Band = 5;//范围半径
        const int MinWidth = 10;//最低宽度
        const int MinHeight = 10;//最低高度
        private EnumMousePointPosition m_MousePointPosition; //鼠标样式枚举
        private Point m_lastPoint; //光标上次移动的位置
        private Point m_endPoint; //光标移动的当前位置

        //鼠标按下事件
        private void MyMouseDown(object sender, MouseEventArgs e)
        {
            m_lastPoint.X = e.X;
            m_lastPoint.Y = e.Y;
            m_endPoint.X = e.X;
            m_endPoint.Y = e.Y;
        }

        //鼠标离开控件的事件
        private void MyMouseLeave(object sender, System.EventArgs e)
        {
            m_MousePointPosition = EnumMousePointPosition.MouseSizeNone;
            this.Cursor = Cursors.Arrow;
        }

        //鼠标移过控件的事件
        private void MyMouseMove(object sender, MouseEventArgs e)
        {
            Control lCtrl = (sender as Control);//获得事件源
            if (lCtrl is CustomControlContainer)
            {
                if (!((CustomControlContainer)lCtrl).IsComplete)
                {
                    return;
                }
            }
            //左键按下移动
            if (e.Button == MouseButtons.Left)
            {
                switch (m_MousePointPosition)
                {
                    case EnumMousePointPosition.MouseDrag: //拖拽移动 修改起点位置Point
                        lCtrl.Left = lCtrl.Left + e.X - m_lastPoint.X;
                        lCtrl.Top = lCtrl.Top + e.Y - m_lastPoint.Y;
                        break;
                    case EnumMousePointPosition.MouseSizeBottom: //下拉高度
                        lCtrl.Height = lCtrl.Height + e.Y - m_endPoint.Y;
                        m_endPoint.X = e.X;
                        m_endPoint.Y = e.Y; //记录光标拖动的当前点                      
                        break;
                    case EnumMousePointPosition.MouseSizeBottomRight: //右下角拉伸 宽度 高度
                        lCtrl.Width = lCtrl.Width + e.X - m_endPoint.X;
                        lCtrl.Height = lCtrl.Height + e.Y - m_endPoint.Y;
                        m_endPoint.X = e.X;
                        m_endPoint.Y = e.Y; //记录光标拖动的当前点                       
                        break;
                    case EnumMousePointPosition.MouseSizeRight: //右边框拉伸 宽度
                        lCtrl.Width = lCtrl.Width + e.X - m_endPoint.X;
                        //lCtrl.Height = lCtrl.Height + e.Y - m_endPoint.Y;
                        m_endPoint.X = e.X;
                        m_endPoint.Y = e.Y; //记录光标拖动的当前点
                        break;
                    case EnumMousePointPosition.MouseSizeTop: //上边框拉伸 高度  和 point y
                        lCtrl.Top = lCtrl.Top + (e.Y - m_lastPoint.Y);
                        lCtrl.Height = lCtrl.Height - (e.Y - m_lastPoint.Y);
                        break;
                    case EnumMousePointPosition.MouseSizeLeft: //左边框拉伸  修改 宽度 和 point x
                        lCtrl.Left = lCtrl.Left + e.X - m_lastPoint.X;
                        lCtrl.Width = lCtrl.Width - (e.X - m_lastPoint.X);
                        break;
                    case EnumMousePointPosition.MouseSizeBottomLeft: //左下角 修改 宽度和高度 point x
                        lCtrl.Left = lCtrl.Left + e.X - m_lastPoint.X;
                        lCtrl.Width = lCtrl.Width - (e.X - m_lastPoint.X);
                        lCtrl.Height = lCtrl.Height + e.Y - m_endPoint.Y;
                        m_endPoint.X = e.X;
                        m_endPoint.Y = e.Y; //记录光标拖动的当前点
                        break;
                    case EnumMousePointPosition.MouseSizeTopRight: //右上角 修改point y 和宽 高
                        lCtrl.Top = lCtrl.Top + (e.Y - m_lastPoint.Y);
                        lCtrl.Width = lCtrl.Width + (e.X - m_endPoint.X);
                        lCtrl.Height = lCtrl.Height - (e.Y - m_lastPoint.Y);
                        m_endPoint.X = e.X;
                        m_endPoint.Y = e.Y; //记录光标拖动的当前点                       
                        break;
                    case EnumMousePointPosition.MouseSizeTopLeft://左上角   修改point y 和宽 高
                        lCtrl.Left = lCtrl.Left + e.X - m_lastPoint.X;
                        lCtrl.Top = lCtrl.Top + (e.Y - m_lastPoint.Y);
                        lCtrl.Width = lCtrl.Width - (e.X - m_lastPoint.X);
                        lCtrl.Height = lCtrl.Height - (e.Y - m_lastPoint.Y);
                        break;
                    default:
                        break;
                }
                if (lCtrl.Width < MinWidth) lCtrl.Width = MinWidth;
                if (lCtrl.Height < MinHeight) lCtrl.Height = MinHeight;
            }
            else
            {
                //'判断光标的位置状态 
                m_MousePointPosition = MousePointPosition(lCtrl.Size, e);
                switch (m_MousePointPosition)  //改变光标
                {
                    case EnumMousePointPosition.MouseSizeNone:
                        this.Cursor = Cursors.Arrow;//箭头
                        break;
                    case EnumMousePointPosition.MouseDrag:
                        this.Cursor = Cursors.SizeAll;//四方向
                        break;
                    case EnumMousePointPosition.MouseSizeBottom:
                        this.Cursor = Cursors.SizeNS;//南北
                        break;
                    case EnumMousePointPosition.MouseSizeTop:
                        this.Cursor = Cursors.SizeNS;//南北
                        break;
                    case EnumMousePointPosition.MouseSizeLeft:
                        this.Cursor = Cursors.SizeWE;//东西
                        break;
                    case EnumMousePointPosition.MouseSizeRight:
                        this.Cursor = Cursors.SizeWE;//东西
                        break;
                    case EnumMousePointPosition.MouseSizeBottomLeft:
                        this.Cursor = Cursors.SizeNESW;//东北到南西
                        break;
                    case EnumMousePointPosition.MouseSizeBottomRight:
                        this.Cursor = Cursors.SizeNWSE;//东南到西北
                        break;
                    case EnumMousePointPosition.MouseSizeTopLeft:
                        this.Cursor = Cursors.SizeNWSE;//东南到西北
                        break;
                    case EnumMousePointPosition.MouseSizeTopRight:
                        this.Cursor = Cursors.SizeNESW;//东北到南西
                        break;
                    default:
                        break;
                }
            }
        }
        //坐标位置判定
        private EnumMousePointPosition MousePointPosition(Size size, System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.X >= -1 * Band) | (e.X <= size.Width) |
                (e.Y >= -1 * Band) | (e.Y <= size.Height))
            {
                if (e.X < Band)
                {
                    if (e.Y < Band)
                    {
                        return EnumMousePointPosition.MouseSizeTopLeft;
                    }
                    else
                    {
                        if (e.Y > -1 * Band + size.Height)
                        {
                            return EnumMousePointPosition.MouseSizeBottomLeft;
                        }
                        else
                        {
                            return EnumMousePointPosition.MouseSizeLeft;
                        }
                    }
                }
                else
                {
                    if (e.X > -1 * Band + size.Width)
                    {
                        if (e.Y < Band)
                        { return EnumMousePointPosition.MouseSizeTopRight; }
                        else
                        {
                            if (e.Y > -1 * Band + size.Height)
                            { return EnumMousePointPosition.MouseSizeBottomRight; }
                            else
                            { return EnumMousePointPosition.MouseSizeRight; }
                        }
                    }
                    else
                    {
                        if (e.Y < Band)
                        { return EnumMousePointPosition.MouseSizeTop; }
                        else
                        {
                            if (e.Y > -1 * Band + size.Height)
                            { return EnumMousePointPosition.MouseSizeBottom; }
                            else
                            { return EnumMousePointPosition.MouseDrag; }
                        }
                    }
                }
            }
            else
            { return EnumMousePointPosition.MouseSizeNone; }
        }


        #endregion

        private void hScrollBar1_SizeChanged(object sender, EventArgs e)
        {
            //label1.Text = hScrollBar1.Value.ToString();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            //label1.Text = hScrollBar1.Value.ToString();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //Form贴图GDI form = new Form贴图GDI();
            //form.Show();
            // 调用

            var singleSelectModel = new SingleSelectModel() { Height = 300 };
            #region 一下为必须设置项 
            singleSelectModel.ObjSimpleItem = (new object[]
                {"Item 1", "Item 2", "Item 3", "Item 4", "Item" + $" 5", "Item 6", "Item 7", "Item 8"});
            singleSelectModel.OneSelect = false;
            //singleSelectModel.TextBoxDispalySelectContext = false;
            singleSelectModel.SelectComplete += SingleSelectModel_SelectComplete;
            singleSelectModel.SetDataSource(ListSourceType.SimpleItem);
            singleSelectModel.ListValueSeparatorChar = '、'; //分隔符
            #endregion 
            PopupControl.Popup pa = new PopupControl.Popup(singleSelectModel);
            pa.Resizable = true;

            pa.RenderMode = ToolStripRenderMode.Professional;
            pa.MinimumSize = new Size(20, 20);
            pa.MaximumSize = new Size(1000, 1000);
            pa.Height = 300;
            pa.Show(button1);
        }

        //接收回调
        private void SingleSelectModel_SelectComplete(object sender, EventArgs e)
        {//在 使用控件时就已知数据对象类型 
            if (sender == null)
            {
                //取消值为空
                return;
            }
            var eExtrend = (EventArgsListSourceType)e;
            switch (eExtrend.CurrentType)
            {
                case ListSourceType.SimpleItem:
                    MessageBox.Show(sender.ToString());
                    break;
                case ListSourceType.Items:
                    if (eExtrend.OneSelect)
                    {
                        //在 使用控件时就已知数据对象类型 
                        var simpleItem = sender as SingelSelectDataModleItems;
                        MessageBox.Show(simpleItem != null
                            ? simpleItem.Value
                            : sender.ToString());
                    }
                    else
                    {
                        var simpleItem = sender as List<SingelSelectDataModleItems>;
                        MessageBox.Show(simpleItem != null
                            ? simpleItem.Count().ToString()
                            : sender.ToString());
                    }
                    break;
                case ListSourceType.BindSouceName:
                    if (eExtrend.OneSelect)
                    {
                        var simpleItem = sender as SingelSelectDataModleItems;
                        MessageBox.Show(simpleItem != null
                            ? simpleItem.Value
                            : sender.ToString());
                    }
                    else
                    {
                        var simpleItem = sender as List<object>;
                        MessageBox.Show(simpleItem != null
                            ? simpleItem.Count().ToString()
                            : sender.ToString());
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void FormImage_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;

        }

        private void FormImage_DragDrop(object sender, DragEventArgs e)
        {
            var imagPath = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            //后缀名判断
            string suffxName = @"BMP(*.bmp) | *.bmp | GIF(*.gif) | *.gif | JPG(*.jpg) | *.jpg | PNG(*.png) | *.png | TIFF(*.tif) | *.tif";
            if (suffxName.Contains(System.IO.Path.GetExtension(imagPath)))
            {
                image = Image.FromFile(imagPath);
                this.Invalidate();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PopupControl.Popup pa = new PopupControl.Popup(new userControlListBoxPanel() { Height = 300 });
            pa.Resizable = true;
            pa.RenderMode = ToolStripRenderMode.Professional;
            pa.MinimumSize = new Size(20, 20);
            pa.MaximumSize = new Size(1000, 1000);
            pa.Height = 300;
            pa.Show(button3);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var singleSelectModel = new SingleSelectModel() { Height = 300 };
            #region 一下为必须设置项 

            singleSelectModel.Items = new List<SingelSelectDataModleItems>
            {
                new SingelSelectDataModleItems
                {
                    DisableText="舒张早期奔马律" ,
                    Value ="有舒张早期奔马律,无舒张晚期奔马律、重叠型奔马律、开瓣音、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="SZZQBML"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="开瓣音" ,
                    Value ="有开瓣音,无舒张早期奔马律、舒张晚期奔马律、重叠型奔马律、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="KBY"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="关瓣音" ,
                    Value ="有关瓣音,无舒张早期奔马律、舒张晚期奔马律、重叠型奔马律、开瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="GBY"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="舒张晚期奔马律" ,
                    Value ="有舒张晚期奔马律,无舒张早期奔马律、重叠型奔马律、开瓣音、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="SZWQBML"
                }
            };
            singleSelectModel.OneSelect = false;
            //singleSelectModel.TextBoxDispalySelectContext = false;
            singleSelectModel.SelectComplete += SingleSelectModel_SelectComplete;
            singleSelectModel.SetDataSource(ListSourceType.Items);
            #endregion

            PopupControl.Popup pa = new PopupControl.Popup(singleSelectModel)
            {
                Resizable = true,
                RenderMode = ToolStripRenderMode.Professional,
                MinimumSize = new Size(20, 20),
                MaximumSize = new Size(1000, 1000),
                Height = 300
            };

            pa.Show(button2);

        }

        //动态绑定多项 
        private void button4_Click(object sender, EventArgs e)
        {
            var singleSelectModel = new SingleSelectModel() { Height = 300 };
            #region 一下为必须设置项 


            singleSelectModel.Items = new List<SingelSelectDataModleItems>
            {
                new SingelSelectDataModleItems
                {
                    DisableText="舒张早期奔马律" ,
                    Value ="有舒张早期奔马律,无舒张晚期奔马律、重叠型奔马律、开瓣音、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="SZZQBML"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="开瓣音" ,
                    Value ="有开瓣音,无舒张早期奔马律、舒张晚期奔马律、重叠型奔马律、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="KBY"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="关瓣音" ,
                    Value ="有关瓣音,无舒张早期奔马律、舒张晚期奔马律、重叠型奔马律、开瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="GBY"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="舒张晚期奔马律" ,
                    Value ="有舒张晚期奔马律,无舒张早期奔马律、重叠型奔马律、开瓣音、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="SZWQBML"
                }
            };
            singleSelectModel.DataSource = new
                SingelSelectDataModleDataSource("DisableText", "Value", singleSelectModel.Items);
            singleSelectModel.OneSelect = false;
            //singleSelectModel.TextBoxDispalySelectContext = false;
            singleSelectModel.SelectComplete += SingleSelectModel_SelectComplete;
            singleSelectModel.SetDataSource(ListSourceType.BindSouceName);
            #endregion

            PopupControl.Popup pa = new PopupControl.Popup(singleSelectModel)
            {
                Resizable = true,
                RenderMode = ToolStripRenderMode.Professional,
                MinimumSize = new Size(20, 20),
                MaximumSize = new Size(1000, 1000),
                Height = 300
            };

            pa.Show(button4);

        }
        //单选
        private void button7_Click(object sender, EventArgs e)
        {

            var singleSelectModel = new SingleSelectModel() { Height = 300 };
            #region 一下为必须设置项 
            singleSelectModel.ObjSimpleItem = (new object[]
                {"Item 1", "Item 2", "Item 3", "Item 4", "Item" + $" 5", "Item 6", "Item 7", "Item 8"});
            singleSelectModel.OneSelect = true;
            singleSelectModel.TextBoxDispalySelectContext = true;
            singleSelectModel.SelectComplete += SingleSelectModel_SelectComplete;
            singleSelectModel.SetDataSource(ListSourceType.SimpleItem);
            singleSelectModel.ListValueSeparatorChar = '、'; //分隔符
            #endregion 
            PopupControl.Popup pa = new PopupControl.Popup(singleSelectModel);
            pa.Resizable = true;

            pa.RenderMode = ToolStripRenderMode.Professional;
            pa.MinimumSize = new Size(20, 20);
            pa.MaximumSize = new Size(1000, 1000);
            pa.Height = 300;
            pa.Show(button7);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var singleSelectModel = new SingleSelectModel() { Height = 300 };
            #region 一下为必须设置项 

            singleSelectModel.Items = new List<SingelSelectDataModleItems>
            {
                new SingelSelectDataModleItems
                {
                    DisableText="舒张早期奔马律" ,
                    Value ="有舒张早期奔马律,无舒张晚期奔马律、重叠型奔马律、开瓣音、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="SZZQBML"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="开瓣音" ,
                    Value ="有开瓣音,无舒张早期奔马律、舒张晚期奔马律、重叠型奔马律、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="KBY"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="关瓣音" ,
                    Value ="有关瓣音,无舒张早期奔马律、舒张晚期奔马律、重叠型奔马律、开瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="GBY"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="舒张晚期奔马律" ,
                    Value ="有舒张晚期奔马律,无舒张早期奔马律、重叠型奔马律、开瓣音、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="SZWQBML"
                }
            };
            singleSelectModel.OneSelect = true;
            //singleSelectModel.TextBoxDispalySelectContext = false;
            singleSelectModel.SelectComplete += SingleSelectModel_SelectComplete;
            singleSelectModel.SetDataSource(ListSourceType.Items);
            #endregion

            PopupControl.Popup pa = new PopupControl.Popup(singleSelectModel)
            {
                Resizable = true,
                RenderMode = ToolStripRenderMode.Professional,
                MinimumSize = new Size(20, 20),
                MaximumSize = new Size(1000, 1000),
                Height = 300
            };

            pa.Show(button6);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var singleSelectModel = new SingleSelectModel() { Height = 300 };
            #region 一下为必须设置项 


            singleSelectModel.Items = new List<SingelSelectDataModleItems>
            {
                new SingelSelectDataModleItems
                {
                    DisableText="舒张早期奔马律" ,
                    Value ="有舒张早期奔马律,无舒张晚期奔马律、重叠型奔马律、开瓣音、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="SZZQBML"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="开瓣音" ,
                    Value ="有开瓣音,无舒张早期奔马律、舒张晚期奔马律、重叠型奔马律、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="KBY"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="关瓣音" ,
                    Value ="有关瓣音,无舒张早期奔马律、舒张晚期奔马律、重叠型奔马律、开瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="GBY"
                },
                new SingelSelectDataModleItems
                {
                    DisableText="舒张晚期奔马律" ,
                    Value ="有舒张晚期奔马律,无舒张早期奔马律、重叠型奔马律、开瓣音、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                    Tag="SZWQBML"
                }
            };
            singleSelectModel.DataSource = new
                SingelSelectDataModleDataSource("DisableText", "Value", singleSelectModel.Items);
            singleSelectModel.OneSelect = true;
            //singleSelectModel.TextBoxDispalySelectContext = false;
            singleSelectModel.SelectComplete += SingleSelectModel_SelectComplete;
            singleSelectModel.SetDataSource(ListSourceType.BindSouceName);
            #endregion

            PopupControl.Popup pa = new PopupControl.Popup(singleSelectModel)
            {
                Resizable = true,
                RenderMode = ToolStripRenderMode.Professional,
                MinimumSize = new Size(20, 20),
                MaximumSize = new Size(1000, 1000),
                Height = 300
            };

            pa.Show(button5);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Maximized;
            //var siz = this.MaximumSize;
            //var workingScreen = Screen.GetWorkingArea(Location);
            //var wScreen = Screen.FromPoint(Location);
            //this.MaximizedBounds = workingScreen;
            //this.Refresh();

            Form贴图GDI sas = new Form贴图GDI();
            sas.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //ScaleBitmapToSize((Bitmap)image, new Size(200, 200)).Save(@"scale2.png");

            CustomControlImageContainer dsd = new CustomControlImageContainer();
            PopupControl.Popup pa = new PopupControl.Popup(dsd)
            {
                Resizable = true,
                RenderMode = ToolStripRenderMode.Professional,
                MinimumSize = new Size(20, 20),
                MaximumSize = new Size(1000, 1000),
                Height = 300
            }; pa.Show(button9);


        }


        #region 图像缩放
        private double LogicalToDeviceUnitsScalingFactor => this.CreateGraphics().DpiX / 96.0;

        private InterpolationMode interpolationMode = InterpolationMode.Invalid;
        private InterpolationMode InterpolationMode
        {
            get
            {
                if (interpolationMode == InterpolationMode.Invalid)
                {
                    int num = (int)Math.Round(LogicalToDeviceUnitsScalingFactor * 100.0);
                    interpolationMode = num % 100 != 0 ? (num >= 100 ? InterpolationMode.HighQualityBicubic : InterpolationMode.HighQualityBilinear) : InterpolationMode.NearestNeighbor;
                }
                return interpolationMode;
            }
        }

        private Bitmap ScaleBitmapToSize(Bitmap logicalImage, Size deviceImageSize)
        {
            Bitmap bitmap = new Bitmap(deviceImageSize.Width, deviceImageSize.Height, logicalImage.PixelFormat);
            using (Graphics graphics = Graphics.FromImage((Image)bitmap))
            {
                graphics.InterpolationMode = InterpolationMode;
                RectangleF srcRect = new RectangleF(0.0f, 0.0f, (float)logicalImage.Size.Width, (float)logicalImage.Size.Height);
                RectangleF destRect = new RectangleF(0.0f, 0.0f, (float)deviceImageSize.Width, (float)deviceImageSize.Height);
                srcRect.Offset(-0.5f, -0.5f);
                graphics.DrawImage((Image)logicalImage, destRect, srcRect, GraphicsUnit.Pixel);
            }
            return bitmap;
        }


        #endregion

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private WheelExtend.MouseWheelHandler szdd = new WheelExtend.MouseWheelHandler();
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);


        }
        private void button10_Click(object sender, EventArgs e)
        {
            BarCodeControl dsd = new BarCodeControl();
            dsd.BarCodeImageChange += Dsd_BarCodeImageChange;
            PopupControl.Popup pa = new PopupControl.Popup(dsd)
            {
                Resizable = true,
                RenderMode = ToolStripRenderMode.Professional,
                MinimumSize = new Size(20, 20),
                MaximumSize = new Size(1000, 1000),
            }; pa.Show(button10);
        }

        private void Dsd_BarCodeImageChange(object sender, HandledEventArgs e)
        {
            if (sender == null) return;
            image = ((BarCodeControl)sender).BarCodeImage;
            //((BarCodeControl)sender).ContextBarCode;
            rectangle = new RectangleF(10, 10, PxConvertMillimetre(image.Width), PxConvertMillimetre(image.Height));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            QRCodeControl dsd = new QRCodeControl();
            dsd.QRCodeImageChange += Dsd_QRCodeImageChange; ;
            PopupControl.Popup pa = new PopupControl.Popup(dsd)
            {
                Resizable = true,
                RenderMode = ToolStripRenderMode.Professional,
                MinimumSize = new Size(20, 20),
                MaximumSize = new Size(1000, 1000),
            }; pa.Show(button11);

        }

        private void Dsd_QRCodeImageChange(object sender, HandledEventArgs e)
        {
            if (sender == null) return;
            image = ((QRCodeControl)sender).QrCodeImage;
            rectangle = new RectangleF(10, 10, PxConvertMillimetre(image.Width), PxConvertMillimetre(image.Height));
        }

        private void button8_Click_1(object sender, EventArgs e)
        {


            var multiSelectModel = new MultiSelectModel() { Height = 300 };
            #region 一下为必须设置项 
            multiSelectModel.ObjSimpleItem = (new object[]
                {"Item 1", "Item 2", "Item 3", "Item 4", "Item" + $" 5", "Item 6", "Item 7", "Item 8"});
            multiSelectModel.OneSelect = false;
            multiSelectModel.TextBoxDispalySelectContext = true;
            multiSelectModel.SelectComplete += SingleSelectModel_SelectComplete;
            multiSelectModel.SetDataSource(ListSourceType.SimpleItem);
            multiSelectModel.ListValueSeparatorChar = '、'; //分隔符
            #endregion 
            PopupControl.Popup pa = new PopupControl.Popup(multiSelectModel);
            pa.Resizable = true;

            pa.RenderMode = ToolStripRenderMode.Professional;
            pa.MinimumSize = new Size(20, 20);
            pa.MaximumSize = new Size(1000, 1000);
            pa.Height = 300;
            pa.Show(button8);
        }


        private void button12_Click(object sender, EventArgs e)
        {
            //Point p = customControlContainer1.PointToClient(MousePosition); 

            DrawRectangleBase recbase = DrawRectangleBase.Create(new Rectangle(customControlContainer1.CurrentFocus.X, customControlContainer1.CurrentFocus.Y, 100, 100), Guid.NewGuid().ToString());
            recbase.BackGroundDescriptText = "我就是一个测试";
            recbase.Stroke = Color.White;
            recbase.Thick = 1;
            recbase.Fill = Color.LightGray;
            customControlContainer1.AddElement(recbase);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


            //资源文件名称不带扩展名
            var asz = (Image)Properties.Resources.ResourceManager.GetObject("Checkbox", System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}


