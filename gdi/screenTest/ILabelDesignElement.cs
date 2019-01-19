using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;  
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging; 
namespace screenTest
{
    ///// <summary>
    ///// 模板元素接口
    ///// </summary>
    //public interface ILabelDesignElement
    //{
    //    /// <summary>
    //    /// PrintData/codeContext里的字段，{} [] 
    //    /// </summary>
    //    string 动态内容 { get; set; }
    //    /// <summary>
    //    /// 是否被选中
    //    /// </summary>
    //    bool DesignSelected { get; set; }
    //    /// <summary>
    //    /// 选择状态发生改变
    //    /// </summary>
    //    event Action<object, bool> SelectedStatusChange;
    //    /// <summary>
    //    /// 本控件被选中时键盘方向键被按下
    //    /// </summary>
    //    /// <param name="keyData"></param>
    //    void KeysChangedLocation(Keys keyData);
    //}

     
    //    /// <summary>
    //    /// 设计时控件基类
    //    /// </summary>
    //    public abstract class DesignCellControl : PictureBox, ILabelDesignElement
    //    {
    //        #region 鼠标移动和缩放
    //        private enum EnumMousePointPosition
    //        {
    //            MouseSizeNone = 0, //'无    
    //            MouseSizeRight = 1, //'拉伸右边框    
    //            MouseSizeLeft = 2, //'拉伸左边框    
    //            MouseSizeBottom = 3, //'拉伸下边框    
    //            MouseSizeTop = 4, //'拉伸上边框    
    //            MouseSizeTopLeft = 5, //'拉伸左上角    
    //            MouseSizeTopRight = 6, //'拉伸右上角    
    //            MouseSizeBottomLeft = 7, //'拉伸左下角    
    //            MouseSizeBottomRight = 8, //'拉伸右下角    
    //            MouseDrag = 9   // '鼠标拖动    
    //        }
    //        const int Band = 5;
    //        const int MinWidth = 10;
    //        const int MinHeight = 10;
    //        private EnumMousePointPosition m_MousePointPosition;
    //        private Point p, p1;
    //        private EnumMousePointPosition MousePointPosition(Size size, System.Windows.Forms.MouseEventArgs e)
    //        {

