/*************************************************
 * 描述：
 * 
 * Author：lican@mozihealthcare.cn
 * Date：2018/11/7 16:48:19
 * Update：
 * ************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screenTest
{

    /* WinForm IME输入法BUG完美修复
    * 编译：csc.exe /target:winexe WinformImeBugFixed.cs
    */
    using System;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Runtime.InteropServices;

    namespace WinformImeBugFixed
    {
        public class Form1 : Form
        {
            #region 解决输入法BUG   
            //解决输入法BUG   
            [DllImport("imm32.dll")]
            public static extern IntPtr ImmGetContext(IntPtr hwnd);
            [DllImport("imm32.dll")]
            public static extern bool ImmSetOpenStatus(IntPtr himc, bool b);

            protected override void OnActivated(EventArgs e)
            {
                base.OnActivated(e);
                IntPtr HIme = ImmGetContext(this.Handle);
                ImmSetOpenStatus(HIme, true);
            }
            #endregion
            #region 不感兴趣的   

            private void DrawTextboxes()
            {
                Controls.Clear();
                int x, y, d;
                x = y = d = 10;
                for (int i = 0; i < 2; i++)
                {
                    var textbox = new TextBox()
                    {
                        Width = 200,
                        Location = new Point(x, y)
                    };
                    y += textbox.Height + d;
                    textbox.DataBindings.Add("Text", textbox, "ImeMode");
                    Controls.Add(textbox);
                }
            }

            private void DrawButton()
            {
                var button = new Button()
                {
                    Text = "Show Form2",
                    Location = new Point(10, 70)
                };
                button.Click += delegate
                {
                    var form2 = new Form();
                    form2.Text = "Form2";
                    var textbox = new TextBox()
                    {
                        Width = 200,
                        Location = new Point(10, 10)
                    };
                    form2.Controls.Add(textbox);
                    form2.Show();
                };
                Controls.Add(button);
            }

            protected override void OnLoad(EventArgs e)
            {
                Text = "IME输入法BUG修复 F5-刷新 F1-博客";
                KeyPreview = true;
                DrawTextboxes();
                DrawButton();
                base.OnLoad(e);
            }

            public Form1()
            {
                InitializeComponent();
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                try { HandleKeyDown(e); }
                finally { base.OnKeyDown(e); }
            }

            private void HandleKeyDown(KeyEventArgs e)
            {
                if (e.KeyCode == Keys.F5) DrawTextboxes();
                else if (e.KeyCode == Keys.F1) NavigateBlog();
            }

            private void NavigateBlog()
            {
                System.Diagnostics.Process.Start("http://hi.baidu.com/wingingbob/blog/item/20741734532af846251f14f1.html");
            }
            #endregion
            #region Form1设计器   
            private System.ComponentModel.IContainer components = null;
            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
                this.SuspendLayout();
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(240, 100);
                this.Name = "Form1";
                this.Text = "Form1";
                this.ResumeLayout(false);
            }
            #endregion
            #region 入口点   
            static class Program
            {
                [STAThread]
                static void Main1()
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
            }
            #endregion
        }
    }

    public class GetComposition
    {
        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        private static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);

        private const int GCS_COMPSTR = 8;

        /// IntPtr handle is the handle to the textbox
        public string CurrentCompStr(IntPtr handle)
        {
            int readType = GCS_COMPSTR;

            IntPtr hIMC = ImmGetContext(handle);
            try
            {
                int strLen = ImmGetCompositionStringW(hIMC, readType, null, 0);

                if (strLen > 0)
                {
                    byte[] buffer = new byte[strLen];

                    ImmGetCompositionStringW(hIMC, readType, buffer, strLen);

                    return Encoding.Unicode.GetString(buffer);

                }
                else
                {
                    return string.Empty;
                }
            }
            finally
            {
                ImmReleaseContext(handle, hIMC);
            }
        }
    }
}
