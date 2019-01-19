using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace screenTest
{
    public sealed partial class CodeEdit : UserControl
    {
        #region 变量
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101, WM_IME_NOTIFY = 0x0282;
        private const int WM_CHAR = 0x0102;
        private const int WM_IME_CONTROL = 0x0283;
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_IME_CHAR = 0x0286;
        private const int WM_IME_COMPOSITION = 0x010F;
        private ImeComponent immManage;
        Font font = new Font("宋体", 14, FontStyle.Regular);

        private float WideSpaceWidth;
        private Win32Caret caret;

        private PointF lastPoint;

        private List<RowTextContext> rowTextBufferList =
            new List<RowTextContext>();
        GapTextBufferStrategy gapTextBufferStrategy;
        const int minGapLength = 128;
        const int maxGapLength = 2048;
        StringFormat sf = (StringFormat)System.Drawing.StringFormat.GenericTypographic.Clone();

        #endregion

        public CodeEdit()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            //实例化
            caret = new Win32Caret(this);
            lastPoint = new Point(0, 0);
            immManage = new ImeComponent(this);
            immManage.InputText += ImmManage_InputText; ;
            this.Cursor = Cursors.IBeam;
        }
        #region 属性
        public int VisibleLineCount => 1 + this.Height / GetFontHeight(font);

        /// <summary>
        /// 显示的列数
        /// </summary>
        public int VisibleColumnCount
        {
            get
            {
                WideSpaceWidth = this.CreateGraphics().MeasureString("1", font, 32768, sf).Width;
                return (int)(this.Width / WideSpaceWidth) - 1;
            }
        }
        #endregion 

        static int GetFontHeight(Font font)
        {
            int height1 = TextRenderer.MeasureText("_", font).Height;
            int height2 = (int)Math.Ceiling(font.GetHeight());
            return Math.Max(height1, height2) + 1;
        }


        protected RectangleF CalcSize(Graphics g, string txt, Font fnt, float x, float y, StringFormat fmt)
        {
            SizeF rectNeed = g.MeasureString(txt, fnt);
            var rect = new RectangleF(x, y, rectNeed.Width, rectNeed.Height);
            if (fmt.Alignment == StringAlignment.Center)
                rect.X -= rect.Width / 2;
            else if (fmt.Alignment == StringAlignment.Far)
                rect.X -= rect.Width;
            return rect;
        }


        private float DrawDocumentWord(Graphics g, string word, PointF position, Font font, Color foreColor)
        {
            if (string.IsNullOrEmpty(word))
            {
                return 0f;
            }

            SizeF wordSize = g.MeasureString(word, font, 32768, sf);

            var rectangleFz = CalcSize(g, word, font, position.X, position.Y, sf);
            g.DrawString(word,
                font,
                Brushes.White,
                rectangleFz,
                sf);
            return wordSize.Width;
        }

        private Tuple<bool, string, string> IsNewLine(string text)
        {
            string oldLineText;
            string newLineText;

            var g = this.CreateGraphics();
            SizeF wordSize = g.MeasureString(text, font, 32768, sf);
            var sw = this.Width - lastPoint.X;
            //判断宽度如果在一个字符宽度内直接换行
            if (sw < WideSpaceWidth * 2)
            {
                oldLineText = "";
                newLineText = text;
                g.Dispose();
                return new Tuple<bool, string, string>(true, oldLineText, newLineText);
            }

            if (lastPoint.X + wordSize.Width > this.Width)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    oldLineText = text.Substring(0, i);
                    var wz = g.MeasureString(oldLineText, font, 32768, sf).Width + WideSpaceWidth * 2;
                    if (wz >= sw)
                    {
                        newLineText = text.Substring(i, text.Length - i);
                        g.Dispose();
                        return new Tuple<bool, string, string>(true, oldLineText, newLineText);
                    }
                }
            }
            g.Dispose();
            return new Tuple<bool, string, string>(false, "", "");
        }


        private void reDrawword()
        {
            if (rowTextBufferList != null)
            {
                var g = this.CreateGraphics();
                foreach (var tb in rowTextBufferList)
                {
                    var rectangleFz = CalcSize(g, tb.TextBufferStrategy.GetText(0, tb.TextBufferStrategy.Length), font, tb.StartOffset, tb.LineNumber * GetFontHeight(font), sf);
                    g.DrawString(tb.TextBufferStrategy.GetText(0, tb.TextBufferStrategy.Length),
                        font,
                        Brushes.White,
                        rectangleFz,
                        sf);
                }
            }
        }
        /// <summary>
        /// 输入事件
        /// </summary>
        /// <param name="text"></param>
        private void ImmManage_InputText(string text)
        {
            Graphics g = this.CreateGraphics();
            //var temp = this.Location;
            //temp.Offset(Point.Round(lastPoint));
            //判断当前集合中是否有没有添加
            var rowTextContexts = rowTextBufferList.Where(p => p.LineNumber == lineNumber);
            var textContexts = rowTextContexts as RowTextContext[] ?? rowTextContexts.ToArray();
            if (textContexts.Any())
            {
                gapTextBufferStrategy = textContexts.ToList()[0].TextBufferStrategy;
            }
            else
            {
                gapTextBufferStrategy = new GapTextBufferStrategy();
                rowTextBufferList.Add(new RowTextContext
                {
                    LineNumber = lineNumber,
                    StartOffset = (int)lastPoint.X,
                    TextBufferStrategy = gapTextBufferStrategy
                });
            }
            //判断文字是否换行
            var tuple = IsNewLine(text);
            if (tuple.Item1)
            {
                //旧行插入
                gapTextBufferStrategy.Insert(gapTextBufferStrategy.Length, tuple.Item2);
                lastPoint.X += DrawDocumentWord(g, tuple.Item2, lastPoint, font, Color.Black);
                //换行
                lineNumber++;
                lastPoint.X = 0;
                lastPoint.Y = lineNumber * GetFontHeight(font);

                rowTextContexts = rowTextBufferList.Where(p => p.LineNumber == lineNumber);
                textContexts = rowTextContexts as RowTextContext[] ?? rowTextContexts.ToArray();
                if (textContexts.Any())
                {
                    gapTextBufferStrategy = textContexts.ToList()[0].TextBufferStrategy;
                }
                else
                {
                    gapTextBufferStrategy = new GapTextBufferStrategy();
                    rowTextBufferList.Add(new RowTextContext
                    {
                        LineNumber = lineNumber,
                        StartOffset = (int)lastPoint.X,
                        TextBufferStrategy = gapTextBufferStrategy
                    });
                }
                gapTextBufferStrategy.Insert(gapTextBufferStrategy.Length, tuple.Item3);

                lastPoint.X += DrawDocumentWord(g, tuple.Item3, lastPoint, font, Color.Black);

            }
            else
            {
                gapTextBufferStrategy.Insert(gapTextBufferStrategy.Length, text);
                lastPoint.X += DrawDocumentWord(g, text, lastPoint, font, Color.Black);
            }

            caret.Destroy();
            caret.Create(2, font.Height);
            caret.SetPosition((int)lastPoint.X, (int)lastPoint.Y);
            caret.Show();
            g.Dispose();
        }

        private void InternalDrawOutline(Graphics graphics, Rectangle rect, string text)
        {
            var path = new GraphicsPath();
            float emSize = graphics.DpiY * this.Font.Size / 72;
            path.AddString(text, this.Font.FontFamily, (int)this.Font.Style, emSize, rect, sf);
            graphics.DrawPath(Pens.HotPink, path);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_IME_SETCONTEXT:
                case WM_KEYDOWN:
                case WM_KEYUP:
                case WM_CHAR:
                case WM_IME_COMPOSITION:
                    this.immManage.ImmOperation(m); //输入法  
                    break;
                case WM_IME_NOTIFY:
                    OnImeChange(EventArgs.Empty);
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private static readonly object EventImechange = new object();
        private void OnImeChange(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[EventImechange];
            handler?.Invoke(this, e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(ColorTranslator.FromHtml("#666699"));
            e.Graphics.Clear(Color.White);
            e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#666699")), 0, 0, Width, Height);
            DrawGrid();
            base.OnPaint(e);
        }

        #region 键盘事件相关处理方法
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
                //isDownControl = true;
            }
            if (KeyDownIsControlKeys(keydownwParam))
            {

                Console.WriteLine("功能键:{0},{1}", (Keys)keydownwParam, keydownwParam);
                //var tuple2 = new Tuple<int, List<Keys>>(0, new List<Keys> { (Keys)keydownwParam });
                //_documentControl.OnKeyInput(tuple2);
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
                //isDownControl = false;
            }
        }
        /// <summary>
        /// 文字输入事件处理
        /// </summary>
        /// <param name="m"></param>
        private void WMChar(ref Message m)
        {
            //if (isDownControl)
            //{
            //    //control按下时为组合件,不作为文字输入
            //    return;
            //}
            int wParam = (int)m.WParam;
            if (WMCharIsControlKeys(wParam))
            {
                Console.WriteLine("功能键:{0},{1}", (Keys)wParam, wParam);
                //var tuple = new Tuple<int, List<Keys>>(0, new List<Keys> { (Keys)wParam });
                //_documentControl.OnKeyInput(tuple);
                return;
            }
            if (wParam > 31 && wParam < 127)
            {
                string txt = char.ConvertFromUtf32(wParam).ToString();
            }
        }

        private void WMIMESetcontext(ref Message m)
        {
            var res = m.WParam.ToInt32();
            string text = this.immManage.CurrentCompStr(this.Handle);
            if (!string.IsNullOrEmpty(text))
            {

            }
        }

        #endregion //键盘事件相关处理方法

        //当前行号
        int lineNumber = 0;
        //行间距
        int lineToppad = 1;
        //当前列号
        int columnNumber = 0;
        //输入内容的真实行和列数
        int contentColumnNum, contentLineNum = 0;
        int fh = 0;

        void DrawGrid()
        {

            Debug.WriteLine($"VisibleLineCount{VisibleLineCount}");
            Debug.WriteLine($"VisibleColumnCount{VisibleColumnCount}");
            fh = GetFontHeight(font);
            //绘制行
            //for (int i = 1; i < VisibleLineCount + 1; i++)
            //{
            //    var rect = new Rectangle(new Point(0, fh * i + lineToppad), new Size(this.Width, fh + lineToppad));
            //    this.CreateGraphics().DrawLine(SystemPens.Highlight, new Point(0, fh * i + lineToppad), new Point(this.Width, fh * i + lineToppad));
            //}

            ////绘制列
            //for (int c = 0; c < VisibleColumnCount + 1; c++)
            //{
            //    var rectC = new RectangleF(new PointF(WideSpaceWidth * c, fh * lineNumber + c * lineToppad), new SizeF(WideSpaceWidth, fh + lineToppad));
            //    this.CreateGraphics().DrawLine(SystemPens.Highlight, new PointF(WideSpaceWidth * c, 0), new PointF(WideSpaceWidth * c, Height));
            //}
        }

        private int Clickrow = 0;
        //Clickcolum = 0;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //var pos = Cursor.Position;
            //var clientPos = PointToClient(pos);
            base.OnMouseClick(e);
            //根据x 坐标计算第几列 坐标除去宽度 算出大概列数 根据余数计算具体那一列 
            //columnNumber = (int)Math.Ceiling(e.X / (WideSpaceWidth));

            //Debug.WriteLine($"列号{columnNumber}");
            // 根据Y 坐标计算第几行坐标除去高度 算出大概行数 根据余数计算具体那一行
            double y1 = (float)e.Y / (float)(fh + lineToppad);
            if (y1 > 1)
            {
                lineNumber = (int)Math.Ceiling(y1);
                lineNumber = lineNumber - 1;
            }
            else
            {
                lineNumber = 0;
            }
            Debug.WriteLine($"行号{lineNumber}");
            #region update 
            //根据行号换算 坐标点 
            lastPoint.Y = lineNumber * fh + lineToppad;
            //lastPoint.X = columnNumber * WideSpaceWidth;
            var time = TimeConsuming(() =>
                 {
                     lastPoint.X = GetCharLocationX(e.Location.X);
                 });
            Debug.WriteLineIf(time > 0, $"耗时：{time}  ms");
            #endregion
            caret.Destroy();
            caret.Create(1, font.Height - 2);
            caret.SetPosition((int)lastPoint.X, (int)lastPoint.Y);
            caret.Show();
            base.OnMouseDown(e);
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
        public Rectangle GetNormalizedRectangle(int x1, int y1, int x2, int y2)
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                var focusRectangle = GetNormalizedRectangle((int)lastPoint.X, (int)lastPoint.Y, e.X, e.Y);

                //ControlPaint.DrawBorder3D(this.CreateGraphics(), focusRectangle);
                //this.Invalidate();
                this.CreateGraphics().FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#666699")), 0, 0, Width, Height);
                this.CreateGraphics().FillRectangle(Brushes.Black, focusRectangle);
                reDrawword();
                //return;
            }
            base.OnMouseMove(e);
        }
        #region 耗时计算方法
        #region API 耗时计算
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        static extern bool QueryPerformanceFrequency(ref long count);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        static extern bool QueryPerformanceCounter(ref long count);
        #endregion
        /// <summary>
        /// 耗时计算
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        double TimeConsuming(Action action)
        {
            long count1 = 0;
            long count2 = 0;
            long freq = 0;
            double result = 0;
            QueryPerformanceFrequency(ref freq);
            QueryPerformanceCounter(ref count1);
            action?.Invoke();
            QueryPerformanceCounter(ref count2);
            result = (double)((count2 - count1) / (double)freq) * 1000;
            return result;
        }

        #endregion
        // Tuple<int, float>
        int GetCharLocationX(int x)
        {
            var rowTextContexts = rowTextBufferList.Where(p => p.LineNumber == lineNumber);
            var textContexts = rowTextContexts as RowTextContext[] ?? rowTextContexts.ToArray();
            if (textContexts.Any())
            {
                var rowText = textContexts.ToList()[0];
                gapTextBufferStrategy = rowText.TextBufferStrategy;
                var g = Graphics.FromHwndInternal(IntPtr.Zero);
                for (int i = 0; i < gapTextBufferStrategy.Length; i++)
                {
                    //字符宽度
                    var z = g.MeasureString(gapTextBufferStrategy.GetText(0, i), font, 32768, sf);
                    //字宽度加起点 计算字符当前位置x
                    var re = z.Width + rowText.StartOffset;
                    //字体坐标减去坐标 第一次大于时说明在它上面 
                    var s = re - x;
                    if (s > 0)
                    {
                        g.Dispose();
                        var str = gapTextBufferStrategy.GetText(i - 1, 1);
                        Debug.WriteLineIf(str != null, $"点击字符：{str}  ");
                        return (int)re;
                    }
                    else
                    {
                        //判断最后一位的字符类型
                        if (i == gapTextBufferStrategy.Length - 1)
                        {
                            var chr = gapTextBufferStrategy.GetText(i, 1);
                            var wideSpacew = g.MeasureString(chr, font, 32768, sf);
                            if (Math.Abs(s) < wideSpacew.Width)
                            {
                                g.Dispose();
                                var str = gapTextBufferStrategy.GetText(i, 1);
                                Debug.WriteLineIf(str != null, $"点击字符：{str}  ");
                                return (int)re;
                            }
                        }
                    }
                }
            }
            else
            {
                return x;
            }

            return x;
        }
    }

    #region imm
    /// <summary>
    /// 输入法组件
    /// </summary>
    class ImeComponent
    {
        #region Event
        /// <summary>
        /// 输入文本事件
        /// </summary>
        public delegate void InputTextEvent(string text);
        /// <summary>
        /// 输入文本事件
        /// </summary>
        public event InputTextEvent InputText;
        #endregion

        #region PrivateField
        /// <summary>
        /// 输入方法上下文句柄
        /// </summary>
        IntPtr hIMC;
        /// <summary>
        /// 窗口句柄
        /// </summary>
        IntPtr handle;
        /// <summary>
        /// 激活窗口时将消息发送到应用程序（ 输入焦点转移到了某个窗口上）
        /// </summary>
        //private const int WM_IME_SETCONTEXT = 0x0281;
        //private const int WM_IME_CHAR = 0x0286;
        private const int WM_CHAR = 0x0102;
        //通知应用程序有关组合字符串的更改
        private const int WM_IME_COMPOSITION = 0x010F;
        private const int GCS_RESULTSTR = 0x0800;
        private const int GCS_COMPSTR = 0x0008;
        //由应用程序直接向IME发出控制请求
        //private const int WM_IME_CONTROL = 0x0283;
        //内部写法 vs
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_IME_NOTIFY = 0x0282;
        private const int WM_IME_CONTROL = 0x0283;
        private const int WM_IME_COMPOSITIONFULL = 0x0284;
        private const int WM_IME_SELECT = 0x0285;
        private const int WM_IME_CHAR = 0x0286;
        private const int WM_IME_KEYDOWN = 0x0290;
        private const int WM_IME_KEYUP = 0x0291;
        /*   IME消息
         　　以下列出IME中用到的消息。
                        //当设备的硬件配置改变时发送此消息给应用程序或设备驱动程序
                 //WM_IME_STARTCOMPOSITION = $010D;
                 //WM_IME_ENDCOMPOSITION = $010E;
                 //WM_IME_COMPOSITION = $010F;
                 //WM_IME_KEYLAST = $010F;
                 //WM_IME_SETCONTEXT = $0281;
                 //WM_IME_NOTIFY = $0282;
                 //WM_IME_CONTROL = $0283;
                 //WM_IME_COMPOSITIONFULL = $0284;
                 //WM_IME_SELECT = $0285;
                 //WM_IME_CHAR = $0286;
                 //WM_IME_REQUEST = $0288;
                 //WM_IME_KEYDOWN = $0290;
                 //WM_IME_KEYUP = $0291;
                 //WM_MDICREATE = $0220;
         WM_IME_CHAR（IME得到了转换结果中的一个字符）
         WM_IME_COMPOSITION（IME根据用户击键的情况更改了按键组合状态）
         WM_IME_COMPOSITIONFULL（IME检测到按键组合窗口的区域无法继续扩展）
         WM_IME_CONTROL（由应用程序直接向IME发出控制请求）
         WM_IME_ENDCOMPOSITION（IME完成了对用户击键情况的组合）
         WM_IME_KEYDOWN（检测到“键盘上的某键被按下”的动作，同时在消息队列中保留该消息）
         WM_IME_KEYUP（检测到“键盘上的某键已弹起”的动作，同时在消息队列中保留该消息）
         WM_IME_NOTIFY（IME窗口发生了改变）
         WM_IME_REQUEST（通知：IME需要应用程序提供命令和请求信息）
         WM_IME_SELECT（操作系统将改变当前IME）
         WM_IME_SETCONTEXT（输入焦点转移到了某个窗口上）
         WM_IME_STARTCOMPOSITION（IME准备生成转换结果）

         IME函数
         　　本节列出了所有IME函数。
         函数
          说明
         EnumInputContext
          由应用程序定义的，提供给ImmEnumInputContext函数用来处理输入环境的一个回调函数。
         EnumRegisterWordProc
          由应用程序定义的，结合ImmEnumRegisterWord函数一起使用的一个回调函数。
         ImmAssociateContext
          建立指定输入环境与窗口之间的关联。
         ImmAssociateContextEx
          更改指定输入环境与窗口（或其子窗口）之间的关联。
         ImmConfigureIME
          显示指定的输入现场标识符的配置对话框。
         ImmCreateContext
          创建一个新的输入环境，并为它分配内存和初始化它。
         ImmDestroyContext
          销毁输入环境并释放和它关联的内存。
         ImmDisableIME
          关闭一个线程或一个进程中所有线程的IME功能。
         ImmDisableTextFrameService
          关闭指定线程的文本服务框架（TSF）功能－－虽然这里把它列了出来，但建议程序员最好不要使用这个函数。
         ImmEnumInputContext
          获取指定线程的输入环境。
         ImmEnumRegisterWord
          列举跟指定读入串、样式和注册串相匹配的注册串。
         ImmEscape
          对那些不能通过IME API函数来访问的特殊输入法程序提供兼容性支持的一个函数。
         ImmGetCandidateList
          获取一个候选列表。
         ImmGetCandidateListCount
          获取候选列表的大小。
         ImmGetCandidateWindow
          获取有关候选列表窗口的信息。
         ImmGetCompositionFont
          获取有关当前用来显示按键组合窗口中的字符的逻辑字体的信息。
         ImmGetCompositionString
          获取有关组合字符串的信息。
         ImmGetCompositionWindow
          获取有关按键组合窗口的信息。
         ImmGetContext
          获取与指定窗口相关联的输入环境。
         ImmGetConversionList
          在不生成任何跟IME有关的消息的情况下，获取输入按键字符组合或输出文字的转换结果列表。
         ImmGetConversionStatus
          获取当前转换状态。
         ImmGetDefaultIMEWnd
          获取缺省IME类窗口的句柄。
         ImmGetDescription
          复制IME的说明信息到指定的缓冲区中。
         ImmGetGuideLine
          获取出错信息。
         ImmGetIMEFileName
          获取跟指定输入现场相关联的IME文件名。
         ImmGetImeMenuItems
          获取注册在指定输入环境的IME菜单上的菜单项。
         ImmGetOpenStatus
          检测IME是否打开。
         ImmGetProperty
          获取跟指定输入现场相关联的IME的属性和功能。
         ImmGetRegisterWordStyle
          获取跟指定输入现场相关联的IME所支持的样式列表。
         ImmGetStatusWindowPos
          获取状态窗口的位置。
         ImmGetVirtualKey
          获取跟IME处理的键盘输入消息相关联的初始虚拟键值。
         ImmInstallIME
          安装一个IME。
         ImmIsIME
          检测指定的输入现场是否有和它相关的IME。
         ImmIsUIMessage
          检查IME窗口消息并发送那些消息到特定的窗口。
         ImmNotifyIME
          通知IME有关输入环境状态已改变的消息。
         ImmRegisterWord
          注册一个输出文字到跟指定输入现场相关联的IME的字典中去。
         ImmReleaseContext
          销毁输入环境并解除对跟它相关联的内存的锁定。
         ImmSetCandidateWindow
          设置有关候选列表窗口的信息。
         ImmSetCompositionFont
          设置用来显示按键组合窗口中的字符的逻辑字体。
         ImmSetCompositionString
          设置按键组合字符串的字符内容、属性和子串信息。
         ImmSetCompositionWindow
          设置按键组合窗口的位置。
         ImmSetConversionStatus
          设置当前转换状态。
         ImmSetOpenStatus
          打开或关闭IME功能。
         ImmSetStatusWindowPos
          设置状态窗口的位置。
         ImmSimulateHotKey
          在指定的窗口中模拟一个特定的IME热键动作，以触发该窗口相应的响应动作。
         ImmUnregisterWord
          从跟指定输入环境相关联的IME的字典中注销一个输出文字。

         IME命令
         　　以下列出IME中用到的命令（控制消息）。

         IMC_CLOSESTATUSWINDOW（隐藏状态窗口）
         IMC_GETCANDIDATEPOS（获取候选窗口的位置）
         IMC_GETCOMPOSITIONFONT（获取用来显示按键组合窗口中的文本的逻辑字体）
         IMC_GETCOMPOSITIONWINDOW（获取按键组合窗口的位置）
         IMC_GETSTATUSWINDOWPOS（获取状态窗口的位置）
         IMC_OPENSTATUSWINDOW（显示状态窗口）
         IMC_SETCANDIDATEPOS（设置候选窗口的位置）
         IMC_SETCOMPOSITIONFONT（设置用来显示按键组合窗口中的文本的逻辑字体）
         IMC_SETCOMPOSITIONWINDOW（设置按键组合窗口的样式）
         IMC_SETSTATUSWINDOWPOS（设置状态窗口的位置）
         IMN_CHANGECANDIDATE（IME通知应用程序：候选窗口中的内容将改变）
         IMN_CLOSECANDIDATE（IME通知应用程序：候选窗口将关闭）
         IMN_CLOSESTATUSWINDOW（IME通知应用程序：状态窗口将关闭）
         IMN_GUIDELINE（IME通知应用程序：将显示一条出错或其他信息）
         IMN_OPENCANDIDATE（IME通知应用程序：将打开候选窗口）
         IMN_OPENSTATUSWINDOW（IME通知应用程序：将创建状态窗口）
         IMN_SETCANDIDATEPOS（IME通知应用程序：已结束候选处理同时将移动候选窗口）
         IMN_SETCOMPOSITIONFONT（IME通知应用程序：输入内容的字体已更改）
         IMN_SETCOMPOSITIONWINDOW（IME通知应用程序：按键组合窗口的样式或位置已更改）
         IMN_SETCONVERSIONMODE（IME通知应用程序：输入内容的转换模式已更改）
         IMN_SETOPENSTATUS（IME通知应用程序：输入内容的状态已更改）
         IMN_SETSENTENCEMODE（IME通知应用程序：输入内容的语句模式已更改）
         IMN_SETSTATUSWINDOWPOS（IME通知应用程序：输入内容中的状态窗口的位置已更改）
         IMR_CANDIDATEWINDOW（通知：选定的IME需要应用程序提供有关候选窗口的信息）
         IMR_COMPOSITIONFONT（通知：选定的IME需要应用程序提供有关用在按键组合窗口中的字体的信息）
         IMR_COMPOSITIONWINDOW（通知：选定的IME需要应用程序提供有关按键组合窗口的信息）
         IMR_CONFIRMRECONVERTSTRING（通知：IME需要应用程序更改RECONVERTSTRING结构）
         IMR_DOCUMENTFEED（通知：选定的IME需要从应用程序那里取得已转换的字符串）
         IMR_QUERYCHARPOSITION（通知：选定的IME需要应用程序提供有关组合字符串中某个字符的位置信息）
         IMR_RECONVERTSTRING（通知：选定的IME需要应用程序提供一个用于自动更正的字符串）

         IME编程中需要用到的数据结构
         　　这里列了所有在使用输入法编辑器函数和消息时需要用到的数据结构。
         　　CANDIDATEFORM（描述候选窗口的位置信息）
         　　CANDIDATELIST（描述有关候选列表的信息）
         　　COMPOSITIONFORM（描述按键组合窗口的样式和位置信息）
         　　IMECHARPOSITION（描述按键组合窗口中的字符的位置信息）
         　　IMEMENUITEMINFO（描述IME菜单项的信息）
         　　RECONVERTSTRING（定义用于IME自动更正功能的字符串）
         　　REGISTERWORD（描述一个要注册的读入信息或文字内容）
         　　STYLEBUF（描述样式的标识符和名称）

         IME常量
         　　这里列出了所有在使用输入法编辑器函数和消息时需要用到的常量。
         　　?　IME转换模式常量
         　　?　IME按键组合字符串常量
         　　?　IME热键标识常量
         　　?　IME句型模式常量
         　　?　IMMEscape函数常量 */
        #endregion

        #region Construction
        public ImeComponent(UserControl control)
        {
            var handle = control.Handle;
            hIMC = ImmGetContext(handle);
            this.handle = handle;
        }

        public ImeComponent(Form from)
        {
            var handle = from.Handle;
            hIMC = ImmGetContext(handle);
            this.handle = handle;
        }
        #endregion

        #region Method
        /// <summary>
        /// 输入事件
        /// </summary>
        /// <param name="m"></param>
        public void ImmOperation(Message m)
        {
            if (m.Msg == ImeComponent.WM_IME_SETCONTEXT && m.WParam.ToInt32() == 1)
            {
                this.ImmAssociateContext(handle);
            }
            else if (m.Msg == WM_IME_COMPOSITION)
            {

                var res = m.WParam.ToInt32();
                string text = CurrentCompStr(this.handle);
                if (!string.IsNullOrEmpty(text))
                {
                    InputText(text);
                }
            }
            else if (m.Msg == WM_CHAR)
            {
                char inputchar = (char)m.WParam;
                if (inputchar > 31 && inputchar < 127)
                {
                    InputText(inputchar.ToString());
                }
            }
        }
        /// <summary>
        /// 当前输入的字符串流
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public string CurrentCompStr(IntPtr handle)
        {
            try
            {
                int strLen = ImmGetCompositionStringW(hIMC, GCS_RESULTSTR, null, 0);
                if (strLen > 0)
                {
                    var buffer = new byte[strLen];
                    ImmGetCompositionStringW(hIMC, GCS_RESULTSTR, buffer, strLen);
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
        #endregion

        #region Win Api 
        //字汇 候选列表  、候选人名单、候选窗口  合成窗口
        /// <summary>
        /// 建立指定输入环境与窗口之间的关联
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        internal IntPtr ImmAssociateContext(IntPtr hWnd)
        {
            return ImeComponent.ImmAssociateContext(hWnd, hIMC);
        }

        /// <summary>
        /// 首先指定窗口关联输入上下文句柄
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        /// <summary>
        /// 释放上下文
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="hIMC"></param>
        /// <returns></returns>
        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        /// <summary>
        /// 将创建输入上下文分配给直添窗体
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hIMC"></param>
        /// <returns></returns>
        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);
        /// <summary>
        /// 创建输入上下文
        /// </summary>
        /// <returns></returns>
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern int ImmCreateContext();

        /// <summary>
        /// 删除输入上下文
        /// </summary>
        /// <param name="hImc"></param>
        /// <returns></returns>
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmDestroyContext(int hImc);
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        /// <summary>
        /// 检索指示的字符串或数据
        /// </summary>
        /// <param name="hIMC">输入上下文句柄</param>
        /// <param name="dwIndex">索引</param>
        /// <param name="lPBuf">缓存</param>
        /// <param name="dwBufLen">缓存长度</param>
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        private static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);

        /// <summary>
        /// 检索指示的字符串或数据
        /// </summary>
        /// <param name="hIMC">输入上下文句柄</param>
        /// <param name="dwIndex">索引</param>
        /// <param name="lPBuf">缓存</param>
        /// <param name="dwBufLen">缓存长度</param>
        /// <returns></returns>
        [DllImport("imm32.dll")]
        static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, StringBuilder lPBuf, int dwBufLen);

        //[DllImport("imm32.dll")]
        //bool ImmGetCompositionWindow(IntPtr HIMC,  LPCOMPOSITIONFORM lpCompForm);

        #endregion
    }
    #endregion


    class RowTextContext
    {
        public int LineNumber { get; set; }

        public GapTextBufferStrategy TextBufferStrategy { get; set; }

        public int StartOffset { get; set; }
    }

}
