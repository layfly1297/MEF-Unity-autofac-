using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TextBoxImitation
{
    /// <summary>
    /// 输入法调用
    /// </summary>
    public class Typewriting
    {
        #region Win Api



        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern int ImmCreateContext();
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmDestroyContext(int hImc);
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        private static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);
        [DllImport("imm32.dll")]
        static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, StringBuilder lPBuf, int dwBufLen);
        #endregion 
        #region PrivateField
        IntPtr hIMC;
        IntPtr handle;
        private const int GCS_RESULTSTR = 0x0800;
        private const int GCS_COMPSTR = 0x0008;
        #endregion
        #region Construction
        public Typewriting(IntPtr winHandle)
        {
            this.hIMC = ImmGetContext(winHandle);
            this.handle = winHandle;
        }

        #endregion
        /// <summary>
        /// 当前输入的字符串流
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public string CurrentCompStr()
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
        /// <summary>
        /// 建立指定输入环境与窗口之间的关联
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public IntPtr ImmAssociateContext()
        {
            return ImmAssociateContext(this.handle, hIMC);
        }
    }
}