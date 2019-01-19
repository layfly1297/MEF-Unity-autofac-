using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using DrawEverything;
using screenTest.PopupControl;

namespace screenTest
{
    public sealed partial class CustomControlImageContainer : Control
    {

#pragma warning disable 649
        //缓冲上下文
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly BufferedGraphicsContext _bufferedGraphicsContext;

        private BufferedGraphics _bufferedGraphics;
#pragma warning restore 649

        //最近点击坐标
        private Point _lastPoint;
        //图像容器
        private ImageContainer _imageContainer;

        private readonly Pen _pen = new Pen(Color.Black);
        //是否完成初始化
        private bool _Isinitialize = false;

        //缩放比例
        private float scaleW = 1f;
        private float scaleH = 1f;
        private float scale = 1f;
        private WheelExtend.MouseWheelHandler wheel = new WheelExtend.MouseWheelHandler();

        private List<object> elementList;
        public CustomControlImageContainer()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            Height = 500;
            Width = 800;
            _imageContainer = new ImageContainer(2, 4, new Size(500, 300), new Point(30, 30));
            _imageContainer.RedrawImageContainer += CustomControl1_RedrawImageContainer;
            _bufferedGraphicsContext = BufferedGraphicsManager.Current;
            _bufferedGraphicsContext.MaximumBuffer = new Size(this.Width, this.Height);
            _bufferedGraphics = _bufferedGraphicsContext.Allocate(this.CreateGraphics(),
                new Rectangle(0, 0, this.Width + 1, Height + 1));
            scaleW = (float)_imageContainer.WidthHeight.Width / Width;
            scaleH = (float)_imageContainer.WidthHeight.Height / Height;

            scale = (float)Width / Height; //oldw/oldh= neww/newh , neww=oldw/oldh*newh 、newh= w/scale

            _Isinitialize = true;

            elementList = new List<object>();
            //添加1
            DrawRectangleBase recbase = DrawRectangleBase.Create(new Rectangle(0, 0, 100, 100), "ele1");
            recbase.Stroke = Color.Black;
            recbase.Thick = 1;
            recbase.Fill = Color.Red;
            elementList.Add(recbase);

            DrawImage imag = DrawImage.Create(new DataStructrueImage()
            {
                //Href = @"C:\Users\lican\Desktop\screenTest\bin\Debug\ioc.png",
                X = 50,
                Y = 50,
                Width = 100,
                Height = 100,
                image = Properties.Resources.ioc
            });
            elementList.Add(imag);
        }

