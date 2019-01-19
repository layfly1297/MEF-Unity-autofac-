using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace screenTest
{
    /// <summary>
    /// OneNode输入编辑框
    /// </summary>
    public partial class CustomControl2OneNodeEditBoxcs : Control
    {
        private BufferedGraphics _bufferedGraphics;
        private BufferedGraphicsContext _bufferedGraphicsContext;
        //private CodeEdit code;
        public CustomControl2OneNodeEditBoxcs()
        {
            InitializeComponent();
            MinimumSize = new Size(125, 125);
            //this.AllowDrop = true;     
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            SetBufferedGraphics();
            //code = new CodeEdit
            //{
            //    Height = this.Height - 10,
            //    Width = this.Width - 2,
            //    BackColor = Color.White,
            //    Location = new Point(1, 10)
            //};
            //code.Dock = DockStyle.None;

            //code.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
            //this.Controls.Add(code);

            //QRCodeControl qRCodeControl = new QRCodeControl {
            //    Height = this.Height - 20,
            //    Width = this.Width - 2, 
            //    Location = new Point(1, 10)
            //};
            //qRCodeControl.Dock = DockStyle.None;  
            //qRCodeControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
            //this.Controls.Add(qRCodeControl);


        }

        private void SetBufferedGraphics()
        {
            //Screen.GetWorkingArea
            int w = this.Width, h = this.Height;
            if (w == 0)
            {
                w = Screen.GetWorkingArea(Control.MousePosition).Width;
            }
            if (h == 0)
            {
                h = Screen.GetWorkingArea(Control.MousePosition).Height;
            }
            _bufferedGraphicsContext = BufferedGraphicsManager.Current;
            _bufferedGraphicsContext.MaximumBuffer = new Size(h, h);
            _bufferedGraphics = _bufferedGraphicsContext.Allocate(this.CreateGraphics(),
                new Rectangle(0, 0, w, h));
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            _bufferedGraphics.Render(pe.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            SetBufferedGraphics();
            base.OnResize(e);
            //code?.Refresh();
        }

        private void DrawTitle()
        {
            var g = _bufferedGraphics.Graphics;
            {
                g.Clear(Color.White);
                using (var bru = new SolidBrush(Color.LightGray))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;//使绘图质量最高，即消除锯齿
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    //绘制样式
                    g.FillRectangle(bru, new RectangleF(0, 0, this.Width, 10));
                    using (var bruz = new SolidBrush(System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(54)))), ((int)(((byte)(82)))))))
                    {
                        g.FillEllipse(bruz, new Rectangle(((this.Width / 2) - 2), 3, 3, 3));
                        g.FillEllipse(bruz, new Rectangle(((this.Width / 2) + 1), 3, 3, 3));
                        g.FillEllipse(bruz, new Rectangle(((this.Width / 2) + 4), 3, 3, 3));
                        g.FillEllipse(bruz, new Rectangle(((this.Width / 2) + 7), 3, 3, 3));
                    }
                    g.DrawImage(Properties.Resources.DataGridViewRow_left.ToBitmap(), new Rectangle(this.Width - 23, 1, 9, 9));
                    g.DrawImage(Properties.Resources.DataGridViewRow_right.ToBitmap(), new Rectangle(this.Width - 11, 1, 9, 9));

                }
                g.DrawRectangle(Pens.LightGray, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                //ControlPaint.DrawGrabHandle(g, new Rectangle(this.Width - 10, this.Height - 10, 10, 10), true, true);
                //ControlPaint.DrawSizeGrip(g,Color.LightGray, new Rectangle(this.Width - 10, this.Height - 10, 10, 10));
            }

        }


        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            //ControlPaint.DrawBorder(_bufferedGraphics.Graphics, new Rectangle(0, 0, this.Width, this.Height), Color.LightSlateGray, ButtonBorderStyle.Solid);
            //////_bufferedGraphics.Graphics.DrawRectangle(Pens.LightSlateGray, new Rectangle(0, 0, this.Width - 1, this.Height - 1));

            //this.Refresh();
            //this.Invalidate();
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            //this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            DrawTitle();
            Form2_Paint(null, null);
            base.OnInvalidated(e);
            //code.Invalidate();
        }



        //获取坐标
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.Text = string.Format("X={0},Y={1}", e.X, e.Y);
            base.OnMouseMove(e);
        }

        /// <summary>
        ///  绘制折线图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            //首先确定原点
            Point centerPoint = new Point(180, 340);
            //自定义一个带有箭头的画笔
            Pen pen = new Pen(Color.Black, 1);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            //得到当前窗体的Graphics对象
            var g = _bufferedGraphics.Graphics;
            //画X轴和Y轴
            //g.DrawLine(pens.Black,centerPoint,new Point(centerPoint.X+600,centerPoint.Y));
            //g.DrawLine(Pens.Black, centerPoint, new Point(centerPoint.X, 40));
            g.DrawLine(pen, centerPoint, new Point(centerPoint.X + 650, centerPoint.Y));
            g.DrawLine(pen, centerPoint, new Point(centerPoint.X, 20));
            //绘制X轴的点
            for (int i = 0; i < 12; i++)
            {
                g.DrawLine(Pens.Black, new Point(centerPoint.X + (i + 1) * 50, centerPoint.Y), new Point(centerPoint.X + (i + 1) * 50, centerPoint.Y - 5));
                g.DrawString((i + 1).ToString() + "月", this.Font, Brushes.Black, new PointF((centerPoint.X + (i + 1) * 50) - 7, centerPoint.Y + 3));
            }
            g.DrawString("X:月份", this.Font, Brushes.Black, new Point(828, 355));
            //绘制Y轴的点
            for (int i = 0; i < 12; i++)
            {
                g.DrawLine(Pens.Black, new Point(centerPoint.X, centerPoint.Y - (i + 1) * 25), new Point(centerPoint.X + 5, centerPoint.Y - (i + 1) * 25));
                //g.DrawLine(Pens.Black, new Point(centerPoint.X , centerPoint.Y), new Point(centerPoint.X + (i + 1) * 50, centerPoint.Y - 5));
                g.DrawString(string.Format("{0}", (i + 1) * 10), this.Font, Brushes.Black, new PointF((centerPoint.X + 5) - 35, (centerPoint.Y - (i + 1) * 25) - 5));
            }
            //计算十二个月销售额对应的坐标点
            double[] data = { 56.2, 66.3, 98.4, 34.5, 55.6, 87.3, 81.4, 33.3, 46.4, 34.6, 114.5, 80.4 };
            PointF[] dataPoint = new PointF[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                float y = (float)(340 - data[i] * 2.5);
                float x = centerPoint.X + (i + 1) * 50;
                PointF point = new PointF(x, y);
                dataPoint[i] = point;
            }
            //绘制十二个点的折线
            for (int i = 0; i < data.Length; i++)
            {
                g.DrawRectangle(Pens.Black, dataPoint[i].X, dataPoint[i].Y, 2, 2);
            }
            //将十二个点串成线
            g.DrawLines(Pens.Black, dataPoint);
            //方法二：Path方法
            //GraphicsPath path = new GraphicsPath();//要导入using System.Drawing.Drawing2D;
            //for (int i = 0; i < data.Length; i++)
            //{
            //    path.AddRectangle(new RectangleF(dataPoint[i], new SizeF(2, 2)));
            //}
            //path.AddLines(dataPoint);
            //g.DrawPath(Pens.Black, path);

            g.DrawString("Y", this.Font, Brushes.Black, new Point(155, 7));
            g.DrawString("销售额：单位（万元）", this.Font, Brushes.Black, new Point(14, 14));
            g.DrawString("某工厂某产品年度销售额图表", this.Font, Brushes.Black, new Point(420, 14));
            pen.Dispose();
        }



    }
}
