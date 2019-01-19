using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TextBoxImitation
{
    /// <summary>
    /// 本程序为窗体控件提供插入符（光标）的控制，可创建，设置显示隐藏和位置
    /// </summary>

    public class CaretCursor
    {

        private readonly IWin32Window mycontrol = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        public CaretCursor(int hwnd)
        {
            Win32Handle handle = new Win32Handle {handle = new IntPtr(hwnd)};
            mycontrol = handle;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        public CaretCursor(IntPtr hwnd)
        {
            Win32Handle handle = new Win32Handle {handle = hwnd};
            mycontrol = handle;
        }
        /// <summary>
        /// 初始化
        /// <param name="ctl">本窗口</param>
        /// </summary>
        public CaretCursor(Control ctl)
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
        #region API

        [StructLayout(LayoutKind.Sequential)]
        private struct APIPoint
        {
            public int x;
            public int y;
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

        #endregion 


        private class Win32Handle : IWin32Window
        {
            public IntPtr handle = IntPtr.Zero; 
            public IntPtr Handle => handle;
        }
    }
}