        private void CustomControl1_RedrawImageContainer()
        {
            //this.Invalidate();
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_Isinitialize)
            {
                //大小缩放 
                var w = (int)(scaleW * Width);
                var h = (int)(scaleH * Height);
                _imageContainer.WidthHeight = new Size(w, h);
                //缩放比 宽高比
                //w = (int)(scale * Height);
                //h = (int)(Width / scale);
                //_imageContainer.WidthHeight = new Size(w, h);
                DrawPart();
                DrawSingleSelect();
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            //判断是否在容器内
            Rectangle rectangle = new Rectangle(_imageContainer.Location, _imageContainer.WidthHeight);
            if (rectangle.Contains(_lastPoint))
            {
                base.OnDragDrop(drgevent);
                var imagPath = ((System.Array)drgevent.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                //后缀名判断
                string suffxName = @"BMP(*.bmp) | *.bmp | GIF(*.gif) | *.gif | JPG(*.jpg) | *.jpg | PNG(*.png) | *.png | TIFF(*.tif) | *.tif";
                if (suffxName.Contains(System.IO.Path.GetExtension(imagPath)))
                {
                    var image = Image.FromFile(imagPath);
                    var sumLineHeight = _imageContainer.WidthHeight.Height / _imageContainer.RowCount;
                    //行号
                    int insetRow = 0;
                    //判断在哪一行 
                    for (int i = 1; i < _imageContainer.RowCount; i++)
                    {
                        var oneImageHeight = _imageContainer.Location.Y + (_imageContainer.RowSpacing + sumLineHeight) * i;
                        if (oneImageHeight >= _lastPoint.Y)
                        {
                            insetRow = i;
                            break;
                        }
                    }
                    int rowImageIndex = 0;
                    //判断哪一个图像位
                    for (int i = 0; i < _imageContainer.LineMaxImageCount; i++)
                    {
                        var oneImageWidth = _imageContainer.Location.X + _imageContainer.WidthHeight.Width;
                        if (oneImageWidth >= _lastPoint.X)
                        {
                            rowImageIndex = i;
                            break;
                        }
                    }
                    var index = insetRow * _imageContainer.LineMaxImageCount + rowImageIndex;
                    //图像大小设置 进行缩放 暂未做
                    var size = new Size();
                    Bitmap bitmap = new Bitmap(image, size);
                    _imageContainer.CollectImage.Insert(index, bitmap);
                    //this.Invalidate();
                }
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {

            base.OnDragEnter(drgevent);
            drgevent.Effect = drgevent.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;

        }
        void DrawPart()
        {
            _bufferedGraphics.Dispose();
            //如果最小等于0时设置对象大小位100
            if (this.Width == 0 || this.Height == 0)
            {
                _bufferedGraphicsContext.MaximumBuffer = new Size(10, 10);
            }
            else
            {
                _bufferedGraphicsContext.MaximumBuffer = new Size(this.Width, this.Height);
            }
            _bufferedGraphics = _bufferedGraphicsContext.Allocate(this.CreateGraphics(),
                new Rectangle(0, 0, this.Width + 1, Height + 1));
            //_bufferedGraphics.Graphics.SmoothingMode=   System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            SolidBrush brush = new SolidBrush(Color.FromArgb(73, 90, 128));
            _bufferedGraphics.Graphics.FillRectangle(brush, 0, 0, this.Width, this.Height);
            brush.Dispose();

            Console.WriteLine("w: " + this.Width);
            Console.WriteLine("h: " + this.Height);
            Console.WriteLine("wi: " + this._imageContainer.WidthHeight.Width);
            Console.WriteLine("hi: " + this._imageContainer.WidthHeight.Height);
            //绘制
            //1.画容器
            _bufferedGraphics.Graphics.DrawRectangle(_pen,
                new Rectangle(_imageContainer.Location, _imageContainer.WidthHeight));
            //2.绘制行线
            var sumLineHeight = _imageContainer.WidthHeight.Height / _imageContainer.RowCount;
            for (int i = 1; i < _imageContainer.RowCount; i++)
            {
                var startPoint = new Point(_imageContainer.Location.X, _imageContainer.Location.Y + (_imageContainer.RowSpacing + sumLineHeight) * i);
                var endPoint = new Point(_imageContainer.Location.X + _imageContainer.WidthHeight.Width, _imageContainer.Location.Y + (_imageContainer.RowSpacing + sumLineHeight) * i);
                _bufferedGraphics.Graphics.DrawLine(_pen, startPoint, endPoint);
            }
            //绘制图像对象
            //foreach (var item in _imageContainer.CollectImage)
            //{

            //}
            OnDraw();
        }

        private void OnDraw()
        {
            Graphics g = _bufferedGraphics.Graphics;
            {
                //73 90 128  背景清理
                //g.Clear(Color.FromArgb(73, 90, 128));

                if (elementList.Count > 0)
                {
                    if (elementList[0] is DrawRectangleBase)
                    {
                        var bas = (DrawRectangleBase)elementList[0];
                        if (bas.Selected)
                        {
                            bas.DrawTracker(g);
                        }
                        bas.Draw(g);
                    }

                    if (elementList[1] is DrawImage)
                    {
                        var bas = (DrawImage)elementList[1];
                        if (bas.Selected)
                        {
                            bas.DrawTracker(g);
                        }
                        bas.Draw(g);
                    }
                }
            }
        }

        void DrawSingleSelect()
        {
            //bufferedSet();
            _bufferedGraphics.Graphics.FillRectangle(SystemBrushes.ControlDarkDark, 80, 60, 100, 40);
            _bufferedGraphics.Graphics.DrawString("药品字典", SystemFonts.DefaultFont, SystemBrushes.WindowText, 85, 62);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            _bufferedGraphics.Render(pe.Graphics);
            base.OnPaint(pe);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //base.OnMouseMove(e);

            //var bas = (DrawRectangleBase)elementList[0];
            //if (bas.Selected && MouseButtons == MouseButtons.Left)
            //{
            //    float dx = e.X - _lastPoint.X;
            //    float dy = e.Y - _lastPoint.Y;
            //    bas.Move(dx, dy);
            //    _lastPoint = e.Location;
            //    this.Invalidate();
            //}

            //var bas2 = (DrawImage)elementList[1];
            //if (bas2.Selected && MouseButtons == MouseButtons.Left)
            //{
            //    float dx = e.X - _lastPoint.X;
            //    float dy = e.Y - _lastPoint.Y;
            //    bas2.Move(dx, dy);
            //    _lastPoint = e.Location;
            //    this.Invalidate();
            //}
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            //base.OnMouseDown(e);
            _lastPoint = e.Location;
            if (elementList.Count > 0)
            {

                if (elementList[0] is DrawRectangleBase)
                {
                    var bas = (DrawRectangleBase)elementList[0];
                    if (bas.HitTest(e.Location) != -1)
                    {
                        bas.Selected = true;
                        this.Invalidate();
                    }
                    else
                    {
                        bas.Selected = false;
                    }
                }
                if (elementList[1] is DrawImage)
                {
                    var bas = (DrawImage)elementList[1];
                    if (bas.HitTest(e.Location) != -1)
                    {
                        bas.Selected = true;
                        this.Invalidate();
                    }
                    else
                    {
                        bas.Selected = false;
                    }
                }
            }

        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            DrawPart();
            base.OnInvalidated(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            //base.OnMouseUp(e);
            var bas = (DrawRectangleBase)elementList[0];
            bas.Selected = false;

            var bas2 = (DrawImage)elementList[1];
            bas2.Selected = false;
            base.OnMouseUp(e);
        }



        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var wheelValue = 0;
            base.OnMouseWheel(e);
            wheelValue += wheel.GetScrollAmount(e);
            Console.WriteLine($@"滚动值：{wheelValue}");
            if (wheelValue != 0)
            {
                this.SuspendLayout();
                //放大 
                if (wheelValue > 0)
                {
                    int fd = (wheelValue / 3);
                    this.Width = this.Width + fd;
                    this.Height = this.Height + fd;
                }
                else
                {
                    //缩小
                    int sx = Math.Abs(wheelValue / 3);
                    this.Width = this.Width - sx;
                    this.Height = this.Height - sx;
                }

                this.Refresh();
                this.ResumeLayout();

            }
        }

        protected override void WndProc(ref Message m)
        {
            var popup = Parent as Popup;
            if (popup != null && popup.ProcessResizing(ref m))
            {
                return;
            }
            base.WndProc(ref m);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            //双击
            Rectangle rectangle = new Rectangle(80, 60, 100, 40);
            if (rectangle.Contains(e.Location))
            {
                PopupControl.Popup pa = new PopupControl.Popup(new CustomControlImageContainer())
                {
                    Resizable = true,
                    RenderMode = ToolStripRenderMode.Professional,
                    Height = 100,
                    Width = 200
                };

                //pa.MinimumSize = new Size(20, 20);
                //pa.MaximumSize = new Size(1000, 1000);
                pa.Show(this, 80, 100);
            }
        }

        //图像容器
        class ImageContainer : IDisposable
        {
            private List<Image> _collectImage;
            private Image _currentImage;
            private int _rowCount;
            private int _lineMaxImageCount;
            private Size _widthHeight;
            private Point _location;
            private readonly int _rowSpacing = 5;
            private readonly int _margin = 10;


            public event ImageContainer.DtawImageContainer RedrawImageContainer;

            public delegate void DtawImageContainer();

            //一行最大图像数
            public int LineMaxImageCount
            {
                get { return _lineMaxImageCount; }
                set { _lineMaxImageCount = value; }
            }

            //行数
            public int RowCount
            {
                get { return _rowCount; }
                set { _rowCount = value; }
            }

            //当前图片
            public Image CurrentImage
            {
                get { return _currentImage; }
                set { _currentImage = value; }
            }
            //图像集合
            public List<Image> CollectImage
            {
                get { return _collectImage; }
                set
                {
                    _collectImage = value;
                    RedrawImageContainer?.Invoke();
                }
            }
            //大小
            public Size WidthHeight
            {
                get { return _widthHeight; }
                set { _widthHeight = value; }
            }
            //位置
            public Point Location
            {
                get { return _location; }
                set { _location = value; }
            }

            //行间距 
            public int RowSpacing
            {
                get { return _rowSpacing; }
            }

            //默认图像列间距
            public int ColoumMargin
            {
                get { return _margin; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="rowCount">行数</param>
            /// <param name="lineMaxImageCount">行最大图像数</param>
            /// <param name="widthHeight">大小</param>
            /// <param name="location">位置</param>
            public ImageContainer(int rowCount, int lineMaxImageCount, Size widthHeight, Point location)
            {
                _rowCount = rowCount;
                _lineMaxImageCount = lineMaxImageCount;
                _widthHeight = widthHeight;
                _location = location;

            }
            //释放
            public void Dispose()
            {
                _currentImage?.Dispose();
                _collectImage.Clear();
            }
        }

    }
}
