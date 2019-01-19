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
    public partial class CustomControl1 : Control
    {
        private FlowLayoutPanel flayout;
        public CustomControl1()
        {
            InitializeComponent();
            List<string> lst = new List<string> { "123", "1234", "234324", "80980980123" };
            flayout = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(3) };
            for (int i = 1; i < lst.Count; i++)
            {
                CheckBox chebox = new CheckBox()
                {
                    Height = 20,
                    Width = 100,
                    Name = i.ToString(),
                    Text = lst[i],
                    TextAlign = ContentAlignment.MiddleLeft,
                    Dock = DockStyle.None,
                    Location = new Point(0, i * 21)
                };
                chebox.MouseMove += Chebox_MouseMove;
                flayout.Controls.Add(chebox);
            }
            this.Controls.Add(flayout);
        }

        private void Chebox_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (var con in flayout.Controls)
            {
                if (con is CheckBox)
                {
                    if (((CheckBox) sender).BackColor == Color.BurlyWood)
                    {
                        ((CheckBox) sender).BackColor = Color.White;
                    }
                }
            }
            ((CheckBox)sender).BackColor = Color.BurlyWood;
            this.Invalidate();

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
