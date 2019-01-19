/*************************************************
 * 描述：
 * 
 * Author：lican@mozihealthcare.cn
 * Date：2018/11/7 15:14:30
 * Update：
 * ************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace screenTest
{ 
    #region Caret implementation （补字符）插入符 实现

    /// <summary>
    /// 抽象类 脱字符实现类
    /// </summary>
    public abstract class CaretImplementation : IDisposable
    {
        //需要重新绘制位置更改
        public bool RequireRedrawOnPositionChange;
        //创建
        public abstract bool Create(int width, int height);
        //隐藏
        public abstract void Hide();
        //显示
        public abstract void Show();
        //设置位置
        public abstract bool SetPosition(int x, int y);
        //脱字符绘制
        public abstract void PaintCaret(Graphics g);
        //销毁
        public abstract void Destroy();
        //释放
        public virtual void Dispose()
        {
            Destroy();
        }
    }
    /// <summary>
    /// 管理脱字符
    /// </summary>
    class ManagedCaret : CaretImplementation
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 300 };
        bool visible;
        bool blink = true;
        int x, y, width, height;
        Control textArea; 

        public ManagedCaret(Control caret)
        {
            base.RequireRedrawOnPositionChange = true;
            this.textArea = caret;
            //this.parentCaret = caret;
            timer.Tick += CaretTimerTick;
        }

        void CaretTimerTick(object sender, EventArgs e)
        {
            blink = !blink;
            //if (visible)
            //    textArea.UpdateLine(parentCaret.Line);
        }

        public override bool Create(int width, int height)
        {
            this.visible = true;
            this.width = width - 2;
            this.height = height;
            timer.Enabled = true;
            return true;
        }
        public override void Hide()
        {
            visible = false;
        }
        public override void Show()
        {
            visible = true;
        }
        public override bool SetPosition(int x, int y)
        {
            this.x = x - 1;
            this.y = y;
            return true;
        }
        public override void PaintCaret(Graphics g)
        {
            if (visible && blink)
                g.DrawRectangle(Pens.Gray, x, y, width, height);
        }
        public override void Destroy()
        {
            visible = false;
            timer.Enabled = false;
        }
        public override void Dispose()
        {
            base.Dispose();
            timer.Dispose();
        }
    }

    internal class Win32Caret : CaretImplementation
    {
        [DllImport("User32.dll")]
        static extern bool CreateCaret(IntPtr hWnd, int hBitmap, int nWidth, int nHeight);

        [DllImport("User32.dll")]
        static extern bool SetCaretPos(int x, int y);

        [DllImport("User32.dll")]
        static extern bool DestroyCaret();

        [DllImport("User32.dll")]
        static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("User32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        // 光标当前位置
        [DllImport("User32.dll")]
        private static extern bool GetCaretPos(ref APIPoint p);

        [StructLayout(LayoutKind.Sequential)]
        private struct APIPoint
        {
            public int x;
            public int y;
        }

        Control textArea;
         
        public Win32Caret(Control caret)
        {
            this.textArea = caret;
        }

        public override bool Create(int width, int height)
        {
            return CreateCaret(textArea.Handle, 0, width, height);
        }
        public override void Hide()
        {
            HideCaret(textArea.Handle);
        }
        public override void Show()
        {
            ShowCaret(textArea.Handle);
        }
        public override bool SetPosition(int x, int y)
        {
            return SetCaretPos(x, y);
        }
        public override void PaintCaret(Graphics g)
        {
        }
        public override void Destroy()
        {
            DestroyCaret();
        }
    }
    #endregion

    /// <summary>
    /// 本程序为窗体控件提供插入符（光标）的控制，可创建，设置显示隐藏和位置
    /// </summary>
    public class Win32CaretW
    {
        private IWin32Window mycontrol = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        public Win32CaretW(int hwnd)
        {
            Win32Handle handle = new Win32Handle();
            handle.handle = new IntPtr(hwnd);
            mycontrol = handle;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        public Win32CaretW(IntPtr hwnd)
        {
            Win32Handle handle = new Win32Handle();
            handle.handle = hwnd;
            mycontrol = handle;
        }
        /// <summary>
        /// 初始化
        /// <param name="ctl">本窗口</param>
        /// </summary>
        public Win32CaretW(IWin32Window ctl)
        {
            this.mycontrol = ctl;
        }
        /// <summary>
        /// 光标当前位置
        /// </summary>
        public static Point CaretPosition2
        {
            get
            {
                APIPoint p = new APIPoint();
                if (GetCaretPos(ref p))
                {
                    return new Point(p.x, p.y);
                }
                return Point.Empty;
            }
        }
        /// <summary>
        /// 光标当前位置
        /// </summary>
        public Point CaretPosition()
        {

            APIPoint p = new APIPoint();
            if (GetCaretPos(ref p))
            {
                return new Point(p.x, p.y);
            }
            return Point.Empty;

        }

        /// <summary>
        /// 创建光标
        /// </summary>
        /// <param name="hBitmap">图片句柄</param>
        /// <param name="nWidth">光标宽度</param>
        /// <param name="nHeight">光标长度</param>
        /// <returns>操作是否成功</returns>
        public bool Create(int hBitmap, int nWidth, int nHeight)
        {
            if (mycontrol == null)
            {
                return false;
            }
            else
            {
                return CreateCaret(mycontrol.Handle, hBitmap, nWidth, nHeight);
            }
        }
        /// <summary>
        /// 设置光标位置
        /// </summary>
        /// <param name="x">光标X坐标</param>
        /// <param name="y">光标Y坐标</param>
        /// <returns>操作是否成功</returns>
        public bool SetPos(int x, int y)
        {
            if (mycontrol == null)
            {
                return false;
            }
            else
            {
                return SetCaretPos(x, y);
            }
        }
        /// <summary>
        /// 设置光标位置
        /// </summary>
        /// <param name="p">坐标点</param>
        /// <returns>操作是否成功</returns>
        public bool SetPos(Point p)
        {
            if (mycontrol == null)
            {
                return false;
            }
            else
            {
                return SetCaretPos(p.X, p.Y);
            }
        }
        /// <summary>
        /// 隐藏光标
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool Hide()
        {
            if (mycontrol == null)
            {
                return false;
            }
            else
            {
                return HideCaret(mycontrol.Handle);
            }
        }
        /// <summary>
        /// 显示光标
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool Show()
        {
            if (mycontrol == null)
            {
                return false;
            }
            else
            {
                return ShowCaret(mycontrol.Handle);
            }
        }

        public bool Destroy()
        {
            if (mycontrol == null)
            {
                return false;
            }
            else
            {
                return DestroyCaret();
            }
        }

        // 创建光标
        [DllImport("User32.dll")]
        private static extern bool CreateCaret(IntPtr hWnd, int hBitmap, int nWidth, int nHeight);
        // 显示光标
        [DllImport("User32.dll")]
        private static extern bool ShowCaret(IntPtr hWnd);
        // 隐藏光标
        [DllImport("User32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);
        // 设置光标位置
        [DllImport("User32.dll")]
        private static extern bool SetCaretPos(int x, int y);
        // 销毁光标
        [DllImport("User32.dll")]
        private static extern bool DestroyCaret();
        // 光标当前位置
        [DllImport("User32.dll")]
        private static extern bool GetCaretPos(ref APIPoint p);

        [StructLayout(LayoutKind.Sequential)]
        private struct APIPoint
        {
            public int x;
            public int y;
        }

        private class Win32Handle : IWin32Window
        {
            public IntPtr handle = IntPtr.Zero;

            public IntPtr Handle
            {
                get { return handle; }
            }
        }
    }
}
