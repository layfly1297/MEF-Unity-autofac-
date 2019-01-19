/*************************************************
 * 描述：
 * 
 * Author：lican@mozihealthcare.cn
 * Date：2018/12/26 10:25:53
 * Update：
 * ************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace screenTest
{
    public class CheckBoxListReset : CheckedListBox
    {
        private Rectangle rect = new Rectangle();
        readonly Pen pen = new Pen(Color.Blue);
        private int index = -1;
        protected override void OnPaint(PaintEventArgs e)
        {
            //var gphic = e.Graphics;
            //e.Graphics.Clear(Color.Blue);
            //e.Graphics.DrawRectangle(pen, rect);
            base.OnPaint(e);
        }


        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            //if (e.Index == index)
            //{
            //    DrawItemEventArgs e2 = new DrawItemEventArgs(e.Graphics,
            //        e.Font, e.Bounds, e.Index, e.State, e.ForeColor, Color.DarkGreen);

            //    base.OnDrawItem(e2);
            //}
            //else
            //{
            //    base.OnDrawItem(e);
            //}
            base.OnDrawItem(e);
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.Opaque, true);
            DoubleBuffered = true;
            base.OnMouseMove(e);
            //获取鼠标停留上的项索引
            index = this.IndexFromPoint(e.Location);
            if (index != -1)
            {
                rect = this.GetItemRectangle(index);
                ////OnDrawItem()
                this.Refresh();
                ////ControlPaint.
                //this.Invalidate();
                //this.CreateGraphics().CompositingMode = CompositingMode.SourceCopy;
                //Pen solidBrush = new Pen(Color.LightSeaGreen);
                //this.CreateGraphics().DrawRectangle(solidBrush, rect);

                ControlPaint.DrawBorder(this.CreateGraphics(), rect, Color.LightSeaGreen, ButtonBorderStyle.Solid);

            }

        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            pen.Dispose();
        }
    }
}
