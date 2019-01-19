using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace screenTest
{
    public partial class UserControl2Listbox : ListBox
    {
        private List<KeyValuePair<Rectangle, int>> recCbLst;
        public UserControl2Listbox()
        {
            InitializeComponent();

            string[] list = new string[] { "张三", "李四", "王五" };
            recCbLst = new List<KeyValuePair<Rectangle, int>>();
            int x = 0, y = 0;
            for (int i = 0; i < list.Count(); i++)
            {
                CheckBox cb = new CheckBox();
                cb.Text = list[i];
                cb.Location = new Point(x, y);
                Controls.Add(cb);
                var rect = new Rectangle(cb.Location, cb.Size);
                var keyvalue = new KeyValuePair<Rectangle, int>(rect, i);
                recCbLst.Add(keyvalue);
                y += 22;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {

            ////var index = this.IndexFromPoint(e.Location);
            //var keyv = this.recCbLst.Where(p => p.Key.Contains(e.Location)).ToArray();
            //var index = -1;
            //if (keyv.Length > 0)
            //{
            //    index = keyv[0].Value;
            //}


            //if (index == -1) return;
            //if (this.Items[index] is CheckBox)
            //{
            //    (this.Items[index] as CheckBox).BackColor = Color.CornflowerBlue;
            //    this.Invalidate();
            //}
            base.OnMouseMove(e);
        }
    }
}
