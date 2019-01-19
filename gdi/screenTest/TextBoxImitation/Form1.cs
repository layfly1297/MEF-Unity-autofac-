using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TextBoxImitation
{
    /// <summary>
    /// 文本框仿制品
    /// </summary>
    public partial class Form1 : Form
    {
        #region 封存
        private readonly CaretCursor _caretCursor;

        private readonly Typewriting _typewriting;

        #region const
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_CHAR = 0x0102;
        private const int WM_IME_CONTROL = 0x0283;
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_IME_CHAR = 0x0286;
        private const int WM_IME_COMPOSITION = 0x010F;
        private const int IMC_SETCOMPOSITIONWINDOW = 0x000c;
        private const int IMC_SETCOMPOSITIONFONT = 0x000a;
        private const int CFS_POINT = 0x0002;
        /// <summary>
        /// Control是否被按下
        /// </summary>
        private bool isDownControl = false;
        #endregion
        private Point _lastPoint;

        public Form1()
        {
            InitializeComponent();

            _caretCursor = new CaretCursor(this);
            _typewriting = new Typewriting(this.Handle);
            _lastPoint = new Point(0, 0);
        }

        void CaretHand(bool visable)
        {
            _caretCursor.Destroy(); //Hide
            _caretCursor.Create(0, 1, SystemFonts.DefaultFont.Height);
            _caretCursor.SetPos(_lastPoint);
            _caretCursor.Show();

        }

        IntPtr himc = IntPtr.Zero;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = Control.MousePosition;
            Point cp = PointToClient(p);
            _lastPoint = e.Location;
            //CaretHand(true);
            System.Windows.Forms.Cursor.Current = Cursors.IBeam;
            System.Windows.Forms.Cursor.Position = _lastPoint;
            System.Windows.Forms.Cursor.Show();
            //himc = SafeNativeMethods.ImmGetDefaultIMEWnd(this.Handle);
            //SetIMEWindowLocation(e.X, e.Y);
            //判断输入法的模式
            //var mode = ImeContext.GetImeMode(this.Handle);
            ////判断释放打开输入法
            //var isopen = ImeContext.IsOpen(this.Handle);

            //Debug.WriteLine(mode);
            //Debug.WriteLine(isopen);
            Debug.WriteLine(this.ContainsFocus);
        }

        public void SetIMEWindowLocation(int x, int y)
        {

            SafeNativeMethods.POINT p = new SafeNativeMethods.POINT();
            p.x = x;
            p.y = y;

            SafeNativeMethods.COMPOSITIONFORM lParam = new SafeNativeMethods.COMPOSITIONFORM();
            lParam.dwStyle = CFS_POINT;
            lParam.ptCurrentPos = p;
            lParam.rcArea = new SafeNativeMethods.RECT();
            SafeNativeMethods.SendMessage(
                    himc,
                    WM_IME_CONTROL,
                    new IntPtr(IMC_SETCOMPOSITIONWINDOW),
                    lParam
                );

        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_IME_SETCONTEXT:
                    if (m.WParam.ToInt32() == 1)
                    {
                        this._typewriting.ImmAssociateContext();
                    }
                    break;
                case WM_KEYDOWN: //键盘按下
                    WMKeydown(ref m);
                    break;
                case WM_KEYUP: //键盘抬起
                    WMKeyup(ref m);
                    break;
                case WM_CHAR: //文字输入
                    WMChar(ref m);
                    break;
                case WM_IME_COMPOSITION:
                    WMIMESetcontext(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        #endregion  

        #region Key
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
                return;
            }
            if (wParam > 31 && wParam < 127)
            {
                string txt = char.ConvertFromUtf32(wParam);
            }
        }

        private void WMIMESetcontext(ref Message m)
        {
            var res = m.WParam.ToInt32();
            string text = this._typewriting.CurrentCompStr();
            if (!string.IsNullOrEmpty(text))
            {
                Graphics g = this.CreateGraphics();
                var temp = this.Location;
                temp.Offset(Point.Round(_lastPoint));
                var isContiant = this.Bounds.Contains(temp);
                if (!isContiant)
                {
                    _lastPoint.Y += SystemFonts.DefaultFont.Height + 2;
                    _lastPoint.X = 0;
                }
                var rect = new RectangleF(_lastPoint.X, _lastPoint.Y, this.Width, this.Height);

                _lastPoint.X += (int)DrawDocumentWord(g, text, _lastPoint, SystemFonts.DefaultFont, Color.Black);
                CaretHand(true);


                g.Dispose();
            }
        }

        #endregion //键盘事件相关处理方法

        #region Draw

        StringFormat sf = (StringFormat)System.Drawing.StringFormat.GenericTypographic.Clone();

        public RectangleF CalcSize(Graphics g, string txt, Font fnt, float x, float y, StringFormat fmt)
        {
            SizeF rectNeed = g.MeasureString(txt, fnt);
            var rect = new RectangleF(x, y, rectNeed.Width, rectNeed.Height);
            if (fmt.Alignment == StringAlignment.Center)
                rect.X -= rect.Width / 2;
            else if (fmt.Alignment == StringAlignment.Far)
                rect.X -= rect.Width;
            return rect;
        }

        public float DrawDocumentWord(Graphics g, string word, PointF position, Font font, Color foreColor)
        {
            if (string.IsNullOrEmpty(word))
            {
                return 0f;
            }

            SizeF wordSize = g.MeasureString(word, font, 32768, sf);

            var rectangleFz = CalcSize(g, word, font, position.X, position.Y, sf);

            g.DrawString(word,
                font,
                Brushes.Black,
                rectangleFz,
                sf);
            return wordSize.Width;
        }


        #endregion

        ///// <summary>
        ///// 响应扫描枪输入
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="keyData"></param>
        ///// <returns></returns>
        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    if (msg.Msg == 0x0100 && ContextMenuStrip != null)
        //    {
        //        foreach (ToolStripMenuItem item in ContextMenuStrip.Items)
        //        {
        //            if (keyData == item.ShortcutKeys)
        //            {
        //                item.PerformClick();
        //            }
        //        }
        //    }
        //    if (msg.Msg == 0x0100 && keyData == Keys.Enter)
        //    {
        //        string barcode = onceScanData;
        //        onceScanData = string.Empty;
        //        if (CoreFlowObj != null && FlowContext.Instance.WorkStatus == WorkStatus.Running && !string.IsNullOrEmpty(barcode))
        //        {
        //            CoreFlowObj.OnExecScanReceiving(TrimSpecialChar(barcode));
        //        }
        //        else
        //            OnScanReceivingData(TrimSpecialChar(barcode));
        //    }
        //    else
        //    {
        //        int key = (int)keyData;
        //        if (key >= (int)Keys.A && key <= (int)Keys.Z ||
        //            key >= (int)Keys.D0 && key <= (int)Keys.D9 ||
        //            key >= (int)Keys.NumPad0 && key <= (int)Keys.NumPad9 ||
        //            key > 65000)
        //        {
        //            #region 无赖，希望找到更好的办法
        //            if (keyData == Keys.NumPad0)
        //                keyData = Keys.D0;
        //            if (keyData == Keys.NumPad1)
        //                keyData = Keys.D1;
        //            if (keyData == Keys.NumPad2)
        //                keyData = Keys.D2;
        //            if (keyData == Keys.NumPad3)
        //                keyData = Keys.D3;
        //            if (keyData == Keys.NumPad4)
        //                keyData = Keys.D4;
        //            if (keyData == Keys.NumPad5)
        //                keyData = Keys.D5;
        //            if (keyData == Keys.NumPad6)
        //                keyData = Keys.D6;
        //            if (keyData == Keys.NumPad7)
        //                keyData = Keys.D7;
        //            if (keyData == Keys.NumPad8)
        //                keyData = Keys.D8;
        //            if (keyData == Keys.NumPad9)
        //                keyData = Keys.D9;
        //            #endregion
        //            onceScanData += (char)keyData;
        //        }
        //    }
        //    if (FlowContext.Instance.WorkStatus != WorkStatus.Running)
        //        return base.ProcessCmdKey(ref msg, keyData);
        //    else
        //        return true;
        //}
        private void button1_Click(object sender, EventArgs e)
        {

            //var test = @" <ImageElement X=""0"" Y=""0"" Width=""200"" Height=""200""  Url="""" DataProperty="""" Base64=""""  Guid="""" >123213</ImageElement>";

            //string xml = @"<ImageElement X = ""0"" Y = ""0"" Width = ""200"" Height = ""200""  Url = """" DataProperty = """" Base64 = """" Guid = """" ></ImageElement>";
            ////转义符处理
            //var s = System.Security.SecurityElement.Escape(xml);
            //////var y = s.Attributes["Y"];
            ////var valid = System.Security.SecurityElement.IsValidText(s);


            //XmlDocument xmlDocument = new XmlDocument();
            //xmlDocument.LoadXml(test);
            //var node = xmlDocument.SelectSingleNode($"/ImageElement");
            ////xmlDocument.Load(@"E:\WorkInformation\编辑器模板\首次病程记录.xml"); 
            //var se = System.Security.SecurityElement.FromString(xmlDocument.OuterXml);
            //var zf = node.Attributes["X"].Value;
            //xmlDocument.LoadXml(xml);
            //var ele = xmlDocument.FirstChild;
            //var attr = ele.Attributes;
            //var name = ele.Name;
            //XmlElement xe = (XmlElement)ele;
            //var x = xe.GetAttribute("X");
        }
        public static float PxConvertMillimetre(float oldPx)
        {
            return oldPx / 96f * 25.4f;
        }



        private void Form2_Paint(object sender, PaintEventArgs e)
        {

            Bitmap btmp = new Bitmap(500, 400);
            ////btmp.SetResolution(600, 300);
            ////btmp.SetResolution(300, 300);
            //Graphics gbt = Graphics.FromImage(btmp);
            //gbt.PageUnit = GraphicsUnit.Millimeter;
            //gbt.Clear(Color.White);
            //Font f = new Font("宋体", 15);
            ////gbt.DrawString("sfsdsfdsfasasf一二三四123456", f, Brushes.Black, 0, 0);
            //FontFamily fontfamily = new FontFamily("宋体");
            ////字体特效 下滑杠 加粗 中间线  
            //Font font = new Font(fontfamily, 15, FontStyle.Regular, GraphicsUnit.Pixel);

            ////文字方向控制
            ////StringFormat strformat = new StringFormat();
            ////strformat.FormatFlags = StringFormatFlags.DirectionRightToLeft | StringFormatFlags.DirectionVertical;//(StringFormatFlagsDirectionRightToLeft | StringFormatFlagsDirectionVertical);
            ////strformat.Alignment = StringAlignment.Center;//(StringAlignmentCenter);
            ////strformat.LineAlignment = StringAlignment.Center;// (StringAlignmentCenter); 

            ////使用与系统相同的处理方式
            //gbt.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;//(TextRenderingHintSystemDefault);
            //gbt.DrawString("什么玩意", font, new SolidBrush(Color.Green), new PointF(0, 0));


            //////不消除锯齿，使用网格匹配
            //gbt.TranslateTransform(0, font.GetHeight(0.0f));
            //gbt.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;// TextRenderingHintSingleBitPerPixelGridFit);
            ////graphics.DrawString(L"什么玩意", -1, &font, PointF(0, 0), &SolidBrush(Color::Green));
            //gbt.DrawString("什么玩意", font, new SolidBrush(Color.Green), new PointF(0, 0));
            //////不消除锯齿，不使用网格匹配
            //gbt.TranslateTransform(0, font.GetHeight(0.0f));
            //gbt.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;//(TextRenderingHintSingleBitPerPixel);
            //gbt.DrawString("什么玩意", font, new SolidBrush(Color.Green), new PointF(0, 0));

            //////消除锯齿，使用网格匹配
            //gbt.TranslateTransform(0, font.GetHeight(0.0f));
            //gbt.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;//(TextRenderingHintAntiAliasGridFit);
            //gbt.DrawString("什么玩意", font, new SolidBrush(Color.Green), new PointF(0, 0));

            //////消除锯齿，不使用网格匹配
            //gbt.TranslateTransform(0, font.GetHeight(0.0f));
            //gbt.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias; //TextRenderingHintAntiAlias);
            //gbt.DrawString("什么玩意", font, new SolidBrush(Color.Green), new PointF(0, 0));

            //////在液晶显示器上使用ClearType技术增强字体清晰度
            //gbt.TranslateTransform(0, font.GetHeight(0.0f));
            //gbt.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            ////gbt.SetTextRenderingHint(TextRenderingHintClearTypeGridFit);
            ////gbt.DrawString("什么玩意", font, new SolidBrush(Color.Green), new PointF(0, 0), strformat);
            //gbt.DrawString("什么玩意", font, new SolidBrush(Color.Green), new PointF(0, 0));

            //font.GetHeight(0.0f);
            //gbt.Dispose();
            //btmp.Save(@"12.bmp");
            ////pictureBox1.Image = btmp;

            //Bitmap cl = new Bitmap(btmp.Width + 300, btmp.Height + 300);
            ////cl.SetResolution(300, 300);
            //Graphics g = Graphics.FromImage(cl);

            //g.PageUnit = GraphicsUnit.Millimeter;

            //g.Clear(Color.Black);
            ////g.DrawImage(btmp, 0.32188F, 0,new RectangleF(0F,0F,(W- 0.32188F),h),GraphicsUnit.Millimeter);
            ////g.TranslateTransform(1, 0);
            ////g.TranslateClip(0.32188F, 0);
            //g.DrawImage(btmp, 0.32188F, 0);
            ////g.DrawImage(btmp, 0, 0);
            //cl.Save(@"12F.bmp");
            ////pictureBox2.Image = btmp;
            ////g.DrawImage(btmp, 0, 0);

            //btmp.Dispose();



            //e.Graphics.Clear(Color.White);

            //e.Graphics.DrawImage(cl, 0, 0);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //IntPtr captureIntptr = ((Bitmap)Image.FromFile("ioc.png")).GetHbitmap();
            //var currentBitmap = Image.FromHbitmap(captureIntptr) as Bitmap;
            //DeleteObject(captureIntptr); 
            //var mode = ImeContext.GetImeMode(this.Handle);
            //var isopen = ImeContext.IsOpen(this.Handle);
            //Debug.WriteLine(mode);
            //Debug.WriteLine(isopen);
            //ImeContext.SetOpenStatus(true, this.Handle);
            if (GetFocus() != IntPtr.Zero)
            {
                var imeMode = ImeContext.GetImeMode(GetFocus());
            }
            Debug.WriteLine(this.ContainsFocus);
        }


        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        [System.Runtime.Versioning.ResourceExposure(ResourceScope.None)]
        public static extern IntPtr GetFocus();

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private void Form1_Activated(object sender, EventArgs e)
        {
            //this.Focus(); 
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //var f = this.button2.Focused;


        }

        //protected override bool CanEnableIme => true;
    }



}