    //            if ((e.X >= -1 * Band) | (e.X <= size.Width) | (e.Y >= -1 * Band) | (e.Y <= size.Height))
    //            {
    //                if (e.X < Band)
    //                {
    //                    if (e.Y < Band) { return EnumMousePointPosition.MouseSizeTopLeft; }
    //                    else
    //                    {
    //                        if (e.Y > -1 * Band + size.Height)
    //                        { return EnumMousePointPosition.MouseSizeBottomLeft; }
    //                        else
    //                        { return EnumMousePointPosition.MouseSizeLeft; }
    //                    }
    //                }
    //                else
    //                {
    //                    if (e.X > -1 * Band + size.Width)
    //                    {
    //                        if (e.Y < Band)
    //                        { return EnumMousePointPosition.MouseSizeTopRight; }
    //                        else
    //                        {
    //                            if (e.Y > -1 * Band + size.Height)
    //                            { return EnumMousePointPosition.MouseSizeBottomRight; }
    //                            else
    //                            { return EnumMousePointPosition.MouseSizeRight; }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (e.Y < Band)
    //                        { return EnumMousePointPosition.MouseSizeTop; }
    //                        else
    //                        {
    //                            if (e.Y > -1 * Band + size.Height)
    //                            { return EnumMousePointPosition.MouseSizeBottom; }
    //                            else
    //                            { return EnumMousePointPosition.MouseDrag; }
    //                        }
    //                    }
    //                }
    //            }
    //            else
    //            { return EnumMousePointPosition.MouseSizeNone; }
    //        }
    //        #endregion
    //        public bool DesignSelected
    //        {
    //            get
    //            {
    //                return designSelected;
    //            }
    //            set
    //            {
    //                designSelected = value;
    //                Invalidate();
    //            }
    //        }
    //        private bool designSelected = false;
    //        public DynamicMapProperty DynamicMapProperty { get; set; }
    //        public StaticMapProperty StaticMapProperty { get; set; }
    //        public string 动态内容 { get; set; }
    //        public RoteDescription RoteDescription { get; set; }
    //        /// <summary>
    //        /// 被选中，获取到焦点的事件
    //        /// </summary>
    //        public event Action<object, bool> SelectedStatusChange;
    //        protected override void OnClick(EventArgs e)
    //        {
    //            DesignSelected = true;
    //            SelectedStatusChange?.Invoke(this, DesignSelected);
    //        }
    //        protected override void OnMouseWheel(MouseEventArgs e)
    //        {
    //            base.OnMouseWheel(e);
    //            double d = 1.068D;
    //            if (e.Delta > 0 && DesignSelected)
    //            {
    //                this.Size = new Size((int)(this.Size.Width * d), (int)(this.Size.Height * d));
    //            }
    //            else
    //            {
    //                this.Size = new Size((int)(this.Size.Width / d), (int)(this.Size.Height / d));
    //            }
    //        }
    //        protected override void OnMouseDown(MouseEventArgs e)
    //        {
    //            p.X = e.X;
    //            p.Y = e.Y;
    //            p1.X = e.X;
    //            p1.Y = e.Y;
    //        }
    //        protected override void OnMouseUp(MouseEventArgs e)
    //        {
    //            m_MousePointPosition = EnumMousePointPosition.MouseSizeNone;
    //            this.Cursor = Cursors.Arrow;
    //        }
    //        protected override void OnMouseMove(MouseEventArgs e)
    //        {
    //            if (e.Button == MouseButtons.Left)
    //            {
    //                switch (m_MousePointPosition)
    //                {
    //                    #region 位置计算
    //                    case EnumMousePointPosition.MouseDrag:
    //                        Left = Left + e.X - p.X;
    //                        Top = Top + e.Y - p.Y;
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeBottom:
    //                        Height = Height + e.Y - p1.Y;
    //                        p1.X = e.X;
    //                        p1.Y = e.Y; //'记录光标拖动的当前点    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeBottomRight:
    //                        Width = Width + e.X - p1.X;
    //                        Height = Height + e.Y - p1.Y;
    //                        p1.X = e.X;
    //                        p1.Y = e.Y; //'记录光标拖动的当前点    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeRight:
    //                        Width = Width + e.X - p1.X;
    //                        Height = Height + e.Y - p1.Y;
    //                        p1.X = e.X;
    //                        p1.Y = e.Y; //'记录光标拖动的当前点    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeTop:
    //                        Top = Top + (e.Y - p.Y);
    //                        Height = Height - (e.Y - p.Y);
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeLeft:
    //                        Left = Left + e.X - p.X;
    //                        Width = Width - (e.X - p.X);
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeBottomLeft:
    //                        Left = Left + e.X - p.X;
    //                        Width = Width - (e.X - p.X);
    //                        Height = Height + e.Y - p1.Y;
    //                        p1.X = e.X;
    //                        p1.Y = e.Y; //'记录光标拖动的当前点    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeTopRight:
    //                        Top = Top + (e.Y - p.Y);
    //                        Width = Width + (e.X - p1.X);
    //                        Height = Height - (e.Y - p.Y);
    //                        p1.X = e.X;
    //                        p1.Y = e.Y; //'记录光标拖动的当前点    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeTopLeft:
    //                        Left = Left + e.X - p.X;
    //                        Top = Top + (e.Y - p.Y);
    //                        Width = Width - (e.X - p.X);
    //                        Height = Height - (e.Y - p.Y);
    //                        break;
    //                    default:
    //                        break;
    //                        #endregion
    //                }
    //                if (Width < MinWidth) Width = MinWidth;
    //                if (Height < MinHeight) Height = MinHeight;
    //                if (Tag != null)
    //                {
    //                    if (Tag is ImageElementNode)
    //                    {
    //                        var tag = Tag as ImageElementNode;
    //                        tag.Location = Location;
    //                    }
    //                    else if (Tag is BarcodeElementNode)
    //                    {
    //                        var tag = Tag as BarcodeElementNode;
    //                        tag.Location = Location;
    //                    }
    //                    else if (Tag is TextBoxElementNode)
    //                    {
    //                        var tag = Tag as TextBoxElementNode;
    //                        tag.Location = Location;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                m_MousePointPosition = MousePointPosition(Size, e);
    //                switch (m_MousePointPosition)
    //                {
    //                    #region 改变光标
    //                    case EnumMousePointPosition.MouseSizeNone:
    //                        this.Cursor = Cursors.Arrow;        //'箭头    
    //                        break;
    //                    case EnumMousePointPosition.MouseDrag:
    //                        this.Cursor = Cursors.SizeAll;      //'四方向    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeBottom:
    //                        this.Cursor = Cursors.SizeNS;       //'南北    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeTop:
    //                        this.Cursor = Cursors.SizeNS;       //'南北    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeLeft:
    //                        this.Cursor = Cursors.SizeWE;       //'东西    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeRight:
    //                        this.Cursor = Cursors.SizeWE;       //'东西    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeBottomLeft:
    //                        this.Cursor = Cursors.SizeNESW;     //'东北到南西    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeBottomRight:
    //                        this.Cursor = Cursors.SizeNWSE;     //'东南到西北    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeTopLeft:
    //                        this.Cursor = Cursors.SizeNWSE;     //'东南到西北    
    //                        break;
    //                    case EnumMousePointPosition.MouseSizeTopRight:
    //                        this.Cursor = Cursors.SizeNESW;     //'东北到南西    
    //                        break;
    //                    default:
    //                        break;
    //                        #endregion
    //                }
    //            }
    //        }

