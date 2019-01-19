using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace screenTest
{
    /// <summary>
    /// 绘制焦点选择框
    /// </summary>
    public partial class DrawFocusSelectionBox : UserControl
    {
        //ControlPaint.DrawFocusRectangle(e.Graphics, this.focusRectangle, Color.Black, Color.Transparent);
        public DrawFocusSelectionBox()
        {
            InitializeComponent();
            //省略部分代码
            this.MouseUp += this.DrawArea_MouseUp;
            this.Paint += this.DrawArea_Paint;
            this.MouseMove += this.DrawArea_MouseMove;
            this.MouseDown += this.DrawArea_MouseDown;
            this.ResumeLayout(false);
            Init(true);
            
        }
 

        private Point startPoint;
        private Rectangle focusRectangle;
        private bool bDrawFocusRectangle;

        public void Init(bool isFill)
        { 
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            //SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            //if (isFill)
            //    this.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// 获取绘制的方框。
        /// 我们选择时，可能是由左到右，由上到下，也有可能由下到上，由右到左。
        /// 为些程序需要判断一下。
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static Rectangle GetNormalizedRectangle(int x1, int y1, int x2, int y2)
        {
            if (x2 < x1)
            {
                int tmp = x2;
                x2 = x1;
                x1 = tmp;
            }

            if (y2 < y1)
            {
                int tmp = y2;
                y2 = y1;
                y1 = tmp;
            }

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        private void DrawArea_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.bDrawFocusRectangle = true;
                this.focusRectangle = GetNormalizedRectangle(this.startPoint.X, this.startPoint.Y, e.X, e.Y);
                this.Invalidate();//重新绘制
            }
        }

        private void DrawArea_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.startPoint = new Point(e.X, e.Y);
        }

        private void DrawArea_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (this.bDrawFocusRectangle)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, this.focusRectangle, Color.Black, Color.Transparent);
                //e.Graphics.Clear(BackColor);
                //e.Graphics.FillRectangle(SystemBrushes.ButtonFace, this.focusRectangle);
            }
        }

        private void DrawArea_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.bDrawFocusRectangle = false;
            this.Refresh();
        }
    }
}
