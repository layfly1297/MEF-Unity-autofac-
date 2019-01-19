using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using screenTest.PopupControl;
using System.Runtime.InteropServices;

namespace screenTest
{
    public partial class userControlListBoxPanel : UserControl
    {
        #region 滚动条宽度修改

        const int LB_GETHORIZONTALEXTENT = 0x0193;
        const int LB_SETHORIZONTALEXTENT = 0x0194;

        const long WS_HSCROLL = 0x00100000L;

        const int SWP_FRAMECHANGED = 0x0020;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOZORDER = 0x0004;

        const int GWL_STYLE = (-16);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hwnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern void SetWindowLong(IntPtr hwnd, int index, uint value);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
            int Y, int cx, int cy, uint uFlags);


        private void AddStyle(IntPtr handle, uint addStyle)
        {
            // Get current window style
            uint windowStyle = GetWindowLong(handle, GWL_STYLE);

            // Modify style
            SetWindowLong(handle, GWL_STYLE, windowStyle | addStyle);

            // Let the window know of the changes
            SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOZORDER | SWP_NOSIZE | SWP_FRAMECHANGED);
        }

        ////listBox1.HorizontalScrollbar = true;
        ////AddStyle(checkedListBox1.Handle, (uint)WS_HSCROLL);
        //SendMessage(listBox1.Handle, LB_SETHORIZONTALEXTENT, 1000, 0);
        #endregion


        //最近使用点
        private Point lastPoint;
        //最近使用索引
        private int lastIndex = -1;
        public userControlListBoxPanel()
        {
            InitializeComponent();

            this.Padding = new Padding(1);
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            panel1.Parent = listBox1;
            panel1.Height = listBox1.ItemHeight;
            panel1.Width = listBox1.Width;
            panel1.Visible = false;
            panel1.BackColor = Color.Transparent;// ColorTranslator.FromHtml("#E5F3FF");// Color.FromArgb(65, 204, 212, 230);//ColorTranslator.FromHtml("#E5F3FF");// Color.Transparent;
            panel1.Padding = new Padding(0);
            panel1.Margin = new Padding(0);
            panel1.Paint += Panel1_Paint;
            label1.Height = listBox1.ItemHeight;
            listBox1.Margin = new Padding(0);
            label1.Padding = new Padding(0);
            label1.Margin = new Padding(0);
            label1.Font = listBox1.Font;
            listBox1.MouseWheel += ListBox1_MouseWheel;

        }
        WheelExtend.MouseWheelHandler w = new WheelExtend.MouseWheelHandler();
        private void ListBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            int newvalue = listBox1.AutoScrollOffset.Y - w.GetScrollAmount(e) * listBox1.ItemHeight;
            //listBox1.AutoScrollOffset.Offset() 
            //scrollBar.Value = Math.Max(scrollBar.Minimum, Math.Min(scrollBar.Maximum - scrollBar.LargeChange + 1, newvalue));

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            using (var graphics = e.Graphics)
            {
                ControlPaint.DrawBorder(graphics, panel1.DisplayRectangle, Color.LightSeaGreen, ButtonBorderStyle.Solid);
                graphics.Dispose();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //panel1.Location = new Point(panel1.Location.X, (listBox1.ItemHeight * listBox1.SelectedIndex - 1) + 3);
            //label1.Text = listBox1.SelectedItem.ToString();
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





        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            panel1.Visible = false;
            panel1.Invalidate();
        }



        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            listBox1.BeginUpdate();
            base.OnMouseMove(e);
            panel1.Visible = true;
            //this.IndexFromPoint(e.Location);
            var index = listBox1.IndexFromPoint(e.Location);
            if (index == -1 || lastIndex == index)
            {
                panel1.Visible = false;
                return;
            }
            panel1.Location = index != 0
                ? new Point(panel1.Location.X, (listBox1.ItemHeight * (index - listBox1.TopIndex)))
                : new Point(0, 0);
            lastPoint = e.Location;
            lastIndex = index;
            label1.Text = listBox1.Items[index].ToString();
            panel1.Invalidate();
            listBox1.EndUpdate();


        }

        private void panel1_Click(object sender, EventArgs e)
        {

            var asr = listBox1.PreferredHeight / listBox1.ItemHeight;
            var index = listBox1.IndexFromPoint(lastPoint);
            if (index != -1)
            {
                listBox1.SelectedIndex = index;
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //this.listBox1.Items.Add("new line");
            //滚动到底部
            //this.listBox1.TopIndex = this.listBox1.Items.Count - (int)(this.listBox1.Height / this.listBox1.ItemHeight);

            //this.listBox1.SelectedIndex = int.Parse(textBox1.Text);
            // this.listBox1.SelectedIndex = -1;
            if (textBox1.Text.TrimEnd().Length > 0)
            {
                //查找内容 
                var inde = listBox1.FindStringExact(textBox1.Text);
                this.listBox1.SelectedIndex = inde;
            }
        }


    }
}