    //        /// <summary>
    //        /// 绘制方框
    //        /// </summary>
    //        /// <param name="g"></param>
    //        protected void DrawSelectedStatus(Graphics g)
    //        {
    //            Rectangle rect = ClientRectangle;
    //            rect.Inflate(-6, -6);
    //            using (Pen p = new Pen(Brushes.Black, 1))
    //            {
    //                p.DashStyle = DashStyle.Dot;
    //                p.DashStyle = DashStyle.Solid;
    //                //8个方块
    //                g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 4, rect.Top - 4, 4, 4));
    //                g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top - 4, 4, 4));
    //                g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top - 4, 4, 4));
    //                g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 4, rect.Top + rect.Height / 2 - 3, 4, 4));
    //                g.FillRectangle(Brushes.White, new Rectangle(rect.Left - 4, rect.Top + rect.Height, 4, 4));
    //                g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height / 2 - 3, 4, 4));
    //                g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top + rect.Height, 4, 4));
    //                g.FillRectangle(Brushes.White, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height, 4, 4));
    //                g.DrawRectangle(p, new Rectangle(rect.Left - 4, rect.Top - 4, 4, 4));
    //                g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top - 4, 4, 4));
    //                g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top - 4, 4, 4));
    //                g.DrawRectangle(p, new Rectangle(rect.Left - 4, rect.Top + rect.Height / 2 - 3, 4, 4));
    //                g.DrawRectangle(p, new Rectangle(rect.Left - 4, rect.Top + rect.Height, 4, 4));
    //                g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height / 2 - 3, 4, 4));
    //                g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width / 2 - 3, rect.Top + rect.Height, 4, 4));
    //                g.DrawRectangle(p, new Rectangle(rect.Left + rect.Width, rect.Top + rect.Height, 4, 4));
    //            }
    //        }
    //        /// <summary>
    //        /// 旋转图片
    //        /// </summary>
    //        /// <param name="bmp">源图</param>
    //        /// <param name="angle">角度</param>
    //        /// <param name="bkColor">背景色</param>
    //        /// <returns></returns>
    //        protected Bitmap ImageRotate(Bitmap bmp, float angle, Color bkColor)
    //        {
    //            int w = bmp.Width + 2;
    //            int h = bmp.Height + 2;
    //            PixelFormat pf;
    //            if (bkColor == Color.Transparent)
    //            {
    //                pf = PixelFormat.Format32bppArgb;
    //            }
    //            else
    //            {
    //                pf = bmp.PixelFormat;
    //            }
    //            Bitmap tmp = new Bitmap(w, h, pf);
    //            Graphics g = Graphics.FromImage(tmp);
    //            g.Clear(bkColor);
    //            g.DrawImageUnscaled(bmp, 1, 1);
    //            g.Dispose();
    //            GraphicsPath path = new GraphicsPath();
    //            path.AddRectangle(new RectangleF(0f, 0f, w, h));
    //            Matrix mtrx = new Matrix();
    //            mtrx.Rotate(angle);
    //            RectangleF rct = path.GetBounds(mtrx);
    //            Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
    //            g = Graphics.FromImage(dst);
    //            g.Clear(bkColor);
    //            g.TranslateTransform(-rct.X, -rct.Y);
    //            g.RotateTransform(angle);
    //            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
    //            g.DrawImageUnscaled(tmp, 0, 0);
    //            g.Dispose();
    //            tmp.Dispose();
    //            return dst;
    //        }
    //        /// <summary>
    //        /// 响应键盘光标键
    //        /// </summary>
    //        /// <param name="keyData"></param>
    //        public virtual void KeysChangedLocation(Keys keyData)
    //        {
    //            if (keyData == Keys.Up)
    //            {
    //                Top -= 1;
    //            }
    //            if (keyData == Keys.Down)
    //            {
    //                Top += 1;
    //            }
    //            if (keyData == Keys.Left)
    //            {
    //                Left -= 1;
    //            }
    //            if (keyData == Keys.Right)
    //            {
    //                Left += 1;
    //            }
    //            if (Tag != null)
    //            {
    //                if (Tag is ImageElementNode)
    //                {
    //                    var tag = Tag as ImageElementNode;
    //                    tag.Location = Location;
    //                }
    //                else if (Tag is BarcodeElementNode)
    //                {
    //                    var tag = Tag as BarcodeElementNode;
    //                    tag.Location = Location;
    //                }
    //                else if (Tag is TextBoxElementNode)
    //                {
    //                    var tag = Tag as TextBoxElementNode;
    //                    tag.Location = Location;
    //                }
    //            }
    //        }
    //    }
     
}
